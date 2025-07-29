using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class SpookyEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override List<AccessoryEffect> ActiveSkillTooltips => 
            [AccessoryEffectLoader.GetEffect<SpookyEffect>()];
        public override Color nameColor => new(100, 78, 116);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Lime;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SpookyEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SpookyHelmet)
            .AddIngredient(ItemID.SpookyBreastplate)
            .AddIngredient(ItemID.SpookyLeggings)
            .AddIngredient(ItemID.ButchersChainsaw)
            .AddIngredient(ItemID.DeathSickle)
            .AddIngredient(ItemID.RavenStaff)

            //psycho knife
            //eoc yoyo
            //dark harvest
            //.AddIngredient(ItemID.CursedSapling);
            //.AddIngredient(ItemID.EyeSpring);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
    public class SpookyEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<SpookyEnchant>();
        public override bool ActiveSkill => Main.LocalPlayer.HasEffectEnchant<SpookyEffect>();
        public override void PostUpdateEquips(Player player)
        {
            
        }
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            if (stunned)
                return;
            if (player.FargoSouls().SpookyCD > 0)
                return;
            bool wiz = player.ForceEffect<SpookyEffect>();
            if (Main.myPlayer == player.whoAmI)
            {
                int baseDmg = wiz ? 1250 : 750;
                float melee = player.ActualClassDamage(DamageClass.Melee);
                float summon = player.ActualClassDamage (DamageClass.Summon);
                int dmg = (int)(baseDmg * (((melee + summon - 2f) / 2f) + 1f)); // melee-summon 50-50 damage scaling
                Projectile.NewProjectile(GetSource_EffectItem(player), player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<SpookySpinScythe>(), dmg, 1f, player.whoAmI);
            }
            int cd = wiz ? 8 : 12;
            player.FargoSouls().SpookyCD = cd * 60;
            CooldownBarManager.Activate("SpookyEnchantCooldown", ModContent.Request<Texture2D>("FargowiltasSouls/Content/Items/Accessories/Enchantments/SpookyEnchant").Value, new(100, 78, 116),
                () => Main.LocalPlayer.FargoSouls().SpookyCD / (cd * 60f), true, activeFunction: player.HasEffect<SpookyEffect>);
        }
    }
}
