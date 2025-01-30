using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Content.Items.Materials
{
    public class FallenStarDayItem : ModItem
    {
        public override string Texture => "Terraria/Images/Item_75";

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.FallenStar);
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 8));
        }

        public override bool OnPickup(Player player)
        {
            int spawnItem = player.QuickSpawnItem(Item.GetSource_FromThis(), ItemID.FallenStar, Item.stack);
            Main.item[spawnItem].beingGrabbed = true;

            return false;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight((int)((Item.position.X + (float)Item.width) / 16f), (int)((Item.position.Y + (float)(Item.height / 2)) / 16f), 0.8f, 0.7f, 0.1f);
            if (Item.timeSinceItemSpawned % 12 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Item.Center + new Vector2(0f, (float)Item.height * 0.2f) + Main.rand.NextVector2CircularEdge(Item.width, (float)Item.height * 0.6f) * (0.3f + Main.rand.NextFloat() * 0.5f), 228, new Vector2(0f, (0f - Main.rand.NextFloat()) * 0.3f - 1.5f), 127);
                dust.scale = 0.5f;
                dust.fadeIn = 1.1f;
                dust.noGravity = true;
                dust.noLight = true;
            }
        }
    }
}
