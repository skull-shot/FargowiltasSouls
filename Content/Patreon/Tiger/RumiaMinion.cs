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
    public class RumiaMinion : TouhouMinionBase
    {
        public override int frameCount => 3;
        public override int attackSpeed => 300;

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

        //-rumia tosses orb that moves and grows in size doing damage to anything in it before going away.
        public override void MinionAttack(Vector2 target)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                Projectile.SafeDirectionTo(target) * 2, ModContent.ProjectileType<RumiaOrb>(),
                Projectile.damage, Projectile.knockBack, Projectile.owner);
            
        }
    }
}
