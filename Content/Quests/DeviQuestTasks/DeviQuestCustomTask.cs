using System;

namespace FargowiltasSouls.Content.Quests.DeviQuestTasks
{
    /// <summary>
    /// Generic task for any boolean condition.
    /// </summary>
    public class DeviQuestCustomTask : DeviQuestBaseTask
    {
        Func<bool> Condition;
        Func<string> Description;

        public DeviQuestCustomTask(Func<bool> CompleteCondition, Func<string> description)
        {
            Condition = CompleteCondition;
            Description = description;

            Init();
        }

        public override Func<bool> CreateTaskFinishCondition() => (() => Condition());

        public override Func<string> CreateTaskDescription() => (() => Description());
    }
}
