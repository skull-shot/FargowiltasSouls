using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class TitaniumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(130, 140, 136);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TitaniumEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyTitaHead")
                .AddIngredient(ItemID.TitaniumBreastplate)
                .AddIngredient(ItemID.TitaniumLeggings)
                .AddIngredient(ItemID.Chik)
                .AddIngredient(ItemID.CrystalStorm)
                .AddIngredient(ItemID.CrystalVileShard)

                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }

    public class TitaniumEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override int ToggleItemType => ModContent.ItemType<TitaniumEnchant>();

        public override float ContactDamageDR(Player player, NPC npc, ref Player.HurtModifiers modifiers)
        {
            return TitaniumDR(player, npc);
        }
        public override float ProjectileDamageDR(Player player, Projectile projectile, ref Player.HurtModifiers modifiers)
        {
            return TitaniumDR(player, projectile);
        }
        public static float TitaniumDR(Player player, Entity attacker)
        {
            if (!player.HasEffectEnchant<TitaniumEffect>())
                return 0;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (!modPlayer.TitaniumDRBuff)
                return 0;
            NPC sourceNPC = null;
            if (attacker is NPC attackerNPC)
                sourceNPC = attackerNPC;
            if (attacker is Projectile projectile && projectile.GetSourceNPC() is NPC projNPC)
                sourceNPC = projNPC;

            if (sourceNPC != null)
            {
                float dr = 0.1f;
                float closeExtraDR = 0.2f;
                if (modPlayer.ForceEffect<TitaniumEnchant>())
                    closeExtraDR += 0.1f;
                float distance = player.Distance(sourceNPC.Center);
                float closeBonus = MathHelper.Lerp(closeExtraDR, 0f, distance / 1000f);
                closeBonus = MathHelper.Clamp(closeBonus, 0f, closeExtraDR);
                dr += closeBonus;
                return dr;
            }
            return 0;
        }

        public static void TitaniumShards(FargoSoulsPlayer modPlayer, Player player)
        {
            if (modPlayer.TitaniumCD)
                return;
            if (!player.HasEffectEnchant<TitaniumEffect>())
                return;

            player.AddBuff(306, 600, true, false);
            if (player.ownedProjectileCounts[ProjectileID.TitaniumStormShard] < 20)
            {
                int damage = 35;
                damage = (int)(damage * player.ActualClassDamage(DamageClass.Melee));
                int sh = Projectile.NewProjectile(player.GetSource_Accessory(player.EffectItem<TitaniumEffect>()), player.Center, Vector2.Zero, ProjectileID.TitaniumStormShard /*ModContent.ProjectileType<TitaniumShard>()*/, damage, 15f, player.whoAmI, 0f, 0f);
                Main.projectile[sh].DamageType = DamageClass.Melee;
            }
            else
            {
                if (!player.HasBuff(ModContent.BuffType<TitaniumDRBuff>()))
                {
                    //dust ring
                    for (int j = 0; j < 20; j++)
                    {
                        Vector2 vector6 = Vector2.UnitY * 5f;
                        vector6 = vector6.RotatedBy((j - (20 / 2 - 1)) * 6.28318548f / 20) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Titanium);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].scale = 1.5f;
                        Main.dust[d].velocity = vector7;
                    }
                }

                int buffDuration = 240;
                player.AddBuff(ModContent.BuffType<TitaniumDRBuff>(), buffDuration);
            }
        }

        public override void PostUpdateMiscEffects(Player player)
        {
            if (!player.HasEffectEnchant<TitaniumEffect>())
                return;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.TitaniumDRBuff && modPlayer.prevDyes == null)
            {
                modPlayer.prevDyes = [];
                int reflectiveSilver = GameShaders.Armor.GetShaderIdFromItemId(ItemID.ReflectiveSilverDye);

                for (int i = 0; i < player.dye.Length; i++)
                {
                    modPlayer.prevDyes.Add(player.dye[i].dye);
                    player.dye[i].dye = reflectiveSilver;
                }

                for (int j = 0; j < player.miscDyes.Length; j++)
                {
                    modPlayer.prevDyes.Add(player.miscDyes[j].dye);
                    player.miscDyes[j].dye = reflectiveSilver;
                }

                player.UpdateDyes();
            }
            else if (!player.HasBuff(ModContent.BuffType<TitaniumDRBuff>()) && modPlayer.prevDyes != null)
            {
                for (int i = 0; i < player.dye.Length; i++)
                {
                    player.dye[i].dye = modPlayer.prevDyes[i];
                }

                for (int j = 0; j < player.miscDyes.Length; j++)
                {
                    player.miscDyes[j].dye = modPlayer.prevDyes[j + player.dye.Length];
                }

                player.UpdateDyes();

                modPlayer.prevDyes = null;
            }
        }
    }
}
