using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Dex.Uwp.Pages;
using Dex.Uwp.Services;
using DexUno.Shared.Helpers;
using DexUno.Shared;
using Microsoft.Extensions.Logging;
using System;

namespace Dex.Uwp
{
    sealed partial class App : Application
    {
        private IAppLifecycleManager _lifecycleManager;
        private INavigationService _navigationService;
        private Shell _rootShell;
        private ILogger _log;
        private ISettingsService _settingsService;

        public App()
        {
            ConfigureFilters(global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);
            Startup.Init();
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (_rootShell == null)
                await InitializeAppAsync(e);

            if (e.PrelaunchActivated)
                return;

            _rootShell = Window.Current.Content as Shell;

            if (_rootShell.Frame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter

                _navigationService = Startup.ServiceProvider.GetService<INavigationService>();
                _navigationService.NavigateToPokedexPage();
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        private async Task InitializeAppAsync(LaunchActivatedEventArgs e)
        {
            _log = Startup.ServiceProvider.GetService<ILogger>();
            _lifecycleManager = Startup.ServiceProvider.GetService<IAppLifecycleManager>();

            if (await _lifecycleManager.IsFirstRun())
            {
                await _lifecycleManager.InitializeAppForFirstRun();
            }

            InitializeShell(e);
        }

        private void InitializeShell(LaunchActivatedEventArgs e)
        {
            _settingsService = Startup.ServiceProvider.GetService<ISettingsService>();
            var appManager = new ApplicationWindowManager();
            appManager.SetAccentColor(_settingsService.AccentColor);
            appManager.InitializeWindow();

            _rootShell = new Shell();

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                LoadPreviousState();
            }

            Window.Current.Content = _rootShell;

            _log.LogDebug("[Startup] Application Shell has been initialized.");
        }

        private void LoadPreviousState()
        {
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            _log.LogDebug("[Suspending] Application is being suspended.");
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }


        private static void ConfigureFilters(ILoggerFactory factory)
        {
            factory
                .WithFilter(new FilterLoggerSettings
                    {
                        { "Uno", LogLevel.Warning },
                        { "Windows", LogLevel.Warning },

						// Debug JS interop
						// { "Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug },

						// Generic Xaml events
						// { "Windows.UI.Xaml", LogLevel.Debug },
						// { "Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug },
						// { "Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.UIElement", LogLevel.Debug },

						// Layouter specific messages
						// { "Windows.UI.Xaml.Controls", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Panel", LogLevel.Debug },
						// { "Windows.Storage", LogLevel.Debug },

						// Binding related messages
						// { "Windows.UI.Xaml.Data", LogLevel.Debug },

						// DependencyObject memory references tracking
						// { "ReferenceHolder", LogLevel.Debug },

						// ListView-related messages
						// { "Windows.UI.Xaml.Controls.ListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.GridView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelLayout", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.NativeListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListViewBaseSource", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.ListViewBaseInternalContainer", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.NativeListViewBaseAdapter", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.BufferViewCache", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelGenerator", LogLevel.Debug }, //WASM
					}
                )
#if DEBUG
                .AddConsole(LogLevel.Debug);
#else
                .AddConsole(LogLevel.Information);
#endif
        }
    }
}