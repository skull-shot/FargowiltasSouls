﻿using Fargowiltas.Assets.Textures;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI.Elements
{
    public class UIToggle : UIElement
    {
        public const int CheckboxTextSpace = 4;

        public static DynamicSpriteFont Font => Terraria.GameContent.FontAssets.ItemStack.Value;

        public AccessoryEffect Effect;
        public string Mod;

        public UIToggle(AccessoryEffect effect, string mod)
        {
            Effect = effect;
            Mod = mod;

            Width.Set(19, 0);
            Height.Set(21, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 position = GetDimensions().Position();
            Player player = Main.LocalPlayer;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (IsMouseHovering && Main.mouseLeft && Main.mouseLeftRelease)
            {
                modPlayer.Toggler.Toggles[Effect].ToggleBool = !modPlayer.Toggler.Toggles[Effect].ToggleBool;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    modPlayer.SyncToggle(Effect);
            }

            bool disabledByMinos = Effect.MinionEffect && modPlayer.GalacticMinionsDeactivated;
            bool disabledByPresence = modPlayer.MutantPresence && (Effect.MutantsPresenceAffects || Effect.MinionEffect);
            bool disabledByGlobalToggle = (Effect.MinionEffect && modPlayer.Toggler_MinionsDisabled) || (Effect.ExtraAttackEffect && modPlayer.Toggler_ExtraAttacksDisabled) || (Effect.ExtraJumpEffect && modPlayer.Toggler_ExtraJumpsDisabled);
            bool toggled = Main.LocalPlayer.GetToggleValue(Effect, true);

            spriteBatch.Draw(FargoMutantAssets.UI.Toggler.CheckBox.Value, position, Color.White);

            if (disabledByMinos)
                spriteBatch.Draw(FargoMutantAssets.UI.Toggler.Cross.Value, position, Color.Cyan);
            else if ((disabledByPresence && modPlayer.PresenceTogglerTimer <= 50) || disabledByGlobalToggle)
                spriteBatch.Draw(FargoMutantAssets.UI.Toggler.Cross.Value, position, toggled ? Color.White : Color.Gray);
            else if (toggled)
                spriteBatch.Draw(FargoMutantAssets.UI.Toggler.CheckMark.Value, position, Color.White);

            string text = Effect.ToggleDescription;
            position += new Vector2(Width.Pixels * Main.UIScale, 0);
            position += new Vector2(CheckboxTextSpace, 0);
            position += new Vector2(0, Font.MeasureString(text).Y * 0.175f);
            Color color = Color.White;
            if (Effect.ToggleItemType > 0)
            {
                Item item = ContentSamples.ItemsByType[Effect.ToggleItemType];
                if (item.ModItem != null)
                {
                    if (item.ModItem is BaseEnchant enchant)
                        color = enchant.nameColor;
                    else if (item.ModItem is BaseForce force)
                        color = Color.BlueViolet;
                }

            }
            if (disabledByMinos)
            {
                color = Color.Cyan * 0.5f;
                //text += $" [i:{ModContent.ItemType<GalacticGlobe>()}]";
            }
            else if (disabledByPresence)
            {
                Color gray = Color.Gray * 0.5f;
                if (modPlayer.PresenceTogglerTimer > 50)
                    color = Color.Lerp(gray, color, (modPlayer.PresenceTogglerTimer - 50) / 50f);
                else
                {
                    color = gray;
                    text += $" [i:{ModContent.ItemType<OncomingMutantItem>()}]";
                }
            }
            Utils.DrawBorderString(spriteBatch, text, position, color);

            if (modPlayer.PresenceTogglerTimer > 0) // draw slash
            {
                //TODO: this doesn't work rn. fix later. probably change to something else

                Vector2 offset = Vector2.UnitX * (float)Utils.Lerp(-1500, 1500, modPlayer.PresenceTogglerTimer / 100f);
                Vector2 start = position + offset;
                Vector2 end = start + Vector2.UnitX * 50;

                Texture2D texture = TextureAssets.Projectile[ModContent.ProjectileType<MonkDashDamage>()].Value;
                Rectangle rect = new(0, 0, texture.Width, texture.Height);
                Vector2 origin = rect.Size() / 2;
                int num149 = 18;
                int num147 = 0;
                int num148 = -2;
                float value12 = 1.3f;
                float num150 = 15f;

                for (int num152 = num149; (num148 > 0 && num152 < num147) || (num148 < 0 && num152 > num147); num152 += num148)
                {
                    Color color32 = Color.Cyan;

                    float num157 = num147 - num152;
                    if (num148 < 0)
                    {
                        num157 = num149 - num152;
                    }
                    color32 *= num157 / ((float)10 * 1.5f);
                    Vector2 vector29 = start + (end - start) * num157 / ((float)10 * 1.5f);
                    float num158 = 0;
                    SpriteEffects effects2 = SpriteEffects.None;
                    if (vector29 == Vector2.Zero)
                    {
                        continue;
                    }
                    Vector2 position3 = vector29;
                    Main.EntitySpriteDraw(texture, position3, rect, color32, num158, origin, MathHelper.Lerp(1, value12, (float)num152 / num150), effects2);
                }
                //spriteBatch.Draw(TextureAssets.Extra[33].Value, start, null, Color.Cyan, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }
    }
    public class ExtraAttacksToggle : UIElement
    {
        public const int CheckboxTextSpace = UIToggle.CheckboxTextSpace;

        public static DynamicSpriteFont Font => UIToggle.Font;


        public ExtraAttacksToggle()
        {

            Width.Set(19, 0);
            Height.Set(21, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 position = GetDimensions().Position();
            Player player = Main.LocalPlayer;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (IsMouseHovering && Main.mouseLeft && Main.mouseLeftRelease)
            {
                modPlayer.Toggler_ExtraAttacksDisabled = !modPlayer.Toggler_ExtraAttacksDisabled;
            }

            spriteBatch.Draw(FargoMutantAssets.UI.Toggler.CheckBox.Value, position, Color.White);

            if (modPlayer.Toggler_ExtraAttacksDisabled)
                spriteBatch.Draw(FargoMutantAssets.UI.Toggler.CheckMark.Value, position, Color.White);

            string text = Language.GetTextValue($"Mods.FargowiltasSouls.Toggler.DisableAllAttackEffects");
            position += new Vector2(Width.Pixels * Main.UIScale, 0);
            position += new Vector2(CheckboxTextSpace, 0);
            position += new Vector2(0, Font.MeasureString(text).Y * 0.175f);
            Color color = Color.White;
            Utils.DrawBorderString(spriteBatch, text, position, color);
        }
    }
    public class MinionsToggle : UIElement
    {
        public const int CheckboxTextSpace = UIToggle.CheckboxTextSpace;

        public static DynamicSpriteFont Font => UIToggle.Font;

        public MinionsToggle()
        {

            Width.Set(19, 0);
            Height.Set(21, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 position = GetDimensions().Position();
            Player player = Main.LocalPlayer;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (IsMouseHovering && Main.mouseLeft && Main.mouseLeftRelease)
            {
                modPlayer.Toggler_MinionsDisabled = !modPlayer.Toggler_MinionsDisabled;
            }

            spriteBatch.Draw(FargoMutantAssets.UI.Toggler.CheckBox.Value, position, Color.White);

            if (modPlayer.Toggler_MinionsDisabled)
                spriteBatch.Draw(FargoMutantAssets.UI.Toggler.CheckMark.Value, position, Color.White);

            string text = Language.GetTextValue($"Mods.FargowiltasSouls.Toggler.DisableAllMinionEffects");
            position += new Vector2(Width.Pixels * Main.UIScale, 0);
            position += new Vector2(CheckboxTextSpace, 0);
            position += new Vector2(0, Font.MeasureString(text).Y * 0.175f);
            Color color = Color.White;
            Utils.DrawBorderString(spriteBatch, text, position, color);
        }
    }
    public class ExtraJumpsDisabledToggle : UIElement
    {
        public const int CheckboxTextSpace = UIToggle.CheckboxTextSpace;

        public static DynamicSpriteFont Font => UIToggle.Font;

        public ExtraJumpsDisabledToggle()
        {

            Width.Set(19, 0);
            Height.Set(21, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 position = GetDimensions().Position();
            Player player = Main.LocalPlayer;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (IsMouseHovering && Main.mouseLeft && Main.mouseLeftRelease)
            {
                modPlayer.Toggler_ExtraJumpsDisabled = !modPlayer.Toggler_ExtraJumpsDisabled;
            }

            spriteBatch.Draw(FargoMutantAssets.UI.Toggler.CheckBox.Value, position, Color.White);

            if (modPlayer.Toggler_ExtraJumpsDisabled)
                spriteBatch.Draw(FargoMutantAssets.UI.Toggler.CheckMark.Value, position, Color.White);

            string text = Language.GetTextValue($"Mods.FargowiltasSouls.Toggler.DisableAllExtraJumpEffects");
            position += new Vector2(Width.Pixels * Main.UIScale, 0);
            position += new Vector2(CheckboxTextSpace, 0);
            position += new Vector2(0, Font.MeasureString(text).Y * 0.175f);
            Color color = Color.White;
            Utils.DrawBorderString(spriteBatch, text, position, color);
        }
    }
}
