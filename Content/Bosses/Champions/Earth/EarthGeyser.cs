using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Earth
{
    public class EarthGeyser : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Geyser");
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.extraUpdates = 14;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Tile tile = Framing.GetTileSafely(Projectile.Center);

            if (Projectile.ai[1] == 0) //spawned, while in ground tile
            {
                Projectile.position.Y -= 16;
                if (!(tile.HasUnactuatedTile && Main.tileSolid[tile.TileType])) //if reached air tile
                {
                    Projectile.ai[1] = 1;
                    Projectile.netUpdate = true;
                }
            }
            else //has exited ground tiles and reached air tiles, now stop the next time you reach a ground tile
            {
                if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && tile.TileType != TileID.Platforms && tile.TileType != TileID.PlanterBox) //if inside solid tile, go back down
                {
                    if (Projectile.timeLeft > 90)
                        Projectile.timeLeft = 90;
                    Projectile.extraUpdates = 0;
                    Projectile.position.Y -= 16;
                    //make warning dusts
                    //int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, -8f);
                    //Main.dust[d].velocity *= 3f;
                }
                else //if in air, go up
                {
                    Projectile.position.Y += 16;
                }
            }

            //if (Projectile.timeLeft <= 120) //about to erupt, make more dust
                //Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);

            /*NPC golem = Main.npc[ai0];
            if (golem.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().Counter == 2 && FargoSoulsUtil.HostCheck) //when golem does second stomp, erupt
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.UnitY * 8, ProjectileID.GeyserTrap, Projectile.damage, 0f, Main.myPlayer);
                Projectile.Kill();
                return;
            }*/
        }

        public override void OnKill(int timeLeft)
        {
            if (FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < 4; i++)
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center - Vector2.UnitY * i * 180, Vector2.UnitY * -20, ProjectileID.GeyserTrap, Projectile.damage, 0f, Main.myPlayer);
            }
                
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] == 0)
                return false;
            Projectile.localAI[2]++;
            Vector2 dir = -Vector2.UnitY;

            Asset<Texture2D> line = TextureAssets.Extra[178];
            float opacity = LumUtils.Saturate(Projectile.localAI[2] / 60f);
            
            Main.EntitySpriteDraw(line.Value, Projectile.Center + Vector2.UnitY * 8 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Color.OrangeRed * opacity, dir.ToRotation(), new Vector2(0, line.Height() * 0.5f), new Vector2(1.8f, Projectile.scale * 18), SpriteEffects.None);

            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCsAndTiles.Add(index);
        }
    }
}