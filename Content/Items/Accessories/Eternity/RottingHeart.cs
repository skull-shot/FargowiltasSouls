using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
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
    [LegacyName("DarkenedHeart")]
    public class RottingHeart : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            player.buffImmune[ModContent.BuffType<RottingBuff>()] = true;
            player.moveSpeed += 0.1f;
            modPlayer.RottingHeartItem = Item;
            player.AddEffect<RottingHeartEaters>(Item);
            if (modPlayer.RottingHeartCD > 0)
                modPlayer.RottingHeartCD--;
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Default;
            tooltipColor = null;
            scaling = null;
            return RottingHeartEaters.BaseDamage(Main.LocalPlayer);
        }
    }
    public class RottingHeartEaters : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<PureHeartHeader>();
        public override int ToggleItemType => ModContent.ItemType<RottingHeart>();
        public override bool ExtraAttackEffect => true;
        public static int BaseDamage (Player player) => (int)((player.FargoSouls().PureHeart? 35 : 13) * player.ActualClassDamage(DamageClass.Generic));
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.WeaponUseTimer > 0 && modPlayer.RottingHeartCD <= 0 && !player.FargoSouls().PureHeart)
            {
                modPlayer.RottingHeartCD = 25;

                for (int i = 0; i < 300; i++) // 300 attempts
                {
                    Vector2 pos = player.Center + Main.rand.NextVector2Circular(16 * 18, 16 * 18);
                    Point tilePos = LumUtils.FindGroundVertical(pos.ToTileCoordinates());
                    if (WorldGen.SolidTile(tilePos) || WorldGen.SolidTile(new Point(tilePos.X, tilePos.Y - 1)))
                    {
                        pos = tilePos.ToWorldCoordinates();
                        if (pos.Distance(player.Center) < 16 * 30)
                        {
                            float yDif = player.Center.Y - pos.Y;
                            while (pos.Y > player.Center.Y && Collision.SolidCollision(pos - Vector2.UnitY * yDif, 1, (int)Math.Abs(yDif) + 1))
                            {
                                pos.Y -= 16;
                                yDif = player.Center.Y - pos.Y;
                            }
                            pos.Y += 16;
                                
                            EaterAttack(pos + pos.DirectionTo(player.Center) * 20, player);
                            break;
                        }
                    }
                }
            }
        }
        public static void EaterAttack(Vector2 pos, Player player)
        {
            //SoundEngine.PlaySound(SoundID.NPCHit1, pos); blegh
            for (int index1 = 0; index1 < 4; ++index1)
            {
                int index2 = Dust.NewDust(pos - player.Size / 2, player.width, player.height, DustID.ScourgeOfTheCorruptor, 0.0f, 0.0f, 0, new Color(), 1f);
                Dust dust = Main.dust[index2];
                dust.scale *= 1.1f;
                Main.dust[index2].noGravity = true;
            }
            for (int index1 = 0; index1 < 6; ++index1)
            {
                int index2 = Dust.NewDust(pos - player.Size / 2, player.width, player.height, DustID.ScourgeOfTheCorruptor, 0.0f, 0.0f, 0, new Color(), 1f);
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 2.5f;
                Dust dust2 = Main.dust[index2];
                dust2.scale *= 0.8f;
                Main.dust[index2].noGravity = true;
            }
            Vector2 vel = pos.DirectionTo(player.Center).RotatedByRandom(MathHelper.PiOver2 * 0.7f) * Main.rand.NextFloat(6, 10);
            int p = Projectile.NewProjectile(player.GetSource_EffectItem<RottingHeartEaters>(), pos.X, pos.Y, vel.X, vel.Y, ProjectileID.TinyEater, BaseDamage(player), 1.75f, player.whoAmI);
            if (p.IsWithinBounds(Main.maxProjectiles))
            {
                Main.projectile[p].DamageType = DamageClass.Default;
            }
        }

    }
    public class TinyEaterGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
            => entity.type == ProjectileID.TinyEater;


        int HeartItemType = -1;
        bool fromEnch => HeartItemType != -1;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (!projectile.owner.IsWithinBounds(Main.maxPlayers))
                return;
            Player player = Main.player[projectile.owner];
            Item heartItem = player.FargoSouls().RottingHeartItem;
            if (player != null && heartItem != null && player.active && source is EntitySource_ItemUse itemSource && itemSource.Item.type == heartItem.type)
            {
                HeartItemType = heartItem.type;
            }
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (fromEnch)
            {
                if (HeartItemType != ModContent.ItemType<RottingHeart>())
                {
                    Texture2D pureSeekerTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/Textures/Misc/PureSeeker", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    FargoSoulsUtil.GenericProjectileDraw(projectile, lightColor, pureSeekerTexture);
                    return false;
                }
            }
            return base.PreDraw(projectile, ref lightColor);
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (fromEnch)
            {
                if (HeartItemType != ModContent.ItemType<RottingHeart>())
                {
                    target.AddBuff(ModContent.BuffType<SublimationBuff>(), 30);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<RottingBuff>(), 30);
                }


            }
        }
    }
}