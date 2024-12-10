using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.AccessoryEffectSystem
{

    /// <summary>
    /// Contains the behavior for an accessory effect. <br/>
    /// All Toggles have a corresponding accessory effect. <br/>
    /// Each accessory effect with a toggle needs a localized toggle description as Mods.YourMod.Toggler.YourAccessoryEffectName. <br/>
    /// This type is not instanced per player. Put instanced things (such as fields) in a ModPlayer.
    /// </summary>
    public abstract class AccessoryEffect : ModType
    {
        public int Index;

        public string ToggleDescription
        {
            get
            {
                string desc = Language.GetTextValue($"Mods.{Mod.Name}.Toggler.{Name}");
                if (ToggleItemType <= 0) return desc;
                string itemIcon = $"[i:{ToggleItemType}]";
                return $"{itemIcon} {desc}";
            }
        }
        /// <summary>
        /// The toggle's header in the display. <br/>
        /// If the effect shouldn't have a toggle, set this to null.
        /// </summary>
        public abstract Header ToggleHeader { get; }
        /// <summary>
        /// The toggle's item icon in the display. <br/>
        /// If the effect shouldn't have a toggle, you don't need to set this.
        /// </summary>
        public virtual int ToggleItemType => -1;
        /// <summary>
        /// Whether the effect has a toggle. <br/>
        /// Will be false if ToggleHeader is null.
        /// </summary>
        public bool HasToggle => ToggleHeader != null;

        public virtual bool MinionEffect => false;
        public virtual bool ExtraAttackEffect => false;
        public virtual bool MutantsPresenceAffects => false;
        public virtual bool DefaultToggle => true;

        /// <summary>
        /// Whether the effect is an Active Skill.<br/>
        /// Active Skill effects implement ActiveSkillJustPressed, ActiveSkillHeld or ActiveSkillJustReleased.
        /// </summary>
        public virtual bool ActiveSkill => false;

        protected sealed override void Register()
        {
            AccessoryEffectLoader.Register(this);
            ModTypeLookup<AccessoryEffect>.Register(this);
        }
        /// <summary>
        /// The item associated with this effect. Null if none is found.
        /// </summary>
        public Item EffectItem(Player player) => player.AccessoryEffects().EffectItems[Index];
        public bool HasEffectEnchant(Player player) => EffectItem(player)?.ModItem is BaseEnchant;
        public IEntitySource GetSource_EffectItem(Player player) => player.GetSource_Accessory(EffectItem(player));

        #region Overridables
        public virtual void PreUpdate(Player player) { }
        public virtual void PostUpdateEquips(Player player) { }
        public virtual void UpdateBadLifeRegen(Player player) { }
        public virtual void PostUpdate(Player player) { }
        public virtual void PostUpdateMiscEffects(Player player) { }
        public virtual void TryAdditionalAttacks(Player player, int damage, DamageClass damageType) { }
        public virtual void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers) { }
        public virtual void ModifyHitNPCWithItem(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers) { }
        public virtual void ModifyHitNPCBoth(Player player, NPC target, ref NPC.HitModifiers modifiers, DamageClass damageClass) { }
        public virtual void ModifyHitInfo(Player player, NPC target, ref NPC.HitInfo hitInfo, DamageClass damageClass) { }
        public virtual void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnHitNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item) { }
        public virtual void MeleeEffects(Player player, Item item, Rectangle hitbox) { }
        public virtual float ModifyUseSpeed(Player player, Item item) { return 0f; }
        public virtual float ContactDamageDR(Player player, NPC npc, ref Player.HurtModifiers modifiers) { return 0f; }
        public virtual float ProjectileDamageDR(Player player, Projectile projectile, ref Player.HurtModifiers modifiers) { return 0f; }
        public virtual void ModifyHitByNPC(Player player, NPC npc, ref Player.HurtModifiers modifiers) { }
        public virtual void ModifyHitByProjectile(Player player, Projectile projectile, ref Player.HurtModifiers modifiers) { }
        public virtual void OnHitByNPC(Player player, NPC npc, Player.HurtInfo hurtInfo) { }
        public virtual void OnHitByProjectile(Player player, Projectile proj, Player.HurtInfo hurtInfo) { }
        public virtual void OnHitByEither(Player player, NPC npc, Projectile proj) { }
        public virtual bool CanBeHitByNPC(Player player, NPC npc) { return true; }
        public virtual bool CanBeHitByProjectile(Player player, Projectile projectile) { return true; }
        public virtual void ModifyHurt(Player player, ref Player.HurtModifiers modifiers) { }
        public virtual void OnHurt(Player player, Player.HurtInfo info) { }
        public virtual bool PreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource) { return true; }
        public virtual void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright) { }

        #region Active Skill methods
        public virtual void ActiveSkillJustPressed(Player player, bool stunned)
        {

        }
        public virtual void ActiveSkillHeld(Player player, bool stunned)
        {

        }
        public virtual void ActiveSkillJustReleased(Player player, bool stunned)
        {

        }
        #endregion
        #endregion


    }

}