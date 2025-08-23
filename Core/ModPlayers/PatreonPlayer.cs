using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Patreon.ParadoxWolf;
using FargowiltasSouls.Content.Patreon.Potato;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Core.ModPlayers
{
    public class PatreonPlayer : ModPlayer
    {
        public bool Gittle;
        public bool RoombaPet;

        public bool Sasha;
        public bool FishMinion;

        public bool CompOrb;
        public int CompOrbDrainCooldown;

        public bool ManliestDove;
        public bool DovePet;

        public bool Cat;
        public bool KingSlimeMinion;

        public bool WolfDashing;

        public bool PiranhaPlantMode;

        public bool JojoTheGamer;
        public bool PrimeMinion;

        public bool Eight3One;

        public bool Crimetroid;
        public bool ROB;

        public bool ChibiiRemii;

        public bool Northstrider;

        public bool RazorContainer;
        public int RazorCD;

        public bool TouhouBuff;

        public bool dolvan;
        public int dolvanDeathTimer;

        public bool Dartslinger;

        public bool AubreyFlower;

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);

            string name = "PatreonSaves" + Player.name;
            var PatreonSaves = new List<string>();

            if (PiranhaPlantMode) PatreonSaves.Add("PiranhaPlantMode");

            tag.Add(name, PatreonSaves);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);

            string name = "PatreonSaves" + Player.name;
            IList<string> PatreonSaves = tag.GetList<string>(name);

            PiranhaPlantMode = PatreonSaves.Contains("PiranhaPlantMode");
        }

        public override void ResetEffects()
        {
            Gittle = false;
            RoombaPet = false;
            Sasha = false;
            FishMinion = false;
            CompOrb = false;
            ManliestDove = false;
            DovePet = false;
            Cat = false;
            KingSlimeMinion = false;
            WolfDashing = false;
            JojoTheGamer = false;
            Crimetroid = false;
            ROB = false;
            PrimeMinion = false;
            ChibiiRemii = false;
            Northstrider = false;
            RazorContainer = false;
            TouhouBuff = false;
            dolvan = false;
            Dartslinger = false;
            AubreyFlower = false;
        }

        public override void OnEnterWorld()
        {
            if (Gittle || Sasha || ManliestDove || Cat || JojoTheGamer || Northstrider || Eight3One || dolvan)
            {
                string text = Language.GetTextValue($"Mods.{Mod.Name}.Message.PatreonNameEffect");
                Main.NewText($"{text}, {Player.name}!");
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (CompOrbDrainCooldown > 0)
                CompOrbDrainCooldown -= 1;
            if (RazorCD > 0)
                RazorCD--;

            switch (Player.name)
            {
                case "iverhcamer":
                    Gittle = true;
                    Player.pickSpeed -= .15f;
                    //shine effect
                    Lighting.AddLight(Player.Center, 0.8f, 0.8f, 0);
                    break;
                case "Sasha":
                    Sasha = true;

                    Player.lavaImmune = true;
                    Player.fireWalk = true;
                    Player.buffImmune[BuffID.OnFire] = true;
                    Player.buffImmune[BuffID.CursedInferno] = true;
                    Player.buffImmune[BuffID.Burning] = true;
                    break;
                case "Dove":
                    ManliestDove = true;
                    break;
                case "cat":
                    Cat = true;

                    if (NPC.downedMoonlord)
                    {
                        Player.maxMinions += 4;
                    }
                    else if (Main.hardMode)
                    {
                        Player.maxMinions += 2;
                    }

                    Player.GetDamage(DamageClass.Summon) += Player.maxMinions * 0.5f;
                    break;
                case "VirtualDefender":
                    JojoTheGamer = true;
                    break;
                case "Northstrider":
                    Northstrider = true;

                    Player.wingsLogic = 3;
                    Player.wings = 3;

                    Player.wingTimeMax = 100;
                    Player.wingAccRunSpeed = 9f;
                    Player.wingRunAccelerationMult = 9f;
                    break;
                case "Eight3One":
                    Eight3One = true;
                    break;
                case "dolvan":
                    dolvan = true;
                    Player.statDefense.FinalMultiplier *= 0;
                    if (Player.active && !Player.dead)
                    {
                        dolvanDeathTimer++;
                    }
                    if (dolvanDeathTimer >= 60 * 60 * 5)//5 minutes
                    {
                        dolvanDeathTimer = 0;
                        Projectile p = Projectile.NewProjectileDirect(Player.GetSource_Death(), Player.Center, Vector2.Zero, ProjectileID.Bomb, 80, 5, Player.whoAmI);
                        p.timeLeft = 3;
                        Player.KillMe(PlayerDeathReason.ByProjectile(Player.whoAmI, p.whoAmI), 10000, Player.direction);
                    }
                    
                    Player.AddBuff(BuffID.Poisoned, 2);
                    Player.AddBuff(BuffID.Darkness, 2);
                    Player.AddBuff(BuffID.Cursed, 2);
                    Player.AddBuff(BuffID.OnFire, 2);
                    Player.AddBuff(BuffID.Bleeding, 2);
                    Player.AddBuff(BuffID.Confused, 2);
                    Player.AddBuff(BuffID.Slow, 2);
                    Player.AddBuff(BuffID.Weak, 2);
                    Player.AddBuff(BuffID.Silenced, 2);
                    Player.AddBuff(BuffID.BrokenArmor, 2);
                    Player.AddBuff(BuffID.Suffocation, 2);
                    Player.AddBuff(ModContent.BuffType<OceanicSealBuff>(), 2);
                    break;
                case "Dartslinger":
                    Dartslinger = true;
                    break;
                case "Sayaka":
                    AubreyFlower = true;
                    Player.lifeRegen += 4;
                    break;
            }

            if (CompOrb && Player.itemAnimation > 0)
            {
                Player.manaRegenDelay = Player.maxRegenDelay;
            }
        }
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            //post update does not run before this, do not check patreon bools it wont work
            if (!mediumCoreDeath && Player.name.Equals("Dartslinger", System.StringComparison.OrdinalIgnoreCase))
            {
                yield return new Item(ItemID.Blowpipe, prefix: PrefixID.Unreal);
                yield return new Item(ItemID.PoisonDart, 500);
                yield return new Item(ItemID.GlommerPetItem);
            }
            if (!mediumCoreDeath && Player.name.Equals("Sayaka", System.StringComparison.OrdinalIgnoreCase))
            {
                yield return new Item(ItemID.Katana, prefix: PrefixID.Legendary);
            }
        }
        public static void AddDash_Eight3One(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.FargoDash = DashManager.DashType.Crystal;
            modPlayer.CrystalAssassinDiagonal = true;
            modPlayer.HasDash = true;
        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CompOrb && CompOrbDrainCooldown <= 0 && item.DamageType != DamageClass.Magic && item.DamageType != DamageClass.Summon)
            {
                CompOrbDrainCooldown = 15;
                if (Player.CheckMana(10, true, false))
                    Player.manaRegenDelay = Player.maxRegenDelay;
            }
            if (Eight3One && Main.rand.NextBool(20))
                target.AddBuff(ModContent.BuffType<LightningRodBuff>(), 60);
            if (AubreyFlower && item.DamageType == DamageClass.Melee)
                target.AddBuff(BuffID.Wet, 60 * Main.rand.Next(5, 11));
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CompOrb && CompOrbDrainCooldown <= 0 && proj.DamageType != DamageClass.Magic && proj.DamageType != DamageClass.Summon)
            {
                CompOrbDrainCooldown = 15;
                if (Player.CheckMana(10, true, false))
                    Player.manaRegenDelay = Player.maxRegenDelay;
            }
            if (Eight3One && Main.rand.NextBool(20))
                target.AddBuff(ModContent.BuffType<LightningRodBuff>(), 60);
            if (AubreyFlower && proj.DamageType == DamageClass.Melee)
                target.AddBuff(BuffID.Wet, 60 * Main.rand.Next(5, 11));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Gittle)
            {
                if (Main.rand.NextBool(10))
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (Vector2.Distance(target.Center, npc.Center) < 50)
                        {
                            npc.AddBuff(BuffID.Venom, 300);
                        }
                    }
                }

                if (FargowiltasSouls.CalamityMod != null)
                {
                    target.SimpleStrikeNPC(int.MaxValue, 0, false, 0, null, false, 0, true);
                }
            }
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (CompOrb && item.DamageType != DamageClass.Magic && item.DamageType != DamageClass.Summon)
            {
                modifiers.FinalDamage *= 1.17f;

                if (Player.manaSick)
                    modifiers.FinalDamage *= 1f - Player.manaSickReduction;

                /*for (int num468 = 0; num468 < 20; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, DustID.MagicMirror, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100, default, 2f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 2f;
                    num469 = Dust.NewDust(new Vector2(target.Center.X, target.Center.Y), target.width, target.height, DustID.MagicMirror, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100);
                    Main.dust[num469].velocity *= 2f;
                }*/
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (CompOrb && proj.DamageType != DamageClass.Magic && proj.DamageType != DamageClass.Summon)
            {
                modifiers.FinalDamage *= 1.17f;

                if (Player.manaSick)
                    modifiers.FinalDamage *= 1f - Player.manaSickReduction;

                /*for (int num468 = 0; num468 < 20; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, DustID.MagicMirror, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100, default, 2f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 2f;
                    num469 = Dust.NewDust(new Vector2(target.Center.X, target.Center.Y), target.width, target.height, DustID.MagicMirror, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100);
                    Main.dust[num469].velocity *= 2f;
                }*/
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            OnHitByEither();
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            OnHitByEither();
        }

        private void OnHitByEither()
        {
            if (PiranhaPlantMode)
            {
                int index = Main.rand.Next(FargowiltasSouls.DebuffIDs.Count);
                Player.AddBuff(FargowiltasSouls.DebuffIDs[index], 180);
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (SoulConfig.Instance.PatreonWolf)
            {
                if (damageSource.TryGetCausingEntity(out Entity entity) && entity is NPC npc && npc.active && npc.type == NPCID.Wolf)
                {
                    Item.NewItem(Player.GetSource_Death(), Player.Hitbox, ModContent.ItemType<ParadoxWolfSoul>());
                }
            }
            if ((Player.numberOfDeathsPVP + Player.numberOfDeathsPVE) % 5 == 0 && (Player.numberOfDeathsPVP + Player.numberOfDeathsPVE) > 0 && dolvan)
            {
                Main.NewText("[c/a80000:" + Language.GetTextValue("Mods.FargowiltasSouls.Message.DolvanSorry") + "]");
            }
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            base.HideDrawLayers(drawInfo);

            if (WolfDashing) //dont draw player during dash
                drawInfo.DrawDataCache.Clear();

            //HashSet<int> layersToRemove = new HashSet<int>();
            //for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
            //{
            //    if (JojoTheGamer && drawInfo.DrawDataCache[i] == PlayerLayer.Skin)
            //    {
            //        layersToRemove.Add(i);
            //    }
            //}
            //foreach (int i in layersToRemove)
            //{
            //    drawInfo.DrawDataCache.RemoveAt(i);
            //}

            //alternative for jojo changes? idk
            //Terraria.DataStructures.PlayerDrawLayers.Skin.Hide();
        }

        public override void FrameEffects()
        {
            //if (JojoTheGamer)
            //{
            //    Player.legs = Mod.GetEquipSlot("BetaLeg", EquipType.Legs);
            //    Player.body = Mod.GetEquipSlot("BetaBody", EquipType.Body);
            //    Player.head = Mod.GetEquipSlot("BetaHead", EquipType.Head);
            //}
        }

        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (RazorContainer)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile projectile = Main.projectile[i];

                    if (projectile.TypeAlive<RazorBlade>() && hitbox.Distance(projectile.Center) < 100)
                    {
                        if (Player.whoAmI == projectile.owner)
                        {
                            RazorBlade.Launch(Player, projectile);
                        }
                    }
                }
            }
        }
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (Dartslinger && (item.ammo == AmmoID.Dart || item.useAmmo == AmmoID.Dart))
            {
                damage *= 1.15f;
            }
            base.ModifyWeaponDamage(item, ref damage);
        }
    }
}