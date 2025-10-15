﻿using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantScythe2 : MutantScythe1
    {
        public override string Texture => FargoSoulsUtil.AprilFools ?
            "FargowiltasSouls/Content/Bosses/MutantBoss/MutantScythe1_April" :
            "FargowiltasSouls/Content/Bosses/AbomBoss/AbomSickle";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.hide = false;
        }

        public override void PostAI()
        {
            if (Projectile.timeLeft == 180) //draw attention to myself
            {
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz, 0, 0, 0, default, 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 6f;
                }
                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<IronParry>(), 0, 0f, Main.myPlayer);
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {

        }
    }
}