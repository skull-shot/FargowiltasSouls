using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.Minions
{
    public class BigBrainProj : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Weapons/Minions", Name);
        public const int MaxMinionSlots = 16;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 112;
            Projectile.height = 104;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.dead) modPlayer.BigBrainMinion = false;
            if (modPlayer.BigBrainMinion) Projectile.timeLeft = 2;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 4;
            }

            float slotsModifier = MaxMinionSlots / 7f * Math.Min(Projectile.minionSlots / MaxMinionSlots, 1f);

            NPC targetnpc = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1000, center: Main.player[Projectile.owner].MountedCenter));
            bool targetting = targetnpc != null; //targetting code, prioritize targetted npcs, then look for closest if none is found

            float spinModifier = 0.1f + 0.3f * slotsModifier;
            if (!targetting)
                spinModifier /= 2;
            Projectile.ai[0] += spinModifier;
            Projectile.alpha = (int)(Math.Cos(Projectile.ai[0] / 0.4f * MathHelper.TwoPi / 180) * 60) + 60;

            float oldScale = Projectile.scale;
            Projectile.scale = 0.75f + 0.5f * slotsModifier;

            Projectile.position = Projectile.Center;
            Projectile.width = (int)(Projectile.width * Projectile.scale / oldScale);
            Projectile.height = (int)(Projectile.height * Projectile.scale / oldScale);
            Projectile.Center = Projectile.position;

            if (targetting)
            {
                float frequency = 60f / Projectile.minionSlots;
                if (++Projectile.localAI[0] > frequency)
                {
                    Projectile.localAI[0] = 0;

                    if (Projectile.owner == Main.myPlayer)
                    {
                        const float speed = 18f;
                        int damage = Projectile.damage;
                        int type = ModContent.ProjectileType<BigBrainIllusion>();

                        //Vector2 spawnpos = targetnpc.Center + Main.rand.NextVector2CircularEdge(150, 150);
                        //Projectile.NewProjectile(spawnpos, speed * Vector2.Normalize(targetnpc.Center - spawnpos), type, damage, Projectile.knockBack, Projectile.owner, Projectile.scale);

                        Vector2 spawnFromMe = Main.player[Projectile.owner].Center + (Projectile.Center - Main.player[Projectile.owner].Center).RotatedBy(MathHelper.TwoPi / 4 * Main.rand.Next(4));
                        Vector2 vel = speed * Vector2.Normalize(targetnpc.Center + targetnpc.velocity * 15 - spawnFromMe);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnFromMe, vel, type, damage, Projectile.knockBack, Projectile.owner, Projectile.scale);
                    }
                }
            }

            Projectile.Center = player.Center + new Vector2(0, (200 + Projectile.alpha) * Projectile.scale).RotatedBy(Projectile.ai[1] + Projectile.ai[0] / MathHelper.TwoPi);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= Projectile.scale;
        }

        /*public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 6;
        }*/

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i <= 3; i++) //simulate collision of 4 Projectiles orbiting because i didnt want to make orbiting illusions seperate Projectiles, also makes collision with scale changes better
            {
                Player player = Main.player[Projectile.owner];
                Vector2 newCenter = player.Center + new Vector2(0, (200 + Projectile.alpha) * Projectile.scale).RotatedBy(i * MathHelper.PiOver2 + Projectile.ai[1] + Projectile.ai[0] / MathHelper.TwoPi);
                int width = (int)(Projectile.scale * Projectile.width);
                int height = (int)(Projectile.scale * Projectile.height);
                Rectangle newprojhitbox = new((int)newCenter.X - width / 2, (int)newCenter.Y - height / 2, width, height);
                if (newprojhitbox.Intersects(targetHitbox))
                    return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameheight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle rectangle = new(0, Projectile.frame * frameheight, texture.Width, frameheight);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, rectangle.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            for (int i = 1; i <= 3; i++)
            {
                Player player = Main.player[Projectile.owner];
                Vector2 newCenter = player.Center + new Vector2(0, (200 + Projectile.alpha) * Projectile.scale).RotatedBy(i * MathHelper.PiOver2 + Projectile.ai[1] + Projectile.ai[0] / MathHelper.TwoPi);
                Color newcolor = lightColor * Projectile.Opacity * Projectile.Opacity * Projectile.Opacity * (Main.mouseTextColor / 255f);

                Main.EntitySpriteDraw(texture, newCenter - Main.screenPosition, new Rectangle?(rectangle), newcolor, Projectile.rotation, rectangle.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}