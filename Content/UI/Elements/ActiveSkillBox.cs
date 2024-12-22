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
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework.Input;
using Terraria.Localization;
using FargowiltasSouls.Core.ModPlayers;
using Humanizer;
using FargowiltasSouls.Content.Bosses.DeviBoss;

namespace FargowiltasSouls.Content.UI.Elements
{
    public abstract class ActiveSkillBox : UIPanel
    {
        public static DynamicSpriteFont Font => Terraria.GameContent.FontAssets.ItemStack.Value;
        public string Mod;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            CheckSlotElegible(Main.LocalPlayer);
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
        public abstract void CheckSlotElegible(Player player);
        public abstract AccessoryEffect GetEffect();
        public abstract void OnClicked();
        public virtual void PreDraw(SpriteBatch spriteBatch, Vector2 position) { }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            AccessoryEffect effect = GetEffect();
            base.DrawSelf(spriteBatch);
            CheckSlotElegible(Main.LocalPlayer);
            Vector2 position = GetDimensions().Position();
            PreDraw(spriteBatch, position);

            bool hovering = ContainsPoint(Main.MouseScreen);
            ActiveSkillBox held = ActiveSkillMenu.MouseHeldElement;
            bool notHoldingOther = held == null || held == this;
            if ((hovering || held == this) && (ActiveSkillMenu.MouseHoveredElement != this))
            {
                if (notHoldingOther)
                {
                    ActiveSkillMenu.MouseHoveredElement = this;
                    return;
                }
            }
            int item = -1;

            if (effect != null)
            {
                if (effect.ToggleItemType > 0)
                    item = effect.ToggleItemType;
                else if (effect.EffectItem(Main.LocalPlayer).type > ItemID.None)
                    item = effect.EffectItem(Main.LocalPlayer).type;
            }
            Vector2 itemPos = position + new Vector2(9, 9);
            float scale = 1f;
            if (ActiveSkillMenu.MouseHeldElement == this)
            {
                scale = 1.2f * Main.cursorScale;
                itemPos = Main.MouseScreen - new Vector2(14, 14);

            }
            if (hovering)
            {
                if (scale == 1 && notHoldingOther)
                    scale = 1.2f;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    OnClicked();
                }
                else if (ActiveSkillMenu.MouseHeldElement == null && effect != null)
                {
                    Vector2 textPosition = Main.MouseScreen + new Vector2(21, 21);
                    string locPath = $"Mods.{effect.Mod.Name}.ActiveSkills.{effect.Name}";
                    string name = Language.GetTextValue($"{locPath}.DisplayName");
                    string tooltip = Language.GetTextValue($"{locPath}.Tooltip");
                    

                    string text = $"{name}\n{tooltip}";
                    if (!text.Contains($"Mods.{effect.Mod.Name}"))
                    {
                        Utils.DrawBorderString(
                            spriteBatch,
                            text,
                            textPosition,
                            Color.White);
                    }
                }
            }
            if (item > -1)
            {
                Vector2 scaleOffset = -Vector2.One * 10 * (scale - 1);
                Utils.DrawBorderString(spriteBatch, $"[i:{item}]", itemPos + scaleOffset, Color.White, scale);
            }

        }
    }
    public class AvailableSkillBox : ActiveSkillBox
    {

        public AccessoryEffect Effect;

        public AvailableSkillBox(AccessoryEffect effect, string mod, float width)
        {
            Effect = effect;
            Mod = mod;

            Width.Set(width, 0);
            Height.Set(width, 0);
        }
        public override void CheckSlotElegible(Player player)
        {
            if (Effect != null && Effect.EffectItem(player) == null)
            {
                Effect = null;
                ActiveSkillMenu.ShouldRefresh = true;
            }

        }
        public override AccessoryEffect GetEffect() => Effect;
        public override void OnClicked()
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (ActiveSkillMenu.MouseHeldElement == null)
            {
                ActiveSkillMenu.MouseHeldElement = this;
            }
            else
            {
                ActiveSkillMenu.MouseHeldElement = null;
                ActiveSkillMenu.ShouldRefresh = true;
            }
        }
    }
    public class EquippedSkillBox : ActiveSkillBox
    {
        public int Slot;

        public EquippedSkillBox(int slot,  float width)
        {
            Slot = slot;

            Width.Set(width, 0);
            Height.Set(width, 0);
        }
        public override void CheckSlotElegible(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.ActiveSkills[Slot] != null && modPlayer.ActiveSkills[Slot].EffectItem(Main.LocalPlayer) == null)
            {
                modPlayer.ActiveSkills[Slot] = null;
                ActiveSkillMenu.ShouldRefresh = true;
                // net sync
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    modPlayer.SyncActiveSkill(Slot);
            }
        }
        public override AccessoryEffect GetEffect() => Main.LocalPlayer.FargoSouls().ActiveSkills[Slot];
        public override void OnClicked()
        {
            FargoSoulsPlayer modPlayer = Main.LocalPlayer.FargoSouls();
            if (ActiveSkillMenu.MouseHeldElement == null)
            {
                if (modPlayer.ActiveSkills[Slot] != null)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    modPlayer.ActiveSkills[Slot] = null;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        modPlayer.SyncActiveSkill(Slot);
                }
                ActiveSkillMenu.ShouldRefresh = true;
            }
            else
            {
                if (ActiveSkillMenu.MouseHeldElement is AvailableSkillBox mouseSkill)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    modPlayer.ActiveSkills[Slot] = mouseSkill.Effect;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        modPlayer.SyncActiveSkill(Slot);
                }
                ActiveSkillMenu.MouseHeldElement = null;
                ActiveSkillMenu.ShouldRefresh = true;
            }
        }
        public override void PreDraw(SpriteBatch spriteBatch, Vector2 position)
        {
            var keys = FargowiltasSouls.ActiveSkillKeys[Slot].GetAssignedKeys();
            string keyText = "";
            foreach (var key in keys)
                keyText += $"{key} ";
            Utils.DrawBorderString(spriteBatch, keyText, position + new Vector2(32, 32), Color.White);
        }
    }
}
