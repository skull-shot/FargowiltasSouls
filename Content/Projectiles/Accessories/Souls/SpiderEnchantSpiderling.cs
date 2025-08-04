using System.Threading;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class SpiderEnchantSpiderling : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.DangerousSpider);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.DangerousSpider];
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DangerousSpider);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            AIType = ProjectileID.DangerousSpider;
            Projectile.minion = false;
            Projectile.minionSlots = 0;
            Projectile.usesIDStaticNPCImmunity = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;

            Projectile.timeLeft = 60 * 4;
        }
        public override bool? CanCutTiles() => false;
        public override void AI()
        {
            base.AI();
        }
    }
}
