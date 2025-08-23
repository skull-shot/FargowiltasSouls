using FargowiltasSouls.Core.Systems;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace FargowiltasSouls.Core.ItemDropRules.Conditions
{
    public class RabiesVaccineDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !info.IsInSimulation && Main.player.Any(p => p.Alive() && !p.FargoSouls().RabiesVaccine);

        public bool CanShowItemDropInUI() => WorldSavingSystem.EternityMode;

        public string GetConditionDescription() => Language.GetTextValue("Mods.FargowiltasSouls.Conditions.RabiesVaccine");
    }
}
