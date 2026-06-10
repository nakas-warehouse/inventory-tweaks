using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Utilities.Extensions;
using JetBrains.Annotations;
using Terraria.Audio;
using Terraria.GameInput;

namespace InventoryTweaks.Common.Tweaks;

public delegate void ItemRefillCallback(in ItemRefillContext context);

public ref struct ItemRefillContext
{
    /// <summary>
    ///     Gets the <see cref="Item" /> that was refilled.
    /// </summary>
    public Item Item { get; }

    /// <summary>
    ///     Gets the <see cref="Item" /> that was used to refill the source <see cref="Item" />.
    /// </summary>
    public Item Source { get; }

    /// <summary>
    ///     Gets the amount of items that were transferred from the source <see cref="Item" /> to the item
    ///     being refilled.
    /// </summary>
    public int Transferred { get; }

    public ItemRefillContext(Item item, Item source, int transferred)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(transferred, nameof(transferred));

        Item = item;
        Source = source;
        Transferred = transferred;
    }
}

public static class ItemRefillUtilities
{
    /// <summary>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="inventory"></param>
    /// <param name="length"></param>
    /// <param name="callback"></param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="item" /> or <paramref name="inventory" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="length" /> is negative or zero.
    /// </exception>
    public static void Refill(Item item, Item[] inventory, int length, [CanBeNull] ItemRefillCallback callback = null)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        ArgumentNullException.ThrowIfNull(inventory, nameof(inventory));

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length, nameof(length));

        var type = item.type;

        for (var i = 0; i < length; i++)
        {
            var other = inventory[i];

            if (other == item || other.IsAir || other.type != type)
            {
                continue;
            }

            var difference = item.maxStack - item.stack;

            if (difference <= 0)
            {
                break;
            }

            var stack = Math.Min(other.stack, difference);

            if (stack <= 0)
            {
                continue;
            }

            other.stack -= stack;
            item.stack += stack;

            var context = new ItemRefillContext(item, other, stack);

            callback?.Invoke(in context);
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="player"></param>
    /// <param name="callback"></param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="item" /> or <paramref name="player" /> is <see langword="null" />.
    /// </exception>
    public static void Refill(Item item, Player player, [CanBeNull] ItemRefillCallback callback = null)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        ArgumentNullException.ThrowIfNull(player, nameof(player));

        var chest = player.TryGetContainer(out var container);

        var inventory = chest ? container : player.inventory;
        var length = chest ? container.Length : Inventory.Length;

        Refill(item, inventory, length, callback);
    }
}

public sealed class SelectedItemRefillGlobalItem : GlobalItem
{
    public override void OnConsumeItem(Item item, Player player)
    {
        base.OnConsumeItem(item, player);

        if (!ClientSideConfiguration.Instance.EnableStackRefill)
        {
            return;
        }

        var refill = item == player.HeldItem && item.stack - 1 <= 0;

        if (!refill)
        {
            return;
        }

        ItemRefillUtilities.Refill(player.HeldItem, player);
        
        if (!ClientSideConfiguration.Instance.EnableInventorySounds)
        {
            return;
        }

        SoundEngine.PlaySound(in SoundID.MenuTick);
    }
}

public sealed class MouseItemRefillSystem : ModSystem
{
    /// <summary>
    ///     Gets the keybind for mouse item refill.
    /// </summary>
    [CanBeNull]
    public static ModKeybind Keybind { get; private set; }

    public override void Load()
    {
        base.Load();

        Keybind = KeybindLoader.RegisterKeybind(Mod, nameof(Keybind), "Mouse3");
    }

    public override void Unload()
    {
        base.Unload();

        Keybind = null;
    }
}

public sealed class MouseItemRefillPlayer : ModPlayer
{
    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        base.ProcessTriggers(triggersSet);

        if (!MouseItemRefillSystem.Keybind.JustPressed || !ClientSideConfiguration.Instance.EnableMouseRefill)
        {
            return;
        }

        Refill();
    }

    private void Refill()
    {
        if (Main.mouseItem.IsAir || Main.mouseItem.IsFull())
        {
            return;
        }

        ItemRefillUtilities.Refill(Main.mouseItem, Player);
        
        if (!ClientSideConfiguration.Instance.EnableInventorySounds)
        {
            return;
        }

        SoundEngine.PlaySound(in SoundID.MenuTick);
    }
}