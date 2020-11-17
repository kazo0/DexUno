using DexUno.Shared;
using DexUno.Shared.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Dex.Uwp.Infrastructure
{
    public class CommandsCollection : List<ButtonBase> { }

    public partial class PageBase : Page
    {
        public static DependencyProperty TitleProperty
           = DependencyProperty.Register("Title", typeof(string), typeof(PageBase), new PropertyMetadata("Dex"));

        private ViewModelBase viewModel;

        public CommandsCollection Commands { get; set; }
        public bool RequiresAppBar { get; set; } = true;

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            viewModel = viewModel ?? ResolveViewModelForPage(e);
            viewModel.OnNavigatedTo(e);

            DataContext = viewModel;
            base.OnNavigatedTo(e);
        }

        private ViewModelBase ResolveViewModelForPage(NavigationEventArgs e)
        {
            var fullName = $"{e.SourcePageType.Namespace.Replace(".Pages", "")}.ViewModels.{e.SourcePageType.Name.Replace("Page", "ViewModel")}";
            Type viewModelType = Type.GetType(fullName);
            Startup.ServiceProvider.GetService<ILogger>().LogDebug($"Getting VM: {fullName}");
            return (ViewModelBase)Startup.ServiceProvider.GetService(viewModelType);
        }
    }
}