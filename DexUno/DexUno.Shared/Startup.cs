using Dex.Core.DataAccess;
using Dex.Core.Repositories;
using Dex.Core.Services;
using Dex.Uwp;
using Dex.Uwp.DataAccess;
using Dex.Uwp.Services;
using DexUno.Shared.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DexUno.Shared
{
    public static class Startup
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static void Init()
        {
            var sc = new ServiceCollection();
            ConfigureServices(sc);


            ServiceProvider = sc.BuildServiceProvider();
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddViewModels();

            services.AddTransient<IPokemonRepository, PokemonRepository>();
            services.AddTransient<IMoveRepository, MoveRepository>();
            services.AddTransient<IEvolutionsRepository, EvolutionsRepository>();
            services.AddTransient<IPokePicturesSource, DefaultPokePicturesSource>();
       
            services.AddTransient<IJsonService, JsonService>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<ITypesService, TypesService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddTransient<IAppLifecycleManager, AppLifecycleManager>();
            services.AddTransient<IPokePicturesSourceProvider, PokePicturesSourceProvider>();

            services.AddTransient<LocalFileDataSource>()
                .AddTransient<IPokemonsDataSource, LocalFileDataSource>()
                .AddTransient<IMovesDataSource, LocalFileDataSource>()
                .AddTransient<ITypeEffectivenessDataSource, LocalFileDataSource>()
                .AddTransient<IEvolutionsDataSource, LocalFileDataSource>();
            services.AddTransient<Microsoft.Extensions.Logging.ILogger>((sp) => global::Uno.Extensions.LogExtensionPoint.Log(typeof(App)));


        }
    }
}
