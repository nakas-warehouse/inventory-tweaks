namespace InventoryTweaks;

/// <summary>
///     The <see cref="Mod" /> implementation of Inventory Tweaks.
/// </summary>
public sealed class InventoryTweaks : Mod
{
    /// <summary>
    ///     Gets the singleton instance of <see cref="InventoryTweaks" />. Shorthand for
    ///     <c>ModContent.GetInstance&lt;InventoryTweaks&gt;()</c>.
    /// </summary>
    public static InventoryTweaks Instance => ModContent.GetInstance<InventoryTweaks>();
}