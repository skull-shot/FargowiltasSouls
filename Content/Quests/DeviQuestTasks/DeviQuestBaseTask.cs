using System;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Quests.DeviQuestTasks
{
    /// <summary>
    /// Abstract Task class that all Tasks derive from.
    /// </summary>
    public abstract class DeviQuestBaseTask
    {
        #region Public Members
        public Func<bool> TaskFinishCondition;
        public Func<string> TaskDescription;

        public bool completedTask;
        public string localPath;
        #endregion

        #region Abstract Functions
        /// <summary>
        /// Dictates the condition needed to complete the quest.
        /// </summary>
        public abstract Func<bool> CreateTaskFinishCondition();

        /// <summary>
        /// Dictates the text displayed for the task.
        /// </summary>
        public abstract Func<string> CreateTaskDescription();
        #endregion

        #region Virtual Methods
        /// <summary>
        /// Use this to save any additional task data. Nullifies the tag by default.
        /// </summary>
        public virtual void OnSave(ref TagCompound tag) { tag = null; }

        /// <summary>
        /// Use this to load any additional task data.
        /// </summary>
        public virtual void OnLoad(TagCompound tag) { }

        /// <summary>
        /// Called whenever the task is marked as completed.
        /// </summary>
        public virtual void OnComplete() { }

        #region OnProgess()
        /// <summary>
        /// Called when the task should progress in some way. Use this to increment and set variables
        /// </summary>
        public virtual void OnProgress() { }

        /// <summary>
        /// Called when the task should progress in some way. Use this to increment and set variables
        /// <para/> Includes up to 3 additional data passed though <seealso cref="DeviQuest.ProgressQuestTask(DeviQuestBaseTask, object, object, object)"/>
        /// </summary>
        public virtual void OnProgress(object arg0) { }

        /// <summary>
        /// Called when the task should progress in some way. Use this to increment and set variables
        /// <para/> Includes up to 3 additional data passed though <seealso cref="DeviQuest.ProgressQuestTask(DeviQuestBaseTask, object, object, object)"/>
        /// </summary>
        public virtual void OnProgress(object arg0, object arg1) { }

        /// <summary>
        /// Called when the task should progress in some way. Use this to increment and set variables
        /// <para/> Includes up to 3 additional data passed though <seealso cref="DeviQuest.ProgressQuestTask(DeviQuestBaseTask, object, object, object)"/>
        /// </summary>
        public virtual void OnProgress(object arg0, object arg1, object arg2) { }
        #endregion
        #endregion

        #region TaskIO
        public TagCompound Save()
        {
            var tag = new TagCompound
            {
                ["completedTask"] = completedTask
            };

            TagCompound customTag = new TagCompound();
            OnSave(ref customTag);
            if (customTag != null)
                tag.Set("taskData", customTag);

            return tag;
        }

        public void Load(TagCompound tag)
        {
            if (tag.ContainsKey("completedTask"))
                completedTask = tag.GetBool("completedTask");

            if (tag.ContainsKey("taskData"))
                OnLoad(tag.GetCompound("taskData"));
        }
        #endregion

        public void ProgressTask(object arg0, object? arg1 = null, object? arg2 = null)
        {
            if (!completedTask)
            {
                if (arg2 != null && arg1 != null)
                    OnProgress(arg0, arg1, arg2);
                else if (arg1 != null)
                    OnProgress(arg0, arg1);
                else
                    OnProgress(arg0);

                CheckComplete();
            }
        }

        public void ProgressTask()
        {
            if (!completedTask)
            {
                OnProgress();
                CheckComplete();
            }
        }

        public bool CheckComplete()
        {
            if (TaskFinishCondition())
            {
                completedTask = true;
                OnComplete();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Initializes public members. Call this at the end of any constructor.
        /// </summary>
        public void Init()
        {
            TaskFinishCondition = CreateTaskFinishCondition();
            TaskDescription = CreateTaskDescription();

            completedTask = false;
            localPath = "";
        }

        public bool IsComplete() => completedTask;
    }
}
