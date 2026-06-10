using InventoryTweaks.Common.Context;
using InventoryTweaks.Utilities;

namespace InventoryTweaks;

public sealed partial class InventoryTweaks
{
    public override object Call(params object[] args)
    {
        ArgumentException.ThrowIfNullOrEmpty(args);

        var command = Fetch<string>(args, 0);

        command = command.ToLowerInvariant();

        switch (command)
        {
            case "addplayercontext":
                ContextSystem.AddPlayerContext(Fetch<int>(command, args, 1));
                return null!;

            case "addnpccontext":
                ContextSystem.AddNPCContext(Fetch<int>(command, args, 1));
                return null!;

            case "removeplayercontext":
                ContextSystem.RemovePlayerContext(Fetch<int>(command, args, 1));
                return null!;

            case "removenpccontext":
                ContextSystem.RemoveNPCContext(Fetch<int>(command, args, 1));
                return null!;

            case "checkplayercontext":
                return ContextSystem.CheckPlayerContext(Fetch<int>(command, args, 1));

            case "checknpccontext":
                return ContextSystem.CheckNPCContext(Fetch<int>(command, args, 1));

            default:
                throw new ArgumentException($"Unknown command: {command}", nameof(args));
        }
    }

    /// <summary>
    ///     Retrieves an argument from <paramref name="args"/> at the specified index as
    ///     <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">
    ///     The expected type of the argument.
    /// </typeparam>
    /// <param name="args">
    ///     The argument array passed to <see cref="Call"/>.
    /// </param>
    /// <param name="index">
    ///     The zero-based index of the argument to retrieve.
    /// </param>
    /// <returns>
    ///     The argument at <paramref name="index"/> cast to <typeparamref name="TValue"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="index"/> is negative.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <paramref name="index"/> is out of range, or the argument at <paramref name="index"/> is not of
    ///     type <typeparamref name="TValue"/>.
    /// </exception>
    private static TValue Fetch<TValue>(object[] args, int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        ArgumentException.ThrowIfNullOrSmallerThan(args, index + 1);

        return args[index] is TValue value ? value : throw new ArgumentException();
    }

    /// <summary>
    ///     Retrieves an argument from <paramref name="args"/> at the specified index as
    ///     <typeparamref name="TValue"/>, including the command name in exception messages.
    /// </summary>
    /// <typeparam name="TValue">
    ///     The expected type of the argument.
    /// </typeparam>
    /// <param name="command">
    ///     The command name, used in exception messages.
    /// </param>
    /// <param name="args">
    ///     The argument array passed to <see cref="Call"/>.
    /// </param>
    /// <param name="index">
    ///     The zero-based index of the argument to retrieve.
    /// </param>
    /// <returns>
    ///     The argument at <paramref name="index"/> cast to <typeparamref name="TValue"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <paramref name="command"/> is <see langword="null"/> or empty, <paramref name="index"/> is out
    ///     of range, or the argument at <paramref name="index"/> is not of type
    ///     <typeparamref name="TValue"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="index"/> is negative.
    /// </exception>
    private static TValue Fetch<TValue>(string command, object[] args, int index)
    {
        ArgumentException.ThrowIfNullOrEmpty(command);

        ArgumentOutOfRangeException.ThrowIfNegative(index);

        ArgumentException.ThrowIfNullOrSmallerThan(args, index + 1);

        return args[index] is TValue value ? value : throw new ArgumentException();
    }
}