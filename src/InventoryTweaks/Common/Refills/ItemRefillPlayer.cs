using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Utilities;
using Terraria.Audio;
using Terraria.GameInput;

namespace InventoryTweaks.Common.Refills;

public sealed class ItemRefillPlayer : ModPlayer
{
    public static readonly SoundStyle RefillSound = SoundID.Grab with
    {
        SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest,
        MaxInstances = 1
    };
    
    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        base.ProcessTriggers(triggersSet);
        
        if (!ItemRefillKeybindSystem.Keybind.JustPressed)
        {
            return;
        }

        var refill = !Main.mouseItem.IsAir && !Main.mouseItem.Full;
        
        if (!refill)
        {
            return;
        }

        ItemRefillUtilities.Refill(Player.inventory, Main.mouseItem, InventorySystem.Length);
        
        if (!ClientSideConfiguration.Instance.EnableRefillSounds)
        {
            return;
        }

        SoundEngine.PlaySound(in RefillSound);
    }
}