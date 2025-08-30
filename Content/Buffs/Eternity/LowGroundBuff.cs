using FargowiltasSouls.Assets.Textures;
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
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        private static ILHook? lowGroundHook;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;

            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;

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
        }
    }
}
