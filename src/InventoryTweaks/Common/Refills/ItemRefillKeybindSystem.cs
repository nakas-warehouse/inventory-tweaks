namespace InventoryTweaks.Common.Refills;

public sealed class ItemRefillKeybindSystem : ModSystem
{
    public static ModKeybind Keybind { get; private set; } = null!;
    
    public override void Load()
    {
        base.Load();

        Keybind = KeybindLoader.RegisterKeybind(Mod, $"{nameof(InventoryTweaks)}:{nameof(ItemRefillKeybindSystem)}", "Mouse3");
    }
}