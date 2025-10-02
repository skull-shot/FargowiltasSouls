using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader.Config;

namespace FargowiltasSouls.Core
{
    class ClientConfig : ModConfig
    {
        public static ClientConfig Instance;
        public override void OnLoaded()
        {
            Instance = this;
        }
        public override ConfigScope Mode => ConfigScope.ClientSide;

        private const string ModName = "FargowiltasSouls";

        [DefaultValue(true)]
        public bool WikiNotification;

        [DefaultValue(true)]
        public bool MusicModNotification;

        [DefaultValue(true)]
        public bool HideTogglerWhenInventoryIsClosed;

        [DefaultValue(true)]
        public bool ItemDisabledTooltip;

        [DefaultValue(BalanceTooltipSetting.Reworks)]
        public BalanceTooltipSetting ItemBalanceTooltip;

        [DefaultValue(false)]
        public bool ToggleSearchReset;

        private const float max4kX = 3840f;

        private const float max4kY = 2160f;

        [DefaultValue(true)]
        public bool CooldownBars;

        [Increment(1f)]
        [Range(0f, max4kX)]
        [DefaultValue(40f)]
        public float CooldownBarsX;

        [Increment(1f)]
        [Range(0f, max4kY)]
        [DefaultValue(400f)]
        public float CooldownBarsY;

        [Increment(1f)]
        [Range(0, max4kX)]
        [DefaultValue(0f)] // Set in ActiveSkillMenu.cs
        public float ActiveSkillMenuX; // From the right of the screen


        [Increment(1f)]
        [Range(0f, max4kY)]
        [DefaultValue(0f)] // Set in ActiveSkillMenu.cs
        public float ActiveSkillMenuY;

        #region maso

        [Header("Maso")]

        [Increment(1)]
        [Range(0, 100)]
        [DefaultValue(50)]
        public int RainbowHealThreshold;

        [DefaultValue(true)]
        public bool PrecisionSealIsHold;


        [Increment(1f)]
        [Range(0f, max4kX)]
        [DefaultValue(610f)]
        public float OncomingMutantX;


        [Increment(1f)]
        [Range(0f, max4kY)]
        [DefaultValue(250f)]
        public float OncomingMutantY;

        [DefaultValue(false)]
        public bool PhotosensitivityMode;
        #endregion

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            OncomingMutantX = Utils.Clamp(OncomingMutantX, 0, max4kX);
            OncomingMutantY = Utils.Clamp(OncomingMutantY, 0, max4kY);
        }
    }
}
