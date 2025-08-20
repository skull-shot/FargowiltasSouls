using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class NymphsPerfume : SoulsItem
    {
        public override bool Eternity => true;

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
            Item.value = Item.sellPrice(0, 4);
        }
        public static void PassiveEffects(Player player, Item item)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            fargoPlayer.NymphsPerfumeRespawn = true;
        }
        public static void ActiveEffects(Player player, Item item)
        {
            PassiveEffects(player, item);
            player.buffImmune[BuffID.Lovestruck] = true;
            player.buffImmune[ModContent.BuffType<LovestruckBuff>()] = true;
            player.buffImmune[ModContent.BuffType<HexedBuff>()] = true;
            player.buffImmune[BuffID.Stinky] = true;
            player.AddEffect<NymphPerfumeEffect>(item);
        }
        public override void UpdateInventory(Player player)
        {
            PassiveEffects(player, Item);
        }

        public override void UpdateVanity(Player player)
        {
            PassiveEffects(player, Item);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ActiveEffects(player, Item);
        }
    }
    public class NymphPerfumeEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<BionomicHeader>();
        public override int ToggleItemType => ModContent.ItemType<NymphsPerfume>();
        public override bool ExtraAttackEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.NymphsPerfume = true;
            if (modPlayer.NymphsPerfumeCD > 0)
                modPlayer.NymphsPerfumeCD -= 1;
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            int healamount = 10;
            if (modPlayer.NymphsPerfumeCD <= 0 && !target.immortal && !player.moonLeech && target.canGhostHeal)
            {
                Projectile.NewProjectile(player.GetSource_EffectItem<NymphPerfumeEffect>(), target.Center, Vector2.Zero, ModContent.ProjectileType<NymphHeart>(), 0, 0, player.whoAmI, player.whoAmI, healamount);
                modPlayer.NymphsPerfumeCD = 600;
            }
        }
    }
}