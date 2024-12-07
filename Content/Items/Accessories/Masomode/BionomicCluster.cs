using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class BionomicCluster : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 6);
            Item.defense = 6;
            Item.useTime = 180;
            Item.useAnimation = 180;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item6;
        }

        public static void PassiveEffect(Player player, Item item)
        {
            player.buffImmune[BuffID.WindPushed] = true;
            player.buffImmune[BuffID.Suffocation] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[ModContent.BuffType<GuiltyBuff>()] = true;
            player.buffImmune[ModContent.BuffType<LoosePocketsBuff>()] = true;

            player.nightVision = true;

            player.manaMagnet = true;
            player.manaFlower = true;
            player.AddEffect<MasoCarrotEffect>(item);

            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            fargoPlayer.SandsofTime = true;
            fargoPlayer.CactusImmune = true;
            fargoPlayer.SecurityWallet = true;
            fargoPlayer.TribalCharm = true;
            fargoPlayer.NymphsPerfumeRespawn = true;
            fargoPlayer.ConcentratedRainbowMatter = true;
            player.AddEffect<RainbowHealEffect>(item);
            fargoPlayer.FrigidGemstoneItem = item;
            player.AddEffect<StabilizedGravity>(item);
        }

        public override void UpdateInventory(Player player) => PassiveEffect(player, Item);
        public override void UpdateVanity(Player player) => PassiveEffect(player, Item);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PassiveEffect(player, Item);

            FargoSoulsPlayer fargoPlayer = player.FargoSouls();

            // Concentrated rainbow matter
            player.buffImmune[ModContent.BuffType<FlamesoftheUniverseBuff>()] = true;
            player.AddEffect<RainbowSlimeMinion>(Item);

            // Dragon fang
            player.buffImmune[ModContent.BuffType<ClippedWingsBuff>()] = true;
            player.buffImmune[ModContent.BuffType<CrippledBuff>()] = true;
            player.AddEffect<ClippedEffect>(Item);

            // Frigid gemstone
            player.buffImmune[BuffID.Frostburn] = true;

            // Wretched pouch
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[ModContent.BuffType<ShadowflameBuff>()] = true;
            player.AddEffect<WretchedPouchEffect>(Item);

            // Sands of time
            player.buffImmune[BuffID.WindPushed] = true;
            fargoPlayer.SandsofTime = true;

            // Squeaky toy
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.SqueakyToyBuff>()] = true;
            player.buffImmune[ModContent.BuffType<GuiltyBuff>()] = true;
            player.AddEffect<SqueakEffect>(Item);

            // Tribal charm
            player.buffImmune[BuffID.Webbed] = true;
            player.buffImmune[ModContent.BuffType<PurifiedBuff>()] = true;
            fargoPlayer.TribalCharm = true;
            fargoPlayer.TribalCharmEquipped = true;
            player.AddEffect<TribalCharmClickBonus>(Item);

            // Mystic skull
            player.buffImmune[BuffID.Suffocation] = true;
            player.manaMagnet = true;
            player.manaFlower = true;

            // Security wallet
            player.buffImmune[ModContent.BuffType<MidasBuff>()] = true;
            fargoPlayer.SecurityWallet = true;

            // Carrot
            player.nightVision = true;
            player.AddEffect<MasoCarrotEffect>(Item);

            // Nymph's perfume
            player.buffImmune[BuffID.Lovestruck] = true;
            player.buffImmune[ModContent.BuffType<LovestruckBuff>()] = true;
            player.buffImmune[ModContent.BuffType<HexedBuff>()] = true;
            player.buffImmune[BuffID.Stinky] = true;
            fargoPlayer.NymphsPerfumeRespawn = true;
            player.AddEffect<NymphPerfumeEffect>(Item);

            // Tim's concoction
            player.AddEffect<TimsConcoctionEffect>(Item);

            // Wyvern feather
            player.AddEffect<WyvernBalls>(Item);
        }

        public override void UseItemFrame(Player player) => SandsofTime.Use(player);
        public override bool? UseItem(Player player) => true;

        public override bool AltFunctionUse(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab);
            player.ReplaceItem(Item, ModContent.ItemType<BionomicClusterInactive>());
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<ConcentratedRainbowMatter>())
            .AddIngredient(ModContent.ItemType<WyvernFeather>())
            .AddIngredient(ModContent.ItemType<FrigidGemstone>())
            .AddIngredient(ModContent.ItemType<SandsofTime>())
            .AddIngredient(ModContent.ItemType<SqueakyToy>())
            .AddIngredient(ModContent.ItemType<TribalCharm>())
            .AddIngredient(ModContent.ItemType<MysticSkull>())
            .AddIngredient(ModContent.ItemType<SecurityWallet>())
            .AddIngredient(ModContent.ItemType<OrdinaryCarrot>())
            .AddIngredient(ModContent.ItemType<WretchedPouch>())
            .AddIngredient(ModContent.ItemType<NymphsPerfume>())
            .AddIngredient(ModContent.ItemType<TimsConcoction>())
            .AddIngredient(ItemID.HallowedBar, 5)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

            .AddTile(TileID.MythrilAnvil)
            .DisableDecraft()
            .Register();
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.ReplaceItem(Item, ModContent.ItemType<BionomicClusterInactive>());
        }
    }
    public class BionomicClusterInactive : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 0;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 6);
            Item.defense = 6;
            Item.useTime = 180;
            Item.useAnimation = 180;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item6;
        }

        public static void PassiveEffect(Player player, Item item)
        {
            player.buffImmune[BuffID.WindPushed] = true;
            player.buffImmune[BuffID.Suffocation] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[ModContent.BuffType<GuiltyBuff>()] = true;
            player.buffImmune[ModContent.BuffType<LoosePocketsBuff>()] = true;

            player.nightVision = true;

            player.manaMagnet = true;
            player.manaFlower = true;
            player.AddEffect<MasoCarrotEffect>(item);

            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            fargoPlayer.SandsofTime = true;
            fargoPlayer.SecurityWallet = true;
            fargoPlayer.TribalCharm = true;
            fargoPlayer.NymphsPerfumeRespawn = true;
            fargoPlayer.ConcentratedRainbowMatter = true;
            player.AddEffect<RainbowHealEffect>(item);
            fargoPlayer.FrigidGemstoneItem = item;
            player.AddEffect<StabilizedGravity>(item);
        }

        public override void UpdateInventory(Player player) { return; }//PassiveEffect(player, Item);
        public override void UpdateVanity(Player player) { return; }//PassiveEffect(player, Item);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PassiveEffect(player, Item);

            FargoSoulsPlayer fargoPlayer = player.FargoSouls();

            // Concentrated rainbow matter
            player.buffImmune[ModContent.BuffType<FlamesoftheUniverseBuff>()] = true;
            player.AddEffect<RainbowSlimeMinion>(Item);

            // Dragon fang
            player.buffImmune[ModContent.BuffType<ClippedWingsBuff>()] = true;
            player.buffImmune[ModContent.BuffType<CrippledBuff>()] = true;
            player.AddEffect<ClippedEffect>(Item);

            // Frigid gemstone
            player.buffImmune[BuffID.Frostburn] = true;

            // Wretched pouch
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[ModContent.BuffType<ShadowflameBuff>()] = true;
            player.AddEffect<WretchedPouchEffect>(Item);

            // Sands of time
            player.buffImmune[BuffID.WindPushed] = true;
            fargoPlayer.SandsofTime = true;

            // Squeaky toy
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.SqueakyToyBuff>()] = true;
            player.buffImmune[ModContent.BuffType<GuiltyBuff>()] = true;
            player.AddEffect<SqueakEffect>(Item);

            // Tribal charm
            player.buffImmune[BuffID.Webbed] = true;
            player.buffImmune[ModContent.BuffType<PurifiedBuff>()] = true;
            fargoPlayer.TribalCharm = true;
            fargoPlayer.TribalCharmEquipped = true;
            player.AddEffect<TribalCharmClickBonus>(Item);

            // Mystic skull
            player.buffImmune[BuffID.Suffocation] = true;
            player.manaMagnet = true;
            player.manaFlower = true;

            // Security wallet
            player.buffImmune[ModContent.BuffType<MidasBuff>()] = true;
            fargoPlayer.SecurityWallet = true;

            // Carrot
            player.nightVision = true;
            player.AddEffect<MasoCarrotEffect>(Item);

            // Nymph's perfume
            player.buffImmune[BuffID.Lovestruck] = true;
            player.buffImmune[ModContent.BuffType<LovestruckBuff>()] = true;
            player.buffImmune[ModContent.BuffType<HexedBuff>()] = true;
            player.buffImmune[BuffID.Stinky] = true;
            fargoPlayer.NymphsPerfumeRespawn = true;
            player.AddEffect<NymphPerfumeEffect>(Item);

            // Tim's concoction
            player.AddEffect<TimsConcoctionEffect>(Item);
        }

        public override void UseItemFrame(Player player) => SandsofTime.Use(player);
        public override bool? UseItem(Player player) => true;

        public override bool AltFunctionUse(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab);
            player.ReplaceItem(Item, ModContent.ItemType<BionomicCluster>());
            return false;
        }

        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.ReplaceItem(Item, ModContent.ItemType<BionomicCluster>());
        }
    }
}
