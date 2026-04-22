namespace InventoryTweaks.Utilities;

/// <summary>
///     Provides input utility methods.
/// </summary>
public static class InputUtilities
{
    /// <summary>
    ///     Gets whether the cursor has an override.
    /// </summary>
    public static bool HasCursorOverride => Main.cursorOverride != -1;
}