using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Common.Contexts;
using InventoryTweaks.Utilities;
using MonoMod.Cil;
using Terraria.UI;

namespace InventoryTweaks.Common.Tweaks;

public sealed class QuickPressSystem : ModSystem
{
    /// <summary>
    ///     Gets the index of the slot that was last sold or trashed.
    /// </summary>
    public static int Index { get; private set; } = -1;
    
    public override void Load()
    {
        base.Load();
        
        On_ItemSlot.LeftClick_SellOrTrash += Hook;
        IL_ItemSlot.LeftClick_ItemArray_int_int += Edit;
    }

    private static bool Hook(On_ItemSlot.orig_LeftClick_SellOrTrash orig, Item[] inv, int context, int slot)
    {
        var result = orig(inv, context, slot);

        if (result)
        {
            Index = slot;
        }

        return result;
    }
    
    private static void Edit(ILContext context)
    {
        var cursor = new ILCursor(context);
        
        if (!cursor.TryGotoNext(MoveType.Before, static i => i.MatchStloc1()))
        {
            throw new Exception();
        }
        
        cursor.Index++;
        
        cursor.EmitLdarg1();
        cursor.EmitLdarg2();

        cursor.EmitLdloca(1);

        cursor.EmitDelegate(Evaluate);
    }
    
    private static void Evaluate(int context, int slot, ref bool value)
    {
        value |= ContextSystem.CheckPlayerContext(context) && Main.mouseLeft && ClientSideConfiguration.Instance.EnableQuickShift && ItemSlot.ShiftInUse && CursorUtilities.Override;
        value |= ContextSystem.CheckPlayerContext(context) && Main.mouseLeft && ClientSideConfiguration.Instance.EnableQuickControl && ItemSlot.ControlInUse && CursorUtilities.Override && slot != Index;
    }
}