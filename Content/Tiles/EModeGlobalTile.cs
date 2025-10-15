﻿using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Environment;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace FargowiltasSouls.Content.Tiles
{
    public class EModeGlobalTile : GlobalTile
    {
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            if (type == TileID.LihzahrdAltar && Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost
                && Main.LocalPlayer.Distance(new Vector2(i * 16 + 8, j * 16 + 8)) < 3000
                && Collision.CanHit(new Vector2(i * 16 + 8, j * 16 + 8), 0, 0, Main.LocalPlayer.Center, 0, 0)
                && Framing.GetTileSafely(Main.LocalPlayer.Center).WallType == WallID.LihzahrdBrickUnsafe)
            {
                if (!Main.LocalPlayer.HasBuff<LihzahrdBlessingBuff>())
                {
                    Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Buffs.LihzahrdBlessingBuff.Message"), Color.Orange);
                    SoundEngine.PlaySound(SoundID.Item4, Main.LocalPlayer.Center);
                    for (int k = 0; k < 50; k++)
                    {
                        int d = Dust.NewDust(Main.LocalPlayer.position, Main.LocalPlayer.width, Main.LocalPlayer.height, DustID.Torch, 0f, 0f, 0, default, Main.rand.NextFloat(3f, 6f));
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 9f;
                    }
                }
                Main.LocalPlayer.AddBuff(ModContent.BuffType<LihzahrdBlessingBuff>(), 60 * 60 * 10 - 1); //10mins
            }
        }

        private static bool CanBreakTileMaso(int i, int j, int type)
        {
            if ((type == TileID.Traps || type == TileID.PressurePlates) && Framing.GetTileSafely(i, j).WallType == WallID.LihzahrdBrickUnsafe)
            {
                int p = Player.FindClosest(new Vector2(i * 16 + 8, j * 16 + 8), 0, 0);
                if (p != -1)
                {
                    //if player INSIDE TEMPLE, but has blessing, its ok to break
                    Tile tile = Framing.GetTileSafely(Main.player[p].Center);
                    if (tile.WallType == WallID.LihzahrdBrickUnsafe && Main.player[p].HasBuff<LihzahrdBlessingBuff>())
                        return true;
                }
                //if player outside temple, or player in temple but blessed, dont break
                return false;
            }
            return true;
        }

        public override bool CanExplode(int i, int j, int type)
        {
            if (!WorldSavingSystem.EternityMode)
                return base.CanExplode(i, j, type);


            if (!CanBreakTileMaso(i, j, type))
                return false;


            return base.CanExplode(i, j, type);
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (!WorldSavingSystem.EternityMode)
                return base.CanKillTile(i, j, type, ref blockDamaged);


            if (!CanBreakTileMaso(i, j, type))
                return false;


            return base.CanKillTile(i, j, type, ref blockDamaged);
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            if (type == TileID.ShadowOrbs && Main.invasionType == 0 && !NPC.downedGoblins && WorldGen.shadowOrbSmashed)
            {
                int p = Player.FindClosest(new Vector2(i * 16, j * 16), 0, 0);
                if (p != -1 && Main.player[p].statLifeMax2 >= 200)
                {
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Main.invasionDelay = 0;
                        Main.StartInvasion(1);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, p, -1f);
                    }
                }
            }
        }
        public override void RandomUpdate(int i, int j, int type)
        {
            if (!WorldSavingSystem.EternityMode)
                return;
            if (type == TileID.Trees && WorldGen.IsTileALeafyTreeTop(i, j))
            {
                WorldGen.GetTreeBottom(i, j, out int x, out int y);
                TreeTypes tree = WorldGen.GetTreeType(Main.tile[x, y].TileType);
                if (tree == TreeTypes.Crimson || tree == TreeTypes.PalmCrimson)
                {
                    Projectile.NewProjectileDirect(new EntitySource_TileUpdate(i, j), new Vector2(i, j).ToWorldCoordinates() + new Vector2(Main.rand.NextFloat(0, 20), 0).RotatedByRandom(MathHelper.TwoPi), Vector2.Zero, ModContent.ProjectileType<BloodDroplet>(), 0, 0);
                }
            }
            base.RandomUpdate(i, j, type);
        }
    }
}