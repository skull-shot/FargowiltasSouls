using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.CustomEnemies.OOA
{
    public abstract class DD2Enemy : ModNPC
    {
        public int invasionPointValue;

        public abstract int AssignPointValue();

        public override void Load()
        {
            invasionPointValue = AssignPointValue();
            base.Load();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.BelongsToInvasionOldOnesArmy[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.value = 0f;
            NPC.npcSlots = 0f;
            NPC.lavaMovementSpeed = 1f;
            NPC.waterMovementSpeed = 1f;
            NPC.honeyMovementSpeed = 1f;
            NPC.shimmerMovementSpeed = 1f;
        }

        public override void OnKill()
        {
            base.OnKill();

            if (!DD2Event.Ongoing)
                return;

            NPC.waveKills += invasionPointValue;
            DD2Event.CheckProgress(NPC.type);
        }
    }
}
