using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.CustomEnemies.OOA
{
    public abstract class DD2Enemy : ModNPC
    {
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
        }
    }
}
