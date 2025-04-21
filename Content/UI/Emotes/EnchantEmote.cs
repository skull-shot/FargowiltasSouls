
using FargowiltasSouls.Core.Systems;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.UI.Emotes
{
    public class EnchantEmote : ModEmoteBubble
    {
        public override void SetStaticDefaults()
        {
            AddToCategory(EmoteID.Category.Items);
        }
    }
}
