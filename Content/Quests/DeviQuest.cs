using FargowiltasSouls.Content.Quests.DeviQuestTasks;
using FargowiltasSouls.Content.Quests.UI;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using static FargowiltasSouls.FargowiltasSouls;

namespace FargowiltasSouls.Content.Quests
{
    public class DeviQuest
    {
        #region DeviQuestIO
        public TagCompound SerializeData()
        {
            TagCompound tag = new TagCompound
            {
                ["interaction"] = interaction,
                ["completedQuest"] = completedQuest
            };

            for (int i = 0; i < tasks.Count; i++)
            {
                tag.Set("Task" + i, tasks[i].Save());
            }

            // rewards should be saved by players


            return tag;
        }

        /// <summary>
        /// Populates the quest instance with data from the tag.
        /// </summary>
        public static void LoadInstance(DeviQuest quest, TagCompound tag)
        {
            if (tag.ContainsKey("interaction"))
                quest.interaction = tag.GetBool("interaction");
            if (tag.ContainsKey("completedQuest"))
                quest.completedQuest = tag.GetBool("completedQuest");

            int tCount = quest.tasks.Count;
            for (int i = 0; i < tCount; i++)
            {
                TagCompound taskTag = tag.GetCompound("Task" + i);

                quest.tasks[i].Load(taskTag);
            }

            int rCount = quest.rewards.Count;
            for (int i = 0; i < tCount; i++)
            {
                TagCompound rewardTag = tag.Get<TagCompound>("Reward" + i);


            }
        }
        #endregion

        public override bool Equals(object? obj)
        {
            if (obj is DeviQuest q)
                return q.GetQuestName() == questName && q.GetMod() == Mod;
            return false;
        }

        public string DeviQuestLocal(string name) => pathName + name;

        #region Public Members
        string Mod;
        string pathName;
        string questName;
        bool completedQuest;
        public bool interaction;

        List<DeviQuestBaseTask> tasks;
        List<DeviQuestReward> rewards;
        Func<bool> unlockCondtion;
        #endregion

        public string GetLocalizedText(string key) => Language.GetTextValue(pathName + questName + key);

        /// <summary>
        /// Creates a new DeviQuest instance
        /// <para/>
        /// A quest without a given unlockCondition will always be available.
        /// <para/>
        /// <b>NOTE:</b> The local path should not include the quest name.
        /// (i.e. A quest in the file "Mods.FargowiltasSouls.Quests.ExampleQuest" should have localPath "Mods.FargowiltasSouls.Quests" and questName "ExampleQuest")
        /// <para/>
        /// <b>NOTE:</b> An ArgumentException will be thrown if a quest instance from the same mod with the same quest name already exists.
        /// </summary>
        /// <param name="Mod">The internal mod name of the instance creator</param>
        /// <param name="localPath">The localization path to the quest</param>
        /// <param name="questName">The name of the </param>
        /// <param name="tasks">The list of quest tasks</param>
        /// <param name="rewards">The list of quest rewards</param>
        /// <param name="unlockCondtion">The condition required for the to become available</param>
        /// <exception cref="ArgumentException"></exception>
        public DeviQuest(string Mod, string localPath, string questName, List<DeviQuestBaseTask> tasks, List<DeviQuestReward> rewards, Func<bool> unlockCondtion = null)
        {
            if (DeviQuestRegistry.IsRegistered(Mod, questName))
                throw new ArgumentException("Quest name '" + questName + "' from Mod '" + Mod + "' has already been registered!");


            this.Mod = Mod;
            this.pathName = localPath + ".";
            this.questName = questName;
            this.tasks = tasks;
            this.rewards = rewards;

            if (unlockCondtion != null)
                this.unlockCondtion = unlockCondtion;
            else
                this.unlockCondtion = (() => true);

            DeviQuestRegistry.Register(this);

            Init();
        }

        /// <summary>
        /// Initializes values after creation.
        /// </summary>
        private void Init()
        {
            interaction = false;
            completedQuest = false;

            foreach (var t in tasks)
                t.localPath = pathName;
        }

        #region Get Methods
        /// <summary>Returns the internal quest name</summary>
        public string GetQuestName() => questName;
        public string GetMod() => Mod;
        public string GetID() => Mod + "_" + questName;
        public string GetName() => Language.GetTextValue(pathName + questName + ".Name");
        public string GetDescription() => Language.GetTextValue(pathName + questName + ".Description");
        public bool HasBeenInteractedWith() => interaction;
        public List<DeviQuestBaseTask> GetTasks() => tasks;
        public List<DeviQuestReward> GetRewardStrings() => rewards;
        #endregion

        #region Helper Methods

        /// <summary>
        ///Use to indicate that the quest has been interacted with. Used to determine whether an exclamation point appears in the quest board.
        ///</summary>
        public void Interact() => interaction = true;

        /// <summary>Indicates whether or not the unlock condition for this quest has been met.</summary>
        public bool IsUnlocked() => unlockCondtion();

