using Dex.Core.Entities;
using Dex.Uwp.Infrastructure;
using Dex.Uwp.ViewModels;

namespace Dex.Uwp.Pages
{
    public sealed partial class MovedexPage : PageBase
    {
        private MovedexViewModel vm;

        public MovedexPage()
        {
            this.InitializeComponent();
            DataContextChanged += (a, b) =>
            {
                vm = b.NewValue as MovedexViewModel;
            };
        }
    }
}