using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Custom.OOA;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.CustomEnemies.OOA
{
    public class DD2Shielder : DD2Enemy
    {
        public override string Texture => "Fargowiltas/Content/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
            NPC.lifeMax = 180;
            NPC.defense = 5;
            NPC.damage = 30;
            NPC.knockBackResist = 0f;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.HitSound = SoundID.NPCHit1;
        }

        public ref float Timer => ref NPC.ai[0];
        public ref float Shield => ref NPC.ai[1];

        public override void AI()
        {
            if (Timer == 0)
            {
                Shield = -1;
            }
            Timer++;

            int n = NPC.FindFirstNPC(NPCID.DD2EterniaCrystal);
            if (n == -1)
            {
                NPC.life = 0;
                NPC.checkDead();
                return;
            }

            NPC crystal = Main.npc[n];
            float dir = NPC.HorizontalDirectionTo(crystal.Center);
            NPC.direction = (int)dir;

            NPC.velocity = new Vector2 (dir, NPC.velocity.Y);
            if (Timer < 40)
                return;

            if (Shield == -1)
            {
                Shield = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, dir * Vector2.UnitX, ModContent.ProjectileType<OOAForcefield>(), NPC.damage, 0f);
            }
            else
            {
                NPC.velocity *= 0.5f;
            }
            if (NPC.Center.Distance(crystal.Center) <= 400f)
            {
                NPC.velocity *= 0.1f;
            }
        }

        public override void OnKill()
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.Shadowflame);
            }
            base.OnKill();
        }
    }
}
