using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    public class DeviRitual : BaseArena
    {
        public DeviRitual() : base(MathHelper.Pi / 140f, 1000f, ModContent.NPCType<DeviBoss>(), 86, 3) { }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Deviantt Seal");
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            CooldownSlot = ImmunityCooldownID.Bosses;
        }
        protected override void Movement(NPC npc)
        {
            if (npc.ai[0] <= 10)
                Projectile.Kill();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            target.AddBuff(ModContent.BuffType<LovestruckBuff>(), 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color color = Color.White;
            color.A = 0;
            float rotation = 2f * MathHelper.Pi + Projectile.ai[0];
            Main.EntitySpriteDraw(FargoAssets.DeviBorderTexture.Value, Projectile.Center - Main.screenPosition, null, color * 0.95f, rotation, FargoAssets.DeviBorderTexture.Value.Size() * 0.5f, 0.67f * Projectile.scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            return false;
        }
    }
}