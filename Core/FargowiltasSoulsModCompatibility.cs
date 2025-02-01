using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.BanishedBaron;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Bosses.Champions.Earth;
using FargowiltasSouls.Content.Bosses.Champions.Life;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.Champions.Spirit;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.Champions.Timber;
using FargowiltasSouls.Content.Bosses.Champions.Will;
using FargowiltasSouls.Content.Bosses.CursedCoffin;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.Lifelight;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Pets;
using FargowiltasSouls.Content.Items.Placables.MusicBoxes;
using FargowiltasSouls.Content.Items.Placables.Trophies;
using FargowiltasSouls.Content.Items.Summons;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls
{
    public partial class FargowiltasSouls
    {
        /// <summary>
        /// <b>Vanilla main bosses:</b><br />
        ///  1.0 = King Slime<br />
        ///  2.0 = Eye of Cthulhu<br />
        ///  3.0 = Eater of Worlds / Brain of Cthulhu<br />
        ///  4.0 = Queen Bee<br />
        ///  5.0 = Skeletron<br />
        ///  6.0 = Deerclops<br />
        ///  7.0 = Wall of Flesh<br />
        ///  8.0 = Queen Slime<br />
        ///  9.0 = The Twins<br />
        /// 10.0 = The Destroyer<br />
        /// 11.0 = Skeletron Prime<br />
        /// 12.0 = Plantera<br />
        /// 13.0 = Golem<br />
        /// 14.0 = Duke Fishron<br />
        /// 15.0 = Empress of Light<br />
        /// 16.0 = Betsy<br />
        /// 17.0 = Lunatic Cultist<br />
        /// 18.0 = Moon Lord
        /// </summary>
        public Dictionary<string, float> BossChecklistValues = new()
        {
            {"DeviBoss", 6.9f},
            {"AbomBoss", 20f},
            {"MutantBoss", 23f},
            {"TimberChampion", 18.1f},
            {"TerraChampion", 18.15f},
            {"EarthChampion", 18.2f},
            {"NatureChampion", 18.25f},
            {"LifeChampion", 18.3f},
            {"ShadowChampion", 18.35f},
            {"SpiritChampion", 18.4f},
            {"WillChampion", 18.45f},
            {"CosmosChampion", 18.5f},
            {"TrojanSquirrel", 0.5f},
            {"LifeChallenger", 11.49f},
            {"BanishedBaron", 8.7f},
            {"CursedCoffin", 2.1f}
        };
        private void BossChecklistCompatibility()
        {
            if (ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist))
            {
                static bool AllPlayersAreDead() => Main.player.All(plr => !plr.active || plr.dead);

                void Add(string type, string bossName, List<int> npcIDs, Func<bool> downed, Func<bool> available, List<int> collectibles, List<int> spawnItems, bool hasKilledAllMessage, string portrait = null)
                {
                    bossChecklist.Call(
                        $"Log{type}",
                        this,
                        bossName,
                        BossChecklistValues[bossName],
                        downed,
                        npcIDs,
                        new Dictionary<string, object>()
                        {
                            { "spawnItems", spawnItems },
                            // { "collectibles", collectibles }, // it's fetched from npc loot? TODO: refactor method calls below
                            { "availability", available },
                            { "despawnMessage", hasKilledAllMessage ? new Func<NPC, LocalizedText>(npc =>
                                        AllPlayersAreDead() ? Language.GetText($"Mods.{Name}.NPCs.{bossName}.BossChecklistIntegration.KilledAllMessage") : Language.GetText($"Mods.{Name}.NPCs.{bossName}.BossChecklistIntegration.DespawnMessage")) :
                                    Language.GetText($"Mods.{Name}.NPCs.{bossName}.BossChecklistIntegration.DespawnMessage") },
                            {
                                "customPortrait",
                                portrait == null ? null : new Action<SpriteBatch, Rectangle, Color>((spriteBatch, rect, color) =>
                                {
                                    Texture2D tex = Assets.Request<Texture2D>(portrait, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                                    Rectangle sourceRect = tex.Bounds;
                                    float scale = Math.Min(1f, (float)rect.Width / sourceRect.Width);
                                    spriteBatch.Draw(tex, rect.Center.ToVector2(), sourceRect, color, 0f, sourceRect.Size() / 2, scale, SpriteEffects.None, 0);
                                })
                            }
                        }
                    // available,
                    // collectibles,
                    // spawnItems,
                    // hasKilledAllMessage ? new Func<NPC, string>(npc => AllPlayersAreDead() ? $"Mods.{Name}.BossChecklist.{bossName}KilledAll" : $"Mods.{Name}.BossChecklist.{bossName}Despawn") : $"Mods.{Name}.BossChecklist.{bossName}Despawn",
                    // portrait == null ? null : new Action<SpriteBatch, Rectangle, Color>((spriteBatch, rect, color) =>
                    // {
                    //     Texture2D tex = Assets.Request<Texture2D>(portrait, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    //     Rectangle sourceRect = tex.Bounds;
                    //     float scale = Math.Min(1f, (float)rect.Width / sourceRect.Width);
                    //     spriteBatch.Draw(tex, rect.Center.ToVector2(), sourceRect, color, 0f, sourceRect.Size() / 2, scale, SpriteEffects.None, 0);
                    // })
                    );
                }
                bool calamity = ModLoader.HasMod("CalamityMod");
                Add("Boss",
                    "DeviBoss",
                    [ModContent.NPCType<DeviBoss>()],
                    () => WorldSavingSystem.DownedDevi,
                    () => true,
                    [
                        ModContent.ItemType<DeviMusicBox>(),
                        ModContent.ItemType<DeviatingEnergy>(),
                        ModContent.ItemType<DeviTrophy>(),
                        ModContent.ItemType<ChibiHat>(),
                        ModContent.ItemType<BrokenBlade>()
                    ],
                    [ModContent.ItemType<DevisCurse>()],
                    true
                );
                float abomValue = calamity ? 22.6f : 20f;
                Add("Boss",
                    "AbomBoss",
                    [ModContent.NPCType<AbomBoss>()],
                    () => WorldSavingSystem.DownedAbom,
                    () => true,
                    [
                        ModContent.ItemType<AbominationnP1MusicBox>(),
                        ModContent.ItemType<AbomEnergy>(),
                        ModContent.ItemType<AbomTrophy>(),
                        ModContent.ItemType<BabyScythe>(),
                        ModContent.ItemType<BrokenHilt>()
                    ],
                    [ModContent.ItemType<AbomsCurse>()],
                    true
                );
                float mutantValue = calamity ? 30 : 23;
                Add("Boss",
                    "MutantBoss",
                    [ModContent.NPCType<MutantBoss>()],
                    () => WorldSavingSystem.DownedMutant,
                    () => true,
                    [
                        ModContent.ItemType<MutantMusicBox>(),
                        ModContent.ItemType<EternalEnergy>(),
                        ModContent.ItemType<MutantTrophy>(),
                        ModContent.ItemType<SpawnSack>(),
                        ModContent.ItemType<PhantasmalEnergy>()
                    ],
                    [ModContent.ItemType<AbominationnVoodooDoll>()],
                    true
                );


                #region champions

                Add("MiniBoss",
                    "TimberChampion",
                    [ModContent.NPCType<TimberChampion>()],
                    () => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.TimberChampion],
                    () => true,
                    new List<int>(BaseForce.EnchantsIn<TimberForce>()).Append(ModContent.ItemType<ChampionMusicBox>()).ToList(),
                    [ModContent.ItemType<SigilOfChampions>()],
                    false
                );
                Add("MiniBoss",
                    "TerraChampion",
                    [ModContent.NPCType<TerraChampion>(), ModContent.NPCType<TerraChampionBody>(), ModContent.NPCType<TerraChampionTail>()],
                    () => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.TerraChampion],
                    () => true,
                    new List<int>(BaseForce.EnchantsIn<TerraForce>()).Append(ModContent.ItemType<ChampionMusicBox>()).ToList(),
                    [ModContent.ItemType<SigilOfChampions>()],
                    false,
                    "Content/Bosses/Champions/Terra/TerraChampion_Still"
                );
                Add("MiniBoss",
                    "EarthChampion",
                    [ModContent.NPCType<EarthChampion>(), ModContent.NPCType<EarthChampionHand>()],
                    () => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.EarthChampion],
                    () => true,
                    new List<int>(BaseForce.EnchantsIn<EarthForce>()).Append(ModContent.ItemType<ChampionMusicBox>()).ToList(),
                    [ModContent.ItemType<SigilOfChampions>()],
                    false,
                    "Content/Bosses/Champions/Earth/EarthChampion_Still"
                );
                Add("MiniBoss",
                    "NatureChampion",
                    [ModContent.NPCType<NatureChampion>(), ModContent.NPCType<NatureChampionHead>()],
                    () => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.NatureChampion],
                    () => true,
                    new List<int>(BaseForce.EnchantsIn<NatureForce>()).Append(ModContent.ItemType<ChampionMusicBox>()).ToList(),
                    [ModContent.ItemType<SigilOfChampions>()],
                    false,
                    "Content/Bosses/Champions/Nature/NatureChampion_Still"
                );
                Add("MiniBoss",
                    "LifeChampion",
                    [ModContent.NPCType<LifeChampion>()],
                    () => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.LifeChampion],
                    () => true,
                    new List<int>(BaseForce.EnchantsIn<LifeForce>()).Append(ModContent.ItemType<ChampionMusicBox>()).ToList(),
                    [ModContent.ItemType<SigilOfChampions>()],
                    false,
                    "Content/Bosses/Champions/Life/LifeChampion_Still"
                );
                Add("MiniBoss",
                    "ShadowChampion",
                    [ModContent.NPCType<ShadowChampion>()],
                    () => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.ShadowChampion],
                    () => true,
                    new List<int>(BaseForce.EnchantsIn<ShadowForce>()).Append(ModContent.ItemType<ChampionMusicBox>()).ToList(),
                    [ModContent.ItemType<SigilOfChampions>()],
                    false
                );
                Add("MiniBoss",
                    "SpiritChampion",
                    [ModContent.NPCType<SpiritChampion>(), ModContent.NPCType<SpiritChampionHand>()],
                    () => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.SpiritChampion],
                    () => true,
                    new List<int>(BaseForce.EnchantsIn<SpiritForce>()).Append(ModContent.ItemType<ChampionMusicBox>()).ToList(),
                    [ModContent.ItemType<SigilOfChampions>()],
                    false,
                    "Content/Bosses/Champions/Spirit/SpiritChampion_Still"
                );
                Add("MiniBoss",
                    "WillChampion",
                    [ModContent.NPCType<WillChampion>()],
                    () => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.WillChampion],
                    () => true,
                    new List<int>(BaseForce.EnchantsIn<WillForce>()).Append(ModContent.ItemType<ChampionMusicBox>()).ToList(),
                    [ModContent.ItemType<SigilOfChampions>()],
                    false
                );

                Add("Boss",
                    "CosmosChampion",
                    [ModContent.NPCType<CosmosChampion>()],
                    () => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.CosmosChampion],
                    () => true,
                    new List<int>(BaseForce.EnchantsIn<CosmoForce>()).Append(ModContent.ItemType<ChampionMusicBox>()).ToList(),
                    [ModContent.ItemType<SigilOfChampions>()],
                    true
                );

                #endregion champions


                #region challengers

                Add("Boss",
                    "TrojanSquirrel",
                    [ModContent.NPCType<TrojanSquirrel>()],
                    () => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.TrojanSquirrel],
                    () => true,
                    [
                        ModContent.ItemType<TrojanSquirrelTrophy>(),
                        ModContent.ItemType<TreeSword>(),
                        ModContent.ItemType<MountedAcornGun>(),
                        ModContent.ItemType<SnowballStaff>(),
                        ModContent.ItemType<KamikazeSquirrelStaff>()
                    ],
                    [ModContent.ItemType<SquirrelCoatofArms>()],
                    false,
                    "Content/Bosses/TrojanSquirrel/TrojanSquirrel_Still"
                );
                Add("Boss",
                    "LifeChallenger",
                    [ModContent.NPCType<LifeChallenger>()],
                    () => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.Lifelight],
                    () => true,
                    [
                        ModContent.ItemType<LifelightTrophy>(),
                        ModContent.ItemType<EnchantedLifeblade>(),
                        ModContent.ItemType<Lightslinger>(),
                        ModContent.ItemType<CrystallineCongregation>(),
                        ModContent.ItemType<KamikazePixieStaff>(),
                        ModContent.ItemType<LifelightMasterPet>()
                    ],
                    [ModContent.ItemType<FragilePixieLamp>()],
                    false,
                    "Content/Bosses/Lifelight/LifeChallenger"
                );

                Add("Boss",
                    "BanishedBaron",
                    [ModContent.NPCType<BanishedBaron>()],
                    () => WorldSavingSystem.downedBoss[(int)WorldSavingSystem.Downed.BanishedBaron],
                    () => true,
                    [
                        ModContent.ItemType<BaronTrophy>(),
                        ModContent.ItemType<TheBaronsTusk>(),
                        ModContent.ItemType<RoseTintedVisor>(),
                        ModContent.ItemType<NavalRustrifle>(),
                        ModContent.ItemType<DecrepitAirstrikeRemote>(),
                    ],
                    [ModContent.ItemType<MechLure>()],
                    false
                );
                if (CursedCoffin.Enabled)
                {
                    Add("Boss",
                    "CursedCoffin",
                    //TODO: ADD LOOT
                    [ModContent.NPCType<CursedCoffin>()],
                    () => WorldSavingSystem.downedBoss[(int)WorldSavingSystem.Downed.CursedCoffin],
                    () => true,
                    [],
                    [ModContent.ItemType<CoffinSummon>()],
                    false
                //"Content/NPCs/Challengers/CursedCoffin"
                );
                }

                #endregion challengers
            }
        }
    }
}
