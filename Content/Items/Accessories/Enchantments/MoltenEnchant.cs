using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Items.Accessories.Forces.TimberForce;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class MoltenEnchant : BaseEnchant
    {

        public override Color nameColor => new(193, 43, 43);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<MoltenEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.MoltenHelmet)
            .AddIngredient(ItemID.MoltenBreastplate)
            .AddIngredient(ItemID.MoltenGreaves)
            //ashwood ench
            .AddIngredient(ItemID.MoltenFury)
            .AddIngredient(ItemID.DemonsEye)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class MoltenEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<MoltenEnchant>();
        public static float AuraSize(Player player)
        {
            if (player.HasEffect<NatureEffect>())
            {
                return ShadewoodEffect.Range(player, true);
            }
            if (player.FargoSouls().ForceEffect<MoltenEnchant>())
                return 200 * 1.2f;
            return 200;
             
        }
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                bool nature = player.HasEffect<NatureEffect>();

                //player.inferno = true;
                int visualProj = ModContent.ProjectileType<MoltenAuraProj>();
                if (player.ownedProjectileCounts[visualProj] <= 0)
                {
                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, Vector2.Zero, visualProj, 0, 0, Main.myPlayer);
                }
                if (!nature)
                    Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0.65f, 0.4f, 0.1f);

                int buff = BuffID.OnFire;
                float distance = AuraSize(player);
                int baseDamage = player.FargoSouls().ForceEffect<MoltenEnchant>() ? 40 : 20;

                int damage = FargoSoulsUtil.HighestDamageTypeScaling(player, baseDamage);

                if (player.whoAmI == Main.myPlayer)
                {
                    bool healed = false;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.friendly && !npc.dontTakeDamage && !(npc.damage == 0 && npc.lifeMax == 5)) //critters
                        {
                            if (Vector2.Distance(player.Center, FargoSoulsUtil.ClosestPointInHitbox(npc.Hitbox, player.Center)) <= distance)
                            {
                                int dmgRate = 30;//60;

                                if (!nature)
                                {
                                    if (player.FindBuffIndex(BuffID.OnFire) == -1)
                                        player.AddBuff(BuffID.OnFire, 10);

                                    if (npc.FindBuffIndex(buff) == -1)
                                        npc.AddBuff(buff, 120);

                                    if (player.infernoCounter % dmgRate == 0)
                                        player.ApplyDamageToNPC(npc, damage, 0f, 0, false);
                                }
                                else
                                {
                                    baseDamage = 50;
                                    int time = player.FargoSouls().TimeSinceHurt;
                                    float minTime = 60 * 4;
                                    if (time > minTime)
                                    {
                                        float maxBonus = 4; // at 16s
                                        float bonus = MathHelper.Clamp(time / (60 * 4), 1, maxBonus);
                                        baseDamage = (int)(baseDamage * bonus);
                                        damage = FargoSoulsUtil.HighestDamageTypeScaling(player, baseDamage);

                                        if (player.infernoCounter % dmgRate == 0)
                                        {
                                            player.ApplyDamageToNPC(npc, damage, 0f, 0, false);
                                            if (player.HasEffect<CrimsonEffect>() && !healed)
                                            {
                                                healed = true;
                                                player.FargoSouls().HealPlayer(damage / 80);
                                            }  
                                        }
                                    }
                                }

                                int moltenDebuff = ModContent.BuffType<Buffs.Souls.MoltenAmplifyBuff>();
                                if (npc.FindBuffIndex(moltenDebuff) == -1)
                                    npc.AddBuff(moltenDebuff, 10);


                            }
                        }
                    }
                }
            }
        }
    }
}
