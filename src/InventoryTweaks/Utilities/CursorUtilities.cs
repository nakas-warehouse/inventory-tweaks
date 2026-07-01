namespace InventoryTweaks.Utilities;

public static class CursorUtilities
{
    /// <summary>
    ///     Gets a value indicating whether the cursor has an override.
    /// </summary>
    public static bool Override => Main.cursorOverride != -1;
}