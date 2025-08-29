
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class TimeFrozenBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Souls", Name);
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;

            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Incapacitate();
            player.velocity = player.oldVelocity;
            player.position = player.oldPosition;

            FargowiltasSouls.ManageMusicTimestop(player.buffTime[buffIndex] < 5);

            if (!Main.dedServ && player.whoAmI == Main.myPlayer)
            {
                ManagedScreenFilter filter = ShaderManager.GetFilter("FargowiltasSouls.Invert");
                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<CosmosChampion>())
                        && Main.npc[EModeGlobalNPC.championBoss].ai[0] == 15)
                {
                    filter.SetFocusPosition(Main.npc[EModeGlobalNPC.championBoss].Center);
                }

                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>())
                    && WorldSavingSystem.MasochistModeReal && Main.npc[EModeGlobalNPC.mutantBoss].ai[0] == -5)
                {
                    filter.SetFocusPosition(Main.npc[EModeGlobalNPC.mutantBoss].Center);
                }

                if (player.buffTime[buffIndex] > 60)
                    filter.Activate();

                if (player.buffTime[buffIndex] == 90)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Accessories/ZaWarudoResume"), player.Center);

                if (SoulConfig.Instance.ForcedFilters && Main.WaveQuality == 0)
                    Main.WaveQuality = 1;
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().TimeFrozen = true;
        }
    }
}