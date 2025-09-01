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
using FargowiltasSouls.Content.Items.Armor.Eridanus;

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

    public class EridanusHelmetGlow : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(Terraria.DataStructures.PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0)
                return false;

            if (player.armor[0].type != ModContent.ItemType<EridanusHat>() && player.armor[10].type != ModContent.ItemType<EridanusHat>())
                return false;

            if (player.head != EquipLoader.GetEquipSlot(Mod, "EridanusHat", EquipType.Head))
                return false;

            return true;
        }

        public override bool IsHeadLayer => true;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
            //bodyFrame.Width += 2;

            DrawData item = new DrawData(FargoAssets.GetTexture2D("Content/Items/Armor/Eridanus", "EridanusHat_HeadGlow").Value, drawInfo.helmetOffset + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.headPosition + drawInfo.headVect, bodyFrame, Color.White, drawInfo.drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect);
            item.shader = drawInfo.cHead;
            drawInfo.DrawDataCache.Add(item);
        }
    }

    public class EridanusShoulderBack : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(Terraria.DataStructures.PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0)
                return false;
            
            if (player.armor[1].type != ModContent.ItemType<EridanusBattleplate>() && player.armor[11].type != ModContent.ItemType<EridanusBattleplate>())
                return false;

            if (player.body != EquipLoader.GetEquipSlot(Mod, "EridanusBattleplate", EquipType.Body))
                return false;

            return true;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Vector2 shoulderPosition = new Vector2(player.direction == 1? -6 : 8, player.gravDir == -1? 22 : 6) + Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
            Texture2D texture = FargoAssets.GetTexture2D("Content/Items/Armor/Eridanus", "EridanusBattleplateShouldersBack").Value;
            DrawData shoulderitem = new DrawData(texture, new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2) + shoulderPosition, null, Color.White, drawInfo.drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
            shoulderitem.shader = drawInfo.cBody;
            drawInfo.DrawDataCache.Add(shoulderitem);
        }
    }
    public class EridanusShoulderFront : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(Terraria.DataStructures.PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0)
                return false;

            if (player.armor[1].type != ModContent.ItemType<EridanusBattleplate>() && player.armor[11].type != ModContent.ItemType<EridanusBattleplate>())
                return false;

            if (player.body != EquipLoader.GetEquipSlot(Mod, "EridanusBattleplate", EquipType.Body))
                return false;

            return true;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Vector2 shoulderPosition = new Vector2(player.direction == 1 ? -6 : 8, player.gravDir == -1 ? 22 : 6) + Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
            Texture2D texture = FargoAssets.GetTexture2D("Content/Items/Armor/Eridanus", "EridanusBattleplateShouldersFront").Value;
            DrawData shoulderitem = new DrawData(texture, new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2) + shoulderPosition, null, Color.White, drawInfo.drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
            shoulderitem.shader = drawInfo.cBody;
            drawInfo.DrawDataCache.Add(shoulderitem);
        }
    }

}
