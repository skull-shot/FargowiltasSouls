using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Audio;
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
            player.AddEffect<TurtleSmashEffect>(Item);
            player.noKnockback = true;

            if (player.HasBuff<ShellSmashBuff>() && player.HasEffect<TurtleSmashEffect>())
            {
                player.FargoSouls().AttackSpeed += 0.2f;
                if (!player.FargoSouls().NoMomentum || !player.mount.Active)
                {
                    player.runAcceleration *= 1.8f;
                    player.runSlowdown *= 1.6f;
                    player.maxRunSpeed *= 1.3f; // all of these affect aerial speed
                }
            }
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<TurtleEffect>(item);
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

                .AddTile<EnchantedTreeSheet>()
            .Register();
        }
    }

    public class TurtleEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<LifeHeader>();
        public override int ToggleItemType => ModContent.ItemType<TurtleEnchant>();
        public override bool MutantsPresenceAffects => true;

        public const float TurtleShellMaxHP = 1000;
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                CooldownBarManager.Activate("TurtleHP", FargoAssets.GetTexture2D("Content/Items/Accessories/Enchantments", "TurtleEnchant").Value, Color.SandyBrown, () => Main.LocalPlayer.FargoSouls().TurtleShellHP / TurtleShellMaxHP, activeFunction: () => player.HasEffect<TurtleEffect>());
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            bool broken = player.FargoSouls().TurtleShellBroken;
            //Main.NewText($"shell HP: {modPlayer.TurtleShellHP}, counter: {modPlayer.TurtleCounter}");

            if (modPlayer.TurtleCounter < 0)
            {
                broken = true;
                modPlayer.TurtleCounter++;
            }

            if (!broken)
            {
                if (!player.HasEffect<LifeForceEffect>() && player.velocity.X == 0 && player.velocity.Y == 0 && !player.controlUseItem && !player.controlUseTile && player.whoAmI == Main.myPlayer)
                {
                    modPlayer.TurtleCounter++;

                    if (modPlayer.TurtleCounter > 20)
                    {
                        modPlayer.ShellHide = true;
                    }
                }
                else if (player.HasEffect<LifeForceEffect>() && player.velocity.X == 0 && player.controlJump && player.TryingToHoverDown == true && !player.controlUseItem && player.whoAmI == Main.myPlayer)
                {
                    modPlayer.TurtleCounter++;

                    if (modPlayer.TurtleCounter > 40)
                    {
                        modPlayer.ShellHide = true;
                    }
                }
                else
                {
                    modPlayer.TurtleCounter = 0;
                    modPlayer.ShellHide = false;
                }
                /*
                if (modPlayer.TurtleShellHP < TurtleShellMaxHP && !broken && !modPlayer.ShellHide && !player.controlUseItem && !player.controlUseTile)
                {
                    modPlayer.TurtleShellHP++;
                }
                */
            }
            if (modPlayer.TurtleShellHP <= 0 && !broken)
            {
                modPlayer.TurtleShellHP = 0;
                if (player.HasEffect<TurtleSmashEffect>())
                    player.AddBuff(ModContent.BuffType<ShellSmashBuff>(), player.ForceEffect<TurtleSmashEffect>() ? 30 * 60 : 20 * 60);
                player.FargoSouls().TurtleShellBroken = true;
                player.immune = true;
                player.immuneTime = 120;
                SoundEngine.PlaySound(SoundID.Shatter, player.Center);

                if (!Main.dedServ)
                {
                    for (int j = 0; j < Main.rand.Next(8, 12); j++)
                    {
                        int i = j % 9;
                        Vector2 pos = Main.rand.NextVector2FromRectangle(player.Hitbox);
                        Vector2 vel = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(4, 8);
                        int type = i + 1;
                        Gore.NewGore(player.GetSource_Accessory(player.EffectItem<TurtleEffect>()), pos, vel, ModContent.Find<ModGore>(Mod.Name, $"TurtleFragment{type}").Type, Main.rand.NextFloat(0.7f, 1.3f));
                    }
                }
            }

            if (broken)
            {
                float divisor = player.ForceEffect<TurtleEffect>() ? 1800f : 1200f;
                modPlayer.TurtleShellHP += TurtleShellMaxHP / divisor;
            }
            if (modPlayer.TurtleShellHP > TurtleShellMaxHP)
            {
                modPlayer.TurtleShellBroken = false;
                modPlayer.TurtleShellHP = TurtleShellMaxHP;
            }
               

            if (modPlayer.ShellHide == true)
            {
                ShellHide(player);
            }
        }

        public static void ShellHide(Player player)
        {
            if (player.thorns == 0f)
                player.thorns = 1f;
            player.thorns *= 5f;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<TurtleShield>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_EffectItem<TurtleEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<TurtleShield>(), 0, 0, player.whoAmI);
            }

            if (player.immune)
            {
                Main.projectile.Where(x => x.active && x.hostile && x.damage > 0 && x.Hitbox.Intersects(player.Hitbox) && ProjectileLoader.CanDamage(x) != false && ProjectileLoader.CanHitPlayer(x, player) && FargoSoulsUtil.CanDeleteProjectile(x)).ToList().ForEach(x =>
                {
                    // Turn around
                    x.velocity *= -1f;

                    // Flip sprite
                    if (x.Center.X > player.Center.X)
                    {
                        x.direction = 1;
                        x.spriteDirection = 1;
                    }
                    else
                    {
                        x.direction = -1;
                        x.spriteDirection = -1;
                    }

                    x.hostile = false;
                    x.friendly = true;
                    x.damage *= 5;
                    SoundEngine.PlaySound(SoundID.Item150, player.Center);
                });
            }
        }

        public override void ModifyHitByNPC(Player player, NPC npc, ref Player.HurtModifiers modifiers)
        {
            float dr = TurtleDR(player);
            modifiers.FinalDamage *= 1 - dr;
        }
        public override void ModifyHitByProjectile(Player player, Projectile projectile, ref Player.HurtModifiers modifiers)
        {
            float dr = TurtleDR(player);
            modifiers.FinalDamage *= 1 - dr;
        }
        public static float TurtleDR(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            float dr = 0;
            if (modPlayer.ShellHide)
            {
                dr += (player.ForceEffect<TurtleEffect>() && player.HasEffectEnchant<TurtleEffect>()) ? 0.8f : 0.66f;
                if (!player.HasEffectEnchant<TurtleEffect>())
                    modPlayer.TurtleCounter = -90;
            }
            return dr;
        }
    }

    public class TurtleSmashEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<LifeHeader>();
        public override int ToggleItemType => ModContent.ItemType<TurtleEnchant>();
    }
}
