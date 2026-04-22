namespace InventoryTweaks.Utilities.Extensions;

/// <summary>
///     Provides <see cref="Item"/> extension methods.
/// </summary>
public static class ItemExtensions
{
    /// <summary>
    ///     Checks whether an item has a full stack.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns><see langword="true"/> if the item has a full stack; otherwise, <see langword="false"/>.</returns>
    public static bool IsFull(this Item item)
    {
        return item.stack >= item.maxStack;
    }
}