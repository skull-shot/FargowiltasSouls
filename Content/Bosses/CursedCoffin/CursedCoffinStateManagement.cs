using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.WorldGeneration;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.StateMachines;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.CursedCoffin
{
	public partial class CursedCoffin
	{
        private PushdownAutomata<EntityAIState<BehaviorStates>, BehaviorStates> stateMachine;

        /// <summary>
        /// The state machine that controls the behavior of this NPC.
        /// </summary>
        public PushdownAutomata<EntityAIState<BehaviorStates>, BehaviorStates> StateMachine
        {
            get
            {
                if (stateMachine == null)
                    LoadStateMachine();

                return stateMachine;
            }
            private set => stateMachine = value;
        }

		private void LoadStateMachine()
		{
			StateMachine = new(new(BehaviorStates.Opening));

			for (int i = 0; i < (int)BehaviorStates.Count; i++)
				StateMachine.RegisterState(new((BehaviorStates)i));

			StateMachine.OnStateTransition += OnStateTransition;
            StateMachine.OnStackEmpty += OnStackEmpty;

            // Autoload the state behaviors.
            AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>.FillStateMachineBehaviors<ModNPC>(StateMachine, this);

            // Load the phase transition.
            LoadTransition_PhaseTwoTransition();

            // Register each attack transition.
            #region Transition Registering
            // A basic example of a transition that doesnt go to anything specific here, it requires the current state to be the opening one, and the timer to be -1 to occur.
            StateMachine.RegisterTransition(BehaviorStates.Opening, null, false, () => Timer == -1);

			// An example of a more complex transition, where it goes from the phase 2 transition to the slam shockwave if the condition is met, and also performs additional stuff on occuring.
			StateMachine.RegisterTransition(BehaviorStates.PhaseTransition, BehaviorStates.SlamWShockwave, false, () => Timer >= 90, () =>
			{
				SoundEngine.PlaySound(PhaseTransitionSFX, NPC.Center);
				NPC.netUpdate = true;
				if (FargoSoulsUtil.HostCheck)
				{
					Vector2 maskCenter = MaskCenter();
					NPC.NewNPC(NPC.GetSource_FromAI(), (int)maskCenter.X, (int)maskCenter.Y, ModContent.NPCType<CursedSpirit>(), ai0: NPC.whoAmI);
				}
				NPC.velocity = Vector2.UnitY * 0.1f;
				LockVector1 = Player.Top - Vector2.UnitY * 250;
                AI2 = 3; // only slam once
            });

			// An example of using this function to apply a transition to a bunch of states at once, in this case for an interrupting attack.
			StateMachine.ApplyToAllStatesExcept((state) =>
			{
				StateMachine.RegisterTransition(state, BehaviorStates.StunPunish, false, () => 
				{
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        return false;
                    return Main.player.Any(p => p.Alive() && p.HasBuff(BuffID.Dazed) && !p.HasBuff<GrabbedBuff>()) && !Main.projectile.Any(p => p.TypeAlive<CoffinHand>());
				});
			}, BehaviorStates.StunPunish, BehaviorStates.PhaseTransition, BehaviorStates.YouCantEscape, BehaviorStates.SpiritGrabPunish);

			// Pull back if you go outside the arena
            StateMachine.ApplyToAllStatesExcept((state) =>
            {
                StateMachine.RegisterTransition(state, BehaviorStates.YouCantEscape, false, () =>
                {
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        return false;
					return Main.player.Any(p => p.Alive() && !CoffinArena.PaddedRectangle.Contains(p.Center.ToTileCoordinates()) && !p.HasBuff<GrabbedBuff>());
                });
            }, BehaviorStates.StunPunish, BehaviorStates.PhaseTransition, BehaviorStates.YouCantEscape, BehaviorStates.SpiritGrabPunish);

            // Same as above, for spirit grab punish
            StateMachine.ApplyToAllStatesExcept((state) =>
            {
                StateMachine.RegisterTransition(state, BehaviorStates.SpiritGrabPunish, false, () => ForceGrabPunish != 0, () => ForceGrabPunish = 0);
            }, BehaviorStates.StunPunish, BehaviorStates.PhaseTransition, BehaviorStates.YouCantEscape, BehaviorStates.SpiritGrabPunish);

            StateMachine.RegisterTransition(BehaviorStates.WavyShotCircle, BehaviorStates.WavyShotSlam, false, () =>
            {
                int telegraphTime = WorldSavingSystem.MasochistModeReal ? 60 : 70;
                bool phase2InitialCondition = Timer > telegraphTime + (WorldSavingSystem.MasochistModeReal || AI3 < 1 ? 20 : 50);
                bool phase2SecondaryCondition = AI3 < 1 && WorldSavingSystem.MasochistModeReal;
                return phase2InitialCondition && !phase2SecondaryCondition;
            },
            () =>
            {
                Frame = 0;
                NPC.velocity.X /= 2;
                NPC.velocity.Y = -6;
            });

            StateMachine.RegisterTransition(BehaviorStates.GrabbyHands, BehaviorStates.SlamWShockwave, false, () => Timer > 72 && Frame <= 0 && Timer > AI3 + 1, () =>
            {
                NPC.noTileCollide = true;
                LockVector1 = Player.Top - Vector2.UnitY * 250;
                NPC.velocity.Y = -4;
                NPC.velocity.X /= 2;
                if (NPC.velocity.X.NonZeroSign() != NPC.HorizontalDirectionTo(Player.Center))
                    NPC.velocity.X = 0;
            });

            #region End-of-sequence attacks

            StateMachine.RegisterTransition(BehaviorStates.SpiritGrabPunish, BehaviorStates.SlamWShockwave, false, () => Timer > 70, () =>
            {
                NPC.noTileCollide = true;
                LockVector1 = Player.Top - Vector2.UnitY * 250;
                NPC.velocity = Vector2.Zero;
                NPC.velocity.Y = -2;
                AI2 = 3; // only slam once
                
            });

            StateMachine.RegisterTransition(BehaviorStates.HoveringForSlam, BehaviorStates.SlamWShockwave, false, () => Timer > 1 && Timer == AI3, () =>
            {
                NPC.velocity.Y = -5;
                NPC.velocity.X /= 2;
                LockVector1 = Player.Top - Vector2.UnitY * 250;
                AI2 = 0;
            });

            StateMachine.RegisterTransition(BehaviorStates.StunPunish, null, false, () => Timer > 20 && Frame <= 0, () =>
			{
				NPC.frameCounter = 0;
				Frame = 0;
                IncrementAttackCounter();

            });

            StateMachine.RegisterTransition(BehaviorStates.YouCantEscape, null, false, () => Timer > 20 && Frame <= 0, () =>
            {
                NPC.frameCounter = 0;
                Frame = 0;
                IncrementAttackCounter();
            });

			StateMachine.RegisterTransition(BehaviorStates.SlamWShockwave, null, false, () => !Enraged && Timer == -1, IncrementAttackCounter);

			StateMachine.RegisterTransition(BehaviorStates.WavyShotSlam, null, false, () => Timer == -1, IncrementAttackCounter);

			StateMachine.RegisterTransition(BehaviorStates.RandomStuff, null, false, () => Timer > RandomStuffOpenTime + 310 && Frame <= 0, () =>
			{
				NPC.velocity = Vector2.Zero;
				NPC.rotation = 0;
				NPC.frameCounter = 0;
				Frame = 0;
                AttackCounter = 0;
            });


            #endregion
            #endregion
        }

		// This is ran whenever a state transition occures and is very useful for resetting variables.
		public void OnStateTransition(bool stateWasPopped, EntityAIState<BehaviorStates> oldState)
		{
			NPC.netUpdate = true;
			NPC.TargetClosest(false);
			AI2 = 0;
			AI3 = 0;

			if (oldState != null && Attacks.Contains(oldState.Identifier))
				LastAttackChoice = (int)oldState.Identifier;

        }
        // This is ran when the stack runs out of attacks.
        public void OnStackEmpty()
        {
            NPC.netUpdate = true;

            if (!FargoSoulsUtil.HostCheck)
                return;

            StateMachine.StateStack.Clear();

            // Get the correct attack list, and remove the last attack to avoid repeating it.
            List<BehaviorStates> attackList = Attacks.Where(attack => attack != (BehaviorStates)LastAttackChoice && !(attack == BehaviorStates.RandomStuff && !WorldSavingSystem.EternityMode)).ToList();

            // Fill a list of indices.
            var indices = new List<int>();
            for (int i = 0; i < attackList.Count; i++)
                indices.Add(i);

            // Randomly push the attack list using the indices list accessed with a random index.
            for (int i = 0; i < attackList.Count; i++)
            {
                var currentIndex = indices[Main.rand.Next(0, indices.Count)];
                StateMachine.StateStack.Push(StateMachine.StateRegistry[attackList[currentIndex]]);
                indices.Remove(currentIndex);
            }
        }
        public void LoadTransition_PhaseTwoTransition()
        {
            // Example of a transition hijack, which is checked for any possible starting state when a transition occurs and will replace any normal transition
            // if it returns anything other than the provided state.
            StateMachine.AddTransitionStateHijack(originalState =>
            {
                NPC.netUpdate = true;
                if (Phase < 3 && Enraged)
                {
                    StateMachine.StateStack.Clear();
                    return BehaviorStates.HoveringForSlam;
                }
                // Phase 2: ghost
                float threshold = WorldSavingSystem.MasochistModeReal ? 0.75f : WorldSavingSystem.EternityMode ? 0.6f : 0.5f;
                // Transition to phase 2 if required.
                if (Phase < 2 && NPC.GetLifePercent() <= threshold)
                {
                    // Clear the stack to ensure states do not linger.
                    StateMachine.StateStack.Clear();
                    return BehaviorStates.PhaseTransition;
                }

                return originalState;
            });
        }
        public void IncrementAttackCounter()
        {
            /*
            // Normal mode doesn't do the mechanic
            if (Main.expertMode && PhaseTwo)
                AttackCounter++;
            if (AttackCounter > 6) // interrupted during random stuff attack, reset anyway
            {
                AttackCounter = 0;
                return;
            }
            if (AttackCounter == 6)
            {
                if (WorldSavingSystem.EternityMode) // Special emode attack
                    StateMachine.StateStack.Push(StateMachine.StateRegistry[BehaviorStates.RandomStuff]);
                else
                    AttackCounter = 0;
            }
            */
        }
	}
}