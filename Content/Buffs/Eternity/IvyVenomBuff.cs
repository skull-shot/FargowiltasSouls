using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class IvyVenomBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] > 1200)
            {
                player.AddBuff(ModContent.BuffType<NeurotoxinBuff>(), player.buffTime[buffIndex]);
                player.buffTime[buffIndex] = 1;
                SoundEngine.PlaySound(SoundID.Roar, player.Center);
                if (player.whoAmI == Main.myPlayer)
                    Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Buffs.IvyVenomBuff.Transform"), 175, 75, 255);
            }
            player.FargoSouls().IvyVenom = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().IvyVenom = true;
            if (npc.FargoSouls().IvyVenomTime < 540)
                npc.FargoSouls().IvyVenomTime += 1;
            if (npc.buffTime[buffIndex] == 0)
                npc.FargoSouls().IvyVenomTime = 0;
        }
    }
}