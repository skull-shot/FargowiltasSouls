using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class LowGroundEridanusBuff : ModBuff
    {
        // This exists so you can't have immunity to it, and to only disable platforms and nothing else.
        public override string Texture => "FargowiltasSouls/Content/Buffs/Masomode/LowGroundBuff";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;

            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().LowGround = true;

            Tile thisTile = Framing.GetTileSafely(player.Bottom);
            Tile bottomTile = Framing.GetTileSafely(player.Bottom + Vector2.UnitY * 8);

            if (!Collision.SolidCollision(player.BottomLeft, player.width, 16))
            {
                if (player.velocity.Y >= 0 && (IsPlatform(thisTile.TileType) || IsPlatform(bottomTile.TileType)))
                {
                    player.position.Y += 2;
                }
                if (player.velocity.Y == 0)
                {
                    player.position.Y += 16;
                }

            }

            static bool IsPlatform(int tileType)
            {
                return tileType == TileID.Platforms || tileType == TileID.PlanterBox;
            }
        }
    }
}
