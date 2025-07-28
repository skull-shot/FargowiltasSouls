using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Accessories.DubiousCircuitry;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class GroundStick : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<RemoteLightningEffect>()];
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Electrified] = true;
            player.buffImmune[ModContent.BuffType<LightningRodBuff>()] = true;
            player.AddEffect<GroundStickDR>(Item);
            player.AddEffect<ProbeMinionEffect>(Item);
            player.AddEffect<RemoteLightningEffect>(Item);
        }
    }
    public class GroundStickDR : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DubiousHeader>();
        public override int ToggleItemType => ModContent.ItemType<GroundStick>();
        public override void ModifyHitByProjectile(Player player, Projectile projectile, ref Player.HurtModifiers modifiers)
        {
            float dr = 0;
            bool lightningskill = projectile.type == ModContent.ProjectileType<RemoteLightning>() || projectile.type == ModContent.ProjectileType<RemoteLightningExplosion>();
            if (projectile.FargoSouls().electricAttack && player.whoAmI == Main.myPlayer && !player.HasBuff<SuperchargedBuff>() || lightningskill)
            {
                dr = 0.5f;
                int duration = projectile.originalDamage * 30;
                if (duration > 3600)
                    duration = 3600;
                if (lightningskill)
                {
                    if (Main.masterMode) duration = 2700;
                    else if (Main.expertMode) duration = 1800;
                    else duration = 1200;
                }
                player.AddBuff(ModContent.BuffType<SuperchargedBuff>(), duration);
                SoundEngine.PlaySound(SoundID.NPCDeath6, player.Center);
                SoundEngine.PlaySound(SoundID.Item92, player.Center);
                SoundEngine.PlaySound(SoundID.Item14, player.Center);

                for (int i = 0; i < duration / 80; i++)
                {
                    Vector2 dir = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) - player.velocity / 10;
                    Vector2 spd = Main.rand.NextVector2Circular(15, 15);
                    Particle p = new ElectricSpark(player.Center + dir * 10, spd, Color.Teal, Main.rand.NextFloat(2f, 4f), 120);
                    p.Spawn();
                }
            }

            modifiers.FinalDamage *= 1 - dr;
        }
    }
    public class ProbeMinionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DubiousHeader>();
        public override int ToggleItemType => ModContent.ItemType<GroundStick>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            player.FargoSouls().Probes = true;
            if (player.whoAmI == Main.myPlayer && player.HasEffect<ProbeMinionEffect>() && player.FargoSouls().Supercharged)
            {
                player.FargoSouls().Probes = true;
                const int damage = 30;
                const int max = 3;
                float rotation = 2f * (float)Math.PI / max;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<RemoteProbe>()] < 3)
                {
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                        FargoSoulsUtil.NewSummonProjectile(GetSource_EffectItem(player), spawnPos, Vector2.Zero, ModContent.ProjectileType<RemoteProbe>(), damage, 10f, player.whoAmI, rotation * i);
                    }
                }
            }
            else
                player.FargoSouls().Probes = false;
        }
    }
    public class RemoteLightningEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DubiousHeader>();
        public override int ToggleItemType => ModContent.ItemType<GroundStick>();
        public override bool ActiveSkill => Main.LocalPlayer.HasEffect<RemoteLightningEffect>();
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            if (stunned)
                return;
            if (player.FargoSouls().RemoteCD > 0)
                return;
            if (Main.myPlayer == player.whoAmI)
            {
                int dmg = (int)(800 * player.ActualClassDamage(DamageClass.Ranged));
                Vector2 pos = new(Main.MouseWorld.ToTileCoordinates().X * 16 + 8, player.Center.Y - 575);
                float angle = MathHelper.Pi * 0.7f;
                Projectile.NewProjectile(GetSource_EffectItem(player), pos, Vector2.Zero, ModContent.ProjectileType<RemoteScanTelegraph>(), dmg, 0f, Main.myPlayer, 0, angle, 1000);
            }
            player.FargoSouls().RemoteCD = 720;
            CooldownBarManager.Activate("RemoteCD", ModContent.Request<Texture2D>("FargowiltasSouls/Content/Items/Accessories/Masomode/GroundStick").Value, Color.Lerp(Color.Gray, Color.DarkOliveGreen, 0.25f), () => Main.LocalPlayer.FargoSouls().RemoteCD / 720f, activeFunction: null);
        }
    }
}