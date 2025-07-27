using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

// Base namespace for convinience
namespace FargowiltasSouls.Assets.ExtraTextures
{
    public static class FargoAssets
    {
        /// <summary>
        /// Grabs the asset string of a texture.
        /// </summary>
        /// <param name="path">The folder structure associated with the asset.</param>
        /// <param name="name">The name of the asset.</param>
        public static string GetAssetString(string path, string name) => $"FargowiltasSouls/Assets/{path}/{name}";



        #region Additive Textures
        public static Asset<Texture2D> BlobBloomTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/BlobGlow");
        public static Asset<Texture2D> BloomTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/Bloom");
        public static Asset<Texture2D> BloomParticleTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/BloomParticle");
        public static Asset<Texture2D> BloomFlareTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/BloomFlare");
		public static Asset<Texture2D> DeviBorderTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/DeviBorder");
        public static Asset<Texture2D> HardEdgeRing => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/HardEdgeRing");
        public static Asset<Texture2D> SoftEdgeRing => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/SoftEdgeRing");
        public static Asset<Texture2D> Scorch => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/Scorch");
        public static Asset<Texture2D> Smoke => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/Smoke");
        #endregion

        #region Misc Shader Textures
        public static Asset<Texture2D> DeviRingTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/MiscShaderTextures/Ring1");
        public static Asset<Texture2D> DeviRing2Texture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/MiscShaderTextures/Ring2");
        public static Asset<Texture2D> DeviRing3Texture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/MiscShaderTextures/Ring3");
        public static Asset<Texture2D> DeviRing4Texture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/MiscShaderTextures/Ring4");
        #endregion

        #region Noise
        public static Asset<Texture2D> PerlinNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/BlurryPerlinNoise");
        public static Asset<Texture2D> ColorNoiseMap => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/ColorNoiseMap");

        public static Asset<Texture2D> CracksNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/CracksNoise");
        public static Asset<Texture2D> CrustyNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/CrustyNoise");

        public static Asset<Texture2D> DottedNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/DottedNoise");
        public static Asset<Texture2D> HarshNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/HarshNoise");
        public static Asset<Texture2D> HoneycombNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/HoneycombNoise");
        public static Asset<Texture2D> LessCrustyNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/LessCrustyNoise");

        public static Asset<Texture2D> SandyNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/SandyNoise");
        public static Asset<Texture2D> SmokyNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/SmokyNoise");
        public static Asset<Texture2D> TurbulentNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/TurbulentNoise");
        public static Asset<Texture2D> WavyNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/WavyNoise");
        public static Asset<Texture2D> Techno1Noise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/Techno1Noise");

        #endregion

        #region Trails
        public static Asset<Texture2D> DeviBackStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/DevBackStreak");
        public static Asset<Texture2D> DeviInnerStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/DevInnerStreak");
        public static Asset<Texture2D> FadedGlowStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/FadedGlowStreak");
        public static Asset<Texture2D> FadedStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/FadedStreak");
        public static Asset<Texture2D> FadedThinGlowStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/FadedThinGlowStreak");
        public static Asset<Texture2D> GenericStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/GenericStreak");
        public static Asset<Texture2D> MagmaStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/MagmaStreak");
        public static Asset<Texture2D> MutantStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/MutantStreak");
        public static Asset<Texture2D> WillStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/WillStreak");
        #endregion

        public class UI
        {   
            public static Asset<Texture2D> CooldownBarTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CooldownBar", AssetRequestMode.ImmediateLoad);
            public static Asset<Texture2D> CooldownBarFillTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CooldownBarFill", AssetRequestMode.ImmediateLoad);
            public static Asset<Texture2D> OncomingMutantnt => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/OncomingMutantnt", AssetRequestMode.ImmediateLoad);
            public static Asset<Texture2D> OncomingMutantTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/OncomingMutant", AssetRequestMode.ImmediateLoad);
            public static Asset<Texture2D> OncomingMutantAura => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/OncomingMutantAura", AssetRequestMode.ImmediateLoad);

            public static Asset<Texture2D> EnchantSlotIcon => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/EnchantSlotIcon", AssetRequestMode.ImmediateLoad);

            public class MainMenu
            {
                public static Asset<Texture2D> MenuLogo => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/MenuLogo", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> MenuLogoGlow => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/MenuLogo_Glow", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> ForgorMenuLogo => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/ForgorMenuLogo", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> ForgorMenuLogoGlow => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/ForgorMenuLogo_Glow", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> TitleLinkButtons => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/TitleLinkButtons", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> MutantWorldBorder => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/MutantWorldBorder", AssetRequestMode.ImmediateLoad);
            }

            public class Toggler
            {
                public static Asset<Texture2D> SoulTogglerButtonTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/SoulTogglerToggle", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> SoulTogglerButton_MouseOverTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/SoulTogglerToggle_MouseOver", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> CheckBox => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CheckBox", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> CheckMark => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CheckMark", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> Cross => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/Cross", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> DisplayAllButton => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/DisplayAllButton", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> PresetCustom => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetCustom", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> PresetMinimal => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetMinimal", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> PresetOff => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetOff", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> PresetOn => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetOn", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> PresetOutline => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetOutline", AssetRequestMode.ImmediateLoad);
                public static Asset<Texture2D> ReloadButton => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/ReloadButton", AssetRequestMode.ImmediateLoad);
            }
        }
    }
}
