using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Cavern
{
    public class GranitePebble : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Granite;
        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }

        public ref float Owner => ref Projectile.ai[0];
        public ref float Rotation => ref Projectile.ai[1];
        public ref float Index => ref Projectile.ai[2];

        public int State = 0;
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write7BitEncodedInt(State);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            State = reader.Read7BitEncodedInt();
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)Owner];
            if (!npc.active)
            {
                Projectile.Kill();
                return;
            }

            // Shielding
            if (State == 0)
            {
                Rotation += 0.1f;
                Projectile.Center = npc.Center -  45 * Vector2.UnitY.RotatedBy(Rotation);
                if (npc.ai[2] >= 0)
                {
                    if (!npc.HasValidTarget)
                    {
                        Projectile.Kill();
                        return;
                    }

                    State = 1;
                }
            }
            // Firing
            else if (State == 1)
            {
                if (npc.ai[2] / 8 >= Index)
                {
                    Player target = Main.player[npc.target];
                    float rot = (target.Center - Projectile.Center).ToRotation();
                    rot += Main.rand.NextFloat(-0.3f, 0.3f);
                    Projectile.velocity = 9 * Vector2.UnitX.RotatedBy(rot);
                    State = 2;
                    Projectile.tileCollide = true;
                    Projectile.netUpdate = true;
                }
            }
            // Traveling
            else
            {
                Projectile.rotation += 0.15f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDust(Projectile.Center, 2, 2, DustID.Granite);
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value;
            Rectangle frame = new(0, 0, texture.Width, texture.Height);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.DeepSkyBlue * 0.7f, Projectile.rotation, texture.Size() / 2, 1.5f * Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
