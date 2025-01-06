using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Common.Graphics.Particles;
using Luminance.Core.Graphics;
using Luminance.Core.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class LeashFlail : ModProjectile
    {
        private const string ChainTexturePath = "FargowiltasSouls/Content/Projectiles/BossWeapons/LeashFlailChain";
        private const string EyeTexturePath = "FargowiltasSouls/Content/Projectiles/BossWeapons/LeashFlailEye";
        private const string FlailTexturePath = "FargowiltasSouls/Content/Projectiles/BossWeapons/LeashFlail";

        private static Asset<Texture2D> chainTexture;
        public static Asset<Texture2D> FlailTexture;
        public static Asset<Texture2D> EyeTexture;

        public bool HasHitEnemy = false;
        public LoopedSoundInstance Loop;

        public override string Texture => "FargowiltasSouls/Content/Projectiles/Empty";

        public int EyeTimer = 0;
        //=> (CurrentAIState == AIState.Spinning) ? "FargowiltasSouls/Content/Projectiles/BossWeapons/LeashFlailEye" : "FargowiltasSouls/Content/Projectiles/BossWeapons/LeashFlail";
        
        private enum AIState
        {
            Spinning,
            LaunchingForward,
            Retracting,
            ForcedRetracting,
            Ricochet,
            Dropping
        }

        private AIState CurrentAIState
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float StateTimer => ref Projectile.ai[1];
        public ref float CollisionCounter => ref Projectile.localAI[0];
        public ref float SpinningStateTimer => ref Projectile.localAI[1];

        public override void Load()
        {
            chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);
            EyeTexture = ModContent.Request<Texture2D>(EyeTexturePath);
            FlailTexture = ModContent.Request<Texture2D>(FlailTexturePath);
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true; 
            Projectile.width = 38; 
            Projectile.height = 42; 
            Projectile.friendly = true; 
            Projectile.penetrate = -1; 
            Projectile.DamageType = DamageClass.Melee;
            //Projectile.aiStyle = ProjAIStyleID.Flail;
            Projectile.usesLocalNPCImmunity = true; 
            Projectile.localNPCHitCooldown = 10; 
        }

        // This AI code was adapted from vanilla code: Terraria.Projectile.AI_015_Flails() 
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f)
            {
                Projectile.Kill();
                return;
            }
            if (Main.myPlayer == Projectile.owner && Main.mapFullscreen)
            {
                Projectile.Kill();
                return;
            }

            Vector2 mountedCenter = player.MountedCenter;
            bool shouldOwnerHitCheck = false;
            int launchTimeLimit = 12;  // How much time the projectile can go before retracting (speed and shootTimer will set the flail's range)
            float launchSpeed = 24f; // How fast the projectile can move
            float maxLaunchLength = 400f; // How far the projectile's chain can stretch before being forced to retract when in launched state
            float retractAcceleration = 1.5f; // How quickly the projectile will accelerate back towards the player while retracting
            float maxRetractSpeed = 20f; // The max speed the projectile will have while retracting
            float forcedRetractAcceleration = 6f; // How quickly the projectile will accelerate back towards the player while being forced to retract
            float maxForcedRetractSpeed = 15f; // The max speed the projectile will have while being forced to retract
            int defaultHitCooldown = 10; // How often your flail hits when resting on the ground, or retracting
            int spinHitCooldown = 20; // How often your flail hits when spinning
            int movingHitCooldown = 10; // How often your flail hits when moving
            int ricochetTimeLimit = launchTimeLimit + 5;

            // Scaling these speeds and accelerations by the players melee speed makes the weapon more responsive if the player boosts it or general weapon speed
            float meleeSpeedMultiplier = player.GetTotalAttackSpeed(DamageClass.Melee);
            launchSpeed *= meleeSpeedMultiplier;
            retractAcceleration *= meleeSpeedMultiplier;
            maxRetractSpeed *= meleeSpeedMultiplier;
            forcedRetractAcceleration *= meleeSpeedMultiplier;
            maxForcedRetractSpeed *= meleeSpeedMultiplier;
            float launchRange = launchSpeed * launchTimeLimit;
            float maxDroppedRange = launchRange + 160f;
            Projectile.localNPCHitCooldown = defaultHitCooldown;



            switch (CurrentAIState)
            {   
               
                case AIState.Spinning:
                    {
                        shouldOwnerHitCheck = true;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Vector2 unitVectorTowardsMouse = mountedCenter.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX * player.direction);
                            player.ChangeDir((unitVectorTowardsMouse.X > 0f).ToDirectionInt());
                            if (!player.channel) // If the player releases then change to moving forward mode
                            {
                                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Weapons/LeashThrow") with { Variants = [1, 2] }, player.Center);
                                CurrentAIState = AIState.LaunchingForward;
                                StateTimer = 0f;
                                Projectile.velocity = unitVectorTowardsMouse * launchSpeed + player.velocity;
                                Projectile.Center = mountedCenter;
                                Projectile.netUpdate = true;
                                Projectile.ResetLocalNPCHitImmunity();
                                Projectile.localNPCHitCooldown = movingHitCooldown;
                                break;
                            }
                        }
                        SpinningStateTimer += 0.85f;
                       
                        Vector2 offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 10f * (SpinningStateTimer / 60f) * player.direction);

                        if (++EyeTimer >= 29)
                        {
                            if (Main.projectile.Where(p => p.TypeAlive<EyeProjectile>() && p.ai[0] == 0 && p.owner == Projectile.owner).Count() < 7)
                            {
                                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(7, 7).RotatedByRandom(MathHelper.TwoPi),
                                    ModContent.ProjectileType<EyeProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            }
                            EyeTimer = 0;
                        }

                        Loop ??= LoopedSoundManager.CreateNew(FargosSoundRegistry.LeashSpin with { Volume = 0.5f }, () =>
                        {
                            return CurrentAIState != AIState.Spinning || !Projectile.active;
                        });
                         
                        Loop.Update(player.Center);

                        offsetFromPlayer.Y *= 0.8f;
                        if (offsetFromPlayer.Y * player.gravDir > 0f)
                        {
                            offsetFromPlayer.Y *= 0.5f;
                        }
                        Projectile.Center = mountedCenter + offsetFromPlayer * 30f + new Vector2(0, player.gfxOffY);
                        Projectile.velocity = Vector2.Zero;
                        Projectile.localNPCHitCooldown = spinHitCooldown;



                        break;
                    }
                case AIState.LaunchingForward:
                    {
                        bool shouldSwitchToRetracting = StateTimer++ >= launchTimeLimit;
                        shouldSwitchToRetracting |= Projectile.Distance(mountedCenter) >= maxLaunchLength;
                        if (false)//player.controlUseItem) 
                        {
                            CurrentAIState = AIState.Dropping;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.2f;
                           
                            break;
                        }
                        if (shouldSwitchToRetracting)
                        {
                            CurrentAIState = AIState.Retracting;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.3f;

                        }
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                        Projectile.localNPCHitCooldown = movingHitCooldown;
                        break;
                    }
                case AIState.Retracting:
                    {
                        Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                        if (Projectile.Distance(mountedCenter) <= maxRetractSpeed)
                        {
                            Projectile.Kill(); 
                            return;
                        }
                        if (false)//player.controlUseItem) 
                        {
                            CurrentAIState = AIState.Dropping;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.2f;
                        }
                        else
                        {
                            Projectile.velocity *= 0.98f;
                            Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxRetractSpeed, retractAcceleration);
                            player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                        }
                        break;
                    }
                case AIState.ForcedRetracting:
                    {
                        Projectile.tileCollide = false;
                        Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                        if (Projectile.Distance(mountedCenter) <= maxForcedRetractSpeed)
                        {
                            Projectile.Kill(); 
                            return;
                        }
                        Projectile.velocity *= 0.98f;
                        Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxForcedRetractSpeed, forcedRetractAcceleration);
                        Vector2 target = Projectile.Center + Projectile.velocity;
                        Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
                        if (Vector2.Dot(unitVectorTowardsPlayer, value) < 0f)
                        {
                            Projectile.Kill(); 
                            return;
                        }
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                        break;
                    }
                case AIState.Ricochet:
                    if (StateTimer++ >= ricochetTimeLimit)
                    {
                        CurrentAIState = AIState.Dropping;
                        StateTimer = 0f;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        Projectile.localNPCHitCooldown = movingHitCooldown;
                        Projectile.velocity.Y += 0.6f;
                        Projectile.velocity.X *= 0.95f;
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                    }
                    break;
                case AIState.Dropping:
                    if (true)//!player.controlUseItem || Projectile.Distance(mountedCenter) > maxDroppedRange)
                    {
                        CurrentAIState = AIState.ForcedRetracting;
                        StateTimer = 0f;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        Projectile.velocity.Y += 0.8f;
                        Projectile.velocity.X *= 0.95f;
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                    }
                    break;
            }
            Projectile.ownerHitCheck = shouldOwnerHitCheck; 
            Vector2 vectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
            Projectile.rotation = vectorTowardsPlayer.ToRotation() - MathHelper.PiOver2;
            
			

            Projectile.timeLeft = 2; 
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2); 
            player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
            if (Projectile.Center.X < mountedCenter.X)
            {
                player.itemRotation += (float)Math.PI;
            }
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            int defaultLocalNPCHitCooldown = 10;
            int impactIntensity = 0;
            Vector2 velocity = Projectile.velocity;
            float bounceFactor = 0.2f;
            if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Ricochet)
            {
                bounceFactor = 0.4f;
            }

            if (CurrentAIState == AIState.Dropping)
            {
                bounceFactor = 0f;
            }

            if (oldVelocity.X != Projectile.velocity.X)
            {
                if (Math.Abs(oldVelocity.X) > 4f)
                {
                    impactIntensity = 1;
                }

                Projectile.velocity.X = (0f - oldVelocity.X) * bounceFactor;
                CollisionCounter += 1f;
            }

            if (oldVelocity.Y != Projectile.velocity.Y)
            {
                if (Math.Abs(oldVelocity.Y) > 4f)
                {
                    impactIntensity = 1;
                }

                Projectile.velocity.Y = (0f - oldVelocity.Y) * bounceFactor;
                CollisionCounter += 1f;
            }


            if (CurrentAIState == AIState.LaunchingForward)
            {
                CurrentAIState = AIState.Ricochet;
                Projectile.localNPCHitCooldown = defaultLocalNPCHitCooldown;
                Projectile.netUpdate = true;
                Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
                Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
                impactIntensity = 2;
                Projectile.CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out bool causedShockwaves);
                Projectile.CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, velocity);
                Projectile.position -= velocity;
            }

            if (impactIntensity > 0)
            {
                Projectile.netUpdate = true;
                for (int i = 0; i < impactIntensity; i++)
                {
                    Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
                }

                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            }
         
            if (CurrentAIState != AIState.Spinning && CurrentAIState != AIState.Ricochet && CurrentAIState != AIState.Dropping && CollisionCounter >= 10f)
            {
                CurrentAIState = AIState.ForcedRetracting;
                Projectile.netUpdate = true;
            }

            return false;
        }

        public override bool? CanDamage()
        {
            
            if (CurrentAIState == AIState.Spinning && SpinningStateTimer <= 12f)
            {
                return false;
            }
            return base.CanDamage();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            
            if (CurrentAIState == AIState.Spinning)
            {
                Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
                Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
                shortestVectorFromPlayerToTarget.Y /= 0.8f; 
                float hitRadius = 55f; 
                return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
            }
            
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {      

            // Flails do 20% more damage while spinning
            if (CurrentAIState == AIState.Spinning)
            {
                modifiers.SourceDamage *= 1.2f;
            }
            // Flails do 100% more damage while launched or retracting. This is the damage the item tooltip for flails aim to match, as this is the most common mode of attack. This is why the item has ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
            else if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Retracting)
            {
                modifiers.SourceDamage *= 2f;
            }

            // The hitDirection is always set to hit away from the player, even if the flail damages the npc while returning
            modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();

            // Knockback is only 25% as powerful when in spin mode
            if (CurrentAIState == AIState.Spinning)
            {
                modifiers.Knockback *= 0.25f;
            }
            // Knockback is only 50% as powerful when in drop down mode
            else if (CurrentAIState == AIState.Dropping)
            {
                modifiers.Knockback *= 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Particle p = new SparkParticle(target.Center, Main.rand.NextVector2Circular(2, 2), Color.DarkRed, 1f, 25);
            Particle p2 = new SparkParticle(target.Center, Main.rand.NextVector2Circular(2, 2), Color.DarkRed, .75f, 25);
            Particle p3 = new SparkParticle(target.Center, Main.rand.NextVector2Circular(2, 2), Color.DarkRed, .5f, 25);
            p.Spawn();
            p2.Spawn();
            p3.Spawn();
            base.OnHitNPC(target, hit, damageDone);
        }

       
        public override bool PreDraw(ref Color lightColor)
        {   
            Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);
            
            playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

            Rectangle? chainSourceRectangle = null;
            
            float chainHeightAdjustment = 0f; 

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
            if (chainSegmentLength == 0)
            {
                chainSegmentLength = 10 * Projectile.scale; 
            }
            float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;
           
            while (chainLengthRemainingToDraw > 0f)
            {              
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                var chainTextureToDraw = chainTexture;
          
                Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, Projectile.scale, SpriteEffects.None, 0f);
              
                chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }
            Main.spriteBatch.Draw((CurrentAIState == AIState.Spinning) ? EyeTexture.Value : FlailTexture.Value, Projectile.Center - Main.screenPosition, null, Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16f)), Projectile.rotation, new Vector2(19, 20), Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}