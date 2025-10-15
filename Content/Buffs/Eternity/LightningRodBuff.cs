﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Eternity.Bosses;
using FargowiltasSouls.Content.Projectiles.Eternity.Buffs;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class LightningRodBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        private static void SpawnLightning(Entity obj, int type, int damage, IEntitySource source)
        {
            //tends to spawn in ceilings if the player goes indoors/underground
            if (obj is Player)
            {
                Point tileCoordinates = obj.Top.ToTileCoordinates();

                tileCoordinates.X += Main.rand.Next(-25, 25);
                tileCoordinates.Y -= Main.rand.Next(4, 8);

                float ai1 = obj.Center.Y;
                //int damage = (Main.hardMode ? 120 : 60) / 4;
                int p = Projectile.NewProjectile(obj.GetSource_Misc(""), tileCoordinates.X * 16 + 8, (tileCoordinates.Y * 16 + 17) - 900, 0f, 0f, ModContent.ProjectileType<LightningRodLightning>(), damage, 2f, Main.myPlayer,
                    Vector2.UnitY.ToRotation(), ai1);
            }
            else
            {
                Point tileCoordinates = obj.Top.ToTileCoordinates();

                tileCoordinates.X += Main.rand.Next(-25, 25);
                tileCoordinates.Y -= 15 + Main.rand.Next(-5, 5) - (type == ModContent.ProjectileType<LightningVortexHostile>() ? 20 : 0);

                for (int index = 0; index < 10 && !WorldGen.SolidTile(tileCoordinates.X, tileCoordinates.Y) && tileCoordinates.Y > 10; ++index) tileCoordinates.Y -= 1;

                Projectile.NewProjectile(source, tileCoordinates.X * 16 + 8, tileCoordinates.Y * 16 + 17,
                    0f, 0f, type, damage, 2f, Main.myPlayer, 0f, obj.whoAmI);
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //spawns lightning once per second
            player.FargoSouls().lightningRodTimer++;
            if (player.FargoSouls().lightningRodTimer >= 60)
            {
                player.FargoSouls().lightningRodTimer = 0;
                int damage = (Main.hardMode ? 120 : 60) / 4;
                SpawnLightning(player, ModContent.ProjectileType<LightningVortexHostile>(), damage, player.GetSource_Buff(buffIndex));
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            FargoSoulsGlobalNPC fargoNPC = npc.FargoSouls();
            fargoNPC.lightningRodTimer++;
            if (fargoNPC.lightningRodTimer >= 60)
            {
                fargoNPC.lightningRodTimer = 0;
                SpawnLightning(npc, ModContent.ProjectileType<LightningVortex>(), 60, npc.GetSource_FromThis());
            }
        }
    }
}