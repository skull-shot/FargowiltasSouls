using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Light;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Globals
{
    public class SwordPlayer : ModPlayer
    {
        public bool shouldShoot;
        public int useDirection = -1;
        public float useRotation = 0;
        public int swingDirection = 1;
        public float itemScale = 1;
        public override void PostUpdate()
        {
        }
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            //for (int i = 0; i < PlayerDrawLayerLoader.Layers.Count; i++)
            //{
            //    if (PlayerDrawLayerLoader.Layers[i] == PlayerDrawLayers.HeldItem && SwordGlobalItem.IsBroadsword(drawInfo.heldItem) && drawInfo.drawPlayer.ItemAnimationActive)
            //    {
            //        PlayerDrawLayerLoader.Layers[i].Hide();
            //    }
            //}
            if (SwordGlobalItem.IsBroadsword(drawInfo.heldItem) && drawInfo.drawPlayer.ItemAnimationActive)
            {
                PlayerDrawLayers.HeldItem.Hide();
            }

            
        }
    }
    public class SwordDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return SwordGlobalItem.IsBroadsword(drawInfo.drawPlayer.HeldItem) && drawInfo.drawPlayer.ItemAnimationActive;
        }
        public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.SolarShield, PlayerDrawLayers.HeldItem);
        
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Asset<Texture2D> t = TextureAssets.Item[drawInfo.drawPlayer.HeldItem.type];

            SwordPlayer player = drawInfo.drawPlayer.GetModPlayer<SwordPlayer>();
            Vector2 position = drawInfo.drawPlayer.itemLocation - Main.screenPosition;

            int dir = drawInfo.drawPlayer.direction;
            int swingDir = drawInfo.drawPlayer.GetModPlayer<SwordPlayer>().swingDirection;

            SpriteEffects effects = SpriteEffects.None;
            float rotationAdd = 0;
            Vector2 origin = new Vector2(2, t.Height() - 2);
            if ((dir == -1 && swingDir == 1) || (swingDir == -1 && dir == 1))
            {

                effects = SpriteEffects.FlipHorizontally;
                origin = new Vector2(t.Width() - 2, t.Height() - 2);
            }
            if (swingDir == -1)
            {
                rotationAdd += MathHelper.ToRadians(90 * dir);
                
            }

   
            drawInfo.DrawDataCache.Add(new DrawData(
                t.Value,
                position,
                null,
                Lighting.GetColor(drawInfo.drawPlayer.itemLocation.ToTileCoordinates()),
                drawInfo.drawPlayer.itemRotation + rotationAdd,
                origin,
                drawInfo.drawPlayer.HeldItem.scale + player.itemScale,
                effects,
                0
                

                )
            );
            
        }
    }
}
