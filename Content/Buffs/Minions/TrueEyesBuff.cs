using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class TrueEyesBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Minions", Name);
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().TrueEyes = true;

            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<TrueEyeL>()] < 1)
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueEyeL>(), MasoTrueEyeMinion.BaseDamage(player), 3f, player.whoAmI, -1f);

                if (player.ownedProjectileCounts[ModContent.ProjectileType<TrueEyeR>()] < 1)
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueEyeR>(), MasoTrueEyeMinion.BaseDamage(player), 3f, player.whoAmI, -1f);

                if (player.ownedProjectileCounts[ModContent.ProjectileType<TrueEyeS>()] < 1)
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueEyeS>(), MasoTrueEyeMinion.BaseDamage(player), 3f, player.whoAmI, -1f);
            }
        }
    }
}