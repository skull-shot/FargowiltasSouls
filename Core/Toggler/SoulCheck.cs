using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Toggler
{
    public static class SoulCheck
    {
        public static Toggle GetToggle<T>(this Player player) where T : AccessoryEffect => player.GetToggle(ModContent.GetInstance<T>());
        public static Toggle GetToggle(this Player player, AccessoryEffect effect)
        {
            return player.FargoSouls().Toggler.Toggles.TryGetValue(effect, out Toggle value) ? value : null;
        }
        public static bool GetToggleValue<T>(this Player player) where T : AccessoryEffect => player.GetToggleValue(ModContent.GetInstance<T>());
        public static bool GetToggleValue(this Player player, AccessoryEffect effect, bool skipChecks = false)
        {
            Toggle toggle = player.GetToggle(effect);
            if (toggle == null)
                return false;
            if (!skipChecks)
            {
                /*
                if (effect.MinionEffect)
                {
                    FargoSoulsPlayer modPlayer = player.FargoSouls();
                    AccessoryEffectPlayer effectPlayer = player.AccessoryEffects();
                    if (modPlayer.GalacticMinionsDeactivated && !effectPlayer.DeactivatedEffects[effect.Index])
                    {
                        effectPlayer.DeactivatedEffects[effect.Index] = true;
                        modPlayer.DeactivatedMinionEffectCount++;
                        return false;
                    }
                }
                */
                if (player.FargoSouls().MutantPresence)
                    if (effect.MutantsPresenceAffects || effect.MinionEffect)
                        return false;
            }
            return toggle.ToggleBool;
        }

        public static bool GetPlayerBoolValue(this Player player, AccessoryEffect effect)
        {
            Toggle toggle = player.GetToggle(effect);
            return toggle.ToggleBool;
        }

        public static void SetToggleValue<T>(this Player player, bool value) where T : AccessoryEffect => player.SetToggleValue(ModContent.GetInstance<T>(), value);

        public static void SetToggleValue(this Player player, AccessoryEffect effect, bool value)
        {
            if (player.FargoSouls().Toggler.Toggles.ContainsKey(effect))
                player.FargoSouls().Toggler.Toggles[effect].ToggleBool = value;
            else
                FargowiltasSouls.Instance.Logger.Warn($"Expected toggle not found: {effect.Name}");
        }

    }
}
