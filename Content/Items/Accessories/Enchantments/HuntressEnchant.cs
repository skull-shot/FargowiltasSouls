using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class HuntressEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(122, 192, 76);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 200000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<HuntressEffect>(Item);
        }

        public static void HuntressBonus(FargoSoulsPlayer modPlayer, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {

        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.HuntressWig)
            .AddIngredient(ItemID.HuntressJerkin)
            .AddIngredient(ItemID.HuntressPants)
            .AddIngredient(ItemID.IceBow)
            .AddIngredient(ItemID.ShadowFlameBow)
            .AddIngredient(ItemID.DD2PhoenixBow)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
    public class HuntressEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<WillHeader>();
        public override int ToggleItemType => ModContent.ItemType<HuntressEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.HuntressCD > 0)
            {
                modPlayer.HuntressCD--;
            }
        }
        public override void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            FargoSoulsGlobalProjectile soulsProj = proj.FargoSouls();

            if (soulsProj.HuntressProj == 1 && target.type != NPCID.TargetDummy)
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                soulsProj.HuntressProj = 2;
                bool redRiding = player.HasEffect<RedRidingHuntressEffect>();

                if (modPlayer.HuntressCD == 0)
                {
                    modPlayer.HuntressStage++;

                    if (modPlayer.HuntressStage >= 10)
                    {
                        modPlayer.HuntressStage = 10;

                        if (player.HasEffect<RedRidingEffect>() && modPlayer.RedRidingArrowCD == 0)
                        {
                            RedRidingEffect.SpawnArrowRain(modPlayer.Player, target);
                        }
                    }

                    modPlayer.HuntressCD = 30;

                    if (player.whoAmI == Main.myPlayer)
                        CooldownBarManager.Activate("HuntressBuildup", ModContent.Request<Texture2D>("FargowiltasSouls/Content/Items/Accessories/Enchantments/HuntressEnchant").Value, new(122, 192, 76),
                            () => modPlayer.HuntressStage / 10f, true, activeFunction: () => player.HasEffect<HuntressEffect>());
                }
                int bonus = modPlayer.ForceEffect<HuntressEnchant>() || redRiding ? 5 : 3;
                if (player.HasBuff<GladiatorSpiritBuff>())
                    bonus = 15;
                proj.ArmorPenetration += bonus * 2 * modPlayer.HuntressStage;
                modifiers.SourceDamage.Flat += bonus * modPlayer.HuntressStage;
            }
        }
    }
}
