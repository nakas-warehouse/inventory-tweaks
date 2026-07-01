using Terraria.UI;

namespace InventoryTweaks.Common.Contexts;

[Autoload(Side = ModSide.Client)]
public sealed class PlayerContexts : ILoadable
{
    void ILoadable.Load(Mod mod)
    {
        ContextSystem.AddPlayerContext(ItemSlot.Context.InventoryItem);
        ContextSystem.AddPlayerContext(ItemSlot.Context.InventoryAmmo);
        ContextSystem.AddPlayerContext(ItemSlot.Context.InventoryCoin);
        
        ContextSystem.AddPlayerContext(ItemSlot.Context.ModdedAccessorySlot);
        ContextSystem.AddPlayerContext(ItemSlot.Context.ModdedDyeSlot);
        ContextSystem.AddPlayerContext(ItemSlot.Context.ModdedVanityAccessorySlot);
        
        ContextSystem.AddPlayerContext(ItemSlot.Context.ChestItem);
        ContextSystem.AddPlayerContext(ItemSlot.Context.BankItem);
        ContextSystem.AddPlayerContext(ItemSlot.Context.VoidItem);
        ContextSystem.AddPlayerContext(ItemSlot.Context.MouseItem);
        ContextSystem.AddPlayerContext(ItemSlot.Context.TrashItem);
        ContextSystem.AddPlayerContext(ItemSlot.Context.ShopItem);
        ContextSystem.AddPlayerContext(ItemSlot.Context.HotbarItem);
        ContextSystem.AddPlayerContext(ItemSlot.Context.GuideItem);
        ContextSystem.AddPlayerContext(ItemSlot.Context.PrefixItem);
        
        ContextSystem.AddPlayerContext(ItemSlot.Context.CreativeSacrifice);
        
        ContextSystem.AddPlayerContext(ItemSlot.Context.EquipArmor);
        ContextSystem.AddPlayerContext(ItemSlot.Context.EquipArmorVanity);
        ContextSystem.AddPlayerContext(ItemSlot.Context.EquipAccessory);
        ContextSystem.AddPlayerContext(ItemSlot.Context.EquipAccessoryVanity);
        ContextSystem.AddPlayerContext(ItemSlot.Context.EquipPet);
        ContextSystem.AddPlayerContext(ItemSlot.Context.EquipDye);
        ContextSystem.AddPlayerContext(ItemSlot.Context.EquipMiscDye);
        ContextSystem.AddPlayerContext(ItemSlot.Context.EquipLight);
        ContextSystem.AddPlayerContext(ItemSlot.Context.EquipMount);
        ContextSystem.AddPlayerContext(ItemSlot.Context.EquipGrapple);
        ContextSystem.AddPlayerContext(ItemSlot.Context.EquipMinecart);
    }
    
    void ILoadable.Unload() { }
}