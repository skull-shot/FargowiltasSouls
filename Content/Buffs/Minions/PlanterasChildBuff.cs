using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class PlanterasChildBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Minions", Name);
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().PlanterasChild = true;
            const int damage = 66;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<PlanterasChild>()] < 1)
                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, -Vector2.UnitY, ModContent.ProjectileType<PlanterasChild>(), damage, 3f, player.whoAmI);
        }
    }
}