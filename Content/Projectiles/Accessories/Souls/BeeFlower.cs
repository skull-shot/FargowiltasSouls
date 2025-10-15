using System.Collections.Generic;
using Fargowiltas.Content.Projectiles;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class BeeFlower : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Accessories/Souls", Name);
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.DontAttachHideToAlpha[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.scale = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 15;
            Projectile.penetrate = 1;
            Projectile.light = 1;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
            Projectile.hide = true;
        }
        public bool LifeFlower => Projectile.ai[2] == 1;
        public override bool? CanDamage() => Projectile.frame == Main.projFrames[Projectile.type] - 1; //only damage when fully grown
        public override void AI()
        {
            Projectile.GetGlobalProjectile<FargoGlobalProjectile>().lowRender = false;

            if (Projectile.frame < Main.projFrames[Projectile.type] - 1) //petalinate
            {
                if (++Projectile.frameCounter % 60 == 0)
                    Projectile.frame++;
            }
            else
            {
                if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && Main.LocalPlayer.Hitbox.Intersects(Projectile.Hitbox))
                {
                    if (LifeFlower)
                    {
                        Main.LocalPlayer.AddBuff(ModContent.BuffType<AmbrosiaBuff>(), 60 * 6);
                        Main.LocalPlayer.wingTime = Main.LocalPlayer.wingTimeMax;
                    }
                    else
                    {
                        BeeSwarm();
                        Main.LocalPlayer.AddBuff(BuffID.Honey, 60 * 15);
                    }
                    FargoGlobalItem.OnRetrievePickup(Main.LocalPlayer);
                    Projectile.Kill();
                }
                if (LifeFlower && Main.player[Projectile.owner].HasEffect<SpiderEffect>())
                    Congregate();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!LifeFlower) BeeSwarm();
            else CactusBomb();
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.Center);
            FargoSoulsUtil.DustRing(Projectile.Center, 4, DustID.Honey2, 4, scale: LifeFlower ? 2 : 1, noLight: true);
            FargoSoulsUtil.DustRing(Projectile.Center, 6, DustID.JungleTorch, 6, scale: LifeFlower ? 2 : 1);
        }

        public void BeeSwarm()
        {
            int damage = (int)(Projectile.damage * 0.75f);
            for (int i = 0; i < 7; i++)
            {
                Vector2 pos = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, (pos - Projectile.Center) / 12,
                    Main.LocalPlayer.beeType(), Main.LocalPlayer.beeDamage(damage), Main.LocalPlayer.beeKB(Projectile.knockBack), Main.LocalPlayer.whoAmI);
                if (p != Main.maxProjectiles)
                {
                    Main.projectile[p].DamageType = Projectile.DamageType;
                    Main.projectile[p].usesIDStaticNPCImmunity = true;
                    Main.projectile[p].idStaticNPCHitCooldown = 10;
                }
            }
        }
        public void CactusBomb()
        {
            if (Main.player[Projectile.owner].HasEffect<CactusEffect>())
            {
                for (int i = 0; i < 24; i++)
                {
                    int spread = (int)MathHelper.Lerp(0.9f, 4.5f, i);
                    Vector2 pos = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Vector2.UnitX.RotatedBy(spread + Main.rand.NextFloat(-0.2f, 0.2f)) * (4 + Main.rand.NextFloat(-0.5f, 0.5f)) * 4, ModContent.ProjectileType<CactusNeedle>(), Projectile.damage, 5f);
                    if (p != Main.maxProjectiles)
                    {
                        Projectile proj = Main.projectile[p];
                        if (proj != null && proj.active)
                        {
                            proj.usesIDStaticNPCImmunity = false;
                            proj.penetrate = 1;
                        }
                    }
                }
            }
        }
        public void Congregate()
        {
            // up to 4 bugs spawning in 2s intervals
            if (Projectile.timeLeft % 120 == 0 && Projectile.ai[1] < 4 && Projectile.owner == Main.myPlayer)
            {
                Projectile.ai[1]++;
                int type = Main.rand.NextFromList(ModContent.ProjectileType<LifeForceWasp>(), ModContent.ProjectileType<LifeForceSpider>(), ModContent.ProjectileType<LifeForceBeetle>());
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.Pi) * 5, type, Projectile.damage / 3, Projectile.knockBack, Projectile.owner, Projectile.identity, Projectile.ai[1]);
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindProjectiles.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Projectile.ai[2] == 1 ? FargoAssets.GetTexture2D("Content/Projectiles/Accessories/Souls", "BeeFlowerForce").Value : Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            int num156 = texture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture, drawPosition, rectangle, color, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}