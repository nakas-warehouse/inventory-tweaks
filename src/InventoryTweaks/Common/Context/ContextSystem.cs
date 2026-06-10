using System.Collections.Generic;

namespace InventoryTweaks.Common.Context;

[Autoload(Side = ModSide.Client)]
public sealed class ContextSystem : ModSystem
{
    private static readonly List<int> player = [];

    private static readonly List<int> npc = [];

    /// <summary>
    ///     Gets a read-only list of all registered player contexts.
    /// </summary>
    public static IReadOnlyList<int> Player => player;

    /// <summary>
    ///     Gets a read-only list of all registered NPC contexts.
    /// </summary>
    public static IReadOnlyList<int> NPC => npc;

    public override void Unload()
    {
        base.Unload();

        player.Clear();
        npc.Clear();
    }

    /// <summary>
    ///     Adds a context value to the player context list.
    /// </summary>
    /// <param name="context">
    ///     The context value to add.
    /// </param>
    /// <remarks>
    ///     If <paramref name="context"/> is already registered, this method does nothing.
    /// </remarks>
    public static void AddPlayerContext(int context)
    {
        if (player.Contains(context))
        {
            return;
        }

        player.Add(context);
    }

    /// <summary>
    ///     Adds a context value to the NPC context list.
    /// </summary>
    /// <param name="context">
    ///     The context value to add.
    /// </param>
    /// <remarks>
    ///     If <paramref name="context"/> is already registered, this method does nothing.
    /// </remarks>
    public static void AddNPCContext(int context)
    {
        if (npc.Contains(context))
        {
            return;
        }

        npc.Add(context);
    }

    /// <summary>
    ///     Removes a context value from the player context list.
    /// </summary>
    /// <param name="context">
    ///     The context value to remove.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="context"/> was found and removed; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    public static bool RemovePlayerContext(int context)
    {
        return player.Remove(context);
    }

    /// <summary>
    ///     Removes a context value from the NPC context list.
    /// </summary>
    /// <param name="context">
    ///     The context value to remove.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="context"/> was found and removed; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    public static bool RemoveNPCContext(int context)
    {
        return npc.Remove(context);
    }

    /// <summary>
    ///     Determines whether a context value is registered in the player context list.
    /// </summary>
    /// <param name="context">
    ///     The context value to locate.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="context"/> is found; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    public static bool CheckPlayerContext(int context)
    {
        return player.Contains(context);
    }

    /// <summary>
    ///     Determines whether a context value is registered in the NPC context list.
    /// </summary>
    /// <param name="context">
    ///     The context value to locate.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="context"/> is found; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    public static bool CheckNPCContext(int context)
    {
        return npc.Contains(context);
    }
}