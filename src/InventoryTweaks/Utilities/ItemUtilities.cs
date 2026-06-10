namespace InventoryTweaks.Utilities;

/// <summary>
///     Provides <see cref="Item"/> extensions.
/// </summary>
public static class ItemExtensions
{
    extension(Item item)
    {
        /// <summary>
        ///     Gets a value indicating whether the item is at its maximum stack size.
        /// </summary>
        public bool Full => item.stack >= item.maxStack;
    }
}