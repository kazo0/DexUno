using Dex.Uwp.Infrastructure;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Dex.Uwp.Pages
{
    public sealed partial class Shell : Page
    {
        private PageBase currentPage;

        public Shell()
        {
            InitializeComponent();
			this.MainFrame.DataContextChanged += MainFrame_CurrentPageChanged;
        }


		public new Frame Frame => MainFrame;

        private void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Set Appbar visibility
            MainCommandBar.Visibility = currentPage.RequiresAppBar ?
                Visibility.Visible : Visibility.Collapsed;

            //Set title binding
            var binding = new Binding() { Path = new PropertyPath("Title"), Source = currentPage };
            PageTitle.SetBinding(TextBlock.TextProperty, binding);

            //Set Application Bar Buttons
            commandsBar.Items.Clear();
            var commands = currentPage.Commands ?? new CommandsCollection();
            foreach (var item in commands)
            {
                commandsBar.Items.Add(item);
                item.DataContext = currentPage.DataContext;
            }

            currentPage.Loaded -= CurrentPage_Loaded;
        }

        private void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (RootSplitView.DisplayMode == SplitViewDisplayMode.CompactInline || RootSplitView.DisplayMode == SplitViewDisplayMode.Inline)
            {
                return;
            }

            RootSplitView.IsPaneOpen = false;
        }
#if WINDOWS_UWP
        private void MainFrame_CurrentPageChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            currentPage = args.NewValue as PageBase;
            if (currentPage == null)
                return;
            currentPage.Loaded += CurrentPage_Loaded;
        }
#else

        private void MainFrame_CurrentPageChanged(DependencyObject sender, DataContextChangedEventArgs args)
        {
            currentPage = args.NewValue as PageBase;
            if (currentPage == null)
                return;
            currentPage.Loaded += CurrentPage_Loaded;
        }
#endif
    }
}