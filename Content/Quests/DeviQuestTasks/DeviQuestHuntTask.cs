using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Quests.DeviQuestTasks
{
    /// <summary>
    /// Generic task for defeating a certain amount of a given enemy
    /// </summary>
    public class DeviQuestHuntTask : DeviQuestBaseTask
    {
        public override void OnSave(ref TagCompound tag)
        {
            tag.Set("currentCount", currentCount);
        }

        public override void OnLoad(TagCompound tag)
        {
            currentCount = tag.GetInt("currentCount");
        }

        int enemyId;
        int currentCount;
        int totalCount;

        /// <summary>
        /// Creates a quest task to kill the specified number of the given enemy ID
        /// </summary>
        /// <param name="enemyId"></param>
        /// <param name="killCount"></param>
        public DeviQuestHuntTask(int enemyId, int killCount)
        {
            this.enemyId = enemyId;
            totalCount = killCount;
            currentCount = 0;

            Init();
        }

        public override void OnProgress() { currentCount++; }

        public int GetID() => enemyId;

        public override Func<string> CreateTaskDescription() 
            => (() => Language.GetTextValue("Mods.FargowiltasSouls.DeviQuests.HuntTask", NPC.GetFullnameByID(enemyId), currentCount, totalCount));
        public override Func<bool> CreateTaskFinishCondition() => (() => currentCount >= totalCount);
    }
}
