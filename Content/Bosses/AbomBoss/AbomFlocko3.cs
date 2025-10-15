using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomFlocko3 : AbomFlocko
    {
        public override string Texture => "Terraria/Images/NPC_345";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Projectile.type] = Main.npcFrameCount[NPCID.IceQueen];
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<AbomBoss>());
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }

            Vector2 target = npc.Center;
            target.X += Projectile.ai[1];
            target.Y -= 1100;

            Vector2 distance = target - Projectile.Center;
            float length = distance.Length();
            if (length > 10f)
            {
                distance /= 8f;
                Projectile.velocity = (Projectile.velocity * 23f + distance) / 24f;
            }
            else
            {
                if (Projectile.velocity.Length() < 12f)
                    Projectile.velocity *= 1.05f;
            }

            if (++Projectile.localAI[0] > 180 && ++Projectile.localAI[1] > (npc.localAI[3] > 1 ? 4 : 2)) //spray shards
            {
                SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
                Projectile.localAI[1] = 0f;
                if (FargoSoulsUtil.HostCheck)
                {
                    Vector2 speed = new(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                    speed.Normalize();
                    speed *= 6f;
                    speed.X /= 2;
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + speed * 4f, speed, ModContent.ProjectileType<AbomFrostShard>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }

            Projectile.rotation = System.Math.Min(MathHelper.PiOver2, Projectile.velocity.X / 16f);
            Projectile.frame = 3;
        }
    }
}