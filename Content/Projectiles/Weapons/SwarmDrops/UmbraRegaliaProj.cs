using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.SwarmDrops
{
    public class UmbraRegaliaProj : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Weapons/SwarmWeapons", Name);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.HeldProjectile;
            Projectile.width = 196;
            Projectile.height = 196;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 150;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.hide = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            ref float thrown = ref Projectile.ai[0];
            if (thrown > 0)
                return;
            Vector2 HitboxSize = new(88 * Projectile.scale, 88 * Projectile.scale);
            Vector2 HitboxCenter = Projectile.Center + Vector2.Normalize(Projectile.velocity) * (Projectile.Size.Length() / 2f - HitboxSize.Length() / 2f);
            hitbox = new Rectangle((int)(HitboxCenter.X - HitboxSize.X / 2f), (int)(HitboxCenter.Y - HitboxSize.Y / 2f), (int)HitboxSize.X, (int)HitboxSize.Y);
            Projectile.EmitEnchantmentVisualsAt(HitboxCenter - (HitboxSize / 2f), hitbox.Width, hitbox.Height);
        }
        public float maxCharge = 90f; // in frames
        public int SwingDirection = 1;
        public float Extension = 0;
        int OrigAnimMax = 30;
        bool Charged;
        bool canDamage;
        public override bool? CanDamage()
        {
            if (canDamage)
                return true;
            else return false;
        }

        public override bool PreAI()
        {
            ref float thrown = ref Projectile.ai[0];

            if (thrown > 0)
            {
                ThrownAI();
                return false;
            }

            ref float timer = ref Projectile.ai[1];

            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            if (Projectile.localAI[0] == 0)
            {
                OrigAnimMax = player.itemAnimationMax;
                Projectile.localAI[0] = 1;
                Projectile.damage = (int)(Projectile.damage * 1.25f);
            }
            float HoldoutRangeMax = (float)Projectile.Size.Length() / 2; //since sprite is diagonal
            float HoldoutRangeMin = (float)-Projectile.Size.Length() / 6;

            canDamage = true;
            int duration = (int)(OrigAnimMax / 1.5f);
            int WaitTime = OrigAnimMax / 5;


            if (timer == 0)
                SwingDirection = Main.rand.NextBool(2) ? 1 : -1;
            float Swing = 13; //higher value = less swing
            Projectile.localNPCHitCooldown = OrigAnimMax;   //only hit once per swing
                                                            //projectile.ai[1] is time from spawn
                                                            //Extension is between 0 and 1
            if (Projectile.timeLeft > OrigAnimMax)
            {
                Projectile.timeLeft = OrigAnimMax;
            }
            if (timer <= duration / 2)
            {
                Extension = timer / (duration / 2);
                Projectile.velocity = Projectile.velocity.RotatedBy(SwingDirection * Projectile.spriteDirection * -Math.PI / (Swing * OrigAnimMax));
            }
            else if (timer <= duration / 2 + WaitTime)
            {
                Extension = 1;
                Projectile.velocity = Projectile.velocity.RotatedBy(SwingDirection * Projectile.spriteDirection * (1.5 * duration / WaitTime) * Math.PI / (Swing * OrigAnimMax)); //i know how wacky this looks
            }
            else
            {
                canDamage = false; //no hit on backswing
                Extension = (duration + WaitTime - timer) / (duration / 2);
                Projectile.velocity = Projectile.velocity.RotatedBy(SwingDirection * Projectile.spriteDirection * -Math.PI / (Swing * OrigAnimMax));
            }

            if (timer == duration / 2)
            {
                float pitch = -0.5f;
                SoundEngine.PlaySound(SoundID.Item1 with { Pitch = pitch }, player.Center);
            }

            timer++;
            Projectile.velocity = Vector2.Normalize(Projectile.velocity); //store direction
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, Extension);
            //}

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;
            player.ChangeDir(Projectile.direction);
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            player.itemTime = 2;
            player.itemAnimation = 2;
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.ToRadians(-45f) + (float)Math.PI;
            }
            else
            {
                Projectile.rotation += MathHelper.ToRadians(-135f) + (float)Math.PI;
            }
            return false;
        }
        public void ThrownAI()
        {
            ref float thrown = ref Projectile.ai[0];
            ref float chargeLevel = ref Projectile.ai[2];
            ref float timer = ref Projectile.ai[1];

            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[0] == 0)
            {
                OrigAnimMax = player.itemAnimationMax;
                Projectile.localAI[0] = 1;
            }
            float HoldoutRangeMax = (float)Projectile.Size.Length() / 2; //since sprite is diagonal
            float HoldoutRangeMin = (float)-Projectile.Size.Length() / 6;

            if ((Main.mouseLeft || chargeLevel < 60) && thrown < 2)
            {
                player.heldProj = Projectile.whoAmI;
                Projectile.velocity = player.SafeDirectionTo(Main.MouseWorld);
                Projectile.Center = player.MountedCenter + Projectile.velocity * HoldoutRangeMin;
                canDamage = false;
                if (chargeLevel < maxCharge * 5) //yes, allow overcharge
                    chargeLevel++;
                if (player.whoAmI == Main.myPlayer)
                    CooldownBarManager.Activate("UmbraRegaliaCharge", FargoAssets.GetTexture2D("Content/Items/Weapons/SwarmDrops", "UmbraRegalia").Value, Color.DarkRed, 
                        () => Projectile.ai[2] / maxCharge, true, activeFunction: () => player.HeldItem != null && player.HeldItem.type == ModContent.ItemType<UmbraRegalia>());
                if (chargeLevel == (int)maxCharge - 1 && player.whoAmI == Main.myPlayer)
                {
                    SoundEngine.PlaySound(FargosSoundRegistry.ChargeSound, Projectile.Center + Projectile.velocity * Projectile.Size.Length() / 2);
                }
                Projectile.localAI[1] = chargeLevel; //store the charge amount
                                                     //int d = Dust.NewDust(player.MountedCenter + Projectile.velocity * Projectile.Size.Length() * 0.95f, 0, 0, DustID.CrystalPulse);
                                                     //Main.dust[d].noGravity = true;
                bool charged = chargeLevel >= maxCharge - 1;

                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.spriteDirection = Projectile.direction;
                player.ChangeDir(Projectile.direction);
                player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
                player.itemTime = 2;
                player.itemAnimation = 2;
                if (Projectile.spriteDirection == -1)
                {
                    Projectile.rotation += MathHelper.ToRadians(-45f) + (float)Math.PI;
                }
                else
                {
                    Projectile.rotation += MathHelper.ToRadians(-135f) + (float)Math.PI;
                }
            }
            else
            {
                canDamage = true;
                if (chargeLevel > -1) //check once
                {
                    Projectile.damage = (int)(Projectile.damage * 1f * (chargeLevel / maxCharge)); //modify this to change damage charge
                    Charged = chargeLevel >= maxCharge;
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (25f + 25f * Math.Min(1f, chargeLevel / maxCharge));
                    chargeLevel = -1;
                    Projectile.timeLeft = 60 * 2;
                    thrown = 2;
                    SoundEngine.PlaySound(SoundID.Item1 with { Pitch = Charged ? -0.6f : -0.2f }, Projectile.Center);
                }
                thrown++;
                float frequency = Charged ? 3 : 5;
                if ((int)(thrown % frequency) < 1)
                {
                    Vector2 pos = Projectile.Center + Vector2.Normalize(Projectile.velocity) * (Projectile.Size.Length() / 2f);
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, -Projectile.velocity.RotatedByRandom(MathHelper.PiOver2 * 0.2f) * 0.6f,
                            ProjectileID.FairyQueenMagicItemShot, Projectile.damage / 6, Projectile.knockBack, Projectile.owner, -1, Main.rand.NextFloat(1)); //random ai1 decides color completely randomly
                    if (Main.projectile[p] != null && p != Main.maxProjectiles)
                    {
                        Main.projectile[p].DamageType = DamageClass.MeleeNoSpeed;
                        Main.projectile[p].noEnchantmentVisuals = true;
                    }
                }
                Projectile.localNPCHitCooldown = OrigAnimMax;   //only hit once per throw
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ref float thrown = ref Projectile.ai[0];

            if (thrown <= 0 && Projectile.numHits < 1)
            {
                Player player = Main.player[Projectile.owner];
                int amount = 2;
                player.statLife += amount;
                if (player.statLife > player.statLifeMax2)
                    player.statLife = player.statLifeMax2;
                player.HealEffect(amount);
            }
            Vector2 pos = Projectile.Center + Vector2.Normalize(Projectile.velocity) * (Projectile.Size.Length() / 2f);
            int count = 4;
            if (Charged)
            {
                count = 10;
                SoundEngine.PlaySound(SoundID.Item68, pos);
            }

            for (int i = 0; i < count; i++)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 10,
                    ProjectileID.FairyQueenMagicItemShot, Projectile.damage / 6, Projectile.knockBack, Projectile.owner, -1, Main.rand.NextFloat(1)); //random ai1 decides color completely randomly
                if (Main.projectile[p] != null && p != Main.maxProjectiles)
                {
                    Main.projectile[p].DamageType = DamageClass.MeleeNoSpeed;
                    Main.projectile[p].noEnchantmentVisuals = true;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);


            if (!Main.player[Projectile.owner].channel)
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                {
                    Color color27 = color26;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Vector2 value4 = Projectile.oldPos[i];
                    float num165 = Projectile.rotation;//Projectile.oldRot[i];
                    Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
                }
            }

            //offset glow
            if (Projectile.ai[0] > 0)
            {
                for (int j = 0; j < 16; j++)
                {
                    float offsetDistance = 4f * Math.Min(1f, Projectile.localAI[1] / maxCharge);
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * offsetDistance;
                    Color glowColor = Color.DarkRed * 0.3f;
                    float scale = Projectile.scale * ((Main.mouseTextColor / 200f - 0.35f) * 0.2f + 0.9f);
                    Main.spriteBatch.Draw(texture2D13, pos + afterimageOffset, null, glowColor, Projectile.rotation, origin2, scale, effects, 0f);
                }
            }

            Main.EntitySpriteDraw(texture2D13, pos, new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);

            ref float chargeLevel = ref Projectile.ai[2];
            if (chargeLevel > maxCharge)
            {
                Color newColor = Color.Red;
                newColor.A = 0;
                newColor *= 0.75f;
                float scale = Projectile.scale * ((Main.mouseTextColor / 200f - 0.35f) * 0.2f + 0.9f);
                Main.EntitySpriteDraw(texture2D13, pos, new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(newColor), Projectile.rotation, origin2, scale, effects, 0);
            }
            return false;
        }
    }
    public class DarkFairyQueenMagicItemShot : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation)
        {
            return projectile.type == ProjectileID.FairyQueenMagicItemShot;
        }
        public bool Dark = false;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is Projectile parentProj && parentProj.type == ModContent.ProjectileType<UmbraRegaliaProj>())
                Dark = true;
        }
        public override void Load()
        {
            On_Projectile.GetFairyQueenWeaponsColor += Detour_GetFairyQueenWeaponsColor;
        }
        public override void Unload()
        {
            On_Projectile.GetFairyQueenWeaponsColor -= Detour_GetFairyQueenWeaponsColor;
        }
        public static Color Detour_GetFairyQueenWeaponsColor(On_Projectile.orig_GetFairyQueenWeaponsColor orig, Projectile self, float alphaChannelMultiplier = 1f, float lerpToWhite = 0f, float? rawHueOverride = null)
        {
            string name = Main.player[self.owner].name;
            bool overrid = false;

            if (self.type == ProjectileID.FairyQueenMagicItemShot && self.TryGetGlobalProjectile(out DarkFairyQueenMagicItemShot modProj) && modProj.Dark)
            {
                Main.player[self.owner].name = "Mid";
                overrid = true;
            }

            Color result = orig(self, alphaChannelMultiplier, lerpToWhite, rawHueOverride);

            if (overrid)
                Main.player[self.owner].name = name;

            return result;
        }
    }
}