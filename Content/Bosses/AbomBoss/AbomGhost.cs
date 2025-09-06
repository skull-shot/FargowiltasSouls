using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Projectiles.EffectVisual;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomGhost : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureNPC(NPCID.PirateGhost);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = Main.npcFrameCount[NPCID.PirateGhost];
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.scale = 1;
            Projectile.width = Player.defaultWidth;
            Projectile.height = Player.defaultHeight;
            Projectile.Opacity = 0f;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        ref float ShipIdentity => ref Projectile.ai[0];
        ref float Order => ref Projectile.ai[1];
        ref float GunRotation => ref Projectile.ai[2];

        ref float Timer => ref Projectile.localAI[0];

        Vector2 GetOffsetFromCenterToShipSpriteCoords(float x, float y, float scale)
        {
            Vector2 offset = new Vector2(x, y) - new Vector2(295, 287);
            offset.X *= Projectile.direction;
            offset *= scale;
            return offset;
        }

        public override void AI()
        {
            int shipWhoAmI = FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, ShipIdentity, ModContent.ProjectileType<AbomShip>());
            if (shipWhoAmI == -1)
                return;

            Projectile pirateShip = Main.projectile[shipWhoAmI];

            if (pirateShip.localAI[2] == 1)
            {
                Projectile.Kill();
                return;
            }

            if (Timer++ == 0)
            {
                Projectile.localAI[1] = Main.rand.Next(60);
            }

            Projectile.direction = Projectile.spriteDirection = (int)pirateShip.ai[0];
            const int maxPirates = 8;
            Projectile.Center = pirateShip.Center + GetOffsetFromCenterToShipSpriteCoords(136 + 220 / maxPirates * Order, 370, pirateShip.scale);
            Projectile.position.Y += 12f * (float)Math.Sin(MathHelper.TwoPi / 40 * (Timer + Projectile.localAI[1]));

            Projectile.Opacity = Math.Min(1f, Timer / 60);

            int flip = Order % 2 == 0 ? -1 : 1;
            flip *= Projectile.direction;
            GunRotation += flip * MathHelper.Pi * 1.75f / 240 * Main.rand.NextFloat(0.9f, 1.1f);

            if (pirateShip.localAI[1] == 1 && Timer % 10 == 0 && FargoSoulsUtil.HostCheck) //time to shoot
            {
                Vector2 gunPos = Projectile.Center;
                Vector2 vel = 6f * GunRotation.ToRotationVector2();
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), gunPos, vel, ModContent.ProjectileType<AbomBullet>(), Projectile.damage, 0f, Main.myPlayer);
            }

            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);

            if (!Main.dedServ)
            {
                for (int i = 0; i < 8; i++)
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Main.rand.Next(Projectile.width), Main.rand.Next(Projectile.height)), Main.rand.NextVector2Circular(3f, 3f), Main.rand.Next(11, 14));
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Bleeding, 240);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<AbomFangBuff>(), 240);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Vector2 drawPos = Projectile.Center;
            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            
            ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ItemID.WispDye);
            shader.Apply(Projectile, new Terraria.DataStructures.DrawData?());

            Main.EntitySpriteDraw(texture2D13, drawPos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}