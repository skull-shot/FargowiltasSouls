using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Armor.Styx;
using FargowiltasSouls.Content.Projectiles.Accessories;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class SparklingAdoration : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 11));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();

            if (player.AddEffect<MasoGraze>(Item))
            {
                fargoPlayer.Graze = true;
                fargoPlayer.DeviGraze = true;
            }

            fargoPlayer.DevianttHeartItem = Item;
            player.AddEffect<DevianttHearts>(Item);

            player.AddEffect<MasoGrazeRing>(Item);
            if (fargoPlayer.Graze && player.whoAmI == Main.myPlayer && player.HasEffect<MasoGrazeRing>() && player.ownedProjectileCounts[ModContent.ProjectileType<GrazeRing>()] < 1)
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GrazeRing>(), 0, 0f, Main.myPlayer);
        }
        public static double GrazeCap(FargoSoulsPlayer fargoPlayer) => 0.25 + (fargoPlayer.AbomWandItem != null ? 0.25 * 0.2 : 0) + (fargoPlayer.MutantEyeItem != null ? 0.25 * 0.25 : 0);
        public static void OnGraze(FargoSoulsPlayer fargoPlayer, int damage)
        {
            double grazeCap = GrazeCap(fargoPlayer);
            double grazeGain = 0.0125;
            grazeGain *= 0.75;
            if (fargoPlayer.AbomWandItem != null)
                grazeGain *= 2;

            fargoPlayer.DeviGrazeBonus += grazeGain;
            if (fargoPlayer.DeviGrazeBonus > grazeCap)
            {
                fargoPlayer.DeviGrazeBonus = grazeCap;
                if (fargoPlayer.StyxSet && fargoPlayer.AbomWandItem != null)
                    fargoPlayer.StyxMeter += (int)(FargoSoulsUtil.HighestDamageTypeScaling(Main.LocalPlayer, damage) * 4 * StyxCrown.StyxChargeMultiplier(fargoPlayer.Player, StyxCrown.ChargeContext.Graze)); //as if gaining the damage, times SOU crit
            }
            fargoPlayer.DeviGrazeCounter = -1; //reset counter whenever successful graze

            if (fargoPlayer.Player.whoAmI == Main.myPlayer)
                CooldownBarManager.Activate("SparklingAdorationGraze", FargoAssets.GetTexture2D("Content/Items/Accessories/Eternity", "SparklingAdoration").Value, Color.Pink, () => (float)(fargoPlayer.DeviGrazeBonus / GrazeCap(fargoPlayer)), true, 0, () => fargoPlayer.DeviGraze, 11);

            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Accessories/Graze") { Volume = 0.5f }, Main.LocalPlayer.Center);
            }

            Vector2 baseVel = Vector2.UnitX.RotatedByRandom(2 * Math.PI);
            const int max = 64; //make some indicator dusts
            for (int i = 0; i < max; i++)
            {
                Vector2 vector6 = baseVel * 3f;
                vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                //changes color when bonus is maxed
                int d = Dust.NewDust(vector6 + vector7, 0, 0, fargoPlayer.DeviGrazeBonus >= grazeCap ? 86 : 228, 0f, 0f, 0, default);
                Main.dust[d].scale = fargoPlayer.DeviGrazeBonus >= grazeCap ? 1f : 0.75f;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }
    }
    public class DevianttHearts : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<SparklingAdoration>();
        public override bool ExtraAttackEffect => true;
    }
    public class MasoGraze : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<SparklingAdoration>();
        
    }
}