using System.Collections.Generic;
using Terraria.UI;

namespace InventoryTweaks.Common.UI;

public sealed class TweaksInterfaceSystem : ModSystem
{
    private const string InsertionLayerName = "Vanilla: Inventory";
    
    public const string InterfaceLayerName = "Inventory Tweaks: Inventory Interface";
    
    public static UserInterface Interface { get; private set; } = null!;

    public override void Load()
    {
        base.Load();
        
        Interface = new UserInterface();
        
        InventorySystem.Opened += OpenInterface;
        InventorySystem.Closed += CloseInterface;
    }

    public override void Unload()
    {
        base.Unload();
        
        Interface.SetState(null);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        base.UpdateUI(gameTime);

        Interface.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        base.ModifyInterfaceLayers(layers);
        
        var index = layers.FindIndex(static layer => layer.Name == InsertionLayerName);

        ArgumentOutOfRangeException.ThrowIfNegative(index);

        layers.Insert(index, new LegacyGameInterfaceLayer(InterfaceLayerName, DrawInterface, InterfaceScaleType.UI));
    }
    
    /// <summary>
    ///     Opens the interface.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Interface"/> is <see langword="null"/>.
    /// </exception>
    public static void OpenInterface()
    {
        ArgumentNullException.ThrowIfNull(Interface);
        
        Interface.SetState(new TweaksInterfaceState());
    }
    
    /// <summary>
    ///     Closes the interface.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Interface"/> is <see langword="null"/>.
    /// </exception>
    public static void CloseInterface()
    {
        ArgumentNullException.ThrowIfNull(Interface);
        
        Interface.SetState(null);
    }

    private static bool DrawInterface()
    {
        Interface.Draw(Main.spriteBatch, new GameTime());
        
        return true;
    }
}