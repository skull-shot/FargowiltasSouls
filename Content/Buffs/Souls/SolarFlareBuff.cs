using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Projectiles;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class SolarFlareBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Souls", Name);
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().SolarFlare = true;

            if (npc.buffTime[buffIndex] < 2)
            {
                int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(), 1000, 0f, Main.myPlayer, npc.whoAmI);
                if (p != Main.maxProjectiles)
                    Main.projectile[p].FargoSouls().CanSplit = false;
            }
            else
            {
                float rotation = Main.rand.NextFloat(0, 2 * MathHelper.Pi);
                Particle spark = new SparkParticle(npc.Center, (8f * Vector2.UnitX).RotatedBy(rotation), Color.OrangeRed, 0.15f, 5);
                spark.Spawn();
            }
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            return true;
        }
    }
}