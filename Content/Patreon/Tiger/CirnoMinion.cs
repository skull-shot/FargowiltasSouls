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
    //-and cirno tosses a ice cube that when it makes contact shatters into little ice shard projectiles that just kinds damage the general area where it landed. Also I would be willing to even forego the name easter egg thing I got if there's a small chance the ice cube has a frog in it and also spawns a frog when it makes contact.*/
    public class CirnoMinion : TouhouMinionBase
    {
        public override int frameCount => 3;
        public override int attackSpeed => 30;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 3;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 31;
            Projectile.height = 29;
        }

        public override void MinionAttack(Vector2 target)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                Projectile.SafeDirectionTo(target) * 12, ModContent.ProjectileType<CirnoIceCube>(),
                Projectile.damage, Projectile.knockBack, Projectile.owner);
            //Projectile.ai[1] = Main.rand.NextFloat(10, 30);
            //Projectile.netUpdate = true;
        }


    }
}
