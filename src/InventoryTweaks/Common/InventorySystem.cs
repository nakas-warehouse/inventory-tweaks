namespace InventoryTweaks.Common;

public sealed class InventorySystem : ModSystem
{
    /// <summary>
    ///     The length of the player's inventory.
    /// </summary>
    /// <remarks>
    ///     Excludes index 57, which is reserved for <see cref="Main.mouseItem" />.
    /// </remarks>
    public const int Length = 57;
    
    /// <summary>
    ///     Invoked when the player's inventory is opened.
    /// </summary>
    public static event Action? Opened;
    
    /// <summary>
    ///     Invoked when the player's inventory is closed.
    /// </summary>
    public static event Action? Closed;

    private static bool flag;

    public override void Unload()
    {
        base.Unload();

        Opened = null;
        Closed = null;
    }

    public override void UpdateUI(GameTime gameTime)
    {
        base.UpdateUI(gameTime);
        
        if (Main.playerInventory == flag)
        {
            return;
        }

        flag = Main.playerInventory;
        
        (Main.playerInventory ? Opened : Closed)?.Invoke();
    }
}