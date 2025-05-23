using Fargowiltas.Common.Systems;
using FargowiltasSouls.Common.Graphics.Drawers;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Armor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.PlayerDrawLayers
{
    public class MutantMaskDrawLayer : PlayerDrawLayer
    {
        public int maxFrames = 4;
        public int frame = 1;
        public int framecounter;

        public override bool IsHeadLayer => true;       

        public override Position GetDefaultPosition() => new AfterParent(Terraria.DataStructures.PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (player.dead || drawInfo.shadow != 0)
                return false;
            
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.armor[0].type != ModContent.ItemType<MutantMask>())
                return false;
            return true;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            
            Player drawPlayer = drawInfo.drawPlayer;

            //yes, all of that is really neccessary to emulate head bobbing.
            Vector2 offset = (drawPlayer.gravDir > 0 ? (drawInfo.drawPlayer.direction == 1 ? new Vector2(-12, -16) : new Vector2(-8, -16)) : (drawInfo.drawPlayer.direction == 1 ? new Vector2(-12, 8) : new Vector2(-8, 8))) + drawPlayer.headPosition + drawInfo.headVect + Main.OffsetsPlayerHeadgear[drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height] * drawPlayer.gravDir;
            Vector2 drawPosition = drawInfo.Position - Main.screenPosition;
            drawPosition += offset;
            drawPosition.Y += drawInfo.drawPlayer.gfxOffY;
            drawPosition = new Vector2((int)drawPosition.X, (int)drawPosition.Y);

            Texture2D FlameTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Items/Armor/MutantMaskFlame", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            if (++framecounter >= 8)
            {
                framecounter = 0;
                frame++;
                if (frame >= maxFrames)
                    frame = 1;
            }
            Rectangle flameframe = new(0, (FlameTexture.Height / maxFrames) * frame, FlameTexture.Width, FlameTexture.Height / maxFrames);
            DrawData data = new(FlameTexture, drawPosition.Floor(), flameframe, drawInfo.colorArmorBody, drawInfo.drawPlayer.bodyRotation, flameframe.Size() * 0.5f, 1.2f, drawInfo.playerEffect);
            drawInfo.DrawDataCache.Add(data);

        }
    }
}
