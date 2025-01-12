using FargowiltasSouls.Content.Patreon.Sasha;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Fargowiltas.Projectiles;

namespace FargowiltasSouls.Content.Patreon.Tiger
{
    
    public class WriggleMinion : TouhouMinionBase
    {
        public override int frameCount => 3;
        public override int attackSpeed => 60;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 3;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 16;
        }

        //-wriggle shoots a swarm of bugs that do the bee projectile behavior 
        public override void MinionAttack(Vector2 target)
        {
            //spawn a ring of bugs
            FargoSoulsUtil.XWay(7, Projectile.GetSource_FromThis(), Projectile.Center, ProjectileID.Bee, 2, Projectile.damage / 2, 0);
        }
    }
}
