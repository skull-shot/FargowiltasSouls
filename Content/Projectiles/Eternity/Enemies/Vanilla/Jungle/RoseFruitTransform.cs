using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Terraria.GameContent;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Jungle
{
    public class RoseFruitTransform : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_208";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
        }

        public override bool? CanDamage() => false;

        public ref float owner => ref Projectile.ai[0];

        public ref float state => ref Projectile.ai[1];

        public enum States {
            Rose = 0,
            Stomach,
            GrowingFruit,
            Fruit
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)owner];
            if (!npc.active)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft++;
            Projectile.Center = npc.Center + ((npc.frame.Height / 2) * Vector2.UnitX).RotatedBy(npc.rotation);

            switch ((States)state)
            {
                case States.Stomach:
                    Projectile.scale = 0;
                    break;
                case States.Fruit:
                    if (FargoSoulsUtil.HostCheck)
                    {
                        int i = Item.NewItem(Projectile.InheritSource(Projectile), Projectile.Center, ModContent.Find<ModItem>("Fargowiltas", "PlanterasFruit").Type);
                        Main.item[i].velocity = 10 * Vector2.UnitX.RotatedBy(npc.rotation);
                    }
                    Projectile.Kill();
                    break;
                default:
                    break;
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture;

            if (state == (int) States.Rose) // rose
                texture = TextureAssets.Item[ItemID.JungleRose].Value;
            else // fruit
                texture = ModContent.Request<Texture2D>("Fargowiltas/Content/Items/Summons/Mutant/PlanterasFruit", AssetRequestMode.ImmediateLoad).Value;

            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
