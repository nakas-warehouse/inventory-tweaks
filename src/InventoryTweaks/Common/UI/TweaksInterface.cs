using System.Linq;
using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Common.Sorting;
using InventoryTweaks.Utilities;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace InventoryTweaks.Common.UI;

public sealed class TweaksInterfaceState : UIState
{
    private abstract class TweaksInterfaceButton : UIElement
    {
        public static readonly SoundStyle HoverSound = SoundID.MenuTick with
        {
            SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest,
            MaxInstances = 1
        };

        public static readonly SoundStyle ClickSound = SoundID.MenuOpen with
        {
            SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest,
            MaxInstances = 1
        };

        public (float Inactive, float Active) TargetScale { get; init; } = (1f, 1.05f);

        public float Interpolation { get; init; } = 0.33f;

        public float Scale
        {
            get => field;
            protected set => field = Math.Clamp(value, 0f, TargetScale.Active);
        }

        public float Hover
        {
            get => field;
            protected set => field = Math.Clamp(value, 0f, 1f);
        }

        public Asset<Texture2D> Texture { get; protected set; }

        public abstract string Tooltip { get; }

        public TweaksInterfaceButton(Asset<Texture2D> texture)
        {
            Texture = texture;
        }

        public override void Recalculate()
        {
            base.Recalculate();

            Texture.Wait();

            Width.Set(Texture.Width(), 0f);
            Height.Set(Texture.Height(), 0f);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

            if (!ClientSideConfiguration.Instance.EnableInterfaceSounds)
            {
                return;
            }

            SoundEngine.PlaySound(in HoverSound);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);

            if (!ClientSideConfiguration.Instance.EnableInterfaceSounds)
            {
                return;
            }

            SoundEngine.PlaySound(in HoverSound);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            Scale = TargetScale.Inactive;

            if (!ClientSideConfiguration.Instance.EnableInterfaceSounds)
            {
                return;
            }

            SoundEngine.PlaySound(in ClickSound);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ClientSideConfiguration.Instance.EnableInterfaceAnimations)
            {
                Hover = MathHelper.SmoothStep(Hover, IsMouseHovering ? 1f : 0f, Interpolation);
                Scale = MathHelper.SmoothStep(Scale, IsMouseHovering ? TargetScale.Active : TargetScale.Inactive, Interpolation);
            }
            else
            {
                Scale = 1f;
                Hover = IsMouseHovering ? 1f : 0f;
            }
            
            Main.LocalPlayer.mouseInterface = IsMouseHovering;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var dimensions = GetDimensions();
            var position = dimensions.Center();

            var icon = Texture.Value;

            spriteBatch.Draw(icon, position, null, Color.White * 0.9f, 0f, icon.Size() / 2f, Scale, SpriteEffects.None, 0f);

            if (!IsMouseHovering)
            {
                return;
            }

            UICommon.TooltipMouseText(Tooltip);
            
            if (!ClientSideConfiguration.Instance.EnableInterfaceHighlights)
            {
                return;
            }

            var highlight = Assets.Textures.UI.Selected.Asset.Value;

            spriteBatch.Draw(highlight, position, null, Color.White * Hover, 0f, highlight.Size() / 2f, Scale, SpriteEffects.None, 0f);
        }

        public virtual void Set(Asset<Texture2D> texture)
        {
            Texture = texture;

            Recalculate();
        }
    }

    private sealed class TweaksInterfacePickupButton() : TweaksInterfaceButton(Assets.Textures.UI.Pickup.Asset)
    {
        public override string Tooltip => Mods.InventoryTweaks.UI.Buttons.Pickup.GetTextValue(ClientSideConfiguration.Instance.PickupMode);

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            ClientSideConfiguration.Instance.PickupMode = ClientSideConfiguration.Instance.PickupMode.Cycle();
        }
    }

    private sealed class TweaksInterfaceSortButton() : TweaksInterfaceButton(Icon)
    {
        private static Asset<Texture2D> Icon => ClientSideConfiguration.Instance.SortingDirection == ItemSortingDirection.Ascending
            ? Assets.Textures.UI.SortAscending.Asset
            : Assets.Textures.UI.SortDescending.Asset;
        
        public override string Tooltip => Mods.InventoryTweaks.UI.Buttons.Sort.GetTextValue(ClientSideConfiguration.Instance.SortingDirection, ClientSideConfiguration.Instance.SortingMode);

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            ClientSideConfiguration.Instance.SortingMode = ClientSideConfiguration.Instance.SortingMode.Cycle();
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);
            
            ClientSideConfiguration.Instance.SortingDirection = ClientSideConfiguration.Instance.SortingDirection.Cycle();
            
            Set(Icon);
        }
    }

    private sealed class TweaksInterfaceRefillButton() : TweaksInterfaceButton(Assets.Textures.UI.Refill.Asset)
    {
        public override string Tooltip => Mods.InventoryTweaks.UI.Buttons.Refill.GetTextValue(ClientSideConfiguration.Instance.EnableRefill);

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            ClientSideConfiguration.Instance.EnableRefill = !ClientSideConfiguration.Instance.EnableRefill;
        }
    }

    private sealed class TweaksInterfaceDistributionButton() : TweaksInterfaceButton(Assets.Textures.UI.Distribution.Asset)
    {
        public override string Tooltip => Mods.InventoryTweaks.UI.Buttons.Distribution.GetTextValue(ClientSideConfiguration.Instance.EnableDistribution);

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            ClientSideConfiguration.Instance.EnableDistribution = !ClientSideConfiguration.Instance.EnableDistribution;
        }
    }

    /// <summary>
    ///     The horizontal offset of the state from the left edge of the screen.
    /// </summary>
    private const float X = 20f;

    /// <summary>
    ///     The vertical offset of the state from the top edge of the screen.
    /// </summary>
    private const float Y = 260f;

    /// <summary>
    ///     Gets the padding between elements from this interface in pixels.
    /// </summary>
    public float Padding { get; init; } = 8f;

    public override void OnInitialize()
    {
        base.OnInitialize();

        Add(new TweaksInterfacePickupButton());
        Add(new TweaksInterfaceSortButton());
        Add(new TweaksInterfaceDistributionButton());
        Add(new TweaksInterfaceRefillButton());
    }

    /// <summary>
    ///     Adds an element to the interface, automatically positioning it to the right of the previously
    ///     added element.
    /// </summary>
    /// <param name="element">
    ///     The element to add to the interface.
    /// </param>
    /// <typeparam name="TElement">
    ///     The type of the element to add to the interface.
    /// </typeparam>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="element"/> is <see langword="null"/>.
    /// </exception>
    public void Add<TElement>(TElement element) where TElement : UIElement
    {
        ArgumentNullException.ThrowIfNull(element);

        var count = Children.Count();

        element.Recalculate();

        element.Left.Set(X + (element.Width.Pixels + Padding) * count, 0f);
        element.Top.Set(Y, 0f);

        Append(element);
    }
}