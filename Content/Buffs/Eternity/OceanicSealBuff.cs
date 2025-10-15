using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Eternity.Bosses.DukeFishron;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class OceanicSealBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;

            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().OceanicMaul = true;
            player.FargoSouls().TinEternityDamage = 0; //fuck it

            player.FargoSouls().MutantPresence = true; //LUL

            player.FargoSouls().noDodge = true;
            player.FargoSouls().noSupersonic = true;
            player.moonLeech = true;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBoss, NPCID.DukeFishron))
            {
                player.buffTime[buffIndex] = 2;
                if (player.whoAmI == Main.npc[EModeGlobalNPC.fishBoss].target
                    && player.whoAmI == Main.myPlayer
                    && player.ownedProjectileCounts[ModContent.ProjectileType<FishronRitual2>()] < 1)
                {
                    Projectile.NewProjectile(Main.npc[EModeGlobalNPC.fishBoss].GetSource_FromThis(), Main.npc[EModeGlobalNPC.fishBoss].Center, Vector2.Zero,
                        ModContent.ProjectileType<FishronRitual2>(), FargoSoulsUtil.ScaledProjectileDamage(Main.npc[EModeGlobalNPC.fishBoss].damage, 0.5f), 0f, player.whoAmI, 0f, EModeGlobalNPC.fishBoss);
                }
            }
            else
            {
                return;
            }
        }
    }
}