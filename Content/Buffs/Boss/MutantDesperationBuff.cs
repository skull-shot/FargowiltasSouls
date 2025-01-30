
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Boss
{
    public class MutantDesperationBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mutant Presence");
            // Description.SetDefault("Defense, damage reduction, and life regen reduced; almost all soul toggles disabled; reduced graze radius");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变驾到");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "减少防御、伤害减免和生命恢复速度; 关闭近乎所有魂的效果; 附带混沌状态减益");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().MutantDesperation = true;
        }
    }
}
