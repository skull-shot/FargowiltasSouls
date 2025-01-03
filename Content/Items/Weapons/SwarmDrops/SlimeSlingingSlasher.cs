using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    public class SlimeSlingingSlasher : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 845;
            Item.DamageType = DamageClass.Melee;
            Item.width = 48;
            Item.height = 64;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Purple;
            //Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.scale = 1.5f;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<SlimeSlingingSlasherProj>();
            Item.shootSpeed = 1f;
        }

        public override void HoldItem(Player player) //fancy momentum swing, this should be generalized and applied to other swords imo
        {
            

        }
        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.onFire || target.onFire2 || target.onFire3)
            {
                modifiers.FinalDamage *= 1.2f;
            }
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slimed, 240);
            SoundEngine.PlaySound(SoundID.Item17);
            Projectile.NewProjectile(player.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<Slimesplosion>(), damageDone, 1f, Item.whoAmI, 1, 1, 1);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "SlimeKingsSlasher")
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerSlime"))
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}
