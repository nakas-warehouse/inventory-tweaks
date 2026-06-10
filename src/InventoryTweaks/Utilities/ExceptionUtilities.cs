using System.Runtime.CompilerServices;

namespace InventoryTweaks.Utilities;

public static class ArgumentNullExceptionUtilities
{
    extension(ArgumentException)
    {
        /// <summary>
        ///     Throws an exception if <paramref name="argument"/> is <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argument">
        ///     The array argument to validate as non-<see langword="null"/> and non-empty.
        /// </param>
        /// <param name="paramName">
        ///     The name of the parameter with which <paramref name="argument"/> corresponds.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="argument"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument"/> is empty.
        /// </exception>
        public static void ThrowIfNullOrEmpty(object[] argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            ArgumentNullException.ThrowIfNull(argument);
            
            if (argument.Length != 0)
            {
                return;
            }
            
            throw new ArgumentException("Array cannot be empty.", paramName);
        }

        /// <summary>
        ///     Throws an exception if <paramref name="argument"/> is <see langword="null"/> or has fewer than <paramref name="length"/> elements.
        /// </summary>
        /// <param name="argument">
        ///     The array argument to validate as non-<see langword="null"/> and having at least <paramref name="length"/> elements.
        /// </param>
        /// <param name="length">
        ///     The minimum required length of <paramref name="argument"/>.
        /// </param>
        /// <param name="paramName">
        ///     The name of the parameter with which <paramref name="argument"/> corresponds.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="argument"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument"/> has fewer than <paramref name="length"/> elements.
        /// </exception>
        public static void ThrowIfNullOrSmallerThan(object[] argument, int length, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            ArgumentNullException.ThrowIfNull(argument);

            if (argument.Length >= length)
            {
                return;
            }

            throw new ArgumentException($"Array must have at least {length} elements.", paramName);
        }
    }
}