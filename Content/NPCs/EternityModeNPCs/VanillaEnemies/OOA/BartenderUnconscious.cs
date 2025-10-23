using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
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
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    internal class BartenderUnconscious : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BartenderUnconscious);

        public int Timer;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            Timer = 300;
            base.OnSpawn(npc, source);
        }

        public override bool SafePreAI(NPC npc)
        {
            Timer--;
            npc.AddBuff(BuffID.OgreSpit, 2);
            int rate = (int)Math.Floor(300f - Timer);
            if (Timer % rate == 0)
                Gore.NewGore(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, GoreID.OgreSpit1);
            return base.SafePreAI(npc);
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D text = TextureAssets.Npc[npc.type].Value;
            Rectangle frame = text.Frame();
            Vector2 origin2 = frame.Size() / 2;
            SpriteEffects flip = npc.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color = Color.Lerp(Color.Green, drawColor, (400 - Timer) / 300f);
            if (Timer < 100)
                color = drawColor;

            Main.EntitySpriteDraw(text, npc.Center - Main.screenPosition, frame, color, 0, origin2, npc.scale, flip);
            return false;
        }
    }
}
