namespace InventoryTweaks;

/// <summary>
///     Represents the entry point for the Inventory Tweaks mod.
/// </summary>
public sealed partial class InventoryTweaks : Mod
{
    /// <summary>
    ///     Gets the singleton instance of <see cref="InventoryTweaks" />.
    /// </summary>
    /// <remarks>
    ///     This property is a shorthand for <see cref="ModContent.GetInstance{T}"/>.
    /// </remarks>
    public static InventoryTweaks Instance => ModContent.GetInstance<InventoryTweaks>();
}