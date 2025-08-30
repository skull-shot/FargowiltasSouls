using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Pets
{
    public class BabyLifelightBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Pets", Name);
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            //Main.vanityPet[Type] = true;
            Main.lightPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.FargoSouls().BabyLifelight = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BabyLifelight>()] <= 0 && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<BabyLifelight>(), 0, 0f, player.whoAmI);
            }
        }
    }
}