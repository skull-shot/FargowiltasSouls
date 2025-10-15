﻿using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Armor.Eridanus;
using FargowiltasSouls.Content.Items.Armor.Nekomi;
using FargowiltasSouls.Content.Items.Armor.Styx;
using FargowiltasSouls.Content.Items.Armor.Gaia;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Systems
{
    public class SetBonusManager : ModSystem
    {
        public override void Load()
        {
            On_Player.KeyDoubleTap += new On_Player.hook_KeyDoubleTap(SetBonusKeyEffects);
        }
        public override void Unload()
        {
            On_Player.KeyDoubleTap -= new On_Player.hook_KeyDoubleTap(SetBonusKeyEffects);
        }
        public static void SetBonusKeyEffects(On_Player.orig_KeyDoubleTap orig, Player player, int keyDir)
        {
            orig.Invoke(player, keyDir);
            if (keyDir == (Main.ReversedUpDownArmorSetBonuses ? 1 : 0))
            {
                //GladiatorBanner.ActivateGladiatorBanner(player);
                //PalmwoodEffect.ActivatePalmwoodSentry(player);
                EridanusHat.EridanusSetBonusKey(player);
                GaiaHelmet.GaiaSetBonusKey(player);
                NekomiHood.NekomiSetBonusKey(player);
                StyxCrown.StyxSetBonusKey(player);
                //ForbiddenEffect.ActivateForbiddenStorm(player);
                //SpiritTornadoEffect.ActivateSpiritStorm(player);
            }

        }
    }
}
