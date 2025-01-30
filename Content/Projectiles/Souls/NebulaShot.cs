using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class NebulaShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_634";

        private static readonly int[] choices =
                [
                                                            3453,
                                                            3454
                ];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            base.OnSpawn(source);
        }

        public override void AI()
        {
            Projectile.Animate(5);
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch, Scale: 1.5f);

            ref float type = ref Projectile.ai[0];

            type = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 3000, true);
            if (type > 0)
            {
                if (Main.npc[(int)type].active) {
                    NPC npc = Main.npc[(int)type];
                    Vector2 vectorToIdlePosition = npc.Center - Projectile.Center;
                    vectorToIdlePosition.Normalize();
                    Projectile.velocity = 0.97f * (Projectile.velocity + vectorToIdlePosition);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            int total = player.FargoSouls().ForceEffect<NebulaEnchant>() ? 2 : 1;
            for (int j = 0; j < total; j++)
            {
                int boosterType = Utils.SelectRandom(Main.rand, choices);
                Item.NewItem(player.GetSource_OpenItem(boosterType), (int)target.position.X, (int)target.position.Y, target.width, target.height, boosterType, 1, false, 0, false, false);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                int j = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch, Scale: 5f);
                Main.dust[j].velocity *= 5f;
                Main.dust[j].noGravity = true;
            }
        }
    }
}
