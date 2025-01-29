using FargowiltasSouls.Assets.UI;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Essences;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Audio;
using Terraria.Localization;
using System;
using Microsoft.Xna.Framework.Input;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
using FargowiltasSouls.Content.Items;
using static System.Net.Mime.MediaTypeNames;
using Fargowiltas.Projectiles;
using FargowiltasSouls.Content.NPCs;
using Terraria.GameContent.Creative;
using static Terraria.GameContent.Creative.CreativePowers;
using static Fargowiltas.FargoSets;

namespace FargowiltasSouls.Content.UI.Elements
{
    public abstract class DifficultyOption : UIPanel
    {
        public static DynamicSpriteFont Font => Terraria.GameContent.FontAssets.ItemStack.Value;
        public float Opacity = 1f;
        public abstract string NameKey { get; }
        public abstract string TooltipText();

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public abstract void OnClicked();
        public virtual void PreDraw(SpriteBatch spriteBatch, Vector2 position, float scale) { }
        public virtual void PostDraw(SpriteBatch spriteBatch, Vector2 position, float scale) { }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 position = GetDimensions().Position();
            bool hovering = ContainsPoint(Main.MouseScreen);

            float scale = 1f;
            if (hovering)
                scale = 1.2f;

            PreDraw(spriteBatch, position, scale);
            //base.DrawSelf(spriteBatch);
            PostDraw(spriteBatch, position, scale);


