using Fargowiltas.Common.Configs;
using FargowiltasSouls.Assets.Textures;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using FargowiltasSouls.Content.Buffs.Masomode;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Snow
{
    public class SnowMist : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.timeLeft = 50;
            Projectile.hostile = true;
            Projectile.coldDamage = true;
        }
        public override void AI()
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FrostStaff);
            d.noGravity = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<HypothermiaBuff>(), 150);
            target.AddBuff(BuffID.Frostburn, 30);
        }
    }
}
