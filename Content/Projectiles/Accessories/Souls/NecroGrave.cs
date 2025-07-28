using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class NecroGrave : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Accessories/Souls", Name);
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1800;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            //move it up when spawned, synchronizes with necro ench spawning at npc.Bottom
            Projectile.Bottom = Projectile.Center;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter % 40 == 0)
            {
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }
            Player player = Main.player[Projectile.owner];

            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f);

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item2, Projectile.Center);
            }
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.ForceEffect<NecroEnchant>())
            {
                Projectile.width = 48 * 2;
                Projectile.height = 32 * 2;
                if (Projectile.Distance(player.Center) < 300)
                {
                    float speed = 1;
                    Projectile.velocity = Projectile.SafeDirectionTo(player.Center) * speed;
                } 
                else //float in place ominously
                {
                    Projectile.velocity = Vector2.Zero;
                    Projectile.ai[1] += 1;
                    if (Projectile.ai[1] >= 60)
                        Projectile.ai[1] = 0;
                    if (Projectile.ai[1] > 30)
                        Projectile.velocity.Y -= 0.1f;
                    else
                        Projectile.velocity.Y += 0.1f;
                }
                //worst possible fucking way to do this LETS FUCKING GOOOOOOOOOOOOOOOO

                /*for (int i = 0; i < 4; i++) //smoke to make the floating convincing
                {
                    int d = Dust.NewDust(Projectile.BottomLeft, Projectile.width, 0, DustID.Wraith);
                    Main.dust[d].position.Y -= 4;
                    Main.dust[d].velocity *= 0.5f;
                    Main.dust[d].noGravity = true;
                }*/
            }
            else
            {
                Projectile.velocity.Y += 0.2f;
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }

            if (Main.LocalPlayer.Alive() && Main.LocalPlayer.Hitbox.Intersects(Projectile.Hitbox) && player.HasEffect<NecroEffect>())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, -12), ModContent.ProjectileType<DungeonGuardianNecro>(), (int)Projectile.ai[0], 1, Projectile.owner);

                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.Center);

                for (int i = 0; i < 36; i++)
                {
                    int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Bone, 0, -3);
                    Main.dust[d].velocity *= 2;
                    Main.dust[d].noGravity = Main.rand.NextBool();
                }

                FargoGlobalItem.OnRetrievePickup(Main.LocalPlayer);

                Projectile.Kill();
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];
            if (!player.FargoSouls().ForceEffect<NecroEnchant>())
                fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            /*Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            int num156 = texture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture, drawPosition, rectangle, color, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return base.PreDraw(ref lightColor);*/

            Texture2D texture = Main.player[Projectile.owner].FargoSouls().ForceEffect<NecroEnchant>() ? FargoAssets.GetTexture2D("Content/Projectiles/Accessories", "NecroGraveForce").Value : Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            int num156 = texture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture, drawPosition, rectangle, color, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
