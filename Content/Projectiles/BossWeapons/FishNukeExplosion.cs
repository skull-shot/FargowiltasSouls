using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using Luminance.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class FishNukeExplosion : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Empty";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fish Nuke");
            //Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.LunarFlare];
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
            Projectile.scale = 0;
            Projectile.Opacity = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;

                //SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width,
                        Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                    Main.dust[dust].velocity *= 1.4f;
                }
                /*for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0f, 0f, 0, default, 3.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                    Main.dust[d].velocity *= 4f;
                }*/
                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width,
                        Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 7f;
                    dust = Dust.NewDust(Projectile.position, Projectile.width,
                        Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dust].velocity *= 3f;
                }

                /*for (int i = 0; i < 5; i++)
                {
                    float scaleFactor9 = 0.5f;
                    if (i == 1 || i == 3) scaleFactor9 = 1f;

                    for (int j = 0; j < 4; j++)
                    {
                        int gore = Gore.NewGore(Projectile.Center,
                            default(Vector2),
                            Main.rand.Next(61, 64));

                        Main.gore[gore].velocity *= scaleFactor9;
                        Main.gore[gore].velocity.X += 1f;
                        Main.gore[gore].velocity.Y += 1f;
                    }
                }*/
            }
        }

        /*public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.whoAmI == NPCs.FargoSoulsGlobalNPC.fishBossEX)
            {
                target.life += damage;
                if (target.life > target.lifeMax)
                    target.life = target.lifeMax;
                CombatText.NewText(target.Hitbox, CombatText.HealLife, damage);
                damage = 0;
                modifiers.DisableCrit();
            }
        }*/

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //target.immune[Projectile.owner] = 0;
            /*target.AddBuff(ModContent.BuffType<OceanicMaul>(), 900);
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 900);
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 900);*/
            target.AddBuff(BuffID.Frostburn, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D flare2 = MiscTexturesRegistry.BloomFlare.Value;
            
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.5f, 0.2f);
            if (Projectile.timeLeft <= 10)
            {
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.2f);
            }
            Main.spriteBatch.Draw(flare2, Projectile.Center - Main.screenPosition, null, Color.Teal with { A = 0 } * Projectile.Opacity, Main.GlobalTimeWrappedHourly * -2f, flare2.Size() * 0.5f, Projectile.scale, 0, 0f);
            Main.spriteBatch.Draw(flare2, Projectile.Center - Main.screenPosition, null, Color.Blue with { A = 0 } * Projectile.Opacity, Main.GlobalTimeWrappedHourly * 2f, flare2.Size() * 0.5f, Projectile.scale, 0, 0f);
            //Main.spriteBatch.Draw(flare2, Projectile.Center - Main.screenPosition, null, Color.Teal with { A = 0 }, Main.GlobalTimeWrappedHourly * -4f, flare.Size() * 0.5f, Projectile.scale, 0, 0f);
            //Main.spriteBatch.Draw(flare2, Projectile.Center - Main.screenPosition, null, Color.Teal with { A = 0 }, Main.GlobalTimeWrappedHourly * 4f, flare.Size() * 0.5f, Projectile.scale, 0, 0f);
            return false;
        }
    }
}