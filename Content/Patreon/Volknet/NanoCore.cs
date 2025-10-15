﻿using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Content.Patreon.Volknet.Projectiles;
using FargowiltasSouls.Content.Projectiles.Weapons.BossWeapons;
using FargowiltasSouls.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace FargowiltasSouls.Content.Patreon.Volknet
{
    public class NanoCore : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nano Core");
            // Tooltip.SetDefault("When holding this, 7 nano units will appear and construct weapons.\nLeft click to attack and right click to switch weapons.\n\'The science sits in our side.\'");
            //DisplayName.AddTranslation(GameCulture.Chinese, "纳米核心");
            //Tooltip.AddTranslation(GameCulture.Chinese, "手持时可以产生7个纳米单元组成武器。\n左键发动攻击，右键切换形态。\n\'科学站在我们这边\'");
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = 175;
            Item.knockBack = 2;
            Item.channel = true;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.value = Item.sellPrice(0, 15);
            Item.shoot = ModContent.ProjectileType<NanoBase>();
            Item.rare = ItemRarityID.Purple;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.crit = 12;
        }
        public override bool? PrefixChance(int pre, UnifiedRandom rand) => false;
        public override void SafePostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D tex = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Patreon/Volknet/NanoCoreGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(tex, Item.Center + new Vector2(0, -4) - Main.screenPosition, new Rectangle(0, 0, tex.Width, tex.Height), Color.White, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);

            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = Main.DiscoColor;
                }
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)             //switch phase
            {
                Item.autoReuse = false;
                Item.channel = false;
            }
            else
            {
                Item.autoReuse = true;
                Item.channel = true;
            }

            if (player.GetModPlayer<NanoPlayer>().NanoCoreMode == 1 && player.altFunctionUse != 2)                //Use ammo
            {
                Item.useAmmo = AmmoID.Arrow;
            }
            else
            {
                Item.useAmmo = AmmoID.None;
            }
            return true;
        }

        int switchCD;

        public override void HoldItem(Player player)
        {
            if (switchCD > 0)
                switchCD--;

            int nanoCoreMode = player.GetModPlayer<NanoPlayer>().NanoCoreMode;

            if (nanoCoreMode == 0)
                Item.DamageType = DamageClass.Melee;
            else if (nanoCoreMode == 1)
                Item.DamageType = DamageClass.Ranged;
            else if (nanoCoreMode == 2)
                Item.DamageType = DamageClass.Magic;
            else
                Item.DamageType = DamageClass.Summon;

            if (nanoCoreMode != 3)        //change holding rotation when not in bombing mode
            {
                player.direction = Math.Sign(Main.MouseWorld.X - player.Center.X);
                Vector2 RotV = Vector2.Normalize(Main.MouseWorld - player.Center);
                if (player.direction < 0)
                {
                    RotV.Y = -RotV.Y;
                }
                player.itemRotation = RotV.ToRotation();
            }

            if (nanoCoreMode == 1 && player.altFunctionUse != 2)                //Use ammo
            {
                Item.useAmmo = AmmoID.Arrow;
            }
            else
            {
                Item.useAmmo = AmmoID.None;
            }

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<NanoBase>()] < 1)
            {
                int protmp = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<NanoBase>(), player.HeldItem.damage, player.GetWeaponKnockback(player.HeldItem, 1), player.whoAmI);
                player.heldProj = protmp;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && switchCD <= 0)            //switch phase
            {
                switchCD = 12;

                player.GetModPlayer<NanoPlayer>().NanoCoreMode = (player.GetModPlayer<NanoPlayer>().NanoCoreMode + 1) % 4;
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<NanoProbe>() && proj.owner == player.whoAmI)
                    {
                        proj.ai[1] = 0;
                    }
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            if (SoulConfig.Instance.PatreonNanoCore)
            {
                CreateRecipe()
                .AddIngredient(ItemID.Arkhalis)
                .AddIngredient(ItemID.FairyQueenRangedItem)
                .AddIngredient(ItemID.ChargedBlasterCannon)
                .AddIngredient(ItemID.XenoStaff)
                .AddIngredient(ModContent.ItemType<Eridanium>(), 99)
                .AddIngredient(ModContent.ItemType<AbomEnergy>(), 99)
                .AddIngredient(ItemID.MartianSaucerTrophy, 1)
                .AddIngredient(ItemID.LunarBar, 99)
                .AddIngredient(ItemID.Nanites, 999)
                .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
                .Register();
            }
        }
    }

    public class NanoPlayer : ModPlayer
    {
        public int NanoCoreMode = 0;
        int oldNanoCoreMode;

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            if (NanoCoreMode != oldNanoCoreMode)
            {
                oldNanoCoreMode = NanoCoreMode;

                ModPacket packet = Mod.GetPacket();

                packet.Write((byte)FargowiltasSouls.PacketID.SyncNanoCoreMode);
                packet.Write((byte)Player.whoAmI);
                packet.Write7BitEncodedInt(NanoCoreMode);

                packet.Send(toWho, fromWho);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            //if (NanoCoreMode == 0 && Player.HeldItem.type == ModContent.ItemType<NanoCore>() && Player.channel)
            //    modifiers.FinalDamage /= 2;
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            //if (NanoCoreMode == 0 && Player.HeldItem.type == ModContent.ItemType<NanoCore>() && Player.channel)
            //    modifiers.FinalDamage /= 2;
        }
    }

    public class NanoCoreArrowGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        bool noGravArrow;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.aiStyle == ProjAIStyleID.Arrow && (projectile.arrow || projectile.type == ProjectileID.MoonlordArrowTrail))
            {
                if (source is EntitySource_ItemUse itemUse && itemUse.Item.ModItem is NanoCore)
                {
                    noGravArrow = true;
                    projectile.extraUpdates += 1;
                }
                else if (source is EntitySource_Parent parent && parent.Entity is Projectile)
                {
                    Projectile sourceProj = parent.Entity as Projectile;
                    if (sourceProj != null && sourceProj.GetGlobalProjectile<NanoCoreArrowGlobalProjectile>().noGravArrow)
                    {
                        noGravArrow = true;
                        projectile.extraUpdates += 1;
                    }
                }
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (noGravArrow)
                projectile.velocity.Y -= 0.1f;

            return base.PreAI(projectile);
        }
    }
}
