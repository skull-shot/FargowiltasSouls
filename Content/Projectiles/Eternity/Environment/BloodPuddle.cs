using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Environment
{
    public class BloodPuddle : ModNPC
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Eternity/Environment", Name);
        public int Frame = 0;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 2;
            NPCID.Sets.TrailCacheLength[Type] = 20;
            Main.npcFrameCount[Type] = 5;

            this.ExcludeFromBestiary();

            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = Main.hardMode ? 150 : 20;
            NPC.damage = 0;

            NPC.width = 20;
            NPC.height = 20;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.timeLeft = 900;
            NPC.scale = Main.rand.NextFloat(1f, 1.6f);
            NPC.hide = true;

            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath11;
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCProjectiles.Add(index);
            base.DrawBehind(index);
        }
        public int Timer;
        public override void AI()
        {
            if (Timer == 0)
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.whoAmI != NPC.whoAmI && n.type == NPC.type && n.DistanceSQ(NPC.Center) < NPC.width * NPC.width * 2)
                        n.StrikeInstantKill();
                }
            }
            if (++Timer > 60 * 10)
            {
                NPC.StrikeInstantKill();
                return;
            }
            NPC.localAI[0]++;
            if (NPC.localAI[0] >= 10)
            {
                Frame++;
                NPC.localAI[0] = 0;
                if (Frame > 4)
                {
                    Frame = 3;
                }
            }
            NPC.localAI[1]++;
            if (NPC.localAI[1] >= 5)
            {
                NPC.localAI[2]++;
                NPC.localAI[1] = 0;
                if (NPC.localAI[2] > 4)
                {
                    NPC.localAI[2] = 0;
                }
            }

            if (!Collision.SolidCollision(NPC.Bottom - Vector2.UnitY * 10, 1, 1))
            {
                NPC.position.Y += 1;
            }
            Player target = null;
            if (NPC.ai[0] == 0)
            {
                NPC.ai[1]++;
                int p = Player.FindClosest(NPC.Center, 1, 1);
                if (p >= 0) target = Main.player[p];

                if (target != null && target.Hitbox.Intersects(NPC.Hitbox) && NPC.ai[0] == 0)
                {
                    NPC.ai[2] = target.whoAmI;
                    NPC.ai[1] = 0;
                    NPC.ai[0] = 1;
                    SoundEngine.PlaySound(SoundID.NPCDeath11, NPC.Center);
                    NPC.netUpdate = true;
                }
            }
            if (NPC.ai[0] == 1)
            {
                NPC.ai[1] += 2;
                NPC.timeLeft = 20;
                if (!((int)NPC.ai[2]).IsWithinBounds(Main.maxPlayers))
                {
                    NPC.StrikeInstantKill();
                    return;
                }
                target = Main.player[(int)NPC.ai[2]];
                if (target == null || !target.Alive())
                {
                    NPC.StrikeInstantKill();
                    return;
                }
                target.AddBuff(ModContent.BuffType<BleedingOut>(), 2);

                if (NPC.ai[1] % 50 == 0)
                    SoundEngine.PlaySound(SoundID.Item3 with { Volume = 0.75f, Pitch = 1.5f }, NPC.Center);

                if (NPC.ai[1] >= 160)
                {
                    NPC.ai[1] = -40;
                }
                if (NPC.Distance(target.Center) > 200)
                {
                    NPC.StrikeInstantKill();
                }
            }
        }
        public override void OnKill()
        {
            for (int i = 0; i < 27; i++)
            {
                Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloat(0, 7), Main.rand.NextFloat(0, 7));
            }
            if (NPC.ai[0] == 1 && ((int)NPC.ai[2]).IsWithinBounds(Main.maxPlayers))
            {
                Player target = Main.player[(int)NPC.ai[2]];
                if (target != null)
                {
                    for (int i = 0; i < NPC.Distance(target.Center); i += 3)
                    {
                        Dust.NewDustDirect(target.Center + target.AngleTo(NPC.Center).ToRotationVector2() * i, 1, 1, DustID.Blood);
                    }
                }
                
            }
            
        }
        public override void FindFrame(int frameHeight)
        {
            //NPC.frame.Y = Frame * frameHeight;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> t = TextureAssets.Npc[Type];
            Asset<Texture2D> vein = FargoAssets.GetTexture2D("Content/Projectiles/Eternity/Environment", "BloodTendril").Asset;
            int frameHeight = t.Height() / Main.npcFrameCount[Type];
            // Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, frameHeight * Projectile.frame, t.Width(), frameHeight), lightColor, Projectile.rotation, new Vector2(t.Width(), frameHeight)/2, Projectile.scale, SpriteEffects.None);

            if (NPC.ai[0] == 1)
            {
                Player target = Main.player[(int)NPC.ai[2]];
                if (target != null && target.active)
                {
                    int veinFrameHeight = vein.Height() / 5;
                    //Dust d = Dust.NewDustDirect(bigPoint, 1, 1, DustID.Terra);
                    //d.velocity *= 0;
                    for (int i = 0; i < NPC.Distance(target.Center); i += (int)(veinFrameHeight * NPC.scale * 0.5f))
                    {
                        float y = NPC.Distance(target.Center) / 300;
                        Vector2 shakeOffset = new Vector2(MathHelper.Lerp(0, 1f, y * y), 0).RotatedByRandom(MathHelper.TwoPi);
                        Vector2 pos = NPC.Center + NPC.AngleTo(target.Center).ToRotationVector2() * i;

                        int frame = (int)pos.Distance(target.Center) / (int)(veinFrameHeight * NPC.scale * 0.5f) % 5;
                        frame += (int)NPC.localAI[2];
                        if (frame > 4) frame -= 5;
                        frame = (int)NPC.localAI[2];
                        //Main.NewText(frame);
                        spriteBatch.Draw(vein.Value, pos - Main.screenPosition + shakeOffset, new Rectangle(0, veinFrameHeight * frame, vein.Width(), veinFrameHeight), drawColor, NPC.AngleTo(target.Center) + MathHelper.PiOver2, new Vector2(vein.Width(), veinFrameHeight) / 2, NPC.scale * 0.5f, SpriteEffects.None, 0);
                    }

                }
            }
            else
            {
                
            }
                
            spriteBatch.Draw(t.Value, NPC.Center - Main.screenPosition - new Vector2(0, 5), new Rectangle(0, frameHeight * Frame, t.Width(), frameHeight), drawColor, NPC.rotation, new Vector2(t.Width(), frameHeight) / 2, NPC.scale, SpriteEffects.None, 0);

            // the famous "please pay attention to me" glow
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            float lerp = MathF.Pow(MathF.Sin(NPC.ai[1] * MathF.Tau / 55), 2f);
            Color glowColor = Color.White;
            float glowOpacity = lerp * 0.75f;
            spriteBatch.Draw(t.Value, NPC.Center - Main.screenPosition - new Vector2(0, 5), new Rectangle(0, frameHeight * Frame, t.Width(), frameHeight), glowColor * glowOpacity, NPC.rotation, new Vector2(t.Width(), frameHeight) / 2, NPC.scale, SpriteEffects.None, 0);
            Main.spriteBatch.ResetToDefault();
            return false;
        }
    }
}