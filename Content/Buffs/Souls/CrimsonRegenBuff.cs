﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class CrimsonRegenBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Souls", Name);
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            modPlayer.CrimsonRegenTime += 1; //add one tick from counter

            if (modPlayer.CrimsonRegenTime % 420 == 0)
            {
                player.statLife += modPlayer.CrimsonRegenAmount;
                player.HealEffect(modPlayer.CrimsonRegenAmount);
            }

            for (int i = 0; i < 6; i++)
            {
                int dustType = modPlayer.CrimsonRegenTime > 420 ? DustID.Crimson : DustID.Blood; //change dust if its wizarded
                int num6 = Dust.NewDust(player.position, player.width, player.height, dustType, 0f, 0f, 175, default, 1.75f);
                Main.dust[num6].noGravity = true;
                Main.dust[num6].velocity *= 0.75f;
                int num7 = Main.rand.Next(-40, 41);
                int num8 = Main.rand.Next(-40, 41);
                Dust dust = Main.dust[num6];
                dust.position.X += num7;
                Dust dust2 = Main.dust[num6];
                dust2.position.Y += num8;
                Main.dust[num6].velocity.X = (float)-(float)num7 * 0.075f;
                Main.dust[num6].velocity.Y = (float)-(float)num8 * 0.075f;
            }

        }
    }
}