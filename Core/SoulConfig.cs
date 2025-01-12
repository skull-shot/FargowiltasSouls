using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader.Config;

namespace FargowiltasSouls.Core
{
    class SoulConfig : ModConfig
    {
        public static SoulConfig Instance;
        public override void OnLoaded()
        {
            Instance = this;
        }
        public override ConfigScope Mode => ConfigScope.ServerSide;

        private const string ModName = "FargowiltasSouls";

        [DefaultValue(true)]
        public bool DeviChatter;

        [DefaultValue(false)]
        public bool BigTossMode;

        [DefaultValue(false)]
        public bool PerformanceMode;

        [DefaultValue(true)]
        public bool ForcedFilters;

        #region maso

        [Header("Maso")]

        [DefaultValue(true)]
        [ReloadRequired]
        public bool BossRecolors;

        #endregion

        #region patreon

        [Header("Patreon")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonRoomba;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonOrb;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonFishingRod;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonDoor;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonWolf;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonDove;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonKingSlime;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonFishron;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonPlant;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonDevious;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonVortex;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonPrime;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonCrimetroid;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonNanoCore;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonROB;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonTouhou;

        #endregion
    }
}
