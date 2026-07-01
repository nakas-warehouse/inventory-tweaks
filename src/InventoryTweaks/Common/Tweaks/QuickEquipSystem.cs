using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Common.Contexts;
using MonoMod.Cil;
using Terraria.UI;

namespace InventoryTweaks.Common.Tweaks;

public sealed class QuickEquipSystem : ModSystem
{
    /// <summary>
    ///     Gets the index of the slot that was last right-clicked.
    /// </summary>
    public static int Index { get; private set; } = -1;
    
    public override void Load()
    {
        base.Load();
        
        On_ItemSlot.RightClick_ItemArray_int_int += Hook;
        IL_ItemSlot.RightClick_ItemArray_int_int += Edit;
    }
    
    private static void Hook(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);

        if (!ClientSideConfiguration.Instance.EnableQuickShift)
        {
            return;
        }

        Index = slot;
    }

    private static void Edit(ILContext context)
    {
        var cursor = new ILCursor(context);

        while (cursor.TryGotoNext(MoveType.After, static i => i.MatchLdsfld<Main>(nameof(Main.mouseRightRelease))))
        {
            cursor.EmitLdarg1();
            cursor.EmitLdarg2();

            cursor.EmitDelegate(Evaluate);
        }
    }

    private static bool Evaluate(bool value, int context, int slot) => ContextSystem.CheckNPCContext(context) && (slot != Index || Main.mouseRightRelease);
}