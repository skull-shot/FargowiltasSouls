using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class SkeletronArmsBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skeletron Arms");
            // Description.SetDefault("The Skeletron arms will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "骷髅王之手");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "骷髅王之手将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().SkeletronArms = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletronArm>()] < 2)
                {
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletronArm>()]  == 1)
                    {
                        
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<SkeletronArm>())
                            {
                                Main.projectile[i].Kill();
                            }
                        }
                    }
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<SkeletronArm>(), SkeleMinionEffect.BaseDamage(player), 8f, player.whoAmI, 1);
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<SkeletronArm>(), SkeleMinionEffect.BaseDamage(player), 8f, player.whoAmI, -1);
                }
            }
        }
    }
}