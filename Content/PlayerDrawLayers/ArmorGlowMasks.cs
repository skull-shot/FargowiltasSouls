using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Armor.Eternal;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Armor.Styx;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;

namespace FargowiltasSouls.Content.PlayerDrawLayers
{
    public class StyxHelmetGlow : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(Terraria.DataStructures.PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0)
                return false;

            if (player.armor[0].type != ModContent.ItemType<StyxCrown>() && player.armor[10].type != ModContent.ItemType<StyxCrown>())
                return false;

            if (player.head != EquipLoader.GetEquipSlot(Mod, "StyxCrown", EquipType.Head))
                return false;

            return true;
        }

        public override bool IsHeadLayer => true;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
            //bodyFrame.Width += 2;

            DrawData item = new DrawData(FargoAssets.GetTexture2D("Content/Items/Armor/Styx", "StyxCrown_HeadGlow").Value, drawInfo.helmetOffset + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.headPosition + drawInfo.headVect, bodyFrame, Color.White, drawInfo.drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect);
            item.shader = drawInfo.cHead;
            drawInfo.DrawDataCache.Add(item);
        }
    }
    public class StyxLeggingsGlow : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(Terraria.DataStructures.PlayerDrawLayers.Leggings);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0)
                return false;

            if (player.armor[2].type != ModContent.ItemType<StyxLeggings>() && player.armor[12].type != ModContent.ItemType<StyxLeggings>())
                return false;

            if (player.legs != EquipLoader.GetEquipSlot(Mod, "StyxLeggings", EquipType.Legs))
                return false;

            return true;
        }

        public override bool IsHeadLayer => false;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            DrawData item = new DrawData(position: drawInfo.legsOffset + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.legFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.legFrame.Height + 4f)) + drawInfo.drawPlayer.legPosition + drawInfo.legVect, texture: FargoAssets.GetTexture2D("Content/Items/Armor/Styx", "StyxLeggings_LeggingsGlow").Value, sourceRect: drawInfo.drawPlayer.legFrame, color: Color.White, rotation: drawInfo.drawPlayer.legRotation, origin: drawInfo.legVect, scale: 1f, effect: drawInfo.playerEffect);
            item.shader = drawInfo.cLegs;
            drawInfo.DrawDataCache.Add(item);
        }
    }

}
