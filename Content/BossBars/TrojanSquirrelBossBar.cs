using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace FargowiltasSouls.Content.BossBars
{
    public class TrojanSquirrelBossBar : ModBossBar
    {
        private int bossHeadIndex = -1;

        public Asset<Texture2D> Frame;

        public Asset<Texture2D> Bars;

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            if (bossHeadIndex != -1)
                return TextureAssets.NpcHeadBoss[bossHeadIndex];

            return base.GetIconTexture(ref iconFrame);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {   
            //bwaaaa

            float lifeRatio = drawParams.Life / drawParams.LifeMax;

            Frame = ModContent.Request<Texture2D>("FargowiltasSouls/Content/BossBars/TrojanSquirrelFrame", AssetRequestMode.ImmediateLoad);
            Bars = ModContent.Request<Texture2D>("FargowiltasSouls/Content/BossBars/TrojanSquirrelBars", AssetRequestMode.ImmediateLoad);

            Rectangle fillingRect = new(0, Bars.Height() / 2, (int)(Bars.Width() * lifeRatio), Bars.Height() / 2);
            Rectangle backingRect = new(0, (Bars.Height() / 2) * 0, Bars.Width(), Bars.Height() / 2);
            Rectangle frameRect = new(0, 0, Frame.Width(), Frame.Height());

            spriteBatch.Draw(Bars.Value, drawParams.BarCenter + new Vector2(-4, 4), new Rectangle?(backingRect), Color.White, 0, backingRect.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(Bars.Value, drawParams.BarCenter + new Vector2(26, 15), new Rectangle?(fillingRect), Color.White, 0, frameRect.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(Frame.Value, drawParams.BarCenter - new Vector2(6, 0), new Rectangle?(frameRect), Color.White, 0, frameRect.Size() / 2, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(drawParams.IconTexture, drawParams.BarCenter + new Vector2(-260, -10), drawParams.IconFrame, Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0);
            Utils.DrawBorderString(spriteBatch, drawParams.Life + " / " + drawParams.LifeMax, drawParams.BarCenter + new Vector2(-54, -6), Color.White, 1f);
            return false;
        }
        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)/* tModPorter Note: life and shield current and max values are now separate to allow for hp/shield number text draw */
        {
            NPC npc = FargoSoulsUtil.NPCExists(info.npcIndexToAimAt);

            if (npc == null || !npc.active)
                return false;

            bossHeadIndex = npc.GetBossHeadTextureIndex();

            if (npc.ModNPC is TrojanSquirrel trojanSquirrel)
            {
                float lifeSegments = 0;
                if (trojanSquirrel.head != null)
                    lifeSegments += trojanSquirrel.head.life;
                if (trojanSquirrel.arms != null)
                    lifeSegments += trojanSquirrel.arms.life;
                life = npc.life + lifeSegments;
                lifeMax = npc.lifeMax + trojanSquirrel.lifeMaxHead + trojanSquirrel.lifeMaxArms;
            }

            return true;
        }
    }
}
