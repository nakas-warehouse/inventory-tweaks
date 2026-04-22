using System.ComponentModel;
using System.Text.Json.Serialization;
using InventoryTweaks.Core.Enums;
using Terraria.ModLoader.Config;

namespace InventoryTweaks.Common.Configuration;

/// <summary>
///     The client-side <see cref="ModConfig" /> implementation of Inventory Tweaks.
/// </summary>
public sealed class ClientConfiguration : ModConfig
{
    /// <summary>
    ///     Gets the <see cref="ClientConfiguration" /> instance. Shorthand for
    ///     <c>ModContent.GetInstance&lt;ClientConfiguration&gt;()</c>.
    /// </summary>
    public static ClientConfiguration Instance => ModContent.GetInstance<ClientConfiguration>();

    public override ConfigScope Mode { get; } = ConfigScope.ClientSide;

    #region Audio

    [Header("Audio")]
    [DefaultValue(true)]
    public bool EnableInventorySounds { get; set; } = true;

    #endregion

    #region Graphics

    [JsonIgnore]
    public bool EnableEffects => EnableHoverEffects || EnableMouseEffects || EnableSelectedEffects;

    [Header("Graphics")]
    [DefaultValue(true)]
    public bool EnableMovementEffects { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableHoverEffects { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableMouseEffects { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableSelectedEffects { get; set; } = true;

    [Increment(0.05f)]
    [Range(0.8f, 2f)]
    [DefaultValue(1.2f)]
    public float HoveredItemScale { get; set; } = 1.2f;

    [Increment(0.05f)]
    [Range(0.4f, 1f)]
    [DefaultValue(0.8f)]
    public float UnhoveredItemScale { get; set; } = 0.8f;

    #endregion

    #region Input

    [Header("Input")]
    [DefaultValue(true)]
    public bool EnableQuickShift { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableQuickControl { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableStackRefill { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableMouseRefill { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableDistribution { get; set; } = true;

    [DefaultValue(typeof(StackType), nameof(StackType.Single))]
    public StackType StackType { get; set; } = StackType.Single;

    [DefaultValue(typeof(SortType), nameof(SortType.Ascending))]
    public SortType SortType { get; set; } = SortType.Ascending;

    #endregion
}