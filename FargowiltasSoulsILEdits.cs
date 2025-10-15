using System;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Luminance.Core.Hooking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
namespace FargowiltasSouls
{
    public static class ILEditUtils
    {
        public static bool CanFallthrough(Player player)
        {
            return player.FargoSouls().FallthroughTimer > 0;
        }
        public static float GetYoyoRangeMultFromProjectile(Projectile projectile)
        {
            float YoyoRangeMult = 1f;
            if (projectile.owner.IsWithinBounds(Main.maxPlayers))
            {
                Player player = Main.player[projectile.owner];
                if (player.HasEffect<TungstenEffect>())
                    YoyoRangeMult += 0.25f;
                if (player.FargoSouls().AttackSpeed > 0f)
                    YoyoRangeMult += player.FargoSouls().AttackSpeed - 1f;
            }
            return YoyoRangeMult;
        }
        public static float GetYoyoRangeMultFromPlayerItem(Player player, Item item)
        {
            float YoyoRangeMult = 1f;
            if (player.whoAmI.IsWithinBounds(Main.maxPlayers) && ItemID.Sets.Yoyo[item.type])
            {
                if (player.HasEffect<TungstenEffect>())
                    YoyoRangeMult += 0.25f;
                if (player.FargoSouls().AttackSpeed > 0f)
                    YoyoRangeMult += player.FargoSouls().AttackSpeed - 1f;
            }
            return YoyoRangeMult;
        }
        public static double NormalizeMult(float value)
        {
            return Math.Ceiling(value);
        }
        public static bool ProjectileIsNotFromArrowRain(Projectile projectile)
        {
            return !projectile.FargoSouls().ArrowRain;
        }
        public static bool NotSafeFromCactusDamage(Player player)
        {
            return !player.HasEffect<CactusPassiveEffect>();
        }
    }
    public sealed class Player_Update_ILEdit : ILEditProvider
    {
        public override void Subscribe(ManagedILEdit edit) => IL_Player.Update += edit.SubscriptionWrapper;
        public override void Unsubscribe(ManagedILEdit edit) => IL_Player.Update -= edit.SubscriptionWrapper;
        public override void PerformEdit(ILContext context, ManagedILEdit edit)
        {
            ILCursor cursor = new(context);
            // move to the end of the long if statement for if player should fallthrough (for example, if grappling downwards or inverted gravity)
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("GoingDownWithGrapple")))
            {
                FargowiltasSouls.Instance.Logger.Warn("Low Ground fallthrough failure on MatchLdfld<Player>('GoingDownWithGrapple')");
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasSouls>(), context);
                return;
            }
            // insert an "or lowground" to enable fallthrough
            cursor.EmitOr(); // for the previous stuff
            cursor.Emit(OpCodes.Ldarg_0); // get player instance as argument
            cursor.EmitDelegate(ILEditUtils.CanFallthrough); // outputs 1 or 0, next instruction is "or" to include this and the previous stuff we collected with EmitOr
        }
    }
    public sealed class Projectile_AI_099_1_ILEdit : ILEditProvider
    {
        public override void Subscribe(ManagedILEdit edit) => IL_Projectile.AI_099_1 += edit.SubscriptionWrapper;
        public override void Unsubscribe(ManagedILEdit edit) => IL_Projectile.AI_099_1 -= edit.SubscriptionWrapper;
        public override void PerformEdit(ILContext context, ManagedILEdit edit)
        {
            ILCursor cursor = new(context);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdfld<Player>("yoyoString"))) // This runs directly under AI_099_1(), therefore the first yoyo string argument is conveniently the last relevant range mod
            {
                FargowiltasSouls.Instance.Logger.Warn("Yoyo range modification 1 failure on MatchLdfld<Player>('yoyoString')");
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasSouls>(), context);
                return;
            }
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdarg0())) // Use this for your own benefit in the next delegate and compensate for it later
            {
                FargowiltasSouls.Instance.Logger.Warn("Yoyo range modification 1 failure on MatchLdarg0()");
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasSouls>(), context);
                return;
            }
            cursor.EmitDelegate(ILEditUtils.GetYoyoRangeMultFromProjectile); // Calc yoyo range mult to be used
            cursor.Emit(OpCodes.Ldloc_1); // Get and prep the yoyo range num to be multiplied
            cursor.Emit(OpCodes.Mul); // Multiply
            cursor.EmitDelegate(ILEditUtils.NormalizeMult); // Make sure this uses Ceiling
            cursor.Emit(OpCodes.Stloc_1); // Set yoyo range num to the multiplied value
            cursor.Emit(OpCodes.Ldarg_0); // Compensate for its usage from earlier by filling in the rest for next instruction}
        }
    }
    public sealed class Projectile_AI_099_2_ILEdit : ILEditProvider
    {
        public override void Subscribe(ManagedILEdit edit) => IL_Projectile.AI_099_2 += edit.SubscriptionWrapper;
        public override void Unsubscribe(ManagedILEdit edit) => IL_Projectile.AI_099_2 -= edit.SubscriptionWrapper;
        public override void PerformEdit(ILContext context, ManagedILEdit edit)
        {
            ILCursor cursor = new(context);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("yoyoString"))) // Go all the way down to the yoyo string section
            {
                FargowiltasSouls.Instance.Logger.Warn("Yoyo range modification 2 failure on MatchLdfld<Player>('yoyoString')");
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasSouls>(), context);
                return;
            }
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(5))) // Skip to the end of yoyo string buff
            {
                FargowiltasSouls.Instance.Logger.Warn("Yoyo range modification 2 failure on MatchStloc(5)");
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasSouls>(), context);
                return;
            }
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdloc(5))) // Use the readily existing yoyo range num from next instruction for your own benefit
            {
                FargowiltasSouls.Instance.Logger.Warn("Yoyo range modification 2 failure on MatchLdloc(5)");
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasSouls>(), context);
                return;
            }
            cursor.Emit(OpCodes.Ldarg_0); // Get the projectile instance to be used on the delegate
            cursor.EmitDelegate(ILEditUtils.GetYoyoRangeMultFromProjectile); // Calc yoyo range mult to be used
            cursor.Emit(OpCodes.Mul); // Multiply
            cursor.EmitDelegate(ILEditUtils.NormalizeMult); // Make sure this uses Ceiling
            cursor.Emit(OpCodes.Stloc_S, (byte)5); // Set yoyo range num to the multiplied value
            cursor.Emit(OpCodes.Ldloc_S, (byte)5); // Compensate for using the original one by filling in the rest for next instruction
        }
    }
    public sealed class PlayerInput_GamePadInput_ILEdit : ILEditProvider
    {
        public override void Subscribe(ManagedILEdit edit) => IL_PlayerInput.GamePadInput += edit.SubscriptionWrapper;
        public override void Unsubscribe(ManagedILEdit edit) => IL_PlayerInput.GamePadInput -= edit.SubscriptionWrapper;
        public override void PerformEdit(ILContext context, ManagedILEdit edit)
        {
            ILCursor cursor = new(context);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("yoyoString"))) // Go after Player.yoyoString bool
            {
                FargowiltasSouls.Instance.Logger.Warn("GamePad Input IL edit failure on MatchLdfld<Player>('yoyoString')");
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasSouls>(), context);
                return;
            }

            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("controlTorch"))) // Go after controlTorch bool
            {
                FargowiltasSouls.Instance.Logger.Warn("GamePad Input IL edit failure on MatchLdfld<Player>('controlTorch')");
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasSouls>(), context);
                return;
            }

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdloc(15))) // Then before the Item stack
            {
                FargowiltasSouls.Instance.Logger.Warn("GamePad Input IL edit failure on MatchLdloc(15)");
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasSouls>(), context);
                return;
            }

            cursor.Emit(OpCodes.Ldloc_3); // Push player field onto the stack
            cursor.Emit(OpCodes.Ldloc_S, (byte)15); // Push item field onto the stack
            cursor.EmitDelegate(ILEditUtils.GetYoyoRangeMultFromPlayerItem); // Get the yoyo range mod
            cursor.Emit(OpCodes.Ldloc_S, (byte)16); // Push num3, the cursor range AKA yoyo range in this case, onto the stack
            cursor.Emit(OpCodes.Mul); // Multiply cursor range with yoyo range mod
            cursor.EmitDelegate(ILEditUtils.NormalizeMult); // Make sure this uses Ceiling
            cursor.Emit(OpCodes.Stloc_S, (byte)16); // Set cursor range to the value
        }
    }
    public sealed class Projectile_Damage_ILEdit : ILEditProvider
    {
        public override void Subscribe(ManagedILEdit edit) => IL_Projectile.Damage += edit.SubscriptionWrapper;
        public override void Unsubscribe(ManagedILEdit edit) => IL_Projectile.Damage -= edit.SubscriptionWrapper;
        public override void PerformEdit(ILContext context, ManagedILEdit edit)
        {
            ILCursor cursor = new(context);
            cursor.Index = 3880; // Get as close as possible to the phantasmTime check
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Projectile>("arrow"))) // Go directly after the Projectile.arrow check
            {
                FargowiltasSouls.Instance.Logger.Warn("Phantasm Arrow Rain fix failure on MatchLdfld<Projectile>('arrow')");
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasSouls>(), context);
                return;
            }
            cursor.Emit(OpCodes.Ldarg_0); // Get Projectile instance
            cursor.EmitDelegate(ILEditUtils.ProjectileIsNotFromArrowRain); // Check whether the arrow is spawned from Red Riding Enchantment's Arrow Rain. If so, fail Phantasm's Phantom Arrow spawn.
            cursor.EmitAnd(); // Push the two bools together
        }
    }
    public sealed class Player_ItemCheck_UseMiningTools_ActuallyUseMiningTool_ILEdit : ILEditProvider
    {
        public override void Subscribe(ManagedILEdit edit) => IL_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool += edit.SubscriptionWrapper;
        public override void Unsubscribe(ManagedILEdit edit) => IL_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool -= edit.SubscriptionWrapper;
        public override void PerformEdit(ILContext context, ManagedILEdit edit)
        {
            ILCursor cursor = new(context);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdsfld<Main>("dontStarveWorld"))) // Go directly after the second Main.dontStarveWorld check
            {
                FargowiltasSouls.Instance.Logger.Warn("dontStarveWorld cactus damage immunity failure on MatchLdfld<Main>('dontStarveWorld')");
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasSouls>(), context);
                return;
            }
            cursor.Emit(OpCodes.Ldarg_0); // Get Player instance
            cursor.EmitDelegate(ILEditUtils.NotSafeFromCactusDamage); // Check if the player has Cactus Passive Effect
            cursor.EmitAnd(); // Push the two bools together
        }
    }
}