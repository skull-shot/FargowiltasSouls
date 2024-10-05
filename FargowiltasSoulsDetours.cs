using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Tiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls
{
    public partial class FargowiltasSouls
    {
        public void LoadDetours()
        {

            On_Player.CheckSpawn_Internal += LifeRevitalizer_CheckSpawn_Internal;
            On_Player.AddBuff += AddBuff;
            On_Player.QuickHeal_GetItemToUse += QuickHeal_GetItemToUse;
            On_Projectile.AI_019_Spears_GetExtensionHitbox += AI_019_Spears_GetExtensionHitbox;
            On_Item.AffixName += AffixName;
            On_Player.ItemCheck_Shoot += InterruptShoot;
            On_PlayerDrawLayers.DrawPlayer_27_HeldItem += DrawSwordsCorrectly;
        }

        

        public void UnloadDetours()
        {
            On_Player.CheckSpawn_Internal -= LifeRevitalizer_CheckSpawn_Internal;
            On_Player.AddBuff -= AddBuff;
            On_Player.QuickHeal_GetItemToUse -= QuickHeal_GetItemToUse;
            On_Projectile.AI_019_Spears_GetExtensionHitbox -= AI_019_Spears_GetExtensionHitbox;
            On_Item.AffixName -= AffixName;
            On_Player.ItemCheck_Shoot -= InterruptShoot;
            On_PlayerDrawLayers.DrawPlayer_27_HeldItem -= DrawSwordsCorrectly;
        }

        private static bool LifeRevitalizer_CheckSpawn_Internal(
    On_Player.orig_CheckSpawn_Internal orig,
    int x, int y)
        {
            if (orig(x, y))
                return true;

            //Main.NewText($"{x} {y}");

            int revitalizerType = ModContent.TileType<LifeRevitalizerPlaced>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -3; j <= -1; j++)
                {
                    int newX = x + i;
                    int newY = y + j;

                    if (!WorldGen.InWorld(newX, newY))
                        return false;

                    Tile tile = Framing.GetTileSafely(newX, newY);
                    if (tile.TileType != revitalizerType)
                        return false;
                }
            }

            return true;
        }
        private void DrawSwordsCorrectly(On_PlayerDrawLayers.orig_DrawPlayer_27_HeldItem orig, ref PlayerDrawSet drawinfo)
        {
            //if (SwordGlobalItem.IsBroadsword(drawinfo.heldItem))
            //{
            //    Asset<Texture2D> t = TextureAssets.Item[drawinfo.heldItem.type];
            //    Main.EntitySpriteDraw(t.Value, drawinfo.Position, )
            //}
            orig(ref drawinfo);
        }
        private void InterruptShoot(On_Player.orig_ItemCheck_Shoot orig, Player self, int i, Item sItem, int weaponDamage)
        {
            
            if (SwordGlobalItem.IsBroadsword(sItem) && sItem.TryGetGlobalItem<SwordGlobalItem>(out SwordGlobalItem sword) && !sword.VanillaShoot)
            {
                SwordPlayer mplayer = self.GetModPlayer<SwordPlayer>();

                mplayer.shouldShoot = true;
                return;
            }
            orig(self, i, sItem, weaponDamage);
        }
        private void AddBuff(
            Terraria.On_Player.orig_AddBuff orig,
            Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            FargoSoulsPlayer modPlayer = self.FargoSouls();
            if (Main.debuff[type]
                && timeToAdd > 3 //dont affect auras
                && !Main.buffNoTimeDisplay[type] //dont affect hidden time debuffs
                && !BuffID.Sets.NurseCannotRemoveDebuff[type] //only affect debuffs that nurse can cleanse
                && (modPlayer.ParryDebuffImmuneTime > 0
                    || modPlayer.ImmuneToDamage
                    || modPlayer.ShellHide
                    || modPlayer.MonkDashing > 0
                    || modPlayer.CobaltImmuneTimer > 0
                    || modPlayer.TitaniumDRBuff)
                && DebuffIDs.Contains(type))
            {
                return; //doing it this way so that debuffs previously had are retained, but existing debuffs also cannot be extended by reapplying
            }

            orig(self, type, timeToAdd, quiet, foodHack);
        }

        private Item QuickHeal_GetItemToUse(On_Player.orig_QuickHeal_GetItemToUse orig, Player self)
        {
            Item value = orig(self);

            int num3 = 58;
            if (self.useVoidBag())
            {
                num3 = 98;
            }
            for (int i = 0; i < num3; i++)
            {
                Item item = ((i >= 58) ? self.bank4.item[i - 58] : self.inventory[i]);
                if (item.stack <= 0 || item.type <= ItemID.None || !item.potion || item.healLife <= 0)
                {
                    continue;
                }
                if (self.HasEffect<ShroomiteMushroomPriority>() && item.type == ItemID.Mushroom)
                    return item;
            }

            return value;
        }

        // Tungsten Enchant extended spear hitbox fix
        public static bool AI_019_Spears_GetExtensionHitbox(On_Projectile.orig_AI_019_Spears_GetExtensionHitbox orig, Projectile self, out Rectangle extensionBox)
        {
            bool ret = orig(self, out extensionBox);
            if (ret)
            {
                Vector2 dif = extensionBox.Center.ToVector2() - self.Center;
                Vector2 extra = dif * (self.scale - 1);
                extensionBox.Location += extra.ToPoint();
                extensionBox.Inflate(0, (int)(extensionBox.Height * 0.5f * (self.scale - 1)));
            }
            return ret;
        }

        public static string AffixName(On_Item.orig_AffixName orig, Item self)
        {
            string text = orig(self);
            if (self.ModItem != null && self.ModItem is SoulsItem soulsItem)
            {
                text = text.ArticlePrefixAdjustmentString(soulsItem.Articles.ToArray());
            }
            return text;
        }
    }
}
