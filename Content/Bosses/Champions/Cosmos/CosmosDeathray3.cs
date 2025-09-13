using FargowiltasSouls.Assets.Textures;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosDeathray3 : CosmosDeathray2
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Deathrays", "ShadowDeathray");
        public CosmosDeathray3() : base(30) { }
    }
}