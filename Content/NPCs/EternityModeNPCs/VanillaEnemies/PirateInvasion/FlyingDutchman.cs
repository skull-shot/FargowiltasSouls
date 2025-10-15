﻿using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.PirateInvasion
{
    public class FlyingDutchman : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.PirateShip);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.noTileCollide = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.HasValidTarget)
            {
                if (npc.velocity.Y < 0f && npc.position.Y + npc.height < Main.player[npc.target].position.Y)
                    npc.velocity.Y = 0f;
            }
        }
    }

    public class FlyingDutchmanCannon : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.PirateShipCannon);

        public int PhaseTimer;
        public int Gun;
        public bool AttackFlag;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            Gun = Main.rand.Next(10);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            const int attackTime = 360;

            if (!AttackFlag && PhaseTimer == attackTime - 90 && NPC.FindFirstNPC(npc.type) == npc.whoAmI)
            {
                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<TargetingReticle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer, npc.whoAmI);
            }

            if (++PhaseTimer > attackTime)
            {
                AttackFlag = !AttackFlag;
                PhaseTimer = AttackFlag ? attackTime / 2 : 0;

                NetSync(npc);
            }

            if (AttackFlag && ++Gun > 10)
            {
                Gun = -Main.rand.Next(5);
                if (npc.HasPlayerTarget)
                {
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.X += Main.rand.Next(-40, 41);
                    speed.Y += Main.rand.Next(-40, 41);
                    speed.Normalize();
                    speed *= 14f;
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ProjectileID.BulletDeadeye /*ModContent.ProjectileType<PirateDeadeyeBullet>()*/, 15, 0f, Main.myPlayer);

                    SoundEngine.PlaySound(SoundID.Item11, npc.Center);
                }
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<SecurityWallet>(), 5));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.CoinGun, 50));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.LuckyCoin, 50));
        }
    }
}
