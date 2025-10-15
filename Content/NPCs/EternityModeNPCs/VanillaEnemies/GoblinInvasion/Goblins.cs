using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.GoblinInvasion;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.GoblinInvasion
{
    public class Goblins : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.GoblinArcher,
            NPCID.GoblinPeon,
            NPCID.GoblinScout,
            NPCID.GoblinSorcerer,
            NPCID.GoblinSummoner,
            NPCID.GoblinThief,
            NPCID.GoblinWarrior
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (npc.type == NPCID.GoblinWarrior)
                npc.knockBackResist /= 10;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (!WorldSavingSystem.DownedAnyBoss && (npc.type == NPCID.GoblinWarrior || npc.type == NPCID.GoblinThief || npc.type == NPCID.GoblinArcher) && NPC.CountNPCS(npc.type) > 3)
            {
                npc.Transform(NPCID.GoblinPeon);
            }
        }

        //public override void AI(NPC npc)
        //{
        //    base.AI(npc);

        //    if (npc.type != NPCID.GoblinSummoner)
        //    {
        //        if (npc.HasPlayerTarget && (!Main.player[npc.target].active || Main.player[npc.target].dead))
        //        {
        //            npc.TargetClosest();
        //            if (npc.HasPlayerTarget && (!Main.player[npc.target].active || Main.player[npc.target].dead))
        //            {
        //                npc.noTileCollide = true;
        //            }
        //        }
        //        if (npc.noTileCollide) //fall through the floor
        //        {
        //            npc.position.Y++;
        //            npc.velocity.Y++;
        //        }
        //    }
        //}

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            if (Main.hardMode)
                target.AddBuff(ModContent.BuffType<ShadowflameBuff>(), 300);

            if (npc.type == NPCID.GoblinThief)
            {
                target.AddBuff(ModContent.BuffType<MidasBuff>(), 600);
            }
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (FargoSoulsUtil.HostCheck)
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, new Vector2(Main.rand.NextFloat(-2f, 2f), -5), ModContent.ProjectileType<GoblinSpikyBall>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0, Main.myPlayer);

            if (NPC.downedGoblins && !WorldSavingSystem.HaveForcedAbomFromGoblins)
            {
                WorldSavingSystem.HaveForcedAbomFromGoblins = true;

                if (ModContent.TryFind("Fargowiltas", "Abominationn", out ModNPC modNPC) && !NPC.AnyNPCs(modNPC.Type))
                {
                    int p = Player.FindClosest(npc.Center, 0, 0);
                    if (p != -1)
                        NPC.SpawnOnPlayer(p, modNPC.Type);
                }
            }
        }
    }
}
