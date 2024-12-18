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

namespace FargowiltasSouls.Content.UI.Elements
{
    public class ActiveSkillBox : UIPanel
    {
        public static DynamicSpriteFont Font => Terraria.GameContent.FontAssets.ItemStack.Value;

        public AccessoryEffect Effect;
        public string Mod;

        public ActiveSkillBox(AccessoryEffect effect, string mod, float width)
        {
            Effect = effect;
            Mod = mod;

            Width.Set(width, 0);
            Height.Set(width, 0);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            CheckSlotElegible(Main.LocalPlayer);
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
        public void CheckSlotElegible(Player player)
        {
            if (Effect != null && Effect.EffectItem(player) == null)
            {
                Effect = null;
                ActiveSkillMenu.ShouldRefresh = true;
            }

        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CheckSlotElegible(Main.LocalPlayer);
            Vector2 position = GetDimensions().Position();
            int item = -1;

            if (Effect != null)
            {
                if (Effect.ToggleItemType > 0)
                    item = Effect.ToggleItemType;
                else if (Effect.EffectItem(Main.LocalPlayer).type > ItemID.None)
                    item = Effect.EffectItem(Main.LocalPlayer).type;
            }
            Vector2 itemPos = position + new Vector2(9, 9);
            if (ActiveSkillMenu.MouseHeldElement == this)
                itemPos = Main.MouseScreen - new Vector2(9, 9);
            if (item > -1)
                Utils.DrawBorderString(spriteBatch, $"[i:{item}]", itemPos, Color.White);
            if (ContainsPoint(Main.MouseScreen) && Main.mouseLeft && Main.mouseLeftRelease)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                //modPlayer.Toggler.Toggles[Effect].ToggleBool = !modPlayer.Toggler.Toggles[Effect].ToggleBool;
                //if (Main.netMode == NetmodeID.MultiplayerClient)
                //    modPlayer.SyncToggle(Effect);
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
    }
    public class EquippedSkillBox : UIPanel
    {
        public static DynamicSpriteFont Font => Terraria.GameContent.FontAssets.ItemStack.Value;

        public int Slot;
        public string Mod;

        public EquippedSkillBox(int slot,  float width)
        {
            Slot = slot;

            Width.Set(width, 0);
            Height.Set(width, 0);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            CheckSlotElegible(Main.LocalPlayer);
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
        public void CheckSlotElegible(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.ActiveSkills[Slot] != null && modPlayer.ActiveSkills[Slot].EffectItem(Main.LocalPlayer) == null)
            {
                modPlayer.ActiveSkills[Slot] = null;
                ActiveSkillMenu.ShouldRefresh = true;
                // net sync
            }
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CheckSlotElegible(Main.LocalPlayer);
            AccessoryEffect effect = Main.LocalPlayer.FargoSouls().ActiveSkills[Slot];
            base.DrawSelf(spriteBatch);
            var dims = GetDimensions();
            Vector2 position = dims.Position();
            int item = -1;

            if (effect != null)
            {
                if (effect.ToggleItemType > 0)
                    item = effect.ToggleItemType;
                else if (effect.EffectItem(Main.LocalPlayer).type > ItemID.None)
                    item = effect.EffectItem(Main.LocalPlayer).type;

                Vector2 itemPos = position + new Vector2(9, 9);
                if (ActiveSkillMenu.MouseHeldElement == this)
                    itemPos = Main.MouseScreen - new Vector2(9, 9);
                if (item > -1)
                    Utils.DrawBorderString(spriteBatch, $"[i:{item}]", itemPos, Color.White);
            }

            var keys = FargowiltasSouls.ActiveSkillKeys[Slot].GetAssignedKeys();
            string keyText = "";
            foreach (var key in keys)
                keyText += $"{key} ";
            Utils.DrawBorderString(spriteBatch, keyText, position + new Vector2(32, 32), Color.White);
            if (ContainsPoint(Main.MouseScreen) && Main.mouseLeft && Main.mouseLeftRelease)
            {
                FargoSoulsPlayer modPlayer = Main.LocalPlayer.FargoSouls();
                //modPlayer.Toggler.Toggles[Effect].ToggleBool = !modPlayer.Toggler.Toggles[Effect].ToggleBool;
                //if (Main.netMode == NetmodeID.MultiplayerClient)
                //    modPlayer.SyncToggle(Effect);
                if (ActiveSkillMenu.MouseHeldElement == null)
                {
                    if (modPlayer.ActiveSkills[Slot] != null)
                    {
                        SoundEngine.PlaySound(SoundID.MenuTick);
                        modPlayer.ActiveSkills[Slot] = null;
                    }
                    
                    ActiveSkillMenu.ShouldRefresh = true;
                }
                else
                {
                    if (ActiveSkillMenu.MouseHeldElement is ActiveSkillBox mouseSkill)
                    {
                        SoundEngine.PlaySound(SoundID.MenuTick);
                        modPlayer.ActiveSkills[Slot] = mouseSkill.Effect;
                    }
                    ActiveSkillMenu.MouseHeldElement = null;
                    ActiveSkillMenu.ShouldRefresh = true;
                }
                
            }
        }
    }
}
