using FargowiltasSouls.Content.Quests.DeviQuestTasks;
using System;
using System.Collections.Generic;

namespace FargowiltasSouls.Content.Quests
{
    /// <summary>
    /// Factory class for making DeviQuests.
    /// </summary>
    public static class DeviQuestFactory
    {
        const string modName = "FargowiltasSouls";
        const string localPath = "Mods.FargowiltasSouls.DeviQuests";

        /// <summary>
        /// Creates a Fargo's Souls DeviQuest with the given parameters.
        /// <para/>
        /// A null unlock condition will make the quest always available.
        /// <para/>
        /// For implementation details, see also <seealso cref="DeviQuest(string, string, string, List{DeviQuestBaseTask}, List{DeviQuestReward}, Func{bool})"/>
        /// </summary>
        /// <param name="questName"></param>
        /// <param name="tasks"></param>
        /// <param name="rewards"></param>
        /// <param name="unlockCondition"></param>
        /// <returns></returns>
        public static DeviQuest NewQuest(string questName, List<DeviQuestBaseTask> tasks, List<DeviQuestReward> rewards, Func<bool> unlockCondition = null)
        {
            return new DeviQuest(modName, localPath, questName, tasks, rewards, unlockCondition);
        }

        #region Tasks
        private static DeviQuestBaseTask TaskCreate(string type, object arg0, object arg1 = null)
        {
            switch (type)
            {
                case "Hunt":
                    if (arg0 is short)
                        arg0 = (int)(short)arg0; // need to unbox short
                    return new DeviQuestHuntTask((int)arg0, (int)arg1);
                case "Craft":
                    if (arg0 is short)
                        arg0 = (int)(short)arg0; // need to unbox short
                    return new DeviQuestCraftTask((int)arg0);
                case "Collect":
                    if (arg0 is short)
                        arg0 = (int)(short)arg0; // need to unbox short
                    if (arg1 is null)
                        arg1 = 1;
                    return new DeviQuestCollectTask((int)arg0, (int)arg1);
                case "Custom":
                    return new DeviQuestCustomTask((Func<bool>)arg0, (Func<string>)arg1);
                default:
                    return null;
            }
        }

        public static List<DeviQuestBaseTask> TaskSingle(string type, object arg0, object arg1 = null)
        {
            return new List<DeviQuestBaseTask> { TaskCreate(type, arg0, arg1) };
        }

        public static List<DeviQuestBaseTask> TaskDouble(string typeA, string typeB, object argA0, object argB0, object argA1 = null, object argB1 = null)
        {
            return new List<DeviQuestBaseTask> { TaskCreate(typeA, argA0, argA1), TaskCreate(typeB, argB0, argB1) };
        }
        #endregion

        #region Rewards
        private static DeviQuestReward RewardCreate(int id, int stack = 1)
        {
            return new DeviQuestReward(id, stack);
        }

        public static List<DeviQuestReward> RewardSingle(int id, int stack = 1)
        {
            return new List<DeviQuestReward> { RewardCreate(id, stack) };
        }

        public static List<DeviQuestReward> RewardDouble(int id1, int id2, int stack1 = 1, int stack2 = 1)
        {
            return new List<DeviQuestReward> { RewardCreate(id1, stack1), RewardCreate(id2, stack2) };
        }
        #endregion
    }
}
