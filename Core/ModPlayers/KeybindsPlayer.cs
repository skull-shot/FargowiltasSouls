using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.UI;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.ModPlayers
{
    public partial class FargoSoulsPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            bool stunned = GoldShell || SpectreGhostTime > 0 || Player.CCed || NoUsingItems > 0;
            
            var keys = FargowiltasSouls.ActiveSkillKeys;
            for (int i = 0; i < keys.Length; i++)
            {
                AccessoryEffectPlayer aPlayer = Player.AccessoryEffects();

                var key = keys[i];
                var skill = ActiveSkills[i];
                if (skill == null || !aPlayer.Active(skill))
                    continue;
                if (key.JustPressed)
                    skill.ActiveSkillJustPressed(Player, stunned);
                if (key.Current)
                    skill.ActiveSkillHeld(Player, stunned);
                if (key.JustReleased)
                    skill.ActiveSkillJustReleased(Player, stunned);
            }

            #region ignores stuns

            if (FargowiltasSouls.ActiveSkillMenuKey.JustPressed && Player.whoAmI == Main.myPlayer)
                FargoUIManager.ToggleActiveSkillMenu();

            if (Mash)
            {
                Player.doubleTapCardinalTimer[0] = 0;
                Player.doubleTapCardinalTimer[1] = 0;
                Player.doubleTapCardinalTimer[2] = 0;
                Player.doubleTapCardinalTimer[3] = 0;

                const int increment = 1;

                if (triggersSet.Up)
                {
                    if (!MashPressed[0])
                        MashCounter += increment;
                    MashPressed[0] = true;
                }
                else
                    MashPressed[0] = false;

                if (triggersSet.Left)
                {
                    if (!MashPressed[1])
                        MashCounter += increment;
                    MashPressed[1] = true;
                }
                else
                    MashPressed[1] = false;

                if (triggersSet.Right)
                {
                    if (!MashPressed[2])
                        MashCounter += increment;
                    MashPressed[2] = true;
                }
                else
                    MashPressed[2] = false;

                if (triggersSet.Down)
                {
                    if (!MashPressed[3])
                        MashCounter += increment;
                    MashPressed[3] = true;
                }
                else
                    MashPressed[3] = false;
            }

            if (PrecisionSeal)
            {
                if (ClientConfig.Instance.PrecisionSealIsHold)
                    PrecisionSealNoDashNoJump = FargowiltasSouls.PrecisionSealKey.Current;
                else
                {
                    if (FargowiltasSouls.PrecisionSealKey.JustPressed)
                        PrecisionSealNoDashNoJump = !PrecisionSealNoDashNoJump;
                }
            }
            else
                PrecisionSealNoDashNoJump = false;

            if (PrecisionSealNoDashNoJump)
            {
                Player.doubleTapCardinalTimer[2] = 0;
                Player.doubleTapCardinalTimer[3] = 0;
            }

            if (FargowiltasSouls.SoulToggleKey.JustPressed)
                FargoUIManager.ToggleSoulToggler();

            #endregion

            if (stunned)
            {
                return;
            }

            #region blocked by stuns

            //if (FargowiltasSouls.SmokeBombKey.JustPressed && CrystalEnchantActive && SmokeBombCD == 0)
            //    CrystalAssassinEnchant.SmokeBombKey(this);

            if (Player.HasEffect<FrigidGemstoneKeyEffect>())
            {
                if (FrigidGemstoneCD > 0)
                    FrigidGemstoneCD--;
            }

            #endregion
        }
    }
}
