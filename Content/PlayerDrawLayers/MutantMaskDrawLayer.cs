using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Armor.Eternal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.PlayerDrawLayers
{
    public class MutantMaskDrawLayer : PlayerDrawLayer
    {
        public int maxFrames = 4;
        public int frame = 0;
        public int framecounter;    

        public override Position GetDefaultPosition() => new AfterParent(Terraria.DataStructures.PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0)
                return false;

            if (player.armor[0].type != ModContent.ItemType<EternalFlame>() && player.armor[10].type != ModContent.ItemType<EternalFlame>())
                return false;

            if (player.head != EquipLoader.GetEquipSlot(Mod, "EternalFlame", EquipType.Head))
                return false;

            return true;
        }

        public override bool IsHeadLayer => true;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {        
            Player drawPlayer = drawInfo.drawPlayer;

            Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;

            Vector2 Position = drawInfo.helmetOffset +
                new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) +
                (float)(drawInfo.drawPlayer.width / 2)),
                (int)(drawInfo.Position.Y - Main.screenPosition.Y +
                (float)drawInfo.drawPlayer.height -
                (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) +
                drawInfo.drawPlayer.headPosition +
                drawInfo.headVect +
                new Vector2(drawPlayer.direction == -1 ? 2 : -2, drawPlayer.gravDir == 1? - 6 : 16) +
                Main.OffsetsPlayerHeadgear[drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height] * drawPlayer.gravDir;

            Texture2D FlameTexture = FargoAssets.GetTexture2D("Content/Items/Armor/Eternal", "EternalFlame").Value;

            if (++framecounter >= 8)
            {
                framecounter = 0;
                frame++;
                if (frame >= maxFrames)
                    frame = 0;
            }
            Rectangle flameframe = new(0, (FlameTexture.Height / maxFrames) * frame, FlameTexture.Width, FlameTexture.Height / maxFrames);

            DrawData item = new DrawData(FlameTexture, Position, flameframe, Color.White, drawInfo.drawPlayer.headRotation, flameframe.Size() / 2f, 1.2f, drawInfo.playerEffect);
            item.shader = drawInfo.cHead;
            drawInfo.DrawDataCache.Add(item);
            return;
        }
    }
}
