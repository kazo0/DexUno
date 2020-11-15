using Dex.Uwp.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DexUno.Shared.Helpers
{
	public static class ServiceCollectionExtensions
	{
		public static void AddViewModels(this IServiceCollection services)
		{
			services.AddAllSubclassesOf<ViewModelBase>(typeof(ViewModelBase).Assembly);
		}


		public static void AddAllSubclassesOf<T>(
			this IServiceCollection services,
			Assembly assembly,
			ServiceLifetime lifetime = ServiceLifetime.Transient)
		{
			var types = assembly
				.DefinedTypes
				.Where(t => t.IsSubclassOf(typeof(T)) &&
							!t.IsAbstract);

			foreach (var type in types)
			{
				services.Add(new ServiceDescriptor(type.AsType(), type.AsType(), lifetime));
			}
		}

		public static T GetService<T>(this IServiceProvider provider) where T : class
		{
			return provider.GetService(typeof(T)) as T;
		}
	}
}
