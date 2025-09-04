using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Projectiles.Accessories;
using FargowiltasSouls.Content.Projectiles.Armor;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor.Nekomi
{
    [AutoloadEquip(EquipType.Head)]
    public class NekomiHood : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Armor/Nekomi", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 1, 50);
            Item.defense = 6;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.07f;
            player.maxMinions += 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<NekomiHoodie>() && legs.type == ModContent.ItemType<NekomiLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = getSetBonusString();
            NekomiSetBonus(player, Item);
        }

        public static string getSetBonusString()
        {
            string key = Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
            return Language.GetTextValue($"Mods.FargowiltasSouls.SetBonus.Nekomi", key);
        }

        public const int MAX_METER = 3000;
        public const int MAX_HEARTS = 9;

        public static void NekomiSetBonusKey(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.NekomiSet && player.whoAmI == Main.myPlayer)
            {
                bool superAttack = modPlayer.NekomiAttackReadyTimer > 0;
                if (superAttack)
                {
                    int baseDamage = 450;
                    if (!Main.hardMode)
                        baseDamage /= 2;
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Misc(""), player.Center, Vector2.Zero, ModContent.ProjectileType<NekomiDevi>(), baseDamage, 16f, player.whoAmI);
                    SoundEngine.PlaySound(SoundID.Item43, player.Center);
                    modPlayer.NekomiMeter = 0;
                    modPlayer.NekomiAttackReadyTimer = 0;
                }
                else
                {
                    int hearts = (int)((double)modPlayer.NekomiMeter / MAX_METER * MAX_HEARTS);
                    for (int i = 0; i < hearts; i++)
                    {
                        Vector2 offset = -150f * Vector2.UnitY.RotatedBy(MathHelper.TwoPi / hearts * i);
                        Vector2 spawnPos = player.Center + offset;
                        const float speed = 12;
                        Vector2 vel = speed * player.DirectionFrom(spawnPos);
                        int baseHeartDamage = 17;
                        const float ai1 = 150 / speed;
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Misc(""), spawnPos, vel, ModContent.ProjectileType<FriendHeart>(), baseHeartDamage, 3f, player.whoAmI, -1, ai1);
                    }

                    if (hearts > 0)
                        modPlayer.NekomiMeter = 0;
                }
            }
        }
        public static void NekomiSetBonus(Player player, Item item)
        {
            player.GetDamage(DamageClass.Generic) += 0.07f;
            player.GetCritChance(DamageClass.Generic) += 7;
            player.maxMinions += 1;

            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            fargoPlayer.Graze = true;
            fargoPlayer.NekomiSet = true;

            player.AddEffect<MasoGrazeRing>(item);
            if (fargoPlayer.Graze && player.whoAmI == Main.myPlayer && player.HasEffect<MasoGrazeRing>() && player.ownedProjectileCounts[ModContent.ProjectileType<GrazeRing>()] < 1)
                Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, Vector2.Zero, ModContent.ProjectileType<GrazeRing>(), 0, 0f, Main.myPlayer);

            const int decayTime = 420;
            if (fargoPlayer.NekomiTimer > 0)
            {
                const int bonusSpeedPoint = 90;
                int increment = fargoPlayer.NekomiTimer / bonusSpeedPoint + 1;
                fargoPlayer.NekomiTimer -= increment;
                fargoPlayer.NekomiMeter += increment;

                if (fargoPlayer.NekomiMeter > MAX_METER)
                    fargoPlayer.NekomiMeter = MAX_METER;
            }
            /* else if (--fargoPlayer.NekomiTimer < -decayTime)
            {
                if (fargoPlayer.NekomiTimer < -decayTime * 2)
                    fargoPlayer.NekomiTimer = -decayTime * 2;

                int depreciation = -decayTime - fargoPlayer.NekomiTimer;
                fargoPlayer.NekomiMeter -= (int)MathHelper.Lerp(1, MAX_METER / decayTime, depreciation / decayTime);
                if (fargoPlayer.NekomiMeter < 0)
                    fargoPlayer.NekomiMeter = 0;
            } */

            if (player.whoAmI == Main.myPlayer)
            {
                CooldownBarManager.Activate("NekomiHoodGraze", FargoAssets.GetTexture2D("Content/Items/Armor/Nekomi", "NekomiHood").Value, Color.DeepPink, () => (float)fargoPlayer.NekomiMeter / MAX_METER, true, 0, () => fargoPlayer.NekomiSet);

                if (fargoPlayer.NekomiMeter >= MAX_METER)
                    fargoPlayer.NekomiAttackReadyTimer = FargoSoulsPlayer.SuperAttackMaxWindow;

                int ritualType = ModContent.ProjectileType<NekomiRitual>();
                if (player.ownedProjectileCounts[ritualType] < 1)
                    Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, Vector2.Zero, ritualType, 0, 0f, player.whoAmI);
            }
        }

        public static void OnGraze(FargoSoulsPlayer fargoPlayer, int damage)
        {
            if (fargoPlayer.NekomiSet)
            {
                fargoPlayer.NekomiTimer = Math.Clamp(fargoPlayer.NekomiTimer + 60, 0, 420);
            }

            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Accessories/Graze") { Volume = 0.5f }, Main.LocalPlayer.Center);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Silk, 10)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 5)
            .AddTile(TileID.Loom)

            .Register();
        }
    }
}