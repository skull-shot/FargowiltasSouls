using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Projectiles.Weapons.ChallengerItems;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class Lightslinger : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/Challengers", Name);
        const int ReqShots = 40;
        int ShotType = ModContent.ProjectileType<LightslingerShot>();

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 76;
            Item.height = 48;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.5f;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item12;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LightslingerShot>();
            Item.shootSpeed = 12f;

            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => !Main.rand.NextBool(4);

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ShotType;
            if (player.altFunctionUse == 2)
                damage *= 6;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-27f, -12f);
        }

        public override bool AltFunctionUse(Player player) => player.FargoSouls().LightslingerHitShots >= ReqShots;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                ShotType = ModContent.ProjectileType<LightslingerBomb>();
                Item.shootSpeed = 12f;
                Item.UseSound = SoundID.Item91;
                Item.useAnimation = 30;
                Item.useTime = 30;

            }
            else
            {
                Item.UseSound = SoundID.Item12;
                ShotType = ModContent.ProjectileType<LightslingerShot>();
                Item.shootSpeed = 16f;
                Item.useAnimation = 6;
                Item.useTime = 6;
            }
            return base.CanUseItem(player);
        }
        public override bool? UseItem(Player player)
        {
            FargoSoulsPlayer soulsPlayer = player.FargoSouls();
            if (player.whoAmI == Main.myPlayer)
                CooldownBarManager.Activate("LightslingerCharge", FargoAssets.GetTexture2D("Content/Bosses/Lifelight", "LifeNuke").Value, Color.Pink, 
                    () => (float)Main.LocalPlayer.FargoSouls().LightslingerHitShots / ReqShots, activeFunction: () => player.HeldItem != null && player.HeldItem.type == ModContent.ItemType<Lightslinger>());
            if (player.altFunctionUse == 2)
            {
                soulsPlayer.LightslingerHitShots = 0;
            }
            else
            {
                if (soulsPlayer.LightslingerHitShots >= ReqShots && player.whoAmI == Main.myPlayer)
                {
                    if (soulsPlayer.ChargeSoundDelay <= 0)
                    {
                        soulsPlayer.ChargeSoundDelay = 120;
                        SoundEngine.PlaySound(FargosSoundRegistry.ChargeSound, player.Center);
                    }
                    Vector2 direction = player.itemRotation.ToRotationVector2() * player.direction;
                    Vector2 perpDirection = direction.RotatedBy(MathHelper.PiOver2) * player.direction;
                    for (int i = 0; i < 7; i++)
                    {
                        Dust.NewDust(player.itemLocation + direction * Item.width * 0.5f + perpDirection * Item.height * 0.3f, 15, 15, DustID.PinkTorch, direction.X * Main.rand.NextFloat(2, 5), direction.Y * Main.rand.NextFloat(2, 5));
                    }
                }
            }

            return base.UseItem(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<LifelightBag>(2).AddTile(TileID.Solidifier).DisableDecraft().Register();
        }
    }
}