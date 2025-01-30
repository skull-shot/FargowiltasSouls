using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.ModPlayers
{
    public class BoosterPlayer : ModPlayer
    {
        public int SolarTimer = 0;
        public int VortexTimer = 0;
        public int NebulaTimer = 0;
        public int StardustTimer = 0;

        public int TimberTimer = 0;
        public int TerraTimer = 0;
        public int EarthTimer = 0;
        public int NatureTimer = 0;
        public int LifeTimer = 0;
        public int DeathTimer = 0;
        public int SpiritTimer = 0;
        public int WillTimer = 0;
        public int CosmosTimer = 0;
        public override void PostUpdateEquips()
        {
            bool hasNebulaDmg = Player.HasBuff(BuffID.NebulaUpDmg1) || Player.HasBuff(BuffID.NebulaUpDmg2) || Player.HasBuff(BuffID.NebulaUpDmg3);
            //if (VortexTimer > 0 && hasNebulaDmg)
            //    VortexTimer = 0;
            if (StardustTimer > 0 && hasNebulaDmg)
                StardustTimer = 0;
            if (NebulaTimer > 0 && (Player.HasBuff(BuffID.NebulaUpLife1) || Player.HasBuff(BuffID.NebulaUpLife2) || Player.HasBuff(BuffID.NebulaUpLife3)))
                NebulaTimer = 0;

            if (SolarTimer > 0)
            {
                SolarTimer--;
                Player.endurance += 0.15f;
            }
            if (VortexTimer > 0)
            {
                VortexTimer--;
                Player.GetCritChance(DamageClass.Generic) += 25;
            }
                
            if (NebulaTimer > 0)
            {
                NebulaTimer--;
            }
            if (StardustTimer > 0)
            {
                StardustTimer--;
                Player.GetDamage(DamageClass.Generic) += 0.25f;
            }

            if (TimberTimer > 0)
            {
                TimberTimer--;
                Player.GetDamage(DamageClass.Generic) += 0.3f;
            }
            if (TerraTimer > 0)
            {
                TerraTimer--;
                Player.GetCritChance(DamageClass.Generic) += 30;
            }
            if (EarthTimer > 0)
            {
                EarthTimer--;
                Player.endurance += 0.25f;
            }
            if (NatureTimer > 0)
            {
                NatureTimer--;
                Player.statDefense += 20;
            }
            if (LifeTimer > 0)
            {
                LifeTimer--;
            }
            if (DeathTimer > 0)
            {
                DeathTimer--;
                Player.GetArmorPenetration(DamageClass.Generic) += 50;
            }
            if (SpiritTimer > 0)
            {
                SpiritTimer--;
                int num = 6;
                Player.nebulaManaCounter += 3;
                if (Player.nebulaManaCounter >= num)
                {
                    Player.nebulaManaCounter -= num;
                    Player.statMana++;
                    if (Player.statMana >= Player.statManaMax2)
                    {
                        Player.statMana = Player.statManaMax2;
                    }
                }
            }
            if (WillTimer > 0)
            {
                WillTimer--;
            }
            if (CosmosTimer > 0)
            {
                CosmosTimer--;
            }
            /* if (SolarTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.Yellow, Language.GetTextValue("Mods.FargowiltasSouls.Items.SolarBooster.Deactivate", 15), true);
            if (VortexTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.LightCyan, Language.GetTextValue("Mods.FargowiltasSouls.Items.VortexBooster.Deactivate", 25), true);
            if (NebulaTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.Magenta, Language.GetTextValue("Mods.FargowiltasSouls.Items.NebulaBooster.Deactivate", 5), true);
            if (StardustTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.Cyan, Language.GetTextValue("Mods.FargowiltasSouls.Items.StardustBooster.Deactivate", 25), true); */

            if (TimberTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.SandyBrown, Language.GetTextValue("Mods.FargowiltasSouls.Items.TimberBooster.Deactivate", 30), true);
            if (TerraTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.Gray, Language.GetTextValue("Mods.FargowiltasSouls.Items.TerraBooster.Deactivate", 30), true);
            if (EarthTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.Cyan, Language.GetTextValue("Mods.FargowiltasSouls.Items.EarthBooster.Deactivate", 25), true);
            if (NatureTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.LightGreen, Language.GetTextValue("Mods.FargowiltasSouls.Items.NatureBooster.Deactivate", 20), true);
            if (LifeTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.Pink, Language.GetTextValue("Mods.FargowiltasSouls.Items.LifeBooster.Deactivate", 8), true);
            if (DeathTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.Purple, Language.GetTextValue("Mods.FargowiltasSouls.Items.DeathBooster.Deactivate", 50), true);
            if (SpiritTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.LightBlue, Language.GetTextValue("Mods.FargowiltasSouls.Items.SpiritBooster.Deactivate", 30), true);
            if (WillTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.Gold, Language.GetTextValue("Mods.FargowiltasSouls.Items.WillBooster.Deactivate", 25), true);
            if (CosmosTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.Black, Language.GetTextValue("Mods.FargowiltasSouls.Items.CosmosBooster.Deactivate", 10), true);
        }
        public override void UpdateLifeRegen()
        {
            if (NebulaTimer > 0)
                Player.lifeRegen += 10;
            if (LifeTimer > 0)
                Player.lifeRegen += 16;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (WillTimer > 0)
                modifiers.FinalDamage.Base += 25;
            if (CosmosTimer > 0)
                modifiers.CritDamage += 0.1f;
            base.ModifyHitNPC(target, ref modifiers);
        }
    }
}
