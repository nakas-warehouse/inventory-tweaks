namespace InventoryTweaks.Common.Refills;

public static class ItemRefillUtilities
{
    public static void Refill(Item[] inventory, Item item, int length)
    {
        ArgumentNullException.ThrowIfNull(inventory);
        ArgumentNullException.ThrowIfNull(item);
        
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

        for (var i = 0; i < length; i++)
        {
            var other = inventory[i];

            if (other == item || other.IsAir || other.type != item.type)
            {
                continue;
            }

            var difference = item.maxStack - item.stack;

            if (difference <= 0)
            {
                break;
            }

            var stack = Math.Min(other.stack, difference);

            if (stack <= 0)
            {
                continue;
            }

            other.stack -= stack;
            item.stack += stack;
        }
    }
}