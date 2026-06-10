using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Utilities;
using MagicStorage;
using MonoMod.Cil;
using Terraria.UI;

namespace InventoryTweaks.Common.Tweaks;

/// <summary>
///     Handles caching the last item slot the player used to trash an <see cref="Item"/> to prevent endless trashing.
/// </summary>
public sealed class ItemTrashActionSystem : ModSystem
{
    /// <summary>
    ///     Gets the index of the last item slot that the player used to trash an item.
    /// </summary>
    /// <value>
    ///     Defaults to <c>-1</c>.
    /// </value>
    public static int Slot { get; private set; } = -1;
    
    public override void Load()
    {
        base.Load();
        
        On_ItemSlot.LeftClick_SellOrTrash += ItemSlot_LeftClick_SellOrTrash_Hook;
    }
    
    private static bool ItemSlot_LeftClick_SellOrTrash_Hook(On_ItemSlot.orig_LeftClick_SellOrTrash orig, Item[] inv, int context, int slot)
    {
        var result = orig(inv, context, slot);

        if (result)
        {
            Slot = slot;
        }

        return result;
    }
}

/// <summary>
///     Handles caching the last item slot the player used to trash an <see cref="Item"/> to prevent endless equipment swapping.
/// </summary>
public sealed class ItemEquipActionSystem : ModSystem
{
    /// <summary>
    ///     Gets the index of the last item slot that the player used to equip an item.
    /// </summary>
    /// <value>
    ///     Defaults to <c>-1</c>.
    /// </value>
    public static int Slot { get; private set; } = -1;

    public override void Load()
    {
        base.Load();
        
        On_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick_Hook;
    }
    
    private static void ItemSlot_RightClick_Hook(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);

        if (!ClientSideConfiguration.Instance.EnableQuickShift)
        {
            return;
        }

        Slot = slot;
    }
}

public sealed class ItemActionSystem : ILoadable
{
    void ILoadable.Load(Mod mod)
    {

        IL_ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick_Edit;
        IL_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick_Edit;
    }

    void ILoadable.Unload() { }

    /// <summary>
    ///     Checks whether the player can quick-shift a given item slot from the Magic Storage mod.
    /// </summary>
    /// <param name="inv">The inventory of the item slot.</param>
    /// <param name="context">The context of the item slot.</param>
    /// <param name="slot">The index of the item slot.</param>
    /// <returns>
    ///     <see langword="true" /> if the player can quick-shift in the given item slot; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    [JITWhenModsEnabled("MagicStorage")]
    public static bool CanQuickShiftMagicStorage(Item[] inv, int context, int slot)
    {
        return ItemSlotUtilities.IsInventoryContext(context) 
               && Main.mouseLeft 
               && ClientSideConfiguration.Instance.EnableQuickShift
               && ItemSlot.ShiftInUse 
               && IsStorageOpen(inv, context, slot);
    }

    /// <summary>
    ///     Checks whether the player can quick-shift a given item slot.
    /// </summary>
    /// <param name="context">The context of the item slot.</param>
    /// <returns>
    ///     <see langword="true" /> if the player can quick-shift in the given item slot; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    public static bool CanQuickShift(int context)
    {
        return ItemSlotUtilities.IsInventoryContext(context) 
               && Main.mouseLeft 
               && ClientSideConfiguration.Instance.EnableQuickShift
               && ItemSlot.ShiftInUse 
               && InputUtilities.HasCursorOverride;
    }

    /// <summary>
    ///     Checks whether the player can quick-control on a given item slot.
    /// </summary>
    /// <param name="context">The context of the item slot.</param>
    /// <param name="slot">The index of the item slot.</param>
    /// <returns>
    ///     <see langword="true" /> if the player can quick-control in the given item slot; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    public static bool CanQuickControl(int context, int slot)
    {
        return ItemSlotUtilities.IsInventoryContext(context) 
               && Main.mouseLeft
               && ClientSideConfiguration.Instance.EnableQuickControl
               && ItemSlot.ControlInUse
               && slot != ItemTrashActionSystem.Slot
               && InputUtilities.HasCursorOverride;
    }

    private static void ItemSlot_LeftClick_Edit(ILContext context)
    {
        try
        {
            var cursor = new ILCursor(context);

            if (!cursor.TryGotoNext(MoveType.Before, static i => i.MatchStloc1()))
            {
#if DEBUG
                throw new Exception();
#else
                return;
#endif
            }

            cursor.Index++;

            cursor.EmitLdarg0();
            cursor.EmitLdarg1();
            cursor.EmitLdarg2();

            cursor.EmitLdloca(1);

            cursor.EmitDelegate
            (
                static (Item[] inv, int context, int slot, ref bool value) =>
                {
                    value |= CanQuickShift(context);
                    value |= CanQuickControl(context, slot);

                    if (!ModLoader.HasMod("MagicStorage"))
                    {
                        return;
                    }

                    value |= CanQuickShiftMagicStorage(inv, context, slot);
                }
            );
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(InventoryTweaks.Instance, context);
        }
    }

    private static void ItemSlot_RightClick_Edit(ILContext context)
    {
        try
        {
            var cursor = new ILCursor(context);

            while (cursor.TryGotoNext(MoveType.After, static i => i.MatchLdsfld<Main>(nameof(Main.mouseRightRelease))))
            {
                cursor.EmitLdarg1();
                cursor.EmitLdarg2();

                cursor.EmitDelegate
                (
                    static (bool value, int context, int slot) =>
                    {
                        return !ItemDistributionManager.Inserting && ItemSlotUtilities.IsInventoryContext(context) && (slot != ItemEquipActionSystem.Slot || Main.mouseRightRelease);
                    }
                );
            }
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(InventoryTweaks.Instance, context);
        }
    }

    [JITWhenModsEnabled("MagicStorage")]
    private static bool IsStorageOpen(Item[] inv, int context, int slot)
    {
        var player = Main.LocalPlayer;

        if (!player.TryGetModPlayer(out StoragePlayer storagePlayer))
        {
            return false;
        }

        var hasContext = context == ItemSlot.Context.InventoryItem || context == ItemSlot.Context.InventoryCoin || context == ItemSlot.Context.InventoryAmmo;

        var item = inv[slot];

        var hasItem = item is
        {
            favorited: false, IsAir: false
        };

        var hasStorage = storagePlayer.ViewingStorage() is
        {
            X: > 0, Y: > 0
        };

        return hasContext && hasItem && hasStorage;
    }
}