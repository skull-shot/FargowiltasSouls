﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Projectiles.Weapons.ChallengerItems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class TheBaronsTusk : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/Challengers", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

            ItemID.Sets.BonusAttackSpeedMultiplier[Type] = 0.25f;
        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Melee;
            Item.width = 66;
            Item.height = 64;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 4, 0);
            Item.rare = ItemRarityID.Pink;
            Item.shootsEveryUse = true;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BaronTuskShrapnel>();
            Item.shootSpeed = 15f;
        }
        int Timer = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = (Item.shootSpeed + Main.rand.Next(-2, 2)) * Vector2.Normalize(Main.MouseWorld - player.itemLocation).RotatedByRandom(MathHelper.Pi / 14);
                int p = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.itemLocation, vel, Item.shoot, (int)(player.ActualClassDamage(DamageClass.Melee) * Item.damage / 3f), Item.knockBack, player.whoAmI);
            }
            return false;
        } 
        public override void HoldItem(Player player) //fancy momentum swing, this should be generalized and applied to other swords imo
        {
            
            //if (Timer > 2 * player.itemAnimationMax / 3)
            //{
            //    player.itemAnimation = player.itemAnimationMax;
            //    Item.noMelee = true;
            //}
            //else
            //{
            //    Item.noMelee = false;
            //    float prog = (float)Timer / (2 * player.itemAnimationMax / 3);
            //    player.itemAnimation = (int)(player.itemAnimationMax * Math.Pow(MomentumProgress(prog), 2));
            //}

        }
        
        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            IEnumerable<Projectile> embeddedShrapnel = Main.projectile.Where(p => p.TypeAlive<BaronTuskShrapnel>() && p.owner == player.whoAmI && p.As<BaronTuskShrapnel>().EmbeddedNPC == target);
            int shrapnel = embeddedShrapnel.Count();
            if (shrapnel >= 15)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    foreach (Projectile proj in embeddedShrapnel)
                    {
                        proj.ai[1] = 2;
                        proj.netUpdate = true;
                    }
                }
                else
                {
                    // remember that this is target client side; we sync to server
                    var netMessage = Mod.GetPacket();
                    netMessage.Write((byte)FargowiltasSouls.PacketID.SyncTuskRip);
                    netMessage.Write((byte)target.whoAmI);
                    netMessage.Write((byte)player.whoAmI);
                    netMessage.Send();
                }

                SoundEngine.PlaySound(SoundID.Item68, target.Center);
                modifiers.FlatBonusDamage += 15 * Item.damage / 2.5f + (shrapnel * Item.damage / 6);
                modifiers.SetCrit();
                foreach (Projectile proj in embeddedShrapnel)
                {
                    proj.ai[1] = 2;
                    proj.netUpdate = true;
                }
            }
        }
        //this is ripped from my own game project
        ///<summary>
        ///Returns distance progress by a sine formula based on linear progress = (% between 1-0). f(1) = 1, f(0) = 0.
        ///</summary>
        public static float MomentumProgress(float x)
        {
            return (x * x * 3) - (x * x * x * 2);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<BanishedBaronBag>(2).AddTile(TileID.Solidifier).DisableDecraft().Register();
        }
    }
}