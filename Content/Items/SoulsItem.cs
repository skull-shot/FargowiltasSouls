using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls.Content.Items
{
    /// <summary>
    /// Abstract class extended by the items of this mod. <br />
    /// Contains useful code for boilerplate reduction.
    /// </summary>
    public abstract class SoulsItem : ModItem
    {
        /// <summary>
        /// Whether or not this item is excluse to Eternity Mode. <br />
        /// If it is, the item's text color will automatically be set to a custom color (can manually be overriden) and "Eternity" will be added to the end of the item's tooltips.
        /// </summary>
        public virtual bool Eternity => false;

        /// <summary>
        /// A list of articles that this item may begin with depending on localization. <br />
        /// Used for the prefix-article fix.
        /// </summary>
        public virtual List<string> Articles => ["The"];

        /// <summary>
        /// Allows you to modify all the tooltips that display for this item. <br />
        /// Called directly after the code in <see cref="SafeModifyTooltips(List{TooltipLine})"/>.
        /// </summary>
        /// <param name="tooltips"></param>
        public virtual void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
        }

        /// <summary>
        /// Return a number greater than 0 to make the item auto-generate a tooltip that shows its damage value. Meant for accessories.
        /// </summary>
        public virtual int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Generic;
            tooltipColor = null;
            scaling = null;
            return 0;
        }

        /// <summary>
        /// The location of the item's glowmask texture, defaults to the item's internal texture name with _glow
        /// </summary>
        public virtual string GlowmaskTexture => Texture + "_glow";

        /// <summary>
        /// The amount of frames in the item's animation. <br />
        /// </summary>
        public virtual int NumFrames => 1;

        /// <summary>
        /// Whether this item currently has togglable effects that are disabled. Used for tooltip. <br />
        /// </summary>
        public bool HasDisabledEffects = false;
        public virtual List<AccessoryEffect> ActiveSkillTooltips => [];

        /// <summary>
        /// Allows you to draw things in front of this item. This method is called even if PreDrawInWorld returns false. <br />
        /// Runs directly after the code for PostDrawInWorld in SoulsItem.
        /// </summary>
        public virtual void SafePostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) 
        { 
        }

        public sealed override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {   
            if (ModContent.RequestIfExists(GlowmaskTexture, out Asset<Texture2D> asset, AssetRequestMode.ImmediateLoad))
            {
                Texture2D texture = asset.Value;
                Rectangle frame = NumFrames > 1 ? Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]) : texture.Frame();
                Vector2 origin = frame.Size() / 2f;
                Vector2 DrawCenter = Item.Bottom - Main.screenPosition - new Vector2(0, origin.Y);

                Main.EntitySpriteDraw(texture, DrawCenter, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0);
            }
            SafePostDrawInWorld(spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
        }


        public sealed override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tooltips.TryFindTooltipLine("ItemName", out TooltipLine itemNameLine))
            {
                // This is often overridden.
                if (Eternity)
                    itemNameLine.OverrideColor = FargowiltasSouls.EModeColor();

                // Call the artcle-prefix adjustment method.
                // This automatically handles fixing item names that begin with an article.
                //itemNameLine.ArticlePrefixAdjustment(Articles.ToArray());
            }

            string vanityKey = $"Mods.{Mod.Name}.Items.{Name}.VanityTooltip";
            if (Language.Exists(vanityKey))
            {
                if (tooltips.FindIndex(line => line.Name == "SocialDesc") is int socialIndex && socialIndex != -1)
                {
                    tooltips.RemoveAt(socialIndex);
                    tooltips.Insert(socialIndex, new TooltipLine(Mod, "SoulsVanityTooltip", Language.GetTextValue(vanityKey)));
                }
            }

            SafeModifyTooltips(tooltips);

            // Add the Eternity toolip after tooltip modification in order to be displayed underneath any manual tooltips (i.e. SoE cycling).
            if (Eternity)
                tooltips.Add(new TooltipLine(Mod, $"{Mod.Name}:Eternity", Language.GetTextValue($"Mods.FargowiltasSouls.Items.Extra.EternityItem")));
            if (HasDisabledEffects)
            {
                string text = $"[i:{ModContent.ItemType<TogglerIconItem>()}] [c/BC5252:{Language.GetTextValue($"Mods.FargowiltasSouls.Items.Extra.DisabledEffects")}]";
                tooltips.Add(new TooltipLine(Mod, $"{Mod.Name}:DisabledEffects", text));
            }
            int damage = DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling);
            if (damage > 0)
            {
                int firstTooltip = tooltips.FindIndex(line => line.Name == "Tooltip0");
                if (firstTooltip > 0)
                {
                    string tip = "";
                    if (scaling != null)
                    {
                        tip += " " + Language.GetTextValue("Mods.FargowiltasSouls.Items.Extra.Scaling");
                    }
                    if (damageClass == DamageClass.Generic)
                    {
                        tip += " " + Language.GetTextValue("Mods.FargowiltasSouls.Items.Extra.HighestClass");
                    }
                    else if (damageClass == DamageClass.MagicSummonHybrid)
                    {
                        tip += " " + Language.GetTextValue("Mods.FargowiltasSouls.Items.Extra.MagicSummonHybrid");
                    }
                    else if (damageClass == DamageClass.SummonMeleeSpeed)
                    {
                        tip += " " + Language.GetTextValue("Mods.FargowiltasSouls.Items.Extra.MeleeSummonHybrid");
                    }
                    else if (damageClass != null)
                    {
                        bool addLeadingSpace = damageClass is not VanillaDamageClass;
                        tip += addLeadingSpace ? " " : "" + damageClass.DisplayName;
                    }
                    else
                    {
                        tip += Lang.tip[55].Value; // No damage class
                    }
                    string damageText = damage.ToString();
                    if (scaling != null)
                        damageText += "%";
                    string text = damageText + tip;
                    if (scaling != null && Main.LocalPlayer.HeldItem.IsWeapon())
                        text += $" ({scaling})";
                    var damageTooltip = new TooltipLine(Mod, $"{Mod.Name}:DamageTooltip", text);
                    if (tooltipColor.HasValue)
                        damageTooltip.OverrideColor = tooltipColor;
                    else if (damageClass != null)
                        damageTooltip.OverrideColor = DamageClassColor(damageClass);
                    tooltips.Insert(firstTooltip, damageTooltip);
                }
            }
            int activeSkills = ActiveSkillTooltips.Count;
            if (activeSkills > 0)
            {
                int firstTooltip = tooltips.FindIndex(line => line.Name == "Tooltip0");
                if (firstTooltip >= 0)
                {
                    string names = "";
                    string description = "";
                    for (int i = 0; i < activeSkills; i++)
                    {
                        var skill = ActiveSkillTooltips[i];
                        if (i == 0)
                            description = Language.GetTextValue($"Mods.{skill.Mod.Name}.ActiveSkills.{skill.Name}.Tooltip");
                        else
                            names += ", ";
                        names += Language.GetTextValue($"Mods.{skill.Mod.Name}.ActiveSkills.{skill.Name}.DisplayName");

                    }
                    string nameLoc = "GrantsSkillsPlural";
                    if (activeSkills == 1)
                        nameLoc = "GrantsSkill";
                    string nameText = Language.GetTextValue($"Mods.FargowiltasSouls.ActiveSkills.{nameLoc}");
                    string key = $"[{Language.GetTextValue("Mods.FargowiltasSouls.ActiveSkills.Unbound")}]";
                    var keys = FargowiltasSouls.ActiveSkillMenuKey.GetAssignedKeys();
                    if (keys.Count > 0)
                        key = keys[0];
                    string keybindMenuText = Language.GetTextValue("Mods.FargowiltasSouls.ActiveSkills.KeybindMenu", key);
                    string boundText = "";
                    if (activeSkills == 1)
                    {
                        boundText = Language.GetTextValue("Mods.FargowiltasSouls.ActiveSkills.Unbound") + " ";
                        var boundSkills = Main.LocalPlayer.FargoSouls().ActiveSkills;
                        for (int i = 0; i < boundSkills.Length; i++)
                        {
                            if (boundSkills[i] == ActiveSkillTooltips[0])
                            {
                                var skillKeys = FargowiltasSouls.ActiveSkillKeys[i].GetAssignedKeys();
                                if (skillKeys.Count > 0)
                                    boundText = Language.GetTextValue("Mods.FargowiltasSouls.ActiveSkills.BoundTo", skillKeys[0]) + " ";
                            }
                        }
                    }

                    var namesTooltip = new TooltipLine(Mod, $"{Mod.Name}:ActiveSkills", nameText + " " + names);
                    var bindTooltip = new TooltipLine(Mod, $"{Mod.Name}:ActiveSkillBind", boundText + keybindMenuText);
                    var descTooltip = new TooltipLine(Mod, $"{Mod.Name}:ActiveSkillTooltip", description);

                    Color color1 = Color.Lerp(Color.Blue, Color.LightBlue, 0.7f);
                    Color color2 = Color.LightBlue;
                    namesTooltip.OverrideColor = color1;
                    bindTooltip.OverrideColor = color1;
                    descTooltip.OverrideColor = color2;
                    tooltips.Insert(firstTooltip, namesTooltip);
                    tooltips.Insert(firstTooltip + 1, bindTooltip);
                    if (activeSkills == 1)
                        tooltips.Insert(firstTooltip + 2, descTooltip);
                }
            }
        }

        public static Color DamageClassColor(DamageClass damageClass)
        {
            Color color = Color.LightGray;
            float lerp = 0.75f;
            if (damageClass.CountsAsClass(DamageClass.Melee))
                return Color.Lerp(new(225, 90, 90), color, lerp);
            if (damageClass.CountsAsClass(DamageClass.Ranged))
                return Color.Lerp(new(38, 168, 35), color, lerp);
            if (damageClass.CountsAsClass(DamageClass.Magic))
                return Color.Lerp(new(204, 45, 239), color, lerp);
            if (damageClass.CountsAsClass(DamageClass.Summon))
                return Color.Lerp(new(0, 80, 224), color, lerp);
            if (damageClass.CountsAsClass(DamageClass.Default) || damageClass.CountsAsClass(DamageClass.Generic))
                return color;
            return Color.White;
        }
    }
}