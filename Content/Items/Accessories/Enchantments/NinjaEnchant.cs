using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class NinjaEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(48, 49, 52);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<NinjaEffect>(Item);
            player.AddEffect<NinjaDamageEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NinjaHood)
                .AddIngredient(ItemID.NinjaShirt)
                .AddIngredient(ItemID.NinjaPants)
                .AddIngredient(ItemID.Gi)
                .AddIngredient(ItemID.Shuriken, 100)
                .AddIngredient(ItemID.ThrowingKnife, 100)

                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
    public class NinjaEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<NinjaEnchant>();
    }
    public class NinjaDamageEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPCWithProj(player, proj, target, hit, damageDone);

            //crit visuals if fake "roll" was successful to visually indicate ninja crit boost
            if (hit.Crit)
            {
                int ninjacrit = FargoSoulsGlobalProjectile.ninjaCritIncrease;
                int critroll = Main.rand.Next(proj.CritChance + ninjacrit);
                if (critroll <= proj.CritChance + ninjacrit && critroll > proj.CritChance)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 velocity = 4 * Vector2.UnitY.RotatedBy(MathHelper.TwoPi / 8 * i);
                        int d = Dust.NewDust(proj.Center, 0, 0, DustID.Smoke, 0, 0, 100, Color.DarkGray, 1.5f);
                        Main.dust[d].velocity = velocity;
                        Main.dust[d].noGravity = true;
                    }
                }
            }
            FargoSoulsGlobalProjectile.ninjaCritIncrease = 0;
        }
    }
}
