using Terraria.ModLoader;
using Terraria.GameContent.UI;

namespace FargowiltasSouls.Content.UI.Emotes
{
    public class GiftEmote : ModEmoteBubble
    {
        public override void SetStaticDefaults()
        {
            AddToCategory(EmoteID.Category.Items);
        }
    }
}
