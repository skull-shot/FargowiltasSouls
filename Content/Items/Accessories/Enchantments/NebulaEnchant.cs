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
    public class NebulaEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public static readonly Color NameColor = new(254, 126, 229);
        public override Color nameColor => NameColor;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Purple;
            Item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<NebulaEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.NebulaHelmet)
            .AddIngredient(ItemID.NebulaBreastplate)
            .AddIngredient(ItemID.NebulaLeggings)
            //.AddIngredient(ItemID.WingsNebula);
            .AddIngredient(ItemID.NebulaArcanum)
            .AddIngredient(ItemID.NebulaBlaze)
            //LeafBlower
            //bubble gun
            //chaarged blaster cannon
            .AddIngredient(ItemID.LunarFlareBook)

            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }

    public class NebulaEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<NebulaEnchant>();
        public override bool ExtraAttackEffect => true;

        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.NebulaEnchCD > 0)
            {
                modPlayer.NebulaEnchCD--;
            }
            
            int max = 3 * 60;
            if (player.whoAmI == Main.myPlayer)
                CooldownBarManager.Activate("NebulaEnchantCharge", ModContent.Request<Texture2D>("FargowiltasSouls/Content/Items/Accessories/Enchantments/NebulaEnchant").Value, NebulaEnchant.NameColor, 
                    () => (float)(max - Main.LocalPlayer.FargoSouls().NebulaEnchCD) / max, activeFunction: () => player.HasEffect<NebulaEffect>());

            if (player.setNebula)
                return;

            if (!modPlayer.TerrariaSoul) //cap boosters
            {
                void DecrementBuff(int buffType)
                {
                    for (int i = 0; i < player.buffType.Length; i++)
                    {
                        if (player.buffType[i] == buffType && player.buffTime[i] > 3)
                        {
                            player.buffTime[i] = 3;
                            break;
                        }
                    }
                };

                if (player.nebulaLevelDamage == 3)
                    DecrementBuff(BuffID.NebulaUpDmg3);
                if (player.nebulaLevelLife == 3)
                    DecrementBuff(BuffID.NebulaUpLife3);
                if (player.nebulaLevelMana == 3)
                    DecrementBuff(BuffID.NebulaUpMana3);
            }

            /*
            if (player.setNebula)
                return;

            player.setNebula = true;
            if (player.nebulaCD > 0)
                player.nebulaCD--;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (!modPlayer.TerrariaSoul && !modPlayer.ForceEffect<NebulaEnchant>()) //cap boosters
            {
                void DecrementBuff(int buffType)
                {
                    for (int i = 0; i < player.buffType.Length; i++)
                    {
                        if (player.buffType[i] == buffType && player.buffTime[i] > 3)
                        {
                            player.buffTime[i] = 3;
                            break;
                        }
                    }
                };

                if (player.nebulaLevelDamage == 3)
                    DecrementBuff(BuffID.NebulaUpDmg3);
                if (player.nebulaLevelLife == 3)
                    DecrementBuff(BuffID.NebulaUpLife3);
                if (player.nebulaLevelMana == 3)
                    DecrementBuff(BuffID.NebulaUpMana3);
            }
            */
        }

        /*
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (damageClass != DamageClass.Magic && player.nebulaCD <= 0 && Main.rand.NextBool(3))
            {
                player.nebulaCD = 30;
                int num35 = Utils.SelectRandom(Main.rand, new int[]
                {
                                                            3453,
                                                            3454,
                                                            3455
                });
                int i = Item.NewItem(player.GetSource_OpenItem(num35), (int)target.position.X, (int)target.position.Y, target.width, target.height, num35, 1, false, 0, false, false);
                Main.item[i].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
                Main.item[i].velocity.X = Main.rand.Next(10, 31) * 0.2f * (projectile == null ? player.direction : projectile.direction);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
                }
            }
        }
        */
    }
}