            if (hovering)
            {
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    OnClicked();
                    FargoUIManager.Close<DifficultySelectionMenu>();
                }
                else
                {
                    string name = Language.GetTextValue(NameKey);
                    string leftClick = Language.GetTextValue("Mods.FargowiltasSouls.UI.LeftClickEnable");

                    string text = name;
                    text += $"\n[c/787878:{leftClick}]";
                    text += "\n" + TooltipText();

                    Item hoverItem = new(2)
                    {
                        createTile = -1,
                        material = false,
                        consumable = false
                    };
                    hoverItem.SetNameOverride(text);
                    Main.HoverItem = hoverItem;
                    Main.instance.MouseText("", 0, 0);
                    Main.hoverItemName = text;
                    Main.mouseText = true;
                    //Main.instance.MouseText(text, 0, 0);
                    /*
                    Utils.DrawBorderString(
                        spriteBatch,
                        text,
                        textPosition,
                        Color.White);
                    */
                }
            }
           
        }
    }

    public class VanillaDifficultyOption : DifficultyOption
    {
        public static void DisableEternity()
        {
            if (!Masochist.CanToggleEternity())
                return;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                WorldSavingSystem.ShouldBeEternityMode = false;
            }
            else
            {
                var netMessage = FargowiltasSouls.Instance.GetPacket();
                netMessage.Write((byte)FargowiltasSouls.PacketID.ToggleEternityMode);
                netMessage.Write((byte)Main.LocalPlayer.whoAmI);
                netMessage.Write((byte)0); // 0 = disable emode
                netMessage.Send();
            }
        }
        public override void OnClicked()
        {
            DisableEternity();
        }
        public override string NameKey => "Mods.FargowiltasSouls.UI.Standard";
        public override string TooltipText()
        {
            return Language.GetTextValue("Mods.FargowiltasSouls.UI.ExpandedStandard");
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 position, float scale)
        {
            var texture = FargoUIManager.OncomingMutantntTexture.Value;
            Vector2 center = position + new Vector2(Width.Pixels / 2, Height.Pixels / 2) - scale * texture.Size() / 2;
            spriteBatch.Draw(texture, center, texture.Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
    public class EternityDifficultyOption : DifficultyOption
    {
        public static void EnableEternity()
        {
            if (!Masochist.CanToggleEternity())
                return;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                bool changed = false;
                if (Main.GameModeInfo.IsJourneyMode)
                {
                    var slider = CreativePowerManager.Instance.GetPower<DifficultySliderPower>();
                    DifficultySelectionMenu.JourneyMode_SetValue.Invoke(slider, [0.66f]);
                }
                else
                {
                    if (Main.GameMode != GameModeID.Expert)
                        changed = true;
                    Main.GameMode = GameModeID.Expert;
                }
                if (changed)
                    FargoSoulsUtil.PrintLocalization("Mods.Fargowiltas.Items.ModeToggle.Expert", new Color(175, 75, 255));

                WorldSavingSystem.ShouldBeEternityMode = true;

                int deviType = ModContent.NPCType<UnconsciousDeviantt>();
                if (FargoSoulsUtil.HostCheck && !WorldSavingSystem.SpawnedDevi && !NPC.AnyNPCs(deviType))
                {
                    WorldSavingSystem.SpawnedDevi = true;

                    Vector2 spawnPos = (Main.zenithWorld || Main.remixWorld) ? Main.LocalPlayer.Center : Main.LocalPlayer.Center - 1000 * Vector2.UnitY;
                    Projectile.NewProjectile(Main.LocalPlayer.GetSource_Misc(""), spawnPos, Vector2.Zero, ModContent.ProjectileType<SpawnProj>(), 0, 0, Main.myPlayer, deviType);

                    FargoSoulsUtil.PrintLocalization("Announcement.HasAwoken", new Color(175, 75, 255), Language.GetTextValue("Mods.Fargowiltas.NPCs.Deviantt.DisplayName"));
                }
            }
            else
            {
                var netMessage = FargowiltasSouls.Instance.GetPacket();
                netMessage.Write((byte)FargowiltasSouls.PacketID.ToggleEternityMode);
                netMessage.Write((byte)Main.LocalPlayer.whoAmI);
                netMessage.Write((byte)1); // 1 = set to emode
                netMessage.Send();
            }
        }
        public override void OnClicked()
        {
            EnableEternity();
        }
        public const string LocPath = "Mods.FargowiltasSouls.UI.";
        public override string NameKey => LocPath + "Eternity";
        public override string TooltipText()
        {
            string text = Language.GetTextValue($"{LocPath}EternityOption");
            text += $"\n{Language.GetTextValue($"{LocPath}ExpandedFeatures")}";
            return text;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 position, float scale)
        {
            var texture = FargoUIManager.OncomingMutantTexture.Value;
            Vector2 center = position + new Vector2(Width.Pixels / 2, Height.Pixels / 2) - scale * texture.Size() / 2;
            spriteBatch.Draw(texture, center, texture.Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
    public class MasoDifficultyOption : DifficultyOption
    {
        public static void EnableMasochist()
        {
            if (!Masochist.CanToggleEternity())
                return;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                bool changed = false;
                if (Main.GameModeInfo.IsJourneyMode)
                {
                    var slider = CreativePowerManager.Instance.GetPower<DifficultySliderPower>();
                    DifficultySelectionMenu.JourneyMode_SetValue.Invoke(slider, [1f]);
                }
                else
                {
                    if (Main.GameMode != GameModeID.Master)
                        changed = true;
                    Main.GameMode = GameModeID.Master;
                }
                if (changed)
                    FargoSoulsUtil.PrintLocalization("Mods.Fargowiltas.Items.ModeToggle.Master", new Color(175, 75, 255));

                WorldSavingSystem.ShouldBeEternityMode = true;

                int deviType = ModContent.NPCType<UnconsciousDeviantt>();
                if (FargoSoulsUtil.HostCheck && !WorldSavingSystem.SpawnedDevi && !NPC.AnyNPCs(deviType))
                {
                    WorldSavingSystem.SpawnedDevi = true;

                    Vector2 spawnPos = (Main.zenithWorld || Main.remixWorld) ? Main.LocalPlayer.Center : Main.LocalPlayer.Center - 1000 * Vector2.UnitY;
                    Projectile.NewProjectile(Main.LocalPlayer.GetSource_Misc(""), spawnPos, Vector2.Zero, ModContent.ProjectileType<SpawnProj>(), 0, 0, Main.myPlayer, deviType);

                    FargoSoulsUtil.PrintLocalization("Announcement.HasAwoken", new Color(175, 75, 255), Language.GetTextValue("Mods.Fargowiltas.NPCs.Deviantt.DisplayName"));
                }
            }
            else
            {
                var netMessage = FargowiltasSouls.Instance.GetPacket();
                netMessage.Write((byte)FargowiltasSouls.PacketID.ToggleEternityMode);
                netMessage.Write((byte)Main.LocalPlayer.whoAmI);
                netMessage.Write((byte)2); // 2 = set to emode
                netMessage.Send();
            }
        }
        public override void OnClicked()
        {
            EnableMasochist();
        }
        public const string LocPath = "Mods.FargowiltasSouls.UI.";
        public override string NameKey => LocPath + "Masochist";
        public override string TooltipText()
        {
            string text = Language.GetTextValue($"{LocPath}MasochistOption");
            text += $"\n{Language.GetTextValue($"{LocPath}ExpandedFeatures")}";
            if (Main.netMode != NetmodeID.SinglePlayer)
                text += $"\n{Language.GetTextValue($"{LocPath}MasochistMultiplayer")}";
            return text;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 position, float scale)
        {
            var texture = FargoUIManager.OncomingMutantTexture.Value;
            Vector2 center = position + new Vector2(Width.Pixels / 2, Height.Pixels / 2) - scale * texture.Size() / 2;
            spriteBatch.Draw(texture, center, texture.Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            texture = FargoUIManager.OncomingMutantAuraTexture.Value;
            center = position + new Vector2(Width.Pixels / 2, Height.Pixels / 2) - scale * texture.Size() / 2;
            spriteBatch.Draw(texture, center, texture.Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
}
