using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class TurtleEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(248, 156, 92);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<TurtleEffect>(item);
            player.noKnockback = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.TurtleHelmet)
            .AddIngredient(ItemID.TurtleScaleMail)
            .AddIngredient(ItemID.TurtleLeggings)
            .AddIngredient(ItemID.ChlorophytePartisan)
            .AddIngredient(ItemID.Yelets)
            .AddIngredient(ItemID.HardySaddle)

            //chloro saber
            //
            //jungle turtle
            //.AddIngredient(ItemID.Seaweed);
            //.AddIngredient(ItemID.LizardEgg);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }

    public class TurtleEffect : AccessoryEffect
    {
        public override bool MutantsPresenceAffects => true;
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                CooldownBarManager.Activate("TurtleHP", ModContent.Request<Texture2D>("FargowiltasSouls/Content/Items/Accessories/Enchantments/TurtleEnchant").Value, Color.SandyBrown, () => Main.LocalPlayer.FargoSouls().TurtleShellHP / 1000f, activeFunction: () => player.HasEffect<TurtleEffect>() || player.HasEffectEnchant<TurtleEffect>());
                FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (!player.HasEffect<LifeForceEffect>() && !player.controlRight && !player.controlLeft && player.velocity.Y == 0 && !player.controlUseItem && !player.controlUseTile && player.whoAmI == Main.myPlayer && !modPlayer.noDodge)
            {
                modPlayer.TurtleCounter++;

                if (modPlayer.TurtleCounter > 20)
                {
                    player.AddBuff(ModContent.BuffType<ShellHideBuff>(), 2);
                }
            }
            else if (player.HasEffect<LifeForceEffect>() && player.velocity.X == 0 && player.controlJump && player.TryingToHoverDown == true && !player.controlUseItem && player.whoAmI == Main.myPlayer && !modPlayer.noDodge)
            {
                if (modPlayer.TurtleCounter > 20)
                    modPlayer.TurtleCounter++;

                if (modPlayer.TurtleCounter > 40)
                {
                    player.AddBuff(ModContent.BuffType<ShellHideBuff>(), 2);
                }
            }
            else
            {
                modPlayer.TurtleCounter = 0;
            }

            if (modPlayer.TurtleShellHP < 2000 && !modPlayer.ShellHide)
            {
                modPlayer.TurtleShellHP++;
                modPlayer.TurtleShellHP++;
            }
            if (modPlayer.TurtleShellHP < 0)
                modPlayer.TurtleShellHP = 0;
            if (modPlayer.TurtleShellHP > 2000)
               modPlayer.TurtleShellHP = 2000;

            //Main.NewText($"shell HP: {TurtleShellHP}, counter: {TurtleCounter}, recovery: {turtleRecoverCD}");
        }

        public override float ContactDamageDR(Player player, NPC npc, ref Player.HurtModifiers modifiers)
        {
            return TurtleDR(player, npc);
        }
        public override float ProjectileDamageDR(Player player, Projectile projectile, ref Player.HurtModifiers modifiers)
        {
            return TurtleDR(player, projectile);
        }
        public static float TurtleDR(Player player, Entity attacker)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (!player.HasEffect<TurtleEffect>())
                return 0f;
            if (!modPlayer.ShellHide)
                return 0f;
            NPC sourceNPC = null;
            if (attacker is NPC attackerNPC)
                sourceNPC = attackerNPC;
            if (attacker is Projectile projectile && projectile.GetSourceNPC() is NPC projNPC)
                sourceNPC = projNPC;
            if (sourceNPC != null)
            {
                float hp = modPlayer.TurtleShellHP;
                float dr = 0.4f + (hp * (1f / 5000f));
                return dr;
            }
            return 0f;
        }

        public override Header ToggleHeader => Header.GetHeader<LifeHeader>();
        public override int ToggleItemType => ModContent.ItemType<TurtleEnchant>();

    }
}
