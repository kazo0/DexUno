namespace DexUno.Modern.Converters;

/// <summary>Picks the list template for mixed search results (Pokémon and moves).</summary>
public sealed class SearchResultTemplateSelector : DataTemplateSelector
{
    public DataTemplate? PokemonTemplate { get; set; }

    public DataTemplate? MoveTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item) => item switch
    {
        Pokemon => PokemonTemplate,
        Move => MoveTemplate,
        _ => base.SelectTemplateCore(item),
    };

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container) =>
        SelectTemplateCore(item);
}
