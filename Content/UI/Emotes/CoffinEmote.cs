using FargowiltasSouls.Core.Systems;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.UI.Emotes
{
    public class CoffinEmote : ModEmoteBubble
    {
        public override void SetStaticDefaults()
        {
            AddToCategory(EmoteID.Category.Dangers);
        }

        public override bool IsUnlocked()
        {
            return WorldSavingSystem.DownedBoss[11];
        }
    }
}
