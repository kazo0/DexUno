using Dex.Core.Entities;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dex.Uwp.Theme
{
    public class MoveDetailTemplateSelector : DataTemplateSelector
    {
        private Dictionary<System.Type, object> TypeToTemplateMapper;

        public MoveDetailTemplateSelector()
        {
            TypeToTemplateMapper = new Dictionary<System.Type, object>()
            {
                [typeof(QuickMove)] = GetTemplateFromResourceKey("QuickMoveDetailsDataTemplate"),
                [typeof(ChargeMove)] = GetTemplateFromResourceKey("ChargeMoveDetailsDataTemplate"),
                [typeof(Pokemon)] = GetTemplateFromResourceKey("PokemonListItemTemplate")
            };
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            //When content still isn't bound, this is called with item set to null
            if (item != null && TypeToTemplateMapper.ContainsKey(item.GetType()))
                return (DataTemplate)TypeToTemplateMapper[item.GetType()];

            return base.SelectTemplateCore(item);
        }

        private object GetTemplateFromResourceKey(string resourceKey)
        {
            return Application.Current.Resources[resourceKey];
        }
    }
}