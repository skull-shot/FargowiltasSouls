using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Waist)]
    public class TribalCharm : SoulsItem
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
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 4);
            Item.defense = 6;
        }
        public static void PassiveEffects(Player player)
        {
            player.FargoSouls().TribalCharm = true;
        }
        public static void ActiveEffects(Player player, Item item)
        {
            PassiveEffects(player);
            player.buffImmune[BuffID.Webbed] = true;
            player.FargoSouls().TribalCharm = true;
            player.FargoSouls().TribalCharmEquipped = true;
            player.AddEffect<TribalCharmClickBonus>(item);
        }
        public override void UpdateInventory(Player player)
        {
            PassiveEffects(player);
        }

        public override void UpdateVanity(Player player)
        {
            PassiveEffects(player);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ActiveEffects(player, Item);
        }

        public static void Effects(FargoSoulsPlayer modPlayer)
        {
            Player player = modPlayer.Player;
            if (player.controlUseItem || player.controlUseTile)
            {
                if (modPlayer.TribalCharmClickBonus)
                {
                    modPlayer.TribalCharmClickBonus = false;
                    player.GetDamage(DamageClass.Generic) += 0.20f;
                    if (player.HasEffect<PungentMinion>() && player.HasEffect<LithosphericEffect>() && player.HeldItem != null && player.HeldItem.IsWeapon())
                    {
                        foreach (Projectile p in Main.ActiveProjectiles)
                        {
                            if (p.TypeAlive<CrystalSkull>() && p.owner == player.whoAmI)
                            {
                                int charge = 30;
                                if (p.localAI[0] >= 0 && p.localAI[0] < charge)
                                    p.localAI[0] = charge;
                                p.netUpdate = true;
                                break;
                            }
                        }
                    }
                }
            }
            else if (player.ItemTimeIsZero && player.HasEffect<TribalCharmClickBonus>())
            {
                modPlayer.TribalCharmClickBonus = true;
            }

            if (modPlayer.TribalCharmClickBonus)
            {
                player.AddBuff(ModContent.BuffType<TribalCharmClickBuff>(), 2);
                int d = Dust.NewDust(player.position, player.width, player.height, DustID.ChlorophyteWeapon, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 0, new Color(), 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 4f;
            }
        }
    }
    public class TribalCharmClickBonus : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<BionomicHeader>();
        public override int ToggleItemType => ModContent.ItemType<TribalCharm>();
    }
}