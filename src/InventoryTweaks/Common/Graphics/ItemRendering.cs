using System.Collections.Generic;
using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Common.Contexts;
using MonoMod.Cil;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace InventoryTweaks.Common.Graphics;

public sealed class ItemRenderingSystem : ModSystem
{
    private readonly struct QueuedItemRenderData
    {
        public readonly required Vector2 Position { get; init; }

        public readonly required Item Item { get; init; }

        public readonly required int Context { get; init; }

        public readonly required float Scale { get; init; }

        public readonly required float Limit { get; init; }
        
        public readonly required Color Color { get; init; }
    }
    
    private sealed class ItemRenderData : GlobalItem
    {
        public Vector2? Position { get; internal set; }

        public float Scale { get; internal set; }

        public float Opacity
        {
            get => field;
            internal set => field = MathHelper.Clamp(value, 0f, 1f);
        }
 
        public bool Hovering { get; internal set; }
        
        public bool Skip { get; set; }
        
        public override bool InstancePerEntity => true;
    }

    private const string InsertionLayerName = "Vanilla: Mouse Text";
    
    public const string InterfaceLayerName = "Inventory Tweaks: Item Icons";
    
    /// <summary>
    ///     The width of the hitbox of an item in the inventory in pixels.
    /// </summary>
    public const int ItemHitboxWidth = 40;
    
    /// <summary>
    ///     The height of the hitbox of an item in the inventory in pixels.
    /// </summary>
    public const int ItemHitboxHeight = 40;
    
    private static readonly List<QueuedItemRenderData> Queue = [];
    
    public override void Load()
    {
        base.Load();

        On_ItemSlot.DrawItemIcon += Hook;
        
        IL_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += Edit;
    }

    public override void Unload()
    {
        base.Unload();
        
        Queue.Clear();
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        base.ModifyInterfaceLayers(layers);
        
        var index = layers.FindIndex(static layer => layer.Name.Equals(InsertionLayerName));
        
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        
        layers.Insert(index, new LegacyGameInterfaceLayer(InterfaceLayerName, DrawInterface, InterfaceScaleType.UI));
    }
    
    private static float Hook
    (
        On_ItemSlot.orig_DrawItemIcon orig, 
        Item item, 
        int context,
        SpriteBatch spriteBatch, 
        Vector2 position, 
        float scale,
        float limit, 
        Color color
    )
    {
        var configuration = ClientSideConfiguration.Instance;
        
        var data = item.GetGlobalItem<ItemRenderData>();
        
        if (!ContextSystem.CheckPlayerContext(context) || !configuration.EnableInventoryAnimations || data.Skip)
        {
            return orig(item, context, spriteBatch, position, scale, limit, color);
        }
        
        var hitbox = new Rectangle((int)position.X - ItemHitboxWidth / 2, (int)position.Y - ItemHitboxHeight / 2, ItemHitboxWidth, ItemHitboxHeight);
        
        data.Hovering = hitbox.Contains(Main.MouseScreen.ToPoint()) || item == Main.mouseItem || item == Main.LocalPlayer.HeldItem;
            
        data.Position = data.Position.HasValue ? Vector2.Lerp(data.Position.Value, position, configuration.InventoryAnimationSpeed) : position;
            
        data.Scale = MathHelper.SmoothStep(data.Scale, data.Hovering ? configuration.ActiveItemScale : configuration.InactiveItemScale, configuration.InventoryAnimationSpeed);
            
        var bounds = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        var contains = bounds.X <= hitbox.X && bounds.Y <= hitbox.Y && bounds.Right >= hitbox.Right && bounds.Bottom >= hitbox.Bottom;
        
        var cull = !contains;
        
        data.Opacity = MathHelper.SmoothStep(data.Opacity, cull ? 0f : 1f, configuration.InventoryAnimationSpeed);

        Main.GetItemDrawFrame(item.type, out _, out var frame);

        var light = color;

        ItemSlot.DrawItem_GetColorAndScale(item, scale, ref light, limit, ref frame, out _, out var final);
        
        var queue = new QueuedItemRenderData
        {
            Position = data.Position ?? position,
            Item = item,
            Context = context,
            Scale = data.Scale,
            Limit = limit,
            Color = color * data.Opacity
        };
        
        Queue.Add(queue);

        return final;
    }
    
