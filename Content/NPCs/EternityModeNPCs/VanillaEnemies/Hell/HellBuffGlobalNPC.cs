using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Items.Placables;
using FargowiltasSouls.Content.Projectiles.Eternity.Environment;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ItemDropRules.Conditions;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Hell
{
    public partial class HellBuffGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public static bool HellBuffActive => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.Lifelight] || NPC.downedPlantBoss;
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (HellBuffActive)
            {
                // you can't manually remove vanilla spawns this easily btw
            }
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // drop loot thing
        }
    }
}
