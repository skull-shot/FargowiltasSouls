using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.PlayerDrawLayers
{
    public class SoulsWingLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(Terraria.DataStructures.PlayerDrawLayers.Wings);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead || (drawInfo.drawPlayer.wings != EquipLoader.GetEquipSlot(Mod, "FlightMasterySoul", EquipType.Wings) && drawInfo.drawPlayer.wings != EquipLoader.GetEquipSlot(Mod, "DimensionSoul", EquipType.Wings) && drawInfo.drawPlayer.wings != EquipLoader.GetEquipSlot(Mod, "EternitySoul", EquipType.Wings)))
                return false;

            if (drawInfo.shadow != 0)
                return false;

            return true;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Vector2 drawPosition = drawInfo.Position - Main.screenPosition + new Vector2(drawInfo.drawPlayer.width / 2,
                drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height / 2) + Vector2.UnitY * 7f;

            drawPosition += new Vector2(-9, -7) * drawInfo.drawPlayer.Directions;

            Texture2D wingTexture = FargoAssets.GetTexture2D("Content/Items/Accessories/Souls", drawInfo.drawPlayer.wings == EquipLoader.GetEquipSlot(Mod, "EternitySoul", EquipType.Wings)? "EternitySoulWing" : "FlightMasterySoulWing").Value;

            int frameCount = 6;
            Rectangle frame = new(0, wingTexture.Height / frameCount * drawInfo.drawPlayer.wingFrame, wingTexture.Width, wingTexture.Height / frameCount);
            DrawData data = new(wingTexture, drawPosition.Floor(), frame, drawInfo.colorArmorBody, drawInfo.drawPlayer.bodyRotation, frame.Size() * 0.5f, 1f, drawInfo.playerEffect)
            {
                shader = drawInfo.cWings
            };
            drawInfo.DrawDataCache.Add(data);

            if (drawInfo.drawPlayer.wings == EquipLoader.GetEquipSlot(Mod, "EternitySoul", EquipType.Wings))
            {
                Texture2D GlowTexture = FargoAssets.GetTexture2D("Content/Items/Accessories/Souls", "EternitySoulWingGlow").Value;
                DrawData glowData = new(GlowTexture, drawPosition.Floor(), frame, drawInfo.colorArmorBody, drawInfo.drawPlayer.bodyRotation, frame.Size() * 0.5f, 1f, drawInfo.playerEffect);
                drawInfo.DrawDataCache.Add(glowData);
            }
        }
    }
}
