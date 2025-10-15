﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Content.Projectiles.Weapons.BossWeapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class EaterLauncherJr : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/BossDrops", Name);
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Item.type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 36;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item95;
            Item.value = Item.sellPrice(0, 2);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EaterRocketJr>();
            Item.shootSpeed = 18f;
        }

        public const int MaxCharge = 1000;
        public int Charge = 0;
        public override void SaveData(TagCompound tag)
        {
            tag.Add("BlastbiterCharge", Charge);
        }
        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("BlastbiterCharge"))
                Charge = tag.GetAsInt("BlastbiterCharge");
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2, -4);
        }
        public override bool CanRightClick() => Main.LocalPlayer.HasItem(ItemID.RottenChunk) && Charge < MaxCharge;
        void LoadChunk(Player player)
        {
            if (player.ConsumeItem(ItemID.RottenChunk))
            {
                SoundEngine.PlaySound(SoundID.Item149 with { Pitch = 0.5f }, player.Center);
                Charge += 100;
                if (Charge > MaxCharge)
                    Charge = MaxCharge;
            }
        }
        public override void RightClick(Player player)
        {
            LoadChunk(player);
        }
        public override bool ConsumeItem(Player player) => false;
        public override bool CanUseItem(Player player)
        {
            return (Charge > 0 || player.HasItem(ItemID.RottenChunk));
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int aura = ModContent.ProjectileType<RockeaterAuraProj>();
            if (player.ownedProjectileCounts[aura] <= 0)
                Projectile.NewProjectile(source, player.Center, player.velocity, aura, 0, 0);
            if (Charge <= 0)
                LoadChunk(player);
            Charge--;
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            Utils.DrawBorderString(spriteBatch, Charge.ToString(), position - Vector2.UnitX * 15 * scale, Color.SandyBrown, scale: 0.75f);
            //spriteBatch.DrawString(FontAssets.ItemStack.Value, Charge, position, drawColor);
        }

        public override void HoldItem(Player player)
        {
            /*
            if (player.itemTime > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 offset = new();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * 300);
                    offset.Y += (float)(Math.Cos(angle) * 300);
                    Dust dust = Main.dust[Dust.NewDust(
                        player.Center + offset - new Vector2(4, 4), 0, 0,
                        DustID.PurpleCrystalShard, 0, 0, 100, Color.White, 1f
                        )];
                    dust.velocity = player.velocity;
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * 5f;
                    dust.noGravity = true;
                    dust.scale = 1f;

                    Vector2 offset2 = new();
                    double angle2 = Main.rand.NextDouble() * 2d * Math.PI;
                    offset2.X += (float)(Math.Sin(angle2) * player.FargoSouls().RockeaterDistance);
                    offset2.Y += (float)(Math.Cos(angle2) * player.FargoSouls().RockeaterDistance);
                    Dust dust2 = Main.dust[Dust.NewDust(
                        player.Center + offset2 - new Vector2(4, 4), 0, 0,
                        DustID.PurpleCrystalShard, 0, 0, 100, Color.White, 1f
                        )];
                    dust2.velocity = player.velocity;
                    if (Main.rand.NextBool(3))
                        dust2.velocity += Vector2.Normalize(offset2) * -5f;
                    dust2.noGravity = true;
                    dust2.scale = 1f;
                }
            }
            */
        }


        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<EaterRocketJr>();
        }

    }
}
