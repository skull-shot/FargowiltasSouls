using FargowiltasSouls.Core.Systems;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Quests.DeviQuestTasks
{
    public class DeviQuestTaskUpdateSystem : ModSystem
    {

        public override void Load()
        {
            On_Recipe.Create += DetectCraft;
            base.Load();
        }

        public override void Unload()
        {
            On_Recipe.Create -= DetectCraft;
            base.Unload();
        }

        public static void DetectCraft(On_Recipe.orig_Create orig, Recipe self)
        {
            orig(self);

            if (!WorldSavingSystem.EternityMode || Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Item result = self.createItem;
            foreach(var quest in FargowiltasSouls.questTracker.SortedQuests)
            {
                if (!quest.ShouldBeUpdated())
                    continue;

                foreach (var task in quest.GetTasks())
                {
                    if (task is DeviQuestCraftTask cTask && !cTask.TaskFinishCondition() && cTask.GetCraftType() == result.type)
                    {
                        quest.ProgressQuestTask(cTask);
                    }
                }
            }
        }

        public override void PostUpdateWorld()
        {
            foreach (var quest in FargowiltasSouls.questTracker.SortedQuests)
            {
                if (quest.ShouldBeUpdated() && quest.GetTasks().Any(t => t is DeviQuestCustomTask && t.TaskFinishCondition()))
                {
                    foreach (DeviQuestBaseTask task in quest.GetTasks())
                    {
                        quest.ProgressQuestTask(task);
                    }
                }
            }
        }
    }

    public class DeviQuestGlobalNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            // OnKill not called on mp clients no need to sync
            foreach (var quest in FargowiltasSouls.questTracker.SortedQuests)
            {
                if (!quest.ShouldBeUpdated())
                    continue;

                foreach(var task in quest.GetTasks())
                {
                    if (task is DeviQuestHuntTask hTask && !hTask.IsComplete() && hTask.GetID() == npc.type)
                    {
                        quest.ProgressQuestTask(hTask);
                    }
                }
            }
        }
    }
}
