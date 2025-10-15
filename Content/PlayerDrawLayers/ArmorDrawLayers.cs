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
using Fargowiltas.Common.Systems;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FargowiltasSouls.Content.PlayerDrawLayers
{
    #region Styx Armor
    public class StyxHelmetGlow : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(Terraria.DataStructures.PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0)
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
    #endregion
    #region EridanusArmor
    public class EridanusHelmetGlow : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(Terraria.DataStructures.PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0)
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
        public override bool IsHeadLayer => false;
        public override Position GetDefaultPosition() => new Between(Terraria.DataStructures.PlayerDrawLayers.Head, Terraria.DataStructures.PlayerDrawLayers.Shield);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0)
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
        public override bool IsHeadLayer => false;
        public override Position GetDefaultPosition() => new Between(Terraria.DataStructures.PlayerDrawLayers.Head, Terraria.DataStructures.PlayerDrawLayers.ArmOverItem);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0)
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
    #endregion

    #region Nekomi Armor
    public class NekomiTail : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(Terraria.DataStructures.PlayerDrawLayers.Torso);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0)
                return false;

            if (player.body != EquipLoader.GetEquipSlot(Mod, "NekomiHoodie", EquipType.Body))
                return false;

            return true;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            FargoSoulsPlayer fargoplayer = drawInfo.drawPlayer.FargoSouls();

            Vector2 shoulderPosition = new Vector2(player.direction == 1 ? -12 : 12, player.gravDir == -1 ? 0 : -4) + Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
            Texture2D texture = FargoAssets.GetTexture2D("Content/Items/Armor/Nekomi", "NekomiTail").Value;
            Rectangle rect = new((texture.Width / 4) * fargoplayer.NekomiFrame, (texture.Height / 2) * (player.velocity.X != 0 ? 0 : 1), texture.Width / 4, texture.Height / 2);

            if (!player.isDisplayDollOrInanimate)
            {
                if (++fargoplayer.NekomiFrameCounter >= 5)
                {
                    fargoplayer.NekomiFrame++;
                    fargoplayer.NekomiFrameCounter = 0;
                    if (fargoplayer.NekomiFrame >= 4)
                    {
                        fargoplayer.NekomiFrame = 0;
                    }
                }
            }
            else
                fargoplayer.NekomiFrame = 0;

            DrawData shoulderitem = new DrawData(texture,
                new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2) + shoulderPosition,
                new Rectangle?(rect), 
                drawInfo.colorArmorBody, 
                drawInfo.drawPlayer.bodyRotation, 
                (rect.Size() / 2), 
                1f, 
                drawInfo.playerEffect);
            shoulderitem.shader = drawInfo.cBody;
            drawInfo.DrawDataCache.Add(shoulderitem);
        }
    }
    #endregion

    #region Eternal Armor
    public class EternalFlameDrawLayer : PlayerDrawLayer
    {
        public int maxFrames = 4;
        public override Position GetDefaultPosition() => new AfterParent(Terraria.DataStructures.PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0)
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
                new Vector2(drawPlayer.direction == -1 ? 2 : -2, drawPlayer.gravDir == 1 ? -6 : 16) +
                Main.OffsetsPlayerHeadgear[drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height] * drawPlayer.gravDir;

            Texture2D FlameTexture = FargoAssets.GetTexture2D("Content/Items/Armor/Eternal", "EternalFlame").Value;

            FargoSoulsPlayer fargoplayer = drawInfo.drawPlayer.FargoSouls();
            if (++fargoplayer.EternalFrameCounter >= 8)
            {
                fargoplayer.EternalFrameCounter = 0;
                fargoplayer.EternalFrame++;
                if (fargoplayer.EternalFrame >= maxFrames)
                    fargoplayer.EternalFrame = 0;
            }
            Rectangle flameframe = new(0, (FlameTexture.Height / maxFrames) * fargoplayer.EternalFrame, FlameTexture.Width, FlameTexture.Height / maxFrames);

            DrawData item = new DrawData(FlameTexture, Position, flameframe, Color.White, drawInfo.drawPlayer.headRotation, flameframe.Size() / 2f, 1.2f, drawInfo.playerEffect);
            item.shader = drawInfo.cHead;
            drawInfo.DrawDataCache.Add(item);
            return;
        }
    }

    public class EternalArmorDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) =>
            drawInfo.drawPlayer.active
            && !drawInfo.drawPlayer.dead
            && !drawInfo.drawPlayer.ghost
            && drawInfo.shadow == 0
            && drawInfo.drawPlayer.FargoSouls().MutantSetBonusItem != null;

        public override Position GetDefaultPosition() => new Between();

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
            {
                return;
            }

            Player drawPlayer = drawInfo.drawPlayer;
            FargoSoulsPlayer modPlayer = drawPlayer.FargoSouls();

            if (modPlayer.MutantSetBonusItem != null)
            {
                if (modPlayer.frameCounter % 4 == 0)
                {
                    if (++modPlayer.frameMutantAura >= 19)
                        modPlayer.frameMutantAura = 0;
                }

                Texture2D texture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/MutantBoss/MutantAura", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                int frameSize = texture.Height / 19;
                int drawX = (int)(drawPlayer.MountedCenter.X - Main.screenPosition.X);
                int drawY = (int)(drawPlayer.MountedCenter.Y - Main.screenPosition.Y - 16 * drawPlayer.gravDir);
                DrawData data = new(texture, new Vector2(drawX, drawY), new Rectangle(0, frameSize * modPlayer.frameMutantAura, texture.Width, frameSize), Color.White, drawPlayer.gravDir < 0 ? MathHelper.Pi : 0, new Vector2(texture.Width / 2f, frameSize / 2f), 1f, drawPlayer.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                drawInfo.DrawDataCache.Add(data);
            }
        }
    }
    #endregion
}
