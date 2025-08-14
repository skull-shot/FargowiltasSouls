using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class BrokenShellBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            float divisor = player.ForceEffect<TurtleEffect>() ? 1800f : 1200f;
            player.FargoSouls().TurtleShellHP += TurtleEffect.TurtleShellMaxHP/divisor; //make bar slowly go up during debuff

            if (player.HasEffect<TurtleEffect>())
            {
                player.FargoSouls().AttackSpeed += 0.2f;
                if (!player.FargoSouls().NoMomentum || !player.mount.Active)
                {
                    player.runAcceleration *= 1.8f;
                    player.runSlowdown *= 1.6f;
                    player.maxRunSpeed *= 1.4f; // all of these affect aerial speed
                }
            }
        }
    }
}