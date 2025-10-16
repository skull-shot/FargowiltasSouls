using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.ModPlayers
{
    public class BoosterPlayer : ModPlayer
    {
        public int SolarTimer = 0;
        public int VortexTimer = 0;
        public int NebulaTimer = 0;
        public int StardustTimer = 0;

        public int TimberTimer = 0;
        public int TerraTimer = 0;
        public int EarthTimer = 0;
        public int NatureTimer = 0;
        public int LifeTimer = 0;
        public int DeathTimer = 0;
        public int SpiritTimer = 0;
        public int WillTimer = 0;
        public int CosmosTimer = 0;
        public override void PostUpdateEquips()
        {
            bool hasNebulaDmg = Player.HasBuff(BuffID.NebulaUpDmg1) || Player.HasBuff(BuffID.NebulaUpDmg2) || Player.HasBuff(BuffID.NebulaUpDmg3);
            //if (VortexTimer > 0 && hasNebulaDmg)
            //    VortexTimer = 0;
            if (StardustTimer > 0 && hasNebulaDmg)
                StardustTimer = 0;
            if (NebulaTimer > 0 && (Player.HasBuff(BuffID.NebulaUpLife1) || Player.HasBuff(BuffID.NebulaUpLife2) || Player.HasBuff(BuffID.NebulaUpLife3)))
                NebulaTimer = 0;

            if (SolarTimer > 0)
            {
                SolarTimer--;
                Player.endurance += 0.05f;
            }
            if (VortexTimer > 0)
            {
                VortexTimer--;
                Player.GetCritChance(DamageClass.Generic) += 15;
            }
                
            if (NebulaTimer > 0)
            {
                NebulaTimer--;
            }
            if (StardustTimer > 0)
            {
                StardustTimer--;
                Player.GetDamage(DamageClass.Generic) += 0.15f;
            }
        }
        public override void UpdateLifeRegen()
        {
            if (NebulaTimer > 0)
                Player.lifeRegen += 4;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
    }
}
