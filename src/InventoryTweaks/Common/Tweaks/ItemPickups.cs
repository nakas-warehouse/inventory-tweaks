using System.Collections.Generic;
using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Core.Enums;
using MonoMod.Cil;
using Terraria.UI;

namespace InventoryTweaks.Common.Tweaks;

public delegate void ItemPickupCallback(ref ItemPickupContext context);

public ref struct ItemPickupContext
{
    /// <summary>
    ///     Gets or sets the stack of the context.
    /// </summary>
    public int Stack { get; set; }

    /// <summary>
    ///     Gets the index of the item within <see cref="Inventory" />.
    /// </summary>
    public int Slot { get; }

    /// <summary>
    ///     Gets the inventory containing the item.
    /// </summary>
    public Item[] Inventory { get; }

    /// <summary>
    ///     Gets the item at the specified <see cref="Slot" />. Shorthand for
    /// </summary>
    public Item Item => Inventory[Slot];

    public ItemPickupContext(int stack, int slot, Item[] inventory)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(stack, nameof(stack));
        ArgumentOutOfRangeException.ThrowIfNegative(slot, nameof(slot));

        ArgumentNullException.ThrowIfNull(inventory, nameof(inventory));

        Stack = stack;
        Slot = slot;
        Inventory = inventory;
    }
}

public sealed class ItemPickupHooks : ModSystem
{
    private static List<ItemPickupCallback> Callbacks { get; set; } = new();

    public override void Unload()
    {
        base.Unload();

        Callbacks?.Clear();
        Callbacks = null;
    }

    /// <summary>
    ///     Subscribes the specified callback to the item pickup event.
    /// </summary>
    /// <param name="callback">The callback to invoke when an item is picked up.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="callback" /> is <see langword="null" />.
    /// </exception>
    public static void Subscribe(ItemPickupCallback callback)
    {
        ArgumentNullException.ThrowIfNull(callback, nameof(callback));

        Callbacks.Add(callback);
    }

    /// <summary>
    ///     Unsubscribes the specified callback from the item pickup event.
    /// </summary>
    /// <param name="callback">The callback to remove.</param>
    /// <returns><see langword="true"/> if the callback was successfully unsubscribed; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="callback" /> is <see langword="null" />.
    /// </exception>
    public static bool Unsubscribe(ItemPickupCallback callback)
    {
        ArgumentNullException.ThrowIfNull(callback, nameof(callback));

        return Callbacks.Remove(callback);
    }

    internal static void Invoke(ref ItemPickupContext context)
    {
        foreach (var callback in Callbacks)
        {
            callback.Invoke(ref context);
        }
    }
}

public sealed class ItemPickupSystem : ModSystem
{
    public override void Load()
    {
        base.Load();

        IL_ItemSlot.PickupItemIntoMouse += ItemSlot_PickupItemIntoMouse_Edit;
    }

    private static void ItemSlot_PickupItemIntoMouse_Edit(ILContext context)
    {
        try
        {
            var cursor = new ILCursor(context);

            if (!cursor.TryGotoNext(MoveType.After, static i => i.MatchLdcI4(1)))
            {
#if DEBUG
                throw new Exception($"Failed to match {nameof(ItemPickupSystem)} instructions.");
#else
                return;
#endif
            }

            cursor.EmitLdarg0();
            cursor.EmitLdarg2();

            cursor.EmitDelegate(Stack);
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(InventoryTweaks.Instance, context);
        }
    }

    private static int Stack(int stack, Item[] inventory, int slot)
    {
        var context = new ItemPickupContext(stack, slot, inventory);

        switch (ClientSideConfiguration.Instance.StackType)
        {
            case StackType.Full:
                context.Stack = context.Item.stack;

                break;
            case StackType.Half:
                context.Stack = context.Item.stack / 2;

                break;
            case StackType.Single:
                context.Stack = stack;

                break;
            default:
                context.Stack = stack;

                break;
        }

        ItemPickupHooks.Invoke(ref context);

        return Math.Max(context.Stack, 1);
    }
}