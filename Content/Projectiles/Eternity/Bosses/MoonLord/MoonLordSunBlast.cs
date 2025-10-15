using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Bosses.Champions.Earth;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity
{
    public class MoonLordSunBlast : EarthChainBlast
    {
        public override string Texture => "Terraria/Images/Projectile_687";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Sun Blast");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 70;
            Projectile.height = 70;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC npc
                && (npc.type == NPCID.GolemFistLeft || npc.type == NPCID.GolemFistRight))
            {
                Projectile.localAI[2] = 1;
                CooldownSlot = -1;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[2] = reader.ReadSingle();
        }

        public override bool? CanDamage()
        {
            return Projectile.frame == 3 || Projectile.frame == 4;
        }

        public override void AI()
        {
            if (Projectile.position.HasNaNs())
            {
                Projectile.Kill();
                return;
            }
            /*Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 0, new Color(), 1f)];
            dust.position = Projectile.Center;
            dust.velocity = Vector2.Zero;
            dust.noGravity = true;
            dust.noLight = true;*/

            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame--;
                    Projectile.Kill();
                    return;
                }
                if (Projectile.frame == 3)
                    Projectile.FargoSouls().GrazeCD = 0;
            }
            //if (++Projectile.ai[0] > Main.projFrames[Projectile.type] * 3) Projectile.Kill();

            if (Projectile.localAI[1] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);
                Projectile.position = Projectile.Center;
                Projectile.scale = Projectile.localAI[2] == 0 ? Main.rand.NextFloat(1.5f, 4f) //ensure no gaps
                    : 3f;
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.width = (int)(Projectile.width * Projectile.scale);
                Projectile.height = (int)(Projectile.height * Projectile.scale);
                Projectile.Center = Projectile.position;
            }

            if (++Projectile.localAI[1] == 6 && Projectile.ai[1] > 0 && FargoSoulsUtil.HostCheck)
            {
                Projectile.ai[1]--;

                Vector2 baseDirection = Projectile.ai[0].ToRotationVector2();
                float random = MathHelper.ToRadians(15);

                if (Projectile.localAI[0] != 2f)
                {
                    //spawn stationary blasts
                    float stationaryPersistence = Math.Min(5, Projectile.ai[1]); //stationaries always count down from this
                    int p = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + Main.rand.NextVector2Circular(20, 20), Vector2.Zero, Projectile.type,
                        Projectile.damage, 0f, Projectile.owner, Projectile.ai[0], stationaryPersistence);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].localAI[0] = 1f; //only make more stationaries, don't propagate forward
                }

                //propagate forward
                if (Projectile.localAI[0] != 1f)
                {
                    //10f / 7f is to compensate for shrunken hitbox
                    float length = Projectile.width / Projectile.scale * 10f / 7f;
                    Vector2 offset = length * baseDirection.RotatedBy(Main.rand.NextFloat(-random, random));
                    int p = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + offset, Vector2.Zero, Projectile.type,
                          Projectile.damage, 0f, Projectile.owner, Projectile.ai[0], Projectile.ai[1]);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].localAI[0] = Projectile.localAI[0];
                }
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<CosmosChampion>()))
            {
                modifiers.ScalingArmorPenetration += 0.25f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>()))
            {
                target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 240);
                if (WorldSavingSystem.EternityMode)
                    target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
            }
            else
            {
                target.AddBuff(ModContent.BuffType<DaybrokenBuff>(), 120);
                target.AddBuff(BuffID.OnFire, 300);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 600);
        } 

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 100) * Projectile.Opacity;
    }
}

