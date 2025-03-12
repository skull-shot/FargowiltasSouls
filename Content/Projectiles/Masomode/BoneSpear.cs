using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class BoneSpear : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            base.SetDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Asset<Texture2D> head = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Masomode/BoneSpearHead");
            Vector2 origin = t.Size() - new Vector2(16, 16);
            lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
            if (Projectile.ai[0] < 0)
            {
                origin = t.Size() / 2;
            }
            
            for (int j = 0; j < 10; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1 ;
                Color glowColor = Color.Blue with { A = 0 } * 0.7f;


                Main.EntitySpriteDraw(head.Value, Projectile.Center + afterimageOffset - Main.screenPosition, null, glowColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, 1, SpriteEffects.None);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            base.OnKill(timeLeft);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 vec = Projectile.Center - new Vector2(29, 29).RotatedBy(Projectile.rotation) * Projectile.scale;
            if (Projectile.ai[0] < 0)
            {
                vec = Projectile.Center - new Vector2(19, 19).RotatedBy(Projectile.rotation) * Projectile.scale;
            }
            Point center = vec.ToPoint();
            
            
            Rectangle hitbox = new Rectangle((int)vec.X - 3, (int)vec.Y - 3, 6, 6);
            return targetHitbox.Intersects(hitbox);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Collision.SolidCollision(Projectile.TopLeft, Projectile.width, -6))
            {
                Projectile.velocity.Y = 5;
                
                Projectile.velocity.X = oldVelocity.X;
                return false;
            }
            if (Projectile.oldVelocity.Y <= 0)
            {
                return false;
            }
            
            return base.OnTileCollide(oldVelocity);
        }
        public static int[] Bones = [NPCID.AngryBones,
            NPCID.AngryBonesBig,
            NPCID.AngryBonesBigHelmet,
            NPCID.AngryBonesBigMuscle];
        public override void AI()
        {
            
            if ((int)Projectile.ai[0] >= 0 && (Main.npc[(int)Projectile.ai[0]] == null || !Main.npc[(int)Projectile.ai[0]].active || !Bones.Contains(Main.npc[(int)Projectile.ai[0]].type)))
            {
                
                Projectile.ai[0] = -1;
            }
            if (Projectile.ai[0] == -1)
            {
                Projectile.tileCollide = true;
                Projectile.velocity = new Vector2(0, -8).RotatedByRandom(MathHelper.ToRadians(10));
                Projectile.ai[0] = -2;
            }else if (Projectile.ai[0] == -2)
            {
                
                Projectile.velocity += new Vector2(0, 0.2f);
                Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length() * 2);
            }
            else
            {
                Projectile.timeLeft = 200;
                NPC owner = Main.npc[(int)Projectile.ai[0]];
                Projectile.Center = owner.Center;
                Projectile.velocity = Vector2.Zero;
                if (owner.spriteDirection == 1)
                {
                    Projectile.rotation = MathHelper.ToRadians(135);
                }
                else
                {
                    Projectile.rotation = MathHelper.ToRadians(315);
                }
            }
            base.AI();
        }
    }
}
