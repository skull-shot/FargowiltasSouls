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
    public class ManaOverlays : ModResourceOverlay
    {
        private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

        private string AssetString()
        {
            if (Main.LocalPlayer.FargoSouls().MagicSoul)
            {
                return "Archmage";
            }
            /*if (Main.LocalPlayer.FargoSouls().UniverseSoul)
            {
                return "Universe";
            }*/
            return string.Empty;
        }
        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> asset = context.texture;

            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

            if (Main.LocalPlayer.FargoSouls().MagicSoul != true)
                return;
            
            if (AssetString() != string.Empty)
            {
                if (asset == TextureAssets.Mana)
                {
                    context.texture = FargoAssets.GetTexture2D("UI/ResourceOverlays", AssetString() + "Mana");
                    context.Draw();
                }
                else if (CompareAssets(asset, fancyFolder + "Star_Fill"))
                {
                    context.texture = FargoAssets.GetTexture2D("UI/ResourceOverlays", AssetString() + "FancyMana");
                    context.Draw();
                }
                else if (CompareAssets(asset, barsFolder + "MP_Fill"))
                {
                    context.texture = FargoAssets.GetTexture2D("UI/ResourceOverlays", AssetString() + "BarMana");
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
