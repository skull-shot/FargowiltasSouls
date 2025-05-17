using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class ShellHideBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shell Hide");
            // Description.SetDefault("Projectiles are being blocked");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "缩壳");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "阻挡抛射物,但受到双倍接触伤害");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            player.noKnockback = true;
            //player.endurance += modPlayer.TurtleShellHP / 250f;
            if (player.thorns == 0f)
                player.thorns = 1f;
            player.thorns *= 5f;
            modPlayer.ShellHide = true;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<TurtleShield>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<TurtleShield>(), 0, 0, player.whoAmI);
            }

                Main.projectile.Where(x => x.active && x.hostile && x.damage > 0 && x.Hitbox.Intersects(player.Hitbox) && ProjectileLoader.CanDamage(x) != false && ProjectileLoader.CanHitPlayer(x, player) && FargoSoulsUtil.CanDeleteProjectile(x)).ToList().ForEach(x =>
                {
                    /*int dustId = Dust.NewDust(new Vector2(x.position.X, x.position.Y + 2f), x.width, x.height + 5, DustID.GoldFlame, x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100,
                        default, 2f);
                    Main.dust[dustId].noGravity = true;
                    int dustId3 = Dust.NewDust(new Vector2(x.position.X, x.position.Y + 2f), x.width, x.height + 5, DustID.GoldFlame, x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100,
                        default, 2f);
                    Main.dust[dustId3].noGravity = true;*/

                    // Turn around
                    x.velocity *= -1f;

                    // Flip sprite
                    if (x.Center.X > player.Center.X)
                    {
                        x.direction = 1;
                        x.spriteDirection = 1;
                    }
                    else
                    {
                        x.direction = -1;
                        x.spriteDirection = -1;
                    }
                    
                    x.hostile = false;
                    x.friendly = true;
                    x.damage *= 5;
                    SoundEngine.PlaySound(SoundID.Item150, player.Center);
                });
        }

    }
}