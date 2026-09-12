namespace DexUno.Modern.Presentation;

public partial record TypesModel(ITypesService Types)
{
    public string Title => "Type chart";

    public IListFeed<TypeEffectiveness> Items => ListFeed.Async<TypeEffectiveness>(async ct => (await Types.GetAllEffectivenessAsync(ct)).ToImmutableList());
}
