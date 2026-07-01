using System.Reflection.Metadata;
using System.Linq;
using InventoryTweaks.Common.UI;

[assembly: MetadataUpdateHandler(typeof(TweaksInterfaceReload))]

namespace InventoryTweaks.Common.UI;

public static class TweaksInterfaceReload
{
    internal static void ClearCache(Type[]? types)
    {
        ArgumentNullException.ThrowIfNull(types);

        if (!types.Contains(typeof(TweaksInterfaceState)))
        {
            return;
        }
        
        TweaksInterfaceSystem.Interface.SetState(null);
    }

    internal static void UpdateApplication(Type[]? types)
    {
        ArgumentNullException.ThrowIfNull(types);

        if (!types.Contains(typeof(TweaksInterfaceState)))
        {
            return;
        }
        
        TweaksInterfaceSystem.Interface.SetState(new TweaksInterfaceState());
    }
}