using Terraria.ModLoader.Config;

namespace InventoryTweaks.Common.Configuration;

/// <summary>
///     The client-side <see cref="ModConfig"/> implementation of Inventory Tweaks.
/// </summary>
public sealed partial class ClientConfiguration : ModConfig
{
    /// <summary>
    ///     Gets the <see cref="ClientConfiguration" /> instance. Shorthand for <c>ModContent.GetInstance&lt;ClientConfiguration&gt;()</c>.
    /// </summary>
    public static ClientConfiguration Instance => ModContent.GetInstance<ClientConfiguration>();

    public override ConfigScope Mode { get; } = ConfigScope.ClientSide;
}