        /// <summary>
        /// Indicates whether the given quest is completed.
        /// </summary>
        public bool IsQuestComplete() => completedQuest;

        /// <summary>
        /// Checks if every task is completed. Will update completion status of this quest.
        /// </summary>
        public void CheckComplete()
        {
            if (completedQuest)
                return;

            foreach (var task in tasks)
            {
                if (!task.TaskFinishCondition())
                {
                    completedQuest = false;
                    return;
                }
            }

            completedQuest = true;
            OnComplete();
            return;
        }

        public int GetTaskIndex(DeviQuestBaseTask task) => tasks.IndexOf(task);

        /// <summary>
        /// Called when the quest is completed. Initiates a quest completion pop up.
        /// </summary>
        public void OnComplete()
        {
            DeviQuestPopupSystem.CreatePopup(DeviQuestPopupSystem.PopupState.QuestComplete);
            interaction = false;
        }

        /// <summary>
        /// Attempts to progress the given task of a quest.
        /// <para/> The task will not be progressed if the task is already completed, the quest should not be updated, or the quest does not contain the given task
        /// </summary>
        /// <param name="task">The task to progress</param>
        /// <returns>True if the task was progressed, false otherwise.</returns>
        public bool ProgressQuestTask(DeviQuestBaseTask task)
        {
            if (!tasks.Contains(task) || !ShouldBeUpdated() || task.completedTask)
                return false;

            task.ProgressTask();
            CheckComplete();
            return true;
        }

        /// <summary>
        /// Attempts to progress the given task of a quest.
        /// <para/> The task will not be progressed if the task is already completed, the quest should not be updated, or the quest does not contain the given task
        /// <para/> Additionally contains 3 object arguments to send data to the task
        /// </summary>
        /// <param name="task">The task to progress</param>
        /// <returns>True if the task was progressed, false otherwise.</returns>
        public bool ProgressQuestTask(DeviQuestBaseTask task, object arg0 = null, object arg1 = null, object arg2 = null)
        {
            if (!tasks.Contains(task) || !ShouldBeUpdated() || task.completedTask)
                return false;

            if (arg0 != null)
                task.ProgressTask(arg0, arg1, arg2);
            else 
                task.ProgressTask();
            CheckComplete();
            return true;
        }

        /// <summary>Indicates whether the quest should recieve progress updates from things that would normally progress tasks.</summary>
        public bool ShouldBeUpdated() => !IsQuestComplete() && IsUnlocked();

        /// <summary>
        /// Handles a submit request from the given player. Will give the player quest rewards if the player has not already claimed them.
        /// </summary>
        /// <param name="player">The player who submitted</param>
        public void HandleSubmitRequest(Player player)
        {
            DeviQuestModPlayer modPlayer = player.QuestPlayer();
            if (modPlayer.HasClaimed(this))
                return;

            modPlayer.ClaimQuest(this);

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                GiveQuestRewards(Main.LocalPlayer);
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                var netMessage = Instance.GetPacket(); // Broadcast item request to server
                netMessage.Write((byte)PacketID.RequestDeviQuestReward);
                netMessage.Write((byte)Main.LocalPlayer.whoAmI);
                netMessage.Write(GetID());
                netMessage.Send();
            }
        }

        /// <summary>Gives the rewards for this quest.
        /// <para/> Should NOT be called on multiplayer clients.
        /// </summary>
        /// <param name="player">The player who will receive the rewards</param>
        public void GiveQuestRewards(Player player)
        {
            foreach (var reward in rewards)
            {
                Item.NewItem(null, player.Center, reward.GetItem(), reward.GetStack());
            }
        }
        #endregion
    }

    /// <summary>
    /// Registry that tracks all created quests.
    /// </summary>
    public static class DeviQuestRegistry
    {
        public static readonly Dictionary<string, DeviQuest> Regisrty = new Dictionary<string, DeviQuest>();

        public static string FormatKey(string Mod, string Name) => Mod + "_" + Name;

        public static bool IsRegistered(string Mod, string Name) => Regisrty.ContainsKey(FormatKey(Mod, Name));

        public static bool IsRegistered(string key) => Regisrty.ContainsKey(key);

        /// <summary>
        /// Registers the given quest provided it is not already in the registry. 
        /// </summary>
        /// <returns>True if the quest was added to the registry, false otherwise.</returns>
        public static bool Register(DeviQuest quest)
        {
            string key = FormatKey(quest.GetMod(), quest.GetQuestName());
            if (!IsRegistered(quest.GetMod(), quest.GetQuestName()))
            {
                Regisrty[key] = quest;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves a quest from the registry
        /// </summary>
        /// <returns>The quest at the given key, or null if the quest is not registered</returns>
        public static DeviQuest? GetQuest(string Mod, string Name)
            => GetQuest(FormatKey(Mod, Name));

        public static DeviQuest? GetQuest(string key)
        {
            if (IsRegistered(key))
                return Regisrty[key];
            return null;
        }
    }
}
