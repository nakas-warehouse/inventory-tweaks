namespace InventoryTweaks.Utilities;

public static class EnumeratorExtensions
{
    extension<TEnumerator>(TEnumerator enumerator) where TEnumerator : struct, Enum
    {
        public TEnumerator Cycle()
        {
            var values = Enum.GetValues<TEnumerator>();
            var next = (Array.IndexOf(values, enumerator) + 1) % values.Length;
            
            return values[next];
        }
    }
}