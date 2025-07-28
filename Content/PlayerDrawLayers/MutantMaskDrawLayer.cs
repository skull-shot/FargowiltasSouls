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

            if (player.head != EquipLoader.GetEquipSlot(Mod, "MutantMask", EquipType.Head))
                return false;

            return true;
        }

        public override bool IsHeadLayer => true;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {        
            Player drawPlayer = drawInfo.drawPlayer;

            //yes, all of that is really neccessary to emulate head bobbing.
            Vector2 offset = (drawPlayer.gravDir > 0 ? (drawInfo.drawPlayer.direction == 1 ? new Vector2(-12, -16) : new Vector2(-8, -16)) : (drawInfo.drawPlayer.direction == 1 ? new Vector2(-12, 8) : new Vector2(-8, 8))) + drawPlayer.headPosition + drawInfo.headVect + Main.OffsetsPlayerHeadgear[drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height] * drawPlayer.gravDir;
            Vector2 drawPosition = drawInfo.Position - Main.screenPosition;
            drawPosition += offset.Floor();


            Texture2D FlameTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Items/Armor/Eternal/MutantMask", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            if (++framecounter >= 8)
            {
                framecounter = 0;
                frame++;
                if (frame >= maxFrames)
                    frame = 0;
            }
            Rectangle flameframe = new(0, (FlameTexture.Height / maxFrames) * frame, FlameTexture.Width, FlameTexture.Height / maxFrames);
            DrawData data = new(FlameTexture, drawPosition.Floor(), flameframe, drawInfo.colorArmorBody, drawInfo.drawPlayer.bodyRotation, flameframe.Size() * 0.5f, 1.2f, drawInfo.playerEffect, 0f);
            data.shader = drawInfo.cHead;
            drawInfo.DrawDataCache.Add(data);

        }
    }
}
