using InventoryTweaks.Common.Configuration;
using MonoMod.Cil;
using Terraria.UI;

namespace InventoryTweaks.Common.Pickups;

public sealed class ItemPickupSystem : ModSystem
{
    public delegate void ItemPickupCallback(ref ItemPickupContext context);
    
    public static event ItemPickupCallback? Pickup;
    
    public override void Load()
    {
        base.Load();

        IL_ItemSlot.PickupItemIntoMouse += EditStack;
    }

    public override void Unload()
    {
        base.Unload();

        Pickup = null;
    }

    private static void EditStack(ILContext context)
    {
        var cursor = new ILCursor(context);

        if (!cursor.TryGotoNext(MoveType.After, static i => i.MatchLdcI4(1)))
        {
            throw new Exception("");
        }

        cursor.EmitLdarg0();
        cursor.EmitLdarg2();

        cursor.EmitDelegate(GetStack);
    }

    private static int GetStack(int stack, Item[] inventory, int slot)
    {
        var context = new ItemPickupContext(stack, slot, inventory);

        switch (ClientSideConfiguration.Instance.PickupMode)
        {
            case ItemPickupMode.Full:
                context.Stack = context.Item.stack;
                break;
            
            case ItemPickupMode.Half:
                context.Stack = context.Item.stack / 2;
                break;
            
            case ItemPickupMode.Single:
                context.Stack = stack;
                break;
            
            case ItemPickupMode.Random:
                context.Stack = Main.rand.Next(1, context.Item.stack + 1);
                break;
            
            default:
                context.Stack = stack;
                break;
        }

        Pickup?.Invoke(ref context);

        return Math.Max(context.Stack, 1);
    }
}