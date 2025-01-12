using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.FrostMoon;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.PumpkinMoon;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.ModPlayers
{
    public partial class EModePlayer : ModPlayer
    {
        public bool ReduceMasomodeMinionNerf;
        public bool HasWhipBuff;
        public const int MaxMasomodeMinionNerfTimer = 300;
        public const int MaxShorterDebuffsTimer = 60;
        public int MasomodeCrystalTimer;
        public int MasomodeMinionNerfTimer;
        public int TorchGodTimer;
        public int ShorterDebuffsTimer;
        public int MythrilHalberdTimer;
        public int CobaltHitCounter;
        public int CrossNecklaceTimer;
        public int PalladiumHealTimer;
        private int WeaponUseTimer => Player.FargoSouls().WeaponUseTimer;
        public int Respawns;

        public bool WaterWet => Player.wet && !Player.lavaWet && !Player.honeyWet && !Player.shimmerWet && !Player.FargoSouls().MutantAntibodies;

        public override void ResetEffects()
        {
            ReduceMasomodeMinionNerf = false;
            HasWhipBuff = false;

            if (!LumUtils.AnyBosses())
                Respawns = 0;
        }

        public override void UpdateDead()
        {
            ResetEffects();

            MasomodeMinionNerfTimer = 0;
            ShorterDebuffsTimer = 0;
            if (WorldSavingSystem.MasochistModeReal && LumUtils.AnyBosses() && Respawns >= 2)
                Player.respawnTimer = 60 * 5;
        }

        public override void OnEnterWorld()
        {
            foreach (NPC npc in Main.npc.Where(npc => npc.active))
            {
                foreach (var entityGlobal in npc.EntityGlobals)
                {
                    if (entityGlobal is EModeNPCBehaviour eModeNPC)
                    {
                        eModeNPC.TryLoadSprites(npc);
                    }
                }
            }
        }
        public override void PreUpdateBuffs()
        {
            MurderGreaterDangersense();
        }
        public override void PostUpdate()
        {
            MurderGreaterDangersense();
        }
        private void MurderGreaterDangersense()//KILL alchnpc greater dangersense (when boss alive)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            if (ModLoader.TryGetMod("AlchemistNPC", out Mod alchNPC) && LumUtils.AnyBosses())
            {
                if (alchNPC.TryFind("GreaterDangersense", out ModBuff greaterDangersense))
                {
                    MurderBuff(greaterDangersense.Type);
                }
            }
            if (ModLoader.TryGetMod("AlchemistNPCLite", out Mod alchNPCLite) && LumUtils.AnyBosses())
            {
                if (alchNPCLite.TryFind("GreaterDangersense", out ModBuff greaterDangersense))
                {
                    MurderBuff(greaterDangersense.Type);
                }
            }
            void MurderBuff(int type)
            {
                if (Player.HasBuff(type))
                {
                    int index = Player.FindBuffIndex(type);
                    Player.DelBuff(index);
                    Player.ClearBuff(type);
                }
            }
        }

        public static List<int> IronTiles =
        [
            TileID.Iron,
            TileID.IronBrick,
            TileID.Lead,
            TileID.LeadBrick,
            TileID.MetalBars
        ];
        public static List<int> IronWalls =
        [
            WallID.IronFence,
            WallID.WroughtIronFence,
            WallID.MetalFence,
        ];

        public override void PostUpdateBuffs()
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            Player.pickSpeed -= 0.25f;

            Player.tileSpeed += 0.25f;
            Player.wallSpeed += 0.25f;

            Player.moveSpeed += 0.25f;

            Player.statManaMax2 += 50;
            Player.manaRegenDelay = Math.Min(Player.manaRegenDelay, 30);
            Player.manaRegenBonus += 5;

            Player.wellFed = true; //no longer expert half regen unless fed

        }

        public override void UpdateBadLifeRegen()
        {
            float regenReductionTime = LumUtils.SecondsToFrames(5);
            
            if (Player.lifeRegen > 0 && Player.lifeRegenTime < regenReductionTime)
                Player.lifeRegen = (int)(Player.lifeRegen * Player.lifeRegenTime / regenReductionTime);
        }

        public override void PostUpdateEquips()
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            if (Player.longInvince && !Player.immune)
            {
                if (CrossNecklaceTimer < 20)
                {
                    Player.longInvince = false;
                    CrossNecklaceTimer++;
                }
            }
            else
            {
                CrossNecklaceTimer = 0;
            }

            if (Player.iceBarrier)
                Player.GetDamage(DamageClass.Generic) -= 0.10f;

            if (Player.setSquireT2 || Player.setSquireT3 || Player.setMonkT2 || Player.setMonkT3 || Player.setHuntressT2 || Player.setHuntressT3 || Player.setApprenticeT2 || Player.setApprenticeT3 || Player.setForbidden)
                ReduceMasomodeMinionNerf = true;
        }

        private void HandleTimersAlways()
        {
            if (MasomodeCrystalTimer > 0)
                MasomodeCrystalTimer--;

            //disable minion nerf during ooa
            if (DD2Event.Ongoing && !FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy))
            {
                int n = NPC.FindFirstNPC(NPCID.DD2EterniaCrystal);
                if (n != -1 && n != Main.maxNPCs && Player.Distance(Main.npc[n].Center) < 3000)
                {
                    MasomodeMinionNerfTimer -= 2;
                    if (MasomodeMinionNerfTimer < 0)
                        MasomodeMinionNerfTimer = 0;
                }
            }

            if (WeaponUseTimer > 0)
                ShorterDebuffsTimer += 1;
            else if (ShorterDebuffsTimer > 0)
                ShorterDebuffsTimer -= 1;

            if (WeaponUseTimer > 0 && Player.HeldItem.DamageType != DamageClass.Summon && Player.HeldItem.DamageType != DamageClass.SummonMeleeSpeed && Player.HeldItem.DamageType != DamageClass.Default)
                MasomodeMinionNerfTimer += 1;
            else if (MasomodeMinionNerfTimer > 0)
                MasomodeMinionNerfTimer -= 1;

            if (MasomodeMinionNerfTimer > MaxMasomodeMinionNerfTimer)
                MasomodeMinionNerfTimer = MaxMasomodeMinionNerfTimer;

            if (ShorterDebuffsTimer > 60)
                ShorterDebuffsTimer = 60;

            if (PalladiumHealTimer > 0)
                PalladiumHealTimer--;

            //Main.NewText($"{MasomodeWeaponUseTimer} {MasomodeMinionNerfTimer} {ReduceMasomodeMinionNerf}");
        }

        public override void PostUpdateMiscEffects()
        {
            HandleTimersAlways();

            if (!WorldSavingSystem.EternityMode)
                return;

            //whips no longer benefit from melee speed bonus
            if (Player.HeldItem.shoot > ProjectileID.None && ProjectileID.Sets.IsAWhip[Player.HeldItem.shoot] && !Player.HasEffect<TikiEffect>())
            {
                Player.GetAttackSpeed(DamageClass.Melee) = 1;
            }
            //Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) /= Player.GetAttackSpeed(DamageClass.Melee);

            if (Player.happyFunTorchTime && ++TorchGodTimer > 60)
            {
                TorchGodTimer = 0;

                float ai0 = Main.rand.NextFloat(-2f, 2f);
                float ai1 = Main.rand.NextFloat(-2f, 2f);
                Projectile.NewProjectile(Player.GetSource_Misc("TorchGod"), Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, ModContent.ProjectileType<TorchGodFlame>(), 20, 0f, Main.myPlayer, ai0, ai1);
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Projectile, consider using ModifyHitNPC instead */
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            //reduce minion damage in emode if using a weapon, scales as you use weapons
            //if (FargoSoulsUtil.IsSummonDamage(proj, true, false) && MasomodeMinionNerfTimer > 0)
            //{
            //    double modifier = ReduceMasomodeMinionNerf ? 0.5 : 0.75;
            //    modifier *= Math.Min((double)MasomodeMinionNerfTimer / MaxMasomodeMinionNerfTimer, 1.0);

            //    damage = (int)(damage * (1.0 - modifier));
            //}
        }

        private void ShadowDodgeNerf()
        {
            if (Player.shadowDodge) //prehurt hook not called on titanium dodge
                Player.AddBuff(ModContent.BuffType<HolyPriceBuff>(), 600);
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            ShadowDodgeNerf();

            if (Player.resistCold && npc.coldDamage) //warmth potion nerf
            {
                modifiers.SourceDamage *= 1f / 0.7f; // warmth potion modifies source damage (pre defense) for some fucking reason. anti-30% 
                modifiers.FinalDamage *= 0.85f;
            }
        }
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            ShadowDodgeNerf();

            if (Player.resistCold && proj.coldDamage) //warmth potion nerf
            {
                modifiers.SourceDamage *= 1f / 0.7f; // warmth potion modifies source damage (pre defense) for some fucking reason. anti-30%
                modifiers.FinalDamage *= 0.85f;
            }
            /*
            if (NPC.AnyNPCs(ModContent.NPCType<CosmosChampion>()))
            {
                Player.AddBuff(ModContent.BuffType<MoonFangBuff>(), LumUtils.SecondsToFrames(5));
            }
            */
        }
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            //ShadowDodgeNerf();
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (!WorldSavingSystem.EternityMode)
                return;
            //ShadowDodgeNerf();
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            ShorterDebuffsTimer = MaxShorterDebuffsTimer;

            if (!WorldSavingSystem.EternityMode)
                base.ModifyHurt(ref modifiers);

            //because NO MODIFY/ONHITPLAYER HOOK WORKS
            if (modifiers.DamageSource.SourceProjectileType == ProjectileID.Explosives)
                Player.FargoSouls().AddBuffNoStack(ModContent.BuffType<StunnedBuff>(), 60);

            if (Player.brainOfConfusionItem != null && !Player.brainOfConfusionItem.IsAir)
            {
                if (Main.rand.NextBool(2)) // 50% chance to not work
                {
                    Player.brainOfConfusionItem = null;
                }
            }


            base.ModifyHurt(ref modifiers);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (WorldSavingSystem.MasochistModeReal && Player.whoAmI == Main.myPlayer)
            {
                if (LumUtils.AnyBosses())
                    Respawns++;
                /*
                foreach (NPC npc in Main.npc.Where(npc => npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsTail)))
                {
                    int heal = npc.lifeMax / 10;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        npc.life += heal;
                        if (npc.life > npc.lifeMax)
                            npc.life = npc.lifeMax;
                        npc.HealEffect(heal);
                        npc.netUpdate = true;
                    }
                    else
                    {
                        var netMessage = Mod.GetPacket();
                        netMessage.Write((byte)FargowiltasSouls.PacketID.HealNPC);
                        netMessage.Write((byte)npc.whoAmI);
                        netMessage.Write(heal);
                        netMessage.Send();
                    }
                }
                */
            }

            if (((Main.snowMoon && NPC.waveNumber < FrostMoonBosses.WAVELOCK) || (Main.pumpkinMoon && NPC.waveNumber < PumpkinMoonBosses.WAVELOCK)) && WorldSavingSystem.MasochistModeReal)
            {
                if (NPC.waveNumber > 1)
                    NPC.waveNumber--;
                NPC.waveKills /= 4;

                FargoSoulsUtil.PrintLocalization($"Mods.FargowiltasSouls.Message.MoonsDeathPenalty", new Color(175, 75, 255));
            }

        }

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            EmodeItemBalance.BalanceWeaponStats(Player, item, ref damage);

            //if (item.DamageType == DamageClass.Ranged) //changes all of these to additive
            //{
            //    //shroomite headpieces
            //    if (item.useAmmo == AmmoID.Arrow || item.useAmmo == AmmoID.Stake)
            //    {
            //        modifiers.FinalDamage Player.arrowDamage.Multiplicative;
            //        damage += Player.arrowDamage.Multiplicative - 1f;
            //    }
            //    else if (item.useAmmo == AmmoID.Bullet || item.useAmmo == AmmoID.CandyCorn)
            //    {
            //        modifiers.FinalDamage /= Player.bulletDamage.Multiplicative;
            //        damage += Player.bulletDamage.Multiplicative - 1f;
            //    }
            //    else if (item.useAmmo == AmmoID.Rocket || item.useAmmo == AmmoID.StyngerBolt || item.useAmmo == AmmoID.JackOLantern || item.useAmmo == AmmoID.NailFriendly)
            //    {
            //        modifiers.FinalDamage /= Player.bulletDamage.Multiplicative;
            //        damage += Player.bulletDamage.Multiplicative - 1f;
            //    }
            //}

        }

        public float AttackSpeed
        {
            get { return Player.FargoSouls().AttackSpeed; }
            set { Player.FargoSouls().AttackSpeed = value; }
        }

        public override bool ModifyNurseHeal(NPC nurse, ref int health, ref bool removeDebuffs, ref string chatText)
        {
            if (!WorldSavingSystem.EternityMode)
                return base.ModifyNurseHeal(nurse, ref health, ref removeDebuffs, ref chatText);

            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<RushJobBuff>()))
            {
                chatText = Language.GetTextValue("Mods.FargowiltasSouls.Buffs.RushJobBuff.NurseChat");
                return false;
            }

            return base.ModifyNurseHeal(nurse, ref health, ref removeDebuffs, ref chatText);
        }

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            if (LumUtils.AnyBosses())
                Main.LocalPlayer.AddBuff(ModContent.BuffType<RushJobBuff>(), 10);
        }
    }
}
