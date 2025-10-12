using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class Deerclawps : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
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
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.AddEffect<DeerclawpsEffect>(Item);
            player.AddEffect<DeerclawpsDashDR>(Item);
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Melee;
            tooltipColor = null;
            scaling = null;
            return DeerclawpsEffect.BaseDamage(Main.LocalPlayer);
        }
    }
    public class DeerclawpsDashDR : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<Deerclawps>();
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.IsInADashState || modPlayer.SpecialDash)
                player.endurance += 0.2f;
        }
    }
    public class DeerclawpsEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupremeFairyHeader>();
        public override int ToggleItemType => ModContent.ItemType<Deerclawps>();
        public static int BaseDamage(Player player) => (int)(24 * player.ActualClassDamage(DamageClass.Melee));
        public static void DeerclawpsAttack(Player player, Vector2 pos)
        {
            if (player.whoAmI == Main.myPlayer && player.timeSinceLastDashStarted % (player.FargoSouls().MasochistSoul ? 3 : 2) == 0)
            {
                Vector2 vel = 16f * -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30));
                int type = ProjectileID.DeerclopsIceSpike;
                float ai0 = -15f;
                float ai1 = Main.rand.NextFloat(0.5f, 1f);
                int dmg = BaseDamage(player);
                if (player.FargoSouls().SupremeDeathbringerFairy)
                {
                    type = ProjectileID.SharpTears;
                    dmg = SupremeDashEffect.BaseDamage(player) * (player.FargoSouls().MasochistSoul ? 10 : 1);
                }
                if (player.FargoSouls().MasochistSoul)
                    ai1 += 0.5f;
                if (player.velocity.Y == 0)
                    Projectile.NewProjectile(player.GetSource_EffectItem<DeerclawpsEffect>(), pos, vel, type, dmg, 4f, Main.myPlayer, ai0, ai1);
                else
                {
                    int npcID = FargoSoulsUtil.FindClosestHostileNPC(pos, 300, true, true);
                    if (!npcID.IsWithinBounds(Main.maxNPCs))
                        return;
                    NPC npc = Main.npc[npcID];
                    if (!npc.Alive())
                        return;
                    vel = pos.DirectionTo(npc.Center) * vel.Length();
                    Projectile.NewProjectile(player.GetSource_EffectItem<DeerclawpsEffect>(), pos, vel.RotatedByRandom(MathHelper.PiOver2 * 0.3f), type, dmg, 4f, Main.myPlayer, ai0, ai1);
                }
            }
        }
    }
}
