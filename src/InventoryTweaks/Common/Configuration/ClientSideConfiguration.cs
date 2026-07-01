using System.ComponentModel;
using InventoryTweaks.Common.Pickups;
using InventoryTweaks.Common.Sorting;
using Terraria.ModLoader.Config;

namespace InventoryTweaks.Common.Configuration;

public sealed class ClientSideConfiguration : ModConfig
{
    public static ClientSideConfiguration Instance => ModContent.GetInstance<ClientSideConfiguration>();

    public override ConfigScope Mode { get; } = ConfigScope.ClientSide;

    [Header("Pickups")]
    [DefaultValue(typeof(ItemPickupMode), nameof(ItemPickupMode.Single))]
    public ItemPickupMode PickupMode { get; set; } = ItemPickupMode.Single;
    
    [Header("Sorting")]
    [DefaultValue(typeof(ItemSortingMode), nameof(ItemSortingMode.Default))]
    public ItemSortingMode SortingMode { get; set; } = ItemSortingMode.Default;
    
    [DefaultValue(typeof(ItemSortingDirection), nameof(ItemSortingDirection.Ascending))]
    public ItemSortingDirection SortingDirection { get; set; } = ItemSortingDirection.Ascending;
    
    /// <summary>
    ///     Gets or sets a value indicating whether to enable automatic item refills.
    /// </summary>
    [Header("Refills")]
    [DefaultValue(true)]
    public bool EnableRefill { get; set; } = true;
    
    /// <summary>
    ///     Gets or sets a value indicating whether to enable item distributions.
    /// </summary>
    [Header("Distribution")]
    [DefaultValue(true)]
    public bool EnableDistribution { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether to enable quick shift features.
    /// </summary>
    [Header("Tweaks")]
    [DefaultValue(true)]
    public bool EnableQuickShift { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether to enable quick control features.
    /// </summary>
    [DefaultValue(true)]
    public bool EnableQuickControl { get; set; } = true;
    
    /// <summary>
    ///     Gets or sets a value indicating whether to enable sounds for automatic item refills.
    /// </summary>
    [Header("Audio")]
    [DefaultValue(true)]
    public bool EnableRefillSounds { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether to enable sounds for user interface interactions.
    /// </summary>
    [DefaultValue(true)]
    public bool EnableInterfaceSounds { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether to enable highlights for user interface elements.
    /// </summary>
    [Header("Interface")]
    [DefaultValue(true)]
    public bool EnableInterfaceHighlights { get; set; } = true;
    
    /// <summary>
    ///     Gets or sets a value indicating whether to enable animations for user interface elements.
    /// </summary>
    [DefaultValue(true)]
    public bool EnableInterfaceAnimations { get; set; } = true;
    
    /// <summary>
    ///     Gets or sets a value indicating whether to enable animations for inventory interactions.
    /// </summary>
    [Header("Inventory")]
    [DefaultValue(true)]
    public bool EnableInventoryAnimations { get; set; } = true;
    
    /// <summary>
    ///     Gets or sets the scale of items when hovered over in the inventory.
    /// </summary>
    [DefaultValue(1.1f)]
    public float ActiveItemScale { get; set; } = 1.1f;
    
    /// <summary>
    ///     Gets or sets the scale of items when not hovered over in the inventory.
    /// </summary>
    [DefaultValue(1f)]
    public float InactiveItemScale { get; set; } = 1f;

    /// <summary>
    ///     Gets or sets the speed of inventory animations.
    /// </summary>
    [Range(0f, 1f)]
    [Increment(0.05f)]
    [DefaultValue(0.35f)]
    public float InventoryAnimationSpeed { get; set; } = 0.35f;
    
    [DefaultValue(typeof(SpriteEffects), nameof(SpriteEffects.None))]
    public SpriteEffects InventoryItemEffects { get; set; } = SpriteEffects.None;
}