using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
    public class BerserkerInstallBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public static void DebuffPlayerStats(Player player)
        {
            player.endurance -= 0.30f;
            player.statDefense -= 30;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.FargoSouls().BerserkedFromAgitation)
                DebuffPlayerStats(player);
            player.FargoSouls().Berserked = true;
            player.FargoSouls().AttackSpeed += 0.35f;
            //player.GetDamage(DamageClass.Generic) += 0.20f;
            //player.GetCritChance(DamageClass.Generic) += 20;
            player.moveSpeed += 0.20f;
            player.hasMagiluminescence = true;
            player.noKnockback = true;

            if (!player.controlLeft && !player.controlRight)
            {
                if (player.velocity.X > 0)
                    player.controlRight = true;
                else if (player.velocity.X < 0)
                    player.controlLeft = true;
                else if (player.direction > 0)
                    player.controlRight = true;
                else
                    player.controlLeft = true;
            }

            if (player.buffTime[buffIndex] > 2)
                player.FargoSouls().NoMomentum = true;

            if (player.buffTime[buffIndex] == 2 && player.FargoSouls().BerserkedFromAgitation)
            {
                int stunDuration = 120; //2sec
                player.AddBuff(ModContent.BuffType<BerserkerInstallCDBuff>(), 60 * 10);
                player.AddBuff(ModContent.BuffType<StunnedBuff>(), stunDuration);
                player.FargoSouls().BerserkedFromAgitation = false;
            }
        }
    }
}