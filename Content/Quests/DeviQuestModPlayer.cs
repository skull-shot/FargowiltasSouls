using FargowiltasSouls.Content.Quests.DeviQuestTasks;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Quests
{
    public class DeviQuestModPlayer : ModPlayer
    {
        public int UpdateTimer = 0;
        public List<string> claimedQuests = new List<string>();

        /// <summary>
        /// Claims the given quest. Does nothing if the quest was already claimed.
        /// </summary>
        /// <param name="quest"></param>
        public void ClaimQuest(DeviQuest quest)
        {
            if (!HasClaimed(quest))
            {
                claimedQuests.Add(quest.GetID());
            }
        }

        /// <summary>
        /// Checks whether this player has claimed a given quest
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        public bool HasClaimed(DeviQuest quest) => claimedQuests.Contains(quest.GetID());

        public override void PreUpdate()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && UpdateTimer++ >= 60) // only update once per second and not on clients
            {
                UpdateQuests(Player);
                UpdateTimer = 0;
            }

            base.PreUpdate();
        }

        // Only called on server
        public void UpdateQuests(Player player)
        {
            foreach (DeviQuest quest in FargowiltasSouls.questTracker.SortedQuests)
            {
                foreach (var task in quest.GetTasks())
                {
                    if (!task.IsComplete() && task is DeviQuestCollectTask cTask)
                    {
                        int id = cTask.GetItemID();
                        if (player.HasItem(id))
                        {
                            quest.ProgressQuestTask(task, Player.CountItem(id, cTask.GetMaxCount()));
                        }
                        else if (player.HeldMouseItem().type == id) // also check mouse index
                        {
                            quest.ProgressQuestTask(task, player.HeldMouseItem().stack);
                        }
                    }
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Set("claimedQuests", claimedQuests);
        }

        public override void LoadData(TagCompound tag)
        {
            claimedQuests = tag.GetList<string>("claimedQuests").ToList();
        }
    }
}
