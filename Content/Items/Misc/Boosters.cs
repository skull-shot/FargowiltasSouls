using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Souls;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Misc
{
    public abstract class Booster : ModItem
    {
        public const int LunarDuration = 10 * 60;
        public const int TerrariaDuration = 20 * 60;
        public virtual int Frames => 1;
        public override void SetStaticDefaults()
        {
            // Pickup sets that Nebula Boosters are in
            ItemID.Sets.IsAPickup[Type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Type] = true;
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;
            ItemID.Sets.ItemNoGravity[Type] = true;
            if (Frames > 1)
            {
                ItemID.Sets.AnimatesAsSoul[Type] = true;
                Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(10, Frames));
            }
        }
        public abstract void PickupEffect(BoosterPlayer boosterPlayer);
        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab, Item.position);
            PickupEffect(player.GetModPlayer<BoosterPlayer>());
            return false;
        }
        public override void GrabRange(Player player, ref int grabRange)
        {
            grabRange += 260;
        }
        public static MethodInfo PullItem_PickupMethod
        {
            get;
            set;
        }
        public override void Load()
        {
            PullItem_PickupMethod = typeof(Player).GetMethod("PullItem_Pickup", LumUtils.UniversalBindingFlags);
        }
        public override bool GrabStyle(Player player)
        {
            object[] args = [Item, 12f, 5];
            PullItem_PickupMethod.Invoke(player, args);
            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
    #region Cosmos
    public class SolarBooster : Booster
    {
        public override string Texture => "Terraria/Images/Item_3458";
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.SolarTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.Yellow, Language.GetTextValue("Mods.FargowiltasSouls.Items.SolarBooster.Activate", 5), true);
            boosterPlayer.SolarTimer = LunarDuration;
            boosterPlayer.Player.AddBuff(ModContent.BuffType<SolarBuff>(), LunarDuration);
        }
    }
    public class VortexBooster : Booster
    {
        public override string Texture => "Terraria/Images/Item_3456";
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            //if (boosterPlayer.Player.HasBuff(BuffID.NebulaUpDmg1) || boosterPlayer.Player.HasBuff(BuffID.NebulaUpDmg2) || boosterPlayer.Player.HasBuff(BuffID.NebulaUpDmg3))
            //    return;
            if (boosterPlayer.VortexTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.LightCyan, Language.GetTextValue("Mods.FargowiltasSouls.Items.VortexBooster.Activate", 20), true);
            boosterPlayer.VortexTimer = LunarDuration;
            boosterPlayer.Player.AddBuff(ModContent.BuffType<VortexBuff>(), LunarDuration);
        }
    }
    public class NebulaBooster : Booster
    {
        public override string Texture => "Terraria/Images/Item_3457";
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.Player.HasBuff(BuffID.NebulaUpLife1) || boosterPlayer.Player.HasBuff(BuffID.NebulaUpLife2) || boosterPlayer.Player.HasBuff(BuffID.NebulaUpLife3))
                return;
            if (boosterPlayer.NebulaTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.Magenta, Language.GetTextValue("Mods.FargowiltasSouls.Items.NebulaBooster.Activate", 2), true);
            boosterPlayer.NebulaTimer = LunarDuration;
            boosterPlayer.Player.AddBuff(ModContent.BuffType<NebulaBuff>(), LunarDuration);
        }
    }
    public class StardustBooster : Booster
    {
        public override string Texture => "Terraria/Images/Item_3459";
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.Player.HasBuff(BuffID.NebulaUpDmg1) || boosterPlayer.Player.HasBuff(BuffID.NebulaUpDmg2) || boosterPlayer.Player.HasBuff(BuffID.NebulaUpDmg3))
                return;
            if (boosterPlayer.StardustTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.Cyan, Language.GetTextValue("Mods.FargowiltasSouls.Items.StardustBooster.Activate", 20), true);
            boosterPlayer.StardustTimer = LunarDuration;
            boosterPlayer.Player.AddBuff(ModContent.BuffType<StardustBuff>(), LunarDuration);
        }
    }
    #endregion

}
