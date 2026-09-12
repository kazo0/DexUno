namespace DexUno.Modern.Presentation;

/// <summary>
/// Navigation payload for the move detail page. Moves are polymorphic (<see cref="QuickMove"/> / <see cref="ChargeMove"/>),
/// so a sealed wrapper keeps the declared and runtime data types identical for route resolution and constructor injection.
/// </summary>
public sealed record MoveSelection(Move Move);
