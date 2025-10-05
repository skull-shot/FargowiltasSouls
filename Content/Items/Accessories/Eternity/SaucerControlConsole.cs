using System.Collections.Generic;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class SaucerControlConsole : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;
        public override List<AccessoryEffect> ActiveSkillTooltips => 
            [AccessoryEffectLoader.GetEffect<AmmoCycleEffect>()];

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 6);
        }

        public override void UpdateInventory(Player player)
        {
            //player.AddEffect<AmmoCycleEffect>(Item);
        }

        public override void UpdateVanity(Player player)
        {
            //player.AddEffect<AmmoCycleEffect>(Item);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<AmmoCycleEffect>(Item);
            player.buffImmune[BuffID.VortexDebuff] = true;
            player.buffImmune[ModContent.BuffType<UnstableBuff>()] = true;
            player.AddEffect<UfoMinionEffect>(Item);
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            Player player = Main.LocalPlayer;
            damageClass = DamageClass.Ranged;
            tooltipColor = null;
            scaling = null;
            return (int)((UfoMinionEffect.BaseDamage(player) + player.FindAmmo([AmmoID.Arrow, AmmoID.Bullet, AmmoID.Rocket]).damage) * player.ActualClassDamage(DamageClass.Ranged));
        }
    }
    public class UfoMinionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<HeartHeader>();
        public override int ToggleItemType => ModContent.ItemType<SaucerControlConsole>();
        public override bool MinionEffect => true;
        public static int BaseDamage(Player player) => NPC.downedGolemBoss ? 30 : 0;
        public override void PostUpdateEquips(Player player)
        {
            if (!player.HasBuff<SouloftheMasochistBuff>())
                player.AddBuff(ModContent.BuffType<SaucerMinionBuff>(), 2);
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (item != null || (projectile != null && projectile.FargoSouls().ItemSource)) // TODO: mark projectiles created by saucer so i dont need to use itemsource
            {
                for (int i = 0; i < Main.maxProjectiles; i++) //make minion attack when you attack a given enemy
                {
                    if (Main.projectile[i].ai[0] == 0 && Main.projectile[i].type == ModContent.ProjectileType<MiniSaucer>() && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].active)
                    {
                        Main.projectile[i].localAI[1] = target.whoAmI;
                        Main.projectile[i].ai[0] = 20;
                        if (Main.projectile[i].localAI[0] == 0 && !Main.dedServ)
                            SoundEngine.PlaySound(SoundID.Zombie68, Main.projectile[i].Center);
                        if (Main.projectile[i].localAI[0] < 120)
                            Main.projectile[i].localAI[0] += 60;
                        else Main.projectile[i].localAI[0] = 120;
                        break;
                    }
                }
            }
        }

    }
    public class AmmoCycleEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool ActiveSkill => true;
        public override int ToggleItemType => ModContent.ItemType<SaucerControlConsole>();
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            player.FargoSouls().AmmoCycleKey();
        }
    }
}