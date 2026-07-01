namespace InventoryTweaks.Common.Pickups;

public ref struct ItemPickupContext
{
    public int Stack { get; set; }

    public readonly int Slot { get; }

    public readonly Item[] Inventory { get; }

    public readonly Item Item => Inventory[Slot];

    public ItemPickupContext(int stack, int slot, Item[] inventory)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(stack);
        ArgumentOutOfRangeException.ThrowIfNegative(slot);

        ArgumentNullException.ThrowIfNull(inventory);

        Stack = stack;
        Slot = slot;
        Inventory = inventory;
    }
}