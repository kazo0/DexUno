# AGENTS.md

Guidance for AI coding agents working in this repository.

## What this is

DexUno is a Pokémon GO pokedex app ported from an older UWP app ("Dex") to Uno Platform 3.x (2020-era, pre-single-project template). Data is bundled JSON (`DexUno/DexUno.Shared/Data/*.db.json`) plus per-Pokémon PNGs under `Assets/Flat/`. There are no tests and no CI. Last commit is a "WIP Dual Screen" spike (Surface Duo / `TwoPaneView`).

Naming mismatch to be aware of: folders and project names are `DexUno.*`, but almost all C# namespaces are the legacy `Dex.Uwp.*` (shared UI) and `Dex.Core.*` (domain). Only `Startup`, `ServiceCollectionExtensions` and the platform heads use `DexUno.*`. Follow the existing namespace of the folder you're editing, not the folder name.

## Solution layout

- `DexUno.Core/` — netstandard2.0 class library, no UI dependencies. Entities (`Pokemon`, `Move`, `CpCalculator`, `TypeEffectiveness`…), data-source interfaces (`Dex.Core.DataAccess`), repositories and `TypesService`.
- `DexUno/DexUno.Shared/` — a **shared project** (`.shproj`/`.projitems`), not a class library. Every head compiles these sources directly, so a change here is rebuilt per head. Contains App, Shell, Pages, ViewModels, Cards/Controls/Partials (XAML user controls), Theme resource dictionaries, converters, services, and the JSON data.
- `DexUno/DexUno.{Droid,iOS,macOS,UWP,Wasm,Skia.Gtk,Skia.Wpf,Skia.Wpf.Host}/` — platform heads. Each imports `DexUno.Shared.projitems` and references `DexUno.Core`. Wasm/Gtk/Wpf heads also declare `UpToDateCheckInput` on the shared XAML so XAML edits trigger rebuilds.

## Build

There is no `global.json`; the repo uses whatever `dotnet` SDK is installed. Only the Core library builds cleanly on a current macOS/.NET 10 machine:

```bash
dotnet build DexUno.Core/DexUno.Core.csproj
```

The heads are legacy (netcoreapp3.1 for Gtk/Wpf, netstandard2.0 + `Uno.Wasm.Bootstrap` 2.0-dev for Wasm, classic Xamarin csproj for iOS/Android/macOS, UWP for Windows) and pin Uno `3.2.0` / `3.3.0-dev.99` packages. Known state on this machine (verified): `dotnet build DexUno/DexUno.Skia.Gtk/DexUno.Skia.Gtk.csproj` restores (slowly, ~9 min cold) but fails in `Uno.SourceGenerationTasks` with "Generation failed, error code 150" because the Uno 3.x source-generation host needs a .NET 5 runtime that isn't installed. Expect the same for Wasm/Wpf. iOS/Android/macOS/UWP heads need Visual Studio / Xamarin tooling (see `.vsconfig`). Don't burn time trying to make a head build unless the task is specifically about upgrading the toolchain.

`DexUno.Skia.Wpf.Host` references `..\DexUno.Skia.WPF\DexUno.Skia.WPF.csproj` with different casing than the real folder `DexUno.Skia.Wpf`; this only resolves on case-insensitive filesystems.

## Architecture

**Composition root.** `DexUno.Shared/Startup.cs` builds a plain `Microsoft.Extensions.DependencyInjection` container and exposes it as the static `Startup.ServiceProvider`. `App()` calls `Startup.Init()` before `InitializeComponent()`. Service location via `Startup.ServiceProvider.GetService<T>()` (extension in `ServiceCollectionExtensions`) is used throughout pages, the side pane, and even inside view models, alongside constructor injection.

**Interface + implementation in one file.** Services and repositories keep the interface and its implementation together in the `I*.cs` file (e.g. `INavigationService.cs` holds `NavigationService`, `IPokemonRepository.cs` holds `PokemonRepository`, `IJsonService.cs` holds `JsonService`). `grep` for `class Foo` will land you in `IFoo.cs`. Keep this convention.

**Page → ViewModel resolution is by naming convention.** All pages derive from `PageBase` (`Infrastructure/PageBase.cs`). On `OnNavigatedTo` it computes the VM type name from the page type: `Dex.Uwp.Pages.XyzPage` → `Dex.Uwp.ViewModels.XyzViewModel`, resolves it from the container via `Type.GetType`, calls `vm.OnNavigatedTo(e)`, and sets it as `DataContext`. Consequences:
- A new page needs a same-named `*ViewModel` in `Dex.Uwp.ViewModels`, subclassing `ViewModelBase`, or navigation throws.
- ViewModels are registered automatically: `AddViewModels()` reflects over every non-abstract `ViewModelBase` subclass in the assembly and registers it as transient. Never add them to `Startup` by hand.
- `[Bindable]` on VMs is required for Uno's reflection-free binding on non-UWP heads.

**Shell / navigation.** `Pages/Shell.xaml` is the window root: accent-colored command bar + `SplitView` with `SidePane` (hamburger menu) and a `Frame` (`MainFrame`). `NavigationService` (singleton) grabs that frame from `Window.Current.Content` and exposes one `NavigateToXPage()` method per page; `SidePane` maps menu labels to those methods in a hard-coded dictionary. Adding a page means touching `INavigationService.cs` and `SidePane.xaml.cs` as well.

`PageBase` also exposes `Title`, `RequiresAppBar` and a `Commands` collection of `ButtonBase`; `Shell` listens for `MainFrame.DataContextChanged` and, when the page loads, binds the title and moves the page's `Commands` into the shell's command bar.

**Data flow.** `LocalFileDataSource` (implements all four `I*DataSource` interfaces from Core) reads the bundled `ms-appx:///Data/*.db.json` files via `StorageFile` and deserializes with Newtonsoft (`JsonService`, enums as camel-case strings). Repositories (`PokemonRepository`, `MoveRepository`, `EvolutionsRepository`) lazily load and cache the whole list in memory on first call; they are registered transient, so each resolution re-reads the JSON. `TypesService` holds the type-effectiveness table and the 1.25 / 0.8 multipliers. Pokémon images are resolved by `IPokePicturesSource.GetPath(dexNumber)` → `ms-appx:///Assets/Flat/{n}.png` (surfaced to XAML through `PokemonDexNumberToPictureConverter`).

**Theming.** `App.xaml` merges `Theme/Brushes.xaml`, `Converters.xaml`, `Styles.xaml`, `Templates.xaml`. Converters are declared once in `Converters.xaml` and referenced by `StaticResource`. `ListItemTemplateSelector` / `MoveDetailTemplateSelector` pick `DataTemplate`s from application resources by key based on the item's CLR type (`Pokemon`, `QuickMove`, `ChargeMove`). The accent color is a user setting (`SettingsService`, stored in `ApplicationData.Current.LocalSettings`) and applied at startup by `ApplicationWindowManager`.

**Dual-screen WIP.** `PokedexPage.xaml` and `MovedexPage.xaml` wrap their content in `muxc:TwoPaneView`, with the list in pane 1 and the detail page (`PokemonDetailPage` / `MoveDetailPage`) embedded in pane 2 bound to a child VM the list VM resolves from the container. The Android head references `Uno.UI.DualScreen` and `Xamarin.DuoSdk` for this.

**Platform branches.** Use `#if WINDOWS_UWP` for UWP-only API shapes (see `Shell.xaml.cs`, `RectangularGauge.xaml.cs`, `IApplicationWindowManager.cs`); the Wasm head defines `WASM` in Debug.
