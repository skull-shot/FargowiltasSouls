using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class NeurotoxinBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer p = player.FargoSouls();

            player.poisoned = true;
            player.venom = true;
            player.slowOgreSpit = true;

            player.ClearBuff(ModContent.BuffType<InfestedBuff>());

            p.MaxInfestTime = 2;
            p.FirstInfection = false;
            p.Infested = true;
        }
    }
}