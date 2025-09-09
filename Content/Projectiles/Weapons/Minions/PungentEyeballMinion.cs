using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.Minions
{
    public class PungentEyeballMinion : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Weapons/Minions", Name);
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 30;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.FargoSouls().PungentEyeballMinion)
                Projectile.timeLeft = 2;

            Vector2 vector2_1 = new(0f, -85f); //movement code
            Vector2 vector2_2 = player.MountedCenter + vector2_1;
            float num1 = Vector2.Distance(Projectile.Center, vector2_2);
            if (num1 > 1000) //teleport when out of range
                Projectile.Center = player.Center + vector2_1;
            Vector2 vector2_3 = vector2_2 - Projectile.Center;
            float num2 = 4f;
            if (num1 < num2)
                Projectile.velocity *= 0.25f;
            if (vector2_3 != Vector2.Zero)
            {
                if (vector2_3.Length() < num2)
                    Projectile.velocity = vector2_3;
                else
                    Projectile.velocity = vector2_3 * 0.1f;
            }

            const float rotationModifier = 0.08f;
            const float chargeTime = 360f;
            if (Projectile.localAI[1] > 0)
            {
                Projectile.localAI[1]--;
                if (Projectile.owner == Main.myPlayer)
                    Projectile.netUpdate = true;
            }
            if (player.HeldItem.IsWeapon() && player.controlUseItem)
            {
                if (Projectile.localAI[1] == 0)
                {
                    Projectile.localAI[0]++;
                    if (player.whoAmI == Main.myPlayer)
                        CooldownBarManager.Activate("FleshLumpMinionCharge", FargoAssets.GetTexture2D("Content/Items/Accessories/Eternity", "LithosphericCluster").Value, Color.Purple, () => Projectile.localAI[0] / chargeTime, displayAtFull: true, activeFunction: Projectile.Alive, animationFrames: 5);
                }
                if (Projectile.localAI[0] == chargeTime)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.netUpdate = true;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -1);
                    }
                }
            }
            else
            {
                if (Projectile.localAI[0] >= chargeTime) //full charge
                {
                    Projectile.localAI[1] = 120f;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.netUpdate = true;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedBy(Projectile.rotation), ModContent.ProjectileType<PhantasmalDeathrayPungent>(), Projectile.damage, Projectile.knockBack * 2, Projectile.owner, Projectile.identity);
                    }
                }
                else if (Projectile.localAI[0] >= 30) //partial charge
                {
                    float timeMult = Projectile.localAI[0] / chargeTime;
                    timeMult = LumUtils.Saturate(timeMult);
                    Projectile.localAI[1] = (int)(120 * timeMult);
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.netUpdate = true;
                }
                else if (Projectile.localAI[1] % 10 == 0 && Projectile.localAI[1] != 0 && player.ownedProjectileCounts[ModContent.ProjectileType<PhantasmalDeathrayPungent>()] < 1)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 10f * Projectile.SafeDirectionTo(Main.MouseWorld).RotatedByRandom(MathHelper.ToRadians(4)), ProjectileID.SpectreWrath, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    if (p != Main.maxProjectiles)
                    {
                        Main.projectile[p].DamageType = DamageClass.Summon;
                        Main.projectile[p].penetrate = 1;
                    }
                }
                Projectile.localAI[0] = 0;
            }

            if (Projectile.owner == Main.myPlayer) //rotation
            {
                if (Projectile.localAI[0] != 0 || Projectile.localAI[1] != 0)
                {
                    Projectile.spriteDirection = Math.Abs(MathHelper.WrapAngle(Projectile.rotation)) > MathHelper.PiOver2 ? -1 : 1;
                    Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Projectile.Center).ToRotation(), rotationModifier);
                }
                else
                {
                    Projectile.spriteDirection = player.direction;
                    float rotmod2;
                    if ((Projectile.rotation < 0.1 && Projectile.rotation > -0.1) || Projectile.rotation > 3 || Projectile.rotation < -3)
                        rotmod2 = 1;
                    else rotmod2 = rotationModifier; //ideally itll smoothly rotate back into place after finishing firing, but rotate instantly if idle otherwise
                    Projectile.rotation = Projectile.rotation.AngleLerp(player.direction == 1 ? 0 : MathHelper.Pi, rotmod2);
                }
                Main.NewText($"{Projectile.localAI[0]}, {Projectile.localAI[1]}, {Projectile.rotation}, {Projectile.frame}, {Projectile.frameCounter}");
            }

            if (Projectile.localAI[0] <= chargeTime && Projectile.localAI[1] == 0)
            {
                if (Projectile.frameCounter++ > 4)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 5)
                        Projectile.frame = 0;
                }
            }
            else
            {
                if (Projectile.frame <= 5)
                    Projectile.frame += 5;
                if (Projectile.frameCounter++ > 4)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 10)
                        Projectile.frame = 6;
                }
            }
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rotation = Projectile.rotation;
            if (Projectile.spriteDirection < 0)
                rotation += MathHelper.Pi;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}