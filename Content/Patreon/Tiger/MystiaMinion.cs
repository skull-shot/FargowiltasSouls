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

namespace FargowiltasSouls.Content.Patreon.Tiger
{
    public class MystiaMinion : TouhouMinionBase
    {
        public override int frameCount => 6;
        public override int attackSpeed => 60;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 6;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 20;
            Projectile.height = 16;
        }

        //-mystia shoots a music note that pulses and spreads smaller notes from it before going away
        public override void MinionAttack(Vector2 target)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                Projectile.SafeDirectionTo(target) * 3, ModContent.ProjectileType<MystiaNote>(),
                Projectile.damage, Projectile.knockBack, Projectile.owner);
            
        }
    }
}
