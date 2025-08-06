using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class BeetleEnchant : BaseEnchant
    {

        public override Color nameColor => new(109, 92, 133);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<BeetleEffect>(item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BeetleHelmet)
            .AddRecipeGroup("FargowiltasSouls:AnyBeetle")
            .AddIngredient(ItemID.BeetleLeggings)
            .AddIngredient(ItemID.BeetleWings)
            .AddIngredient(ItemID.BeeWings)
            .AddIngredient(ItemID.ButterflyWings)
            //.AddIngredient(ItemID.MothronWings);
            //breaker blade
            //amarok
            //beetle minecart

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }

    public class BeetleEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<LifeHeader>();
        public override int ToggleItemType => ModContent.ItemType<BeetleEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            var modPlayer = player.FargoSouls();
            if (modPlayer.BeetleAttackCD > 0)
                modPlayer.BeetleAttackCD--;
            if (modPlayer.BeetleHitCD > 0)
                modPlayer.BeetleHitCD--;
            //modPlayer.BeetleCharge;
            //modPlayer.Beetles;
            int beetle = ModContent.ProjectileType<BeetleEnchantBeetle>();

            player.endurance += 0.03f * modPlayer.Beetles;

            int spawnBeetles = modPlayer.Beetles - player.ownedProjectileCounts[beetle];
            if (spawnBeetles > 0 && player.whoAmI == Main.myPlayer)
            {
                for (int i = 0; i < spawnBeetles; i++)
                {
                    int dmg = 35; //melee scaling handled by continuous projectile damage updating
                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, Vector2.Zero, beetle, dmg, 1f, player.whoAmI);
                }

            }
            if (spawnBeetles < 0) // too many
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.TypeAlive(beetle) && p.owner == player.whoAmI)
                    {
                        p.Kill();
                    }
                }
            }
        }

        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (projectile != null && projectile.type == ModContent.ProjectileType<BeetleEnchantBeetle>())
                return;
            var modPlayer = player.FargoSouls();
            bool wiz = player.ForceEffect<BeetleEffect>();
            int beetleCap = wiz ? 9 : 6;
            if (modPlayer.BeetleHitCD > 0)
                return;
            if (modPlayer.Beetles >= beetleCap)
                return;

            float buildup = baseDamage;
            int hitCap = 500;
            if (buildup > hitCap)
                buildup = hitCap;
            if (wiz)
                buildup *= 1.5f;
            modPlayer.BeetleCharge += buildup / 2000f;
            CooldownBarManager.Activate("BeetleEnchantCharge", ModContent.Request<Texture2D>("FargowiltasSouls/Content/Items/Accessories/Enchantments/BeetleEnchant").Value, new(109, 92, 133), () => (modPlayer.Beetles + modPlayer.BeetleCharge) / beetleCap, true, activeFunction: player.HasEffect<BeetleEffect>);
            if (modPlayer.BeetleCharge > 1)
            {
                modPlayer.BeetleCharge = 0;
                modPlayer.Beetles += 1;
            }
        }
        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            var modPlayer = player.FargoSouls();
            if (modPlayer.BeetleHitCD > 0 || modPlayer.Beetles <= 0)
                return;
            modPlayer.Beetles -= 3;
            if (modPlayer.Beetles < 0)
                modPlayer.Beetles = 0;
            modPlayer.BeetleHitCD = 60 * 3;
        }

    }

}
