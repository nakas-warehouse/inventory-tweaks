using Terraria.UI;

namespace InventoryTweaks.Common.Context;

[Autoload(Side = ModSide.Client)]
public sealed class NPCContexts : ILoadable
{
    void ILoadable.Load(Mod mod)
    {
        ContextSystem.AddNPCContext(ItemSlot.Context.ShopItem);
    }
    
    void ILoadable.Unload() { }
}