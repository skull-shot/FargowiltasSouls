using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantPillar : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.AprilFools ?
            "FargowiltasSouls/Content/Bosses/MutantBoss/MutantPillar_April" :
            "FargowiltasSouls/Assets/Textures/Content/Projectiles/Eternity/Bosses/LunaticCultist/CelestialPillar";

        private int target = -1;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.FargoSouls().TimeFreezeImmune = true;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(target);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            target = reader.ReadInt32();
        }

        public override bool? CanDamage()
        {
            return Projectile.alpha == 0;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                var type = (int)Projectile.ai[0] switch
                {
                    0 => 242,
                    1 => 127,
                    2 => 229,
                    _ => 135,
                };
                for (int index = 0; index < 50; ++index)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.velocity *= 10f;
                    dust.fadeIn = 1f;
                    dust.scale = 1 + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                    if (!Main.rand.NextBool(3))
                    {
                        dust.noGravity = true;
                        dust.velocity *= 3f;
                        dust.scale *= 2f;
                    }
                }
            }
            if (Projectile.alpha > 0)
            {
                Projectile.velocity.Y += 5f / 120f;
                Projectile.rotation += Projectile.velocity.Length() / 20f * 2f;
                Projectile.localAI[1] += Projectile.velocity.Y;
                Projectile.alpha -= 2;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    if (target != -1)
                    {
                        SoundEngine.PlaySound(FargosSoundRegistry.ThrowShort with { Pitch = -0.5f }, Projectile.Center);
                        //SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
                        Projectile.velocity = Main.player[target].Center - Projectile.Center;
                        float distance = Projectile.velocity.Length();
                        Projectile.velocity.Normalize();
                        const float speed = 32f;
                        Projectile.velocity *= speed;
                        Projectile.timeLeft = (int)(distance / speed);
                        Projectile.netUpdate = true;
                        return;
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    NPC npc = Main.npc[(int)Projectile.ai[1]];
                    target = npc.target;
                    Projectile.Center = npc.Center;
                    Projectile.position.Y += Projectile.localAI[1];
                }

                if (target >= 0 && Main.player[target].active && !Main.player[target].dead)
                {
                    if (Projectile.alpha < 100)
                    {
                        Projectile.rotation = Projectile.rotation.AngleLerp(
                          (Main.player[target].Center - Projectile.Center).ToRotation(), (255 - Projectile.alpha) / 255f * 0.08f);
                    }
                }
                else
                {
                    int possibleTarget = Player.FindClosest(Projectile.Center, 0, 0);
                    if (possibleTarget != -1)
                    {
                        target = possibleTarget;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            Projectile.frame = (int)Projectile.ai[0];
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (target.mount.Active)
                target.mount.Dismount(target);
            target.velocity.X = Projectile.velocity.X < 0 ? -15f : 15f;
            target.velocity.Y = -10f;
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 240);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
            Projectile.timeLeft = 0;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.LocalPlayer.active && !Main.dedServ)
                ScreenShakeSystem.StartShake(10, shakeStrengthDissipationIncrement: 10f / 30);

            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
            var type = (int)Projectile.ai[0] switch
            {
                0 => 242,
                1 => 127,
                2 => 229,
                _ => 135,
            };
            for (int index = 0; index < 80; ++index)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
                dust.velocity *= 10f;
                dust.fadeIn = 1f;
                dust.scale = 1 + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                if (!Main.rand.NextBool(3))
                {
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    dust.scale *= 2f;
                }
            }
            if (FargoSoulsUtil.HostCheck)
            {
                int fragmentDuration = 240;
                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>())
                    && Main.npc[EModeGlobalNPC.mutantBoss].ai[0] == 19)
                {
                    fragmentDuration = (int)Main.npc[EModeGlobalNPC.mutantBoss].localAI[0];
                }

                const int max = 24;
                const float rotationInterval = 2f * (float)Math.PI / max;
                float speed = WorldSavingSystem.MasochistModeReal ? 5.5f : 5f;
                for (int j = 0; j < 4; j++)
                {
                    Vector2 vel = new Vector2(0f, speed * (j + 0.5f)).RotatedBy(Projectile.rotation);
                    for (int i = 0; i < max; i++)
                    {
                        int p = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, vel.RotatedBy(rotationInterval * i),
                            ModContent.ProjectileType<MutantFragment>(), Projectile.damage / 2, 0f, Main.myPlayer, Projectile.ai[0]);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = fragmentDuration;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255 - Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 3)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}