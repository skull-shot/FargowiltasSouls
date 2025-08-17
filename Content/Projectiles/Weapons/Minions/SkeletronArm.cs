using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.Minions
{
    public class SkeletronArm : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Accessories/SupremeDeathbringerFairy", "SupremeDash");

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skeletron Hand");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.FargoSouls().SkeletronArms)
                Projectile.timeLeft = 2;

            //if (Projectile.damage == 0)
            //{
            //    Projectile.damage = 18;
            //    if (player.FargoSouls().SupremeDeathbringerFairy)
            //        Projectile.damage = 24;
            //    if (player.FargoSouls().MasochistSoul)
            //        Projectile.damage = 48;
            //    Projectile.damage = (int)(Projectile.damage * player.GetDamage(DamageClass.Summon));
            //}

            Vector2 restingPos = player.Center + new Vector2(140 * Projectile.ai[0], -40);
            NPC npc = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 350, center: player.Center));
            if (npc != null)
            {
                if (Projectile.ai[1] == -2)
                {
                    Projectile.ai[1] = Projectile.ai[0] == 1 ? 25 : 0;
                }
                Projectile.ai[2] = 1;
                restingPos = npc.Center + npc.AngleTo(player.Center).ToRotationVector2().RotatedBy(MathHelper.ToRadians(30 * -Projectile.ai[0])) * (170 + npc.width);
                Projectile.rotation = Projectile.AngleTo(npc.Center) + MathHelper.ToRadians(180) ;
                if (Projectile.ai[1] <= 0)
                {
                    if (Projectile.ai[1] == 0)
                    {
                        Projectile.velocity = -Projectile.AngleTo(npc.Center).ToRotationVector2() * 35;
                        Projectile.ai[1] = -1;
                    }
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.AngleTo(npc.Center).ToRotationVector2()*45, 0.08f);
                    if (Projectile.Hitbox.Intersects(npc.Hitbox))
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.Center);
                        Projectile.ai[1] = 40;
                        Projectile.velocity = Projectile.AngleTo(restingPos).ToRotationVector2() * 10;
                    }
                }
                else
                {
                    Projectile.ai[1]--;
                    
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.AngleTo(restingPos).ToRotationVector2() * Projectile.Distance(restingPos) / 5, 0.08f);
                }
                
            }
            else
            {
                Projectile.ai[2] = 0;
                Projectile.ai[1] = -2;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.AngleTo(restingPos).ToRotationVector2() * Projectile.Distance(restingPos)/5, 0.08f) ;
                Projectile.rotation = MathHelper.ToRadians(90);
            }
            //max distance from player
            if (Projectile.Distance(player.Center) > 300)
            {
                Projectile.Center = player.Center + Projectile.AngleFrom(player.Center).ToRotationVector2() * 300;
                
            }

            //tentacle head movement (homing)
            //Vector2 playerVel = player.position - player.oldPosition;
            //Projectile.position += playerVel;
            //Projectile.ai[0]++;
            //if (Projectile.ai[0] >= 0f)
            //{
            //    Vector2 home = player.Center;
            //    home.X -= 200f;
            //    home.Y -= 50f;
            //    Vector2 distance = home - Projectile.Center;
            //    float range = distance.Length();
            //    distance.Normalize();
            //    if (Projectile.ai[0] == 0f)
            //    {
            //        if (range > 15f)
            //        {
            //            Projectile.ai[0] = -1f; //if in fast mode, stay fast until back in range
            //            if (range > 1300f)
            //            {
            //                Projectile.Kill();
            //                return;
            //            }
            //        }
            //        else
            //        {
            //            Projectile.velocity.Normalize();
            //            Projectile.velocity *= 3f + Main.rand.NextFloat(3f);
            //            Projectile.netUpdate = true;
            //        }
            //    }
            //    else
            //    {
            //        distance /= 8f;
            //    }
            //    if (range > 120f) //switch to fast return mode
            //    {
            //        Projectile.ai[0] = -1f;
            //        Projectile.netUpdate = true;
            //    }
            //    Projectile.velocity += distance;
            //    if (range > 30f)
            //        Projectile.velocity *= 0.96f;

            //    if (Projectile.ai[0] > 90f) //attack nearby enemy
            //    {
            //        Projectile.ai[0] = 20f;
            //        NPC npc = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 400));
            //        if (npc != null)
            //        {
            //            Projectile.velocity = npc.Center - Projectile.Center;
            //            Projectile.velocity.Normalize();
            //            Projectile.velocity *= 16f;
            //            Projectile.velocity += npc.velocity / 2f;
            //            Projectile.velocity -= playerVel / 2f;
            //            Projectile.ai[0] *= -1f;
            //        }
            //        Projectile.netUpdate = true;
            //    }
            //}

            //Vector2 angle = player.Center - Projectile.Center;
            //angle.X -= 200f;
            //angle.Y += 180f;
            //Projectile.rotation = (float)Math.Atan2(angle.Y, angle.X) + (float)Math.PI / 2f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Math.Sign(target.Center.X - Main.player[Projectile.owner].Center.X);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || !player.FargoSouls().SkeletronArms)
                return false ;
            Asset<Texture2D> fist = TextureAssets.Projectile[Projectile.type];
            Asset<Texture2D> hand = Main.Assets.Request<Texture2D>("Images/NPC_" + NPCID.SkeletronHand);
            Asset<Texture2D> arm = Main.Assets.Request<Texture2D>("Images/Arm_Bone");

            Vector2 wrist = Projectile.Center + new Vector2(0, Projectile.height/2).RotatedBy(Projectile.rotation - MathHelper.PiOver2);

            float distPlayer = wrist.Distance(player.Center);
            float armlength = 130;
            float armlengthsqr = MathF.Pow(armlength, 2);
            
            float forarmAngle = MathF.Acos((MathF.Pow(distPlayer, 2))/(2*distPlayer*armlength));
            if (MathHelper.ToDegrees(forarmAngle) is float.NaN) forarmAngle = 0;
            
            //if (Projectile.ai[0] == 1)
            //Main.NewText(MathHelper.ToDegrees(forarmAngle));
            Vector2 jointPos = wrist + (wrist.AngleTo(player.Center) + forarmAngle * -Projectile.ai[0]).ToRotationVector2()*armlength;
            float armScale = (armlength-20) / arm.Height();

            Main.EntitySpriteDraw(arm.Value, wrist - Main.screenPosition, null, Projectile.GetAlpha(lightColor), wrist.AngleTo(jointPos) - MathHelper.PiOver2, new Vector2(arm.Width(), -20) / 2, new Vector2(1, armScale), SpriteEffects.None);
            Main.EntitySpriteDraw(arm.Value, jointPos - Main.screenPosition, null, Projectile.GetAlpha(lightColor), jointPos.AngleTo(player.Center) - MathHelper.PiOver2, new Vector2(arm.Width(), -10) / 2, new Vector2(1, armScale), SpriteEffects.None);
            if (Projectile.ai[2] == 0)
                Main.EntitySpriteDraw(hand.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.PiOver2 * Projectile.ai[0], hand.Size() / 2, Projectile.scale, Projectile.ai[0] == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 0);
            else
            {
                if (Projectile.ai[1] == -1)
                {
                    for (int i = 0; i < Projectile.oldPos.Length; i++)
                    {
                        Main.EntitySpriteDraw(fist.Value, Projectile.oldPos[i] + Projectile.Size/2 - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * 0.5f * (1 - (float)i / Projectile.oldPos.Length), Projectile.rotation, fist.Size() / 2, Projectile.scale, Projectile.ai[0] == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 0);
                    }
                }
                Main.EntitySpriteDraw(fist.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, fist.Size() / 2, Projectile.scale, Projectile.ai[0] == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 0);
            }
            return false;
        }
    }
}