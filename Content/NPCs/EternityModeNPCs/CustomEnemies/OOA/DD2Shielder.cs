using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Custom.OOA;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.CustomEnemies.OOA
{
    public class DD2Shielder : DD2Enemy
    {
        public override string Texture => "Terraria/Images/NPC_" + NPCID.DD2WitherBeastT2;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 17;
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.lifeMax = 210;
            NPC.defense = 5;
            NPC.damage = 30;
            NPC.knockBackResist = 0f;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.HitSound = SoundID.NPCHit1;
        }

        public ref float Timer => ref NPC.ai[0];
        public ref float Shield => ref NPC.ai[1];

        public override void AI()
        {
            if (Timer == 0)
            {
                Shield = -1;
            }
            Timer++;

            int n = NPC.FindFirstNPC(NPCID.DD2EterniaCrystal);
            if (n == -1)
            {
                NPC.life = 0;
                NPC.checkDead();
                return;
            }

            NPC crystal = Main.npc[n];
            float dir = NPC.Center.X < crystal.Center.X ? 1 : -1;
            NPC.direction = (int)dir;
            NPC.spriteDirection = -NPC.direction;

            NPC.velocity = new Vector2 (dir, NPC.velocity.Y);
            if (Timer < 3 * 40)
                return;

            if (Shield == -1)
            {
                SoundEngine.PlaySound(SoundID.Item28 with { Pitch = -0.5f, Volume = 2f }, NPC.Center);
                for (int i = 0; i < 10; i++)
                {
                    float rot = MathHelper.TwoPi * i / 10f;
                    new SmallSparkle(NPC.Center + Vector2.UnitX, 1.5f * Vector2.UnitX.RotatedBy(rot), Color.Purple, 1f, 15).Spawn();
                }
                Shield = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - 10 * Vector2.UnitY, dir * Vector2.UnitX, ModContent.ProjectileType<OOAForcefield>(), NPC.damage, 0f);
            }
            else
            {
                NPC.velocity *= 0.35f;
            }
            if (NPC.Center.Distance(crystal.Center) <= 400f)
            {
                NPC.velocity *= 0;
            }
        }

        public override void OnKill()
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.Shadowflame);
            }
            base.OnKill();
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Length() == 0)
            {
                NPC.frame.Y = 0;
                return;
            }
            NPC.frameCounter++;
            if (NPC.frameCounter == 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y == frameHeight * 11)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Timer < 60)
            {
                float opacity = (Timer / 60);
                Texture2D texture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
                Rectangle frame = NPC.frame;
                SpriteEffects flip = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Main.EntitySpriteDraw(texture, NPC.Center - screenPos, frame, drawColor * opacity, NPC.rotation, frame.Size() / 2, NPC.scale, flip);
                return false;
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
