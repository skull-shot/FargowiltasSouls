using FargowiltasSouls.Content.Projectiles.Minions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Tiger
{
    public abstract class TouhouMinionBase : ModProjectile
    {
        public abstract int frameCount { get;}
        public abstract int attackSpeed { get;}

        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.netImportant = true;
            Projectile.scale = .9f;
        }

        public void CheckActive(Player player)
        {
            PatreonPlayer modPlayer = player.GetModPlayer<PatreonPlayer>();
            if (player.dead) modPlayer.TouhouBuff = false;
            if (modPlayer.TouhouBuff) Projectile.timeLeft = 2;
        }

        public void SelectFrame()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % frameCount;
            }
        }

        public abstract void MinionAttack(Vector2 target);

        private bool spawn;

        public override void AI()
        {
            if (!spawn)
            {
                spawn = true;
                Projectile.ai[0] = -1;
            }

            Player player = Main.player[Projectile.owner];

            CheckActive(player);

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs) //has target
            {
                NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
                if (minionAttackTargetNpc != null && Projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(Projectile))
                    Projectile.ai[0] = minionAttackTargetNpc.whoAmI;

                NPC npc = Main.npc[(int)Projectile.ai[0]];

                if (npc.CanBeChasedBy(Projectile))
                {
                    Projectile.position += npc.velocity / 4f;

                    Vector2 target = npc.Center + npc.velocity * Projectile.ai[1];
                    Vector2 targetPos = target + Projectile.DirectionFrom(target) * 300;

                    if (Projectile.Distance(targetPos) > 50)
                        Movement(targetPos, 0.5f);

                    if (++Projectile.localAI[0] > attackSpeed)
                    {
                        Projectile.localAI[0] = 0;

                        if (Projectile.owner == Main.myPlayer)
                        {
                            //face target
                            Projectile.spriteDirection = (target.X - Projectile.Center.X) > 0 ? 1 : -1;

                            //Main.NewText(Projectile.spriteDirection);

                            MinionAttack(target);
                        }
                    }
                }
                else //forget target
                {
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1500);
                    Projectile.netUpdate = true;
                }
            }
            else //no target
            {
                Projectile.ai[1] = 0;

                Vector2 targetPos = player.Center;
                targetPos.Y -= 100;

                if (Projectile.Distance(targetPos) > 3000)
                    Projectile.Center = player.Center;
                else if (Projectile.Distance(targetPos) > 200)
                    Movement(targetPos, 0.5f);

                if (++Projectile.localAI[1] > 6)
                {
                    Projectile.localAI[1] = 0;
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1500);
                    if (Projectile.ai[0] != -1)
                        Projectile.netUpdate = true;
                }
            }

            SelectFrame();

            const float IdleAccel = 0.05f;
            int otherMinion = ModContent.ProjectileType<OpticSpazmatism>();
            foreach (Projectile p in Main.projectile.Where(p => p.active && p.owner == Projectile.owner && (p.type == Projectile.type || p.type == otherMinion) && p.whoAmI != Projectile.whoAmI && p.Distance(Projectile.Center) < Projectile.width))
            {
                Projectile.velocity.X += IdleAccel * (Projectile.Center.X < p.Center.X ? -1 : 1);
                Projectile.velocity.Y += IdleAccel * (Projectile.Center.Y < p.Center.Y ? -1 : 1);
                p.velocity.X += IdleAccel * (p.Center.X < Projectile.Center.X ? -1 : 1);
                p.velocity.Y += IdleAccel * (p.Center.Y < Projectile.Center.Y ? -1 : 1);
            }
        }

        private void Movement(Vector2 targetPos, float speedModifier)
        {
            if (Projectile.Center.X < targetPos.X)
            {
                Projectile.velocity.X += speedModifier;
                if (Projectile.velocity.X < 0)
                    Projectile.velocity.X += speedModifier * 2;
            }
            else
            {
                Projectile.velocity.X -= speedModifier;
                if (Projectile.velocity.X > 0)
                    Projectile.velocity.X -= speedModifier * 2;
            }
            if (Projectile.Center.Y < targetPos.Y)
            {
                Projectile.velocity.Y += speedModifier;
                if (Projectile.velocity.Y < 0)
                    Projectile.velocity.Y += speedModifier * 2;
            }
            else
            {
                Projectile.velocity.Y -= speedModifier;
                if (Projectile.velocity.Y > 0)
                    Projectile.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(Projectile.velocity.X) > 24)
                Projectile.velocity.X = 24 * Math.Sign(Projectile.velocity.X);
            if (Math.Abs(Projectile.velocity.Y) > 24)
                Projectile.velocity.Y = 24 * Math.Sign(Projectile.velocity.Y);
        }



    }
}
