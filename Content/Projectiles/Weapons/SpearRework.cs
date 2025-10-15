﻿using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons
{
    public class SpearRework : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public int SwingDirection = 1;

        public static List<int> ReworkedSpears =
        [
                ProjectileID.Spear,
                ProjectileID.AdamantiteGlaive,
                ProjectileID.CobaltNaginata,
                ProjectileID.MythrilHalberd,
                ProjectileID.OrichalcumHalberd,
                ProjectileID.PalladiumPike,
                ProjectileID.TitaniumTrident,
                ProjectileID.Trident,
                ProjectileID.ObsidianSwordfish,
                ProjectileID.Swordfish,
                ProjectileID.ChlorophytePartisan
            ];
        public override void PostAI(Projectile projectile)
        {
            if (WorldSavingSystem.EternityMode && SoulConfig.Instance.WeaponReworks)
            {
                if (ReworkedSpears.Contains(projectile.type))
                {
                    ReworkedSpearSwing(projectile, ref SwingDirection);
                }
            }
        }
        public static void ReworkedSpearSwing(Projectile projectile, ref int swingDirection)
        {
            Texture2D tex = (Texture2D)TextureAssets.Projectile[projectile.type];
            float HoldoutRangeMax = (float)tex.Size().Length() * projectile.scale; //since sprite is diagonal
            float HoldoutRangeMin = (float)projectile.Size.Length(); //(float)-tex.Size().Length() / 4 * projectile.scale; 
            Player player = Main.player[projectile.owner];

            int duration = (int)(player.itemAnimationMax / 1.5f);
            int WaitTime = player.itemAnimationMax / 5;
            player.heldProj = projectile.whoAmI;
            projectile.spriteDirection = player.direction;
            if (projectile.ai[1] == 0)
                swingDirection = Main.rand.NextBool(2) ? 1 : -1;
            float Swing = 13; //higher value = less swing
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = player.itemAnimationMax; //only hit once per swing
                                                                      //projectile.ai[1] is time from spawn
                                                                      //projectile.ai[0] is extension, between 0 and 1
            if (projectile.timeLeft > player.itemAnimationMax)
            {
                projectile.timeLeft = player.itemAnimationMax;
            }
            if (projectile.ai[1] <= duration / 2)
            {
                projectile.ai[0] = projectile.ai[1] / (duration / 2);
                projectile.velocity = projectile.velocity.RotatedBy(swingDirection * projectile.spriteDirection * -Math.PI / (Swing * player.itemAnimationMax));
            }
            else if (projectile.ai[1] <= duration / 2 + WaitTime)
            {
                projectile.ai[0] = 1;
                projectile.velocity = projectile.velocity.RotatedBy(swingDirection * projectile.spriteDirection * (1.5 * duration / WaitTime) * Math.PI / (Swing * player.itemAnimationMax)); //i know how wacky this looks
            }
            else //backswing
            {
                //projectile.friendly = false; //no hit on backswing
                projectile.ai[0] = (duration + WaitTime - projectile.ai[1]) / (duration / 2);
                projectile.velocity = projectile.velocity.RotatedBy(swingDirection * projectile.spriteDirection * -Math.PI / (Swing * player.itemAnimationMax));
            }
            //if (projectile.ai[1] == duration / 2)
            //SoundEngine.PlaySound(SoundID.Item1, player.Center);

            projectile.ai[1]++;
            //projectile.velocity = Vector2.Normalize(projectile.velocity); //store direction
            projectile.Center = player.MountedCenter + Vector2.SmoothStep(Vector2.Normalize(projectile.velocity) * HoldoutRangeMin, Vector2.Normalize(projectile.velocity) * HoldoutRangeMax, projectile.ai[0]);
            projectile.position -= projectile.velocity;

            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation += MathHelper.ToRadians(45f);
            }
            else
            {
                projectile.rotation += MathHelper.ToRadians(135f);
            }


            //extra effects
            switch (projectile.type)
            {
                /*
                case ProjectileID.ChlorophytePartisan:
                    {
                        if (projectile.ai[1] == duration / 2 + WaitTime * 2 / 3 && FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity * 5, ProjectileID.SporeCloud, projectile.damage / 3, projectile.knockBack / 3, Main.myPlayer);
                        }
                        break;
                    }
                */
                    /*
                case ProjectileID.TheRottedFork:
                    {
                        break;
                    }
                    */
            }
        }

        public static float OrichalcumDoTDamageModifier(float lifeRegen)
        {
            if (lifeRegen > 0)
                return 1f;
            float result = 1f - lifeRegen * 0.001f;
            result = MathHelper.Clamp(result, 1f, 2f);
            return result;
        }
    }
}
