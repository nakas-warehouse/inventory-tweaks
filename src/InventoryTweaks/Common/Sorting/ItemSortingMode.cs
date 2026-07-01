namespace InventoryTweaks.Common.Sorting;

public enum ItemSortingMode : byte
{
    /// <summary>
    ///     Sorts items by vanilla sorting rules.
    /// </summary>
    Default,
    
    /// <summary>
    ///     Sorts items by their sell value.
    /// </summary>
    Value,
    
    /// <summary>
    ///     Sorts items by their numerical type, grouping identical items together.
    /// </summary>
    Type,
    
    /// <summary>
    ///     Sorts items by their damage.
    /// </summary>
    Damage,
    
    /// <summary>
    ///     Sorts items by their rarity.
    /// </summary>
    Rarity
}