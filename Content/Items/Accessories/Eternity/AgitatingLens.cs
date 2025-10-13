using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Accessories.SupremeDeathbringerFairy;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class AgitatingLens : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<DebuffInstallKeyEffect>()];
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<BerserkedBuff>()] = true;

            player.AddEffect<AgitatingLensEffect>(Item);
            player.AddEffect<AgitatingLensInstall>(Item);
            player.AddEffect<DebuffInstallKeyEffect>(Item);
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Magic;
            tooltipColor = null;
            scaling = null;
            return AgitatingLensEffect.BaseDamage(Main.LocalPlayer);
        }
    }
    public class AgitatingLensEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupremeFairyHeader>();
        public override int ToggleItemType => ModContent.ItemType<AgitatingLens>();
        public override bool ExtraAttackEffect => true;
        public static int BaseDamage(Player player) => (int)(18 * player.ActualClassDamage(DamageClass.Magic));
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.AgitatingLensCD++ >= 60)
            {
                modPlayer.AgitatingLensCD = 60;
                if ((Math.Abs(player.velocity.X) >= 5 || Math.Abs(player.velocity.Y) >= 5) && player.whoAmI == Main.myPlayer)
                {
                    modPlayer.AgitatingLensCD = 0;
                    if (!modPlayer.SupremeDeathbringerFairy)
                        Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, player.velocity * 0.1f, ModContent.ProjectileType<BloodScytheFriendly>(), BaseDamage(player), 5f, player.whoAmI);
                    else if (modPlayer.IsDashingTimer == 0 && !modPlayer.SpecialDash)
                    {
                        for (int i = 0; i < Main.rand.Next(1, 3); i++)
                        {
                            Vector2 vel = -player.velocity + new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-6, -4));
                            Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, vel, ModContent.ProjectileType<KingSlimeBallFriendly>(), SupremeDashEffect.BaseDamage(player), 5f, player.whoAmI, ai2: 1);
                        }
                    }
                }
            }
        }
    }
    public class AgitatingLensInstall : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupremeFairyHeader>();
        public override int ToggleItemType => ModContent.ItemType<AgitatingLens>();
        
    }
    public class DebuffInstallKeyEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool ActiveSkill => true;
        public override int ToggleItemType => ModContent.ItemType<AgitatingLens>();
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            if (stunned)
                return;
            player.FargoSouls().DebuffInstallKey();
        }
    }
}