    private static void Edit(ILContext context)
    {
        var cursor = new ILCursor(context);

        ILLabel? label = cursor.MarkLabel();

        cursor.GotoNext
        (
            MoveType.After, 
            static i => i.MatchLdloc1(), 
            static i => i.MatchLdfld<Item>(nameof(Item.stack)), 
            static i => i.MatchLdcI4(1),
            i => i.MatchBle(out label)
        );
        
        cursor.EmitBr(label);

        label = cursor.MarkLabel();
        
        cursor.GotoNext
        (
            MoveType.After, 
            static i => i.MatchLdloc(31), 
            static i => i.MatchLdcI4(-1), 
            i => i.MatchBeq(out label)
        );
        
        cursor.EmitBr(label);
    }

    private static bool DrawInterface()
    {
        DrawQueue();

        return true;
    }

    private static void DrawQueue(bool clear = true)
    {
        QueuedItemRenderData? mouse = null;
        
        foreach (var data in Queue)
        {
            if (data.Item == Main.mouseItem)
            {
                mouse = data;
            }
            else
            {
                DrawQueuedData(data);
            }
        }
        
        if (mouse.HasValue)
        {
            DrawQueuedData(mouse.Value);
        }

        if (!clear)
        {
            return;
        }
        
        Queue.Clear();
    }

    private static void DrawQueuedData(QueuedItemRenderData queued)
    {
        var batch = Main.spriteBatch;
        
        var item = queued.Item;

        var data = item.GetGlobalItem<ItemRenderData>();
        
        data.Skip = true;
        
        ItemSlot.DrawItemIcon(item, queued.Context, batch, queued.Position, queued.Scale, queued.Limit, queued.Color);

        data.Skip = false;
        
        if (item.stack <= 1)
        {
            return;
        }

        var cache = Main.inventoryScale;
        ref var scale = ref Main.inventoryScale;

        switch (queued.Context)
        {
            case ItemSlot.Context.MouseItem:
            case ItemSlot.Context.HotbarItem:
            case ItemSlot.Context.InventoryItem:
                scale = 0.8f;
                break;
            case ItemSlot.Context.ChestItem:
            case ItemSlot.Context.BankItem:
                scale = 0.7f;
                break;
            default:
                scale = 0.6f;
                break;
        }
        
        var holding = queued.Item == Main.LocalPlayer.HeldItem;

        if (holding)
        {
            scale = 0.8f;
        }

        var font = FontAssets.ItemStack.Value;

        var text = item.stack.ToString();
        var size = font.MeasureString(text);

        var origin = new Vector2(0f, size.Y / 2f);

        var texture = TextureAssets.InventoryBack.Value;

        var configuration = ClientSideConfiguration.Instance;
        
        var progress = MathHelper.Clamp((queued.Scale - configuration.InactiveItemScale) / (configuration.ActiveItemScale - configuration.InactiveItemScale), 0f, 1f);

        var bottom = queued.Position + new Vector2(-texture.Width / 3f, texture.Height / 2f - size.Y / 2f);
        var center = queued.Position - new Vector2(size.X / 2f, -texture.Height / 8f);

        var position = Vector2.SmoothStep(bottom, center, progress);

        var final = queued.Scale - (1f - scale);
        
        ChatManager.DrawColorCodedStringWithShadow
        (
            batch,
            font,
            text,
            position,
            queued.Color,
            0f,
            origin,
            new Vector2(final),
            -1f,
            final
        );
        
        scale = cache;
    }
}