using Luminance.Core.Hooking;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class LowGroundBuff : ModBuff
    {
        private static ILHook? lowGroundHook;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Low Ground");
            // Description.SetDefault("No hooks, cannot stand on platforms or liquids");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;

            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "低地");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "不能站在平台或液体上");

            MethodInfo? updateMethod = typeof(Player).GetMethod("Update");
            if (updateMethod is not null)
            {
                new ManagedILEdit("Low Ground fallthrough", Mod, edit =>
                {
                    lowGroundHook = new(updateMethod, new(c => edit.EditingFunction(c, edit)));
                }, edit =>
                {
                    lowGroundHook?.Undo();
                    lowGroundHook?.Dispose();
                }, LowGroundILEdit).Apply();
            }
        }
        public override void Unload()
        {
            lowGroundHook?.Undo();
            lowGroundHook?.Dispose();
        }
        public static void LowGroundILEdit(ILContext context, ManagedILEdit edit)
        {
            ILCursor cursor = new(context);
            // move to the end of the long if statement for if player should fallthrough (for example, if grappling downwards or inverted gravity)
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld<Player>("GoingDownWithGrapple"));
            // insert an "or lowground" to enable fallthrough
            cursor.EmitOr(); // for the previous stuff
            cursor.Emit(OpCodes.Ldarg_0); // get player instance as argument
            cursor.EmitDelegate(CanFallthrough); // outputs 1 or 0, next instruction is "or" to include this and the previous stuff we collected with EmitOr
        }
        public static bool CanFallthrough(Player player)
        {
            return player.FargoSouls().FallthroughTimer > 0;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().LowGround = true;
            if (player.grapCount > 0)
                player.RemoveAllGrapplingHooks();

            if (player.mount.Active)
                player.mount.Dismount(player);
            /*
            Tile thisTile = Framing.GetTileSafely(player.Bottom);
            Tile bottomTile = Framing.GetTileSafely(player.Bottom + Vector2.UnitY * 8);

            if (!Collision.SolidCollision(player.BottomLeft, player.width, 16))
            {
                if (player.velocity.Y >= 0 && (IsPlatform(thisTile.TileType) || IsPlatform(bottomTile.TileType)))
                {
                    player.position.Y += 2;
                }
                if (player.velocity.Y == 0)
                {
                    player.position.Y += 16;
                }

            }

            static bool IsPlatform(int tileType)
            {
                return tileType == TileID.Platforms || tileType == TileID.PlanterBox;
            }
            */

            /*
            for (int i = -2; i <= 2; i++)
            {
                Vector2 pos = player.Center;
                pos.X += i * 16;
                pos.Y += player.height / 2;
                if (player.mount.Active)
                    pos.Y += player.mount.HeightBoost;
                pos.Y += 8;

                int x = (int)(pos.X / 16);
                int y = (int)(pos.Y / 16);
                Tile tile = Framing.GetTileSafely(x, y);
                if ((tile.TileType == TileID.Platforms || tile.TileType == TileID.PlanterBox) && !tile.IsActuated)
                {
                    tile.IsActuated = true;
                    //if (Main.netMode == NetmodeID.Server)
                    //    NetMessage.SendTileSquare(-1, x, y, 1);
                }
            }
            */
        }
    }
}
