using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.BloodMoon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class FusedBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().Fused = true;

            if (player.buffTime[buffIndex] == 2)
            {
                player.immune = false;
                player.immuneTime = 0;
                int damage = (int)(Math.Max(player.statLife, player.statLifeMax) * 2.0 / 3.0);
                // TODO: 1.4.4 porting: I have no idea what previous falses were
                LocalizedText DeathText = Language.GetText("Mods.FargowiltasSouls.DeathMessage.Fused");
                player.Hurt(PlayerDeathReason.ByCustomReason(DeathText.ToNetworkText(player.name)), damage, 0, dodgeable: false);
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<FusedExplosion>(), damage, 12f, Main.myPlayer);
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().Fused = true;

            if (npc.buffTime[buffIndex] == 2 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<FusedExplosion>(), 400, 12f, Main.myPlayer);
            }
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            return true;
        }
    }
}