using InventoryTweaks.Common.Configuration;
using Terraria.Audio;

namespace InventoryTweaks.Common.Refills;

public sealed class ItemRefillGlobalItem : GlobalItem
{
    public static readonly SoundStyle RefillSound = SoundID.Grab with
    {
        SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest,
        MaxInstances = 1
    };
    
    public override void OnConsumeItem(Item item, Player player)
    {
        base.OnConsumeItem(item, player);

        if (!ClientSideConfiguration.Instance.EnableRefill)
        {
            return;
        }
        
        var refill = item == player.HeldItem && item.stack - 1 <= 0;

        if (!refill)
        {
            return;
        }
        
        ItemRefillUtilities.Refill(player.inventory, player.HeldItem, InventorySystem.Length);

        if (!ClientSideConfiguration.Instance.EnableRefillSounds)
        {
            return;
        }

        SoundEngine.PlaySound(in RefillSound);
    }
}