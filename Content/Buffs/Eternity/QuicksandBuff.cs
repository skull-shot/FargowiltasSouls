using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class QuicksandBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().Quicksanding = true;//does nothing rn

            Main.SceneMetrics.ShimmerMonolithState = 2; //gets around shimmer background
            player.shimmerTransparency = 0f;
            Main.shimmerDarken = 10;
            Main.shimmerBrightenDelay = 0f;

            //vanilla shimmering modified
            player.shimmering = true;

            player.fallStart = (int)(player.position.Y / 16f);

            if (player.wet)
            {
                player.buffTime[buffIndex] = 60;
                return;
            }

            //we still in solid blocks, so keep buff going
            player.frozen = true;

            bool flag32 = false;
            for (int num25 = (int)(player.position.X / 16f); (float)num25 <= (player.position.X + (float)player.width) / 16f; num25++)
            {
                for (int num26 = (int)(player.position.Y / 16f); (float)num26 <= (player.position.Y + (float)player.height) / 16f; num26++)
                {
                    if (WorldGen.SolidTile3(num25, num26))
                    {
                        flag32 = true;
                    }
                }
            }
            if (flag32)
            {
                player.buffTime[buffIndex] = 6;
            }
            else
            {
                player.DelBuff(buffIndex);
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            //??
        }
    }
}
