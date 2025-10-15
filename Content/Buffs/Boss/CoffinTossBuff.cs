using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Boss
{
    public class CoffinTossBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Boss", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Incapacitate();
            player.FargoSouls().Stunned = true;

            player.velocity = Vector2.Normalize(player.velocity) * 30;
            player.fullRotation = player.velocity.ToRotation() + MathHelper.PiOver2;
            player.fullRotationOrigin = player.Center - player.position;

            if (player.buffTime[buffIndex] < 2) // make sure you get unrotated
            {
                player.fullRotation = 0;
                player.DelBuff(buffIndex);
            }


            if (Collision.SolidCollision(player.position + player.velocity, player.width, player.height))
            {
                int damage = 35;
                LocalizedText DeathText = Language.GetText("Mods.FargowiltasSouls.DeathMessage.CoffinToss");
                player.Hurt(PlayerDeathReason.ByCustomReason(DeathText.ToNetworkText(player.name)), damage, 0, false, false, 0, false);
                player.DelBuff(buffIndex);
                player.ClearBuff(ModContent.BuffType<StunnedBuff>());
                SoundEngine.PlaySound(SoundID.NPCHit18, player.Center);
                float multiplier = WorldSavingSystem.MasochistModeReal ? 1f : 0.05f; // markiplier
                player.velocity *= -multiplier;
                player.fullRotation = 0;
            }
        }
    }
}