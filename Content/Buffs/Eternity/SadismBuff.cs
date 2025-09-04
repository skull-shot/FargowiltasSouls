using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Core.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class SadismBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs", "PlaceholderBuff");

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true; //ignore most debuff immunity
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffImmune[ModContent.BuffType<AnticoagulationBuff>()] = true;
            player.buffImmune[ModContent.BuffType<AntisocialBuff>()] = true;
            player.buffImmune[ModContent.BuffType<AtrophiedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<BerserkedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<BloodthirstyBuff>()] = true;
            player.buffImmune[ModContent.BuffType<ClippedWingsBuff>()] = true;
            player.buffImmune[ModContent.BuffType<CurseoftheMoonBuff>()] = true;
            player.buffImmune[ModContent.BuffType<DefenselessBuff>()] = true;
            player.buffImmune[ModContent.BuffType<FlamesoftheUniverseBuff>()] = true;
            player.buffImmune[ModContent.BuffType<FlippedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<HallowIlluminatedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<FusedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<GodEaterBuff>()] = true;
            player.buffImmune[ModContent.BuffType<HexedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<InfestedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<IvyVenomBuff>()] = true;
            player.buffImmune[ModContent.BuffType<JammedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<LethargicBuff>()] = true;
            player.buffImmune[ModContent.BuffType<LightningRodBuff>()] = true;
            player.buffImmune[ModContent.BuffType<LovestruckBuff>()] = true;
            player.buffImmune[ModContent.BuffType<LowGroundBuff>()] = true;
            player.buffImmune[ModContent.BuffType<MarkedforDeathBuff>()] = true;
            player.buffImmune[ModContent.BuffType<MidasBuff>()] = true;
            //player.buffImmune[ModContent.BuffType<PoweroftheCosmosBuff>()] = true;
            player.buffImmune[ModContent.BuffType<OiledBuff>()] = true;
            player.buffImmune[ModContent.BuffType<OceanicMaulBuff>()] = true;
            player.buffImmune[ModContent.BuffType<ReverseManaFlowBuff>()] = true;
            player.buffImmune[ModContent.BuffType<RottingBuff>()] = true;
            player.buffImmune[ModContent.BuffType<ShadowflameBuff>()] = true;
            player.buffImmune[ModContent.BuffType<SmiteBuff>()] = true;
            player.buffImmune[ModContent.BuffType<SqueakyToyBuff>()] = true;
            player.buffImmune[ModContent.BuffType<SwarmingBuff>()] = true;
            player.buffImmune[ModContent.BuffType<StunnedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<UnluckyBuff>()] = true;
            player.buffImmune[ModContent.BuffType<UnstableBuff>()] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            FargoSoulsGlobalNPC fargoNPC = npc.FargoSouls();
            //npc.poisoned = true;
            //npc.venom = true;
            npc.ichor = true;
            //npc.onFire2 = true;
            npc.betsysCurse = true;
            npc.midas = true;
            //fargoNPC.Electrified = true;
            fargoNPC.OceanicMaul = true;
            fargoNPC.CurseoftheMoon = true;
            //fargoNPC.Infested = true;
            fargoNPC.Rotting = true;
            fargoNPC.Sadism = true;
        }
    }
}