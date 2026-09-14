using System.Diagnostics.CodeAnalysis;
using Uno.Resizetizer;

namespace Dexter;

public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }

    protected IHost? Host { get; private set; }

    // UseNavigation / NavigateAsync below are annotated RequiresUnreferencedCode by
    // Uno.Extensions Navigation itself, which raises IL2026 now that the head sets
    // IsAotCompatible. The annotation cannot be propagated to this method instead:
    // Application.OnLaunched is not annotated, and marking an override that its base
    // does not mark is IL2046. Suppressing is safe here because the types Navigation
    // resolves reflectively - the pages, the models and their generated view-models -
    // are all rooted at head compile time by Uno's BindableTypeProvidersSourceGenerator.
    // SuppressMessage would not do: it is [Conditional("CODE_ANALYSIS")], so ILLink and
    // ILCompiler never see it at publish time; UnconditionalSuppressMessage is kept.
    [UnconditionalSuppressMessage("Trimming", "IL2026",
        Justification = "Navigation's reflected types are rooted by Uno's BindableTypeProvidersSourceGenerator.")]
    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)
                        .CoreLogLevel(LogLevel.Warning);
                }, enableUnoLogging: true)
                .ConfigureServices((context, services) =>
                {
                    // Pokedex data: the bundled JSON is parsed once and cached for the lifetime of the app.
                    services.AddSingleton<IPokedexDataSource, EmbeddedJsonPokedexDataSource>();
                    services.AddSingleton<IPokemonRepository, PokemonRepository>();
                    services.AddSingleton<IMoveRepository, MoveRepository>();
                    services.AddSingleton<IEvolutionsRepository, EvolutionsRepository>();
                    services.AddSingleton<ITypesService, TypesService>();
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainModel>(),
            new ViewMap<PokedexPage, PokedexModel>(),
            new ViewMap<MovedexPage, MovedexModel>(),
            new ViewMap<TypesPage, TypesModel>(),
            new ViewMap<SearchPage, SearchModel>(),
            new ViewMap<AboutPage, AboutModel>(),
            new DataViewMap<PokemonDetailPage, PokemonDetailModel, Pokemon>(),
            new DataViewMap<MoveDetailPage, MoveDetailModel, MoveSelection>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    // The responsive shell: its sections are visibility regions driven by NavigationView / TabBar.
                    new RouteMap("Main", View: views.FindByViewModel<MainModel>(), IsDefault: true,
                        Nested:
                        [
                            new RouteMap("Pokedex", View: views.FindByViewModel<PokedexModel>(), IsDefault: true),
                            new RouteMap("Movedex", View: views.FindByViewModel<MovedexModel>()),
                            new RouteMap("Types", View: views.FindByViewModel<TypesModel>()),
                            new RouteMap("Search", View: views.FindByViewModel<SearchModel>()),
                            new RouteMap("About", View: views.FindByViewModel<AboutModel>()),
                        ]),
                    // Detail pages are pushed on the root frame so they cover the shell and support back navigation.
                    new RouteMap("PokemonDetail", View: views.FindByViewModel<PokemonDetailModel>()),
                    new RouteMap("MoveDetail", View: views.FindByViewModel<MoveDetailModel>()),
                ]
            )
        );
    }
}
