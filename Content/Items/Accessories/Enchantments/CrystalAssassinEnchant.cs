using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler.Content;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class CrystalAssassinEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(36, 157, 207);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }

        public static void AddEffects(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.CrystalEnchantActive = true;

            //cooldown
            if (modPlayer.SmokeBombCD != 0)
            {
                modPlayer.SmokeBombCD--;
            }

            player.AddEffect<CrystalAssassinDash>(item);
            player.AddEffect<CrystalDiagonalDash>(item);
        }
        /*
        public static void SmokeBombKey(FargoSoulsPlayer modPlayer)
        {
            //throw smoke bomb
            if (modPlayer.CrystalSmokeBombProj == null)
            {
                const float gravity = 0.18f;
                float time = 60f;
                Vector2 distance = Main.MouseWorld - modPlayer.Player.Center;
                distance.X /= time;
                distance.Y = distance.Y / time - 0.5f * gravity * time;

                modPlayer.CrystalSmokeBombProj = Main.projectile[Projectile.NewProjectile(modPlayer.Player.GetSource_Misc(""), modPlayer.Player.Center, distance + Main.rand.NextVector2Square(0, 0) * 2,
                        ProjectileID.SmokeBomb, 0, 0f, Main.myPlayer)];

                modPlayer.SmokeBombCD = 15;
            }
            //already threw smoke bomb, tele to it
            else
            {
                Vector2 teleportPos = new(modPlayer.CrystalSmokeBombProj.position.X, modPlayer.CrystalSmokeBombProj.position.Y - 30);
                Vector2 originalPos = new(teleportPos.X, teleportPos.Y);

                //spiral out to find a save spot
                int count = 0;
                int increase = 10;
                while (Collision.SolidCollision(teleportPos, modPlayer.Player.width, modPlayer.Player.height))
                {
                    teleportPos = originalPos;

                    switch (count)
                    {
                        case 0:
                            teleportPos.X -= increase;
                            break;
                        case 1:
                            teleportPos.X += increase;
                            break;
                        case 2:
                            teleportPos.Y += increase;
                            break;
                        default:
                            teleportPos.Y -= increase;
                            increase += 10;
                            break;
                    }
                    count++;

                    if (count >= 4)
                    {
                        count = 0;
                    }

                    if (increase > 100)
                    {
                        return;
                    }
                }

                if (teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
                {
                    modPlayer.Player.Teleport(teleportPos, 1);
                    NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, modPlayer.Player.whoAmI, teleportPos.X, teleportPos.Y, 1);

                    modPlayer.Player.AddBuff(ModContent.BuffType<FirstStrikeBuff>(), 60);

                    modPlayer.CrystalSmokeBombProj.timeLeft = 120;
                    modPlayer.SmokeBombCD = 300;
                    modPlayer.CrystalSmokeBombProj = null;
                }
            }
        }
        */

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrystalNinjaHelmet)
                .AddIngredient(ItemID.CrystalNinjaChestplate)
                .AddIngredient(ItemID.CrystalNinjaLeggings)
                .AddIngredient(ItemID.FlyingKnife)
                .AddIngredient(ItemID.UnicornonaStick)
                .AddIngredient(ItemID.SmokeBomb, 50)

                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
    public class CrystalAssassinDash : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<CrystalAssassinEnchant>();
        public override bool MutantsPresenceAffects => true;
        public static void AddDash(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.FargoDash = DashManager.DashType.Crystal;
            modPlayer.HasDash = true;
        }
        public static void CrystalDash(Player player, int direction)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            float dashSpeed = 22f;
            if (player.HasEffect<CrystalDiagonalDash>())
            {
                player.velocity.Y *= 0;
                if (player.controlUp)
                    player.velocity.Y = dashSpeed * -0.5f;
                else if (player.controlDown)
                    player.velocity.Y = dashSpeed * 0.7f;
                else
                    player.velocity.Y = float.Epsilon;
            }

            if (player.ForceEffect<CrystalAssassinDash>() && modPlayer.CrystalDashFirstStrikeCD <= 0)
            {
                player.AddBuff(ModContent.BuffType<FirstStrikeBuff>(), 60);
                modPlayer.CrystalDashFirstStrikeCD = 60 * 5;
            }


            player.velocity.X = dashSpeed * direction;
            if (modPlayer.IsDashingTimer < 20)
                modPlayer.IsDashingTimer = 20;

            int cd = player.ForceEffect<CrystalAssassinDash>() ? 30 : 60;
            player.dashDelay = cd;
            modPlayer.DashCD = cd;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);

            for (int num17 = 0; num17 < 20; num17++)
            {
                int num18 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Dust expr_CDB_cp_0 = Main.dust[num18];
                expr_CDB_cp_0.position.X += Main.rand.Next(-5, 6);
                Dust expr_D02_cp_0 = Main.dust[num18];
                expr_D02_cp_0.position.Y += Main.rand.Next(-5, 6);
                Main.dust[num18].velocity *= 0.2f;
                Main.dust[num18].scale *= 1f + Main.rand.Next(20) * 0.01f;
                //Main.dust[num18].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, this);
            }
            int num19 = Gore.NewGore(player.GetSource_FromThis(), new Vector2(player.position.X + player.width / 2 - 24f, player.position.Y + player.height / 2 - 34f), default, Main.rand.Next(61, 64), 1f);
            Main.gore[num19].velocity.X = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num19].velocity.Y = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num19].velocity *= 0.4f;
            num19 = Gore.NewGore(player.GetSource_FromThis(), new Vector2(player.position.X + player.width / 2 - 24f, player.position.Y + player.height / 2 - 14f), default, Main.rand.Next(61, 64), 1f);
            Main.gore[num19].velocity.X = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num19].velocity.Y = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num19].velocity *= 0.4f;
        }
        public static void WhileDashing(Player player)
        {
            Particle bub = new SparkParticle(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), -(player.velocity * 0.1f), Color.DeepPink, 0.5f, 10, true, Color.Pink);
            bub.Spawn();

            player.velocity *= 0.95f;
            if (player.HasEffect<CrystalDiagonalDash>())
            {
                if (player.velocity.Y == 0)
                    player.FargoSouls().CoyoteTime = 30;
                if (player.FargoSouls().CoyoteTime > 0 && player.controlJump)
                {
                    player.FargoSouls().CoyoteTime = 0;
                    if (player.velocity.X > 0 || player.velocity.X < 0)
                    {
                        player.velocity.X = Math.Sign(player.velocity.X) * 30;
                        player.velocity.Y = -10;
                    }
                }
            } 
        }
    }
    public class CrystalDiagonalDash : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<CrystalAssassinEnchant>();
        public override bool MutantsPresenceAffects => true;
    }
}
