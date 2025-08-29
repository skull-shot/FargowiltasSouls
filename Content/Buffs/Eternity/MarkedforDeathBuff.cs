using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Eternity.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class MarkedforDeathBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().DeathMarked = true;

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<DeathSkull>()] < 1)
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex),
                    player.Center - 200f * Vector2.UnitY, Vector2.Zero,
                    ModContent.ProjectileType<DeathSkull>(), 0, 0f, player.whoAmI);
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().DeathMarked = true;
        }
    }
}
