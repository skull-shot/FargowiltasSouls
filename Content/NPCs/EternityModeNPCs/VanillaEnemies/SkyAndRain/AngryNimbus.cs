using FargowiltasSouls.Content.Projectiles.Eternity.Bosses;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.SkyAndRain
{
    public class AngryNimbus : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.AngryNimbus);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);
            if (npc.HasPlayerTarget)
            {
                if (++Counter >= 360)
                {
                    Counter = 0;
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 vel = 10 * npc.SafeDirectionTo(Main.player[npc.target].Center);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), new Vector2(npc.Center.X + 100, npc.Center.Y), Vector2.Zero, ModContent.ProjectileType<LightningVortexHostile>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 0.8f), 1, Main.myPlayer);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), new Vector2(npc.Center.X - 100, npc.Center.Y), Vector2.Zero, ModContent.ProjectileType<LightningVortexHostile>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 0.8f), 1, Main.myPlayer);
                    }
                }
            }
        }
    }
}
