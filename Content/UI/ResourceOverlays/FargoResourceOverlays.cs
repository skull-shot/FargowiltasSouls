using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.UI.ResourceOverlays
{   
    public class HealthOverlays : ModResourceOverlay
    {
        private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

        private string AssetString()
        {
            if (Main.LocalPlayer.FargoSouls().DimensionSoul)
            {
                return "SoD";
            }
            if (Main.LocalPlayer.FargoSouls().ColossusSoul)
            {
                return "Colossus";
            }
            return string.Empty;
        }
        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> asset = context.texture;

            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

            if (Main.LocalPlayer.FargoSouls().ColossusSoul != true && Main.LocalPlayer.FargoSouls().DimensionSoul != true)
                return;

            if (AssetString() != string.Empty)
            {
                if (asset == TextureAssets.Heart || asset == TextureAssets.Heart2)
                {
                    context.texture = FargoAssets.GetTexture2D("UI/ResourceOverlays", AssetString() + "Heart", AssetRequestMode.ImmediateLoad).Asset;
                    context.Draw();
                }
                else if (CompareAssets(asset, fancyFolder + "Heart_Fill") || CompareAssets(asset, fancyFolder + "Heart_Fill_B"))
                {
                    context.texture = FargoAssets.GetTexture2D("UI/ResourceOverlays", AssetString() + "FancyHeart", AssetRequestMode.ImmediateLoad).Asset;
                    context.Draw();
                }
                else if (CompareAssets(asset, barsFolder + "HP_Fill") || CompareAssets(asset, barsFolder + "HP_Fill_Honey"))
                {
                    context.texture = FargoAssets.GetTexture2D("UI/ResourceOverlays", AssetString() + "BarHeart", AssetRequestMode.ImmediateLoad).Asset;
                    context.Draw();
                }
            }
        }
        private bool CompareAssets(Asset<Texture2D> existingAsset, string compareAssetPath)
        {
            // This is a helper method for checking if a certain vanilla asset was drawn
            if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
                asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);

            return existingAsset == asset;
        }
    }
    public class ManaResourceOverlays : ModResourceOverlay
    {
        private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

        private string AssetString()
        {
            if (Main.LocalPlayer.FargoSouls().UniverseSoul)
            {
                return "SoTU";
            }
            if (Main.LocalPlayer.FargoSouls().MagicSoul)
            {
                return "ArchMage";
            }
            return string.Empty;
        }
        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> asset = context.texture;

            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

            if (Main.LocalPlayer.FargoSouls().MagicSoul != true && Main.LocalPlayer.FargoSouls().UniverseSoul != true)
                return;
            
            if (AssetString() != string.Empty)
            {
                if (asset == TextureAssets.Mana)
                {
                    context.texture = FargoAssets.GetTexture2D("UI/ResourceOverlays", AssetString() + "Mana", AssetRequestMode.ImmediateLoad).Asset;
                    context.Draw();
                }
                else if (CompareAssets(asset, fancyFolder + "Star_Fill"))
                {
                    context.texture = FargoAssets.GetTexture2D("UI/ResourceOverlays", AssetString() + "FancyMana", AssetRequestMode.ImmediateLoad).Asset;
                    context.Draw();
                }
                else if (CompareAssets(asset, barsFolder + "MP_Fill"))
                {
                    context.texture = FargoAssets.GetTexture2D("UI/ResourceOverlays", AssetString() + "BarMana", AssetRequestMode.ImmediateLoad).Asset;
                    context.Draw();
                }
            }
        }
        private bool CompareAssets(Asset<Texture2D> existingAsset, string compareAssetPath)
        {
            // This is a helper method for checking if a certain vanilla asset was drawn
            if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
                asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);

            return existingAsset == asset;
        }
    }
}
