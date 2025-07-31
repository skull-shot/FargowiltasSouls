using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.Assets.Textures;



namespace FargowiltasSouls.Core.Systems
{
    public class WorldBorderSystem : ModSystem
    {
        private static Asset<Texture2D> mutantBorder;
        
        public override void OnModLoad()
        {
            Main.QueueMainThreadAction(() =>
            {
                On_UIWorldListItem.ctor += UseCustomBorder;
            });

            mutantBorder = FargoAssets.UI.MainMenu.MutantWorldBorder;
        }

        private static void UseCustomBorder(On_UIWorldListItem.orig_ctor orig, UIWorldListItem self, WorldFileData data, int orderInList, bool canBePlayed)
        {   
            orig(self, data, orderInList, canBePlayed);

            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_worldIcon")]
            extern static ref UIElement GetWorldIcon(UIWorldListItem item);
            UIElement worldIcon = GetWorldIcon(self);

            if (data.TryGetHeaderData<WorldBorderSystem>(out TagCompound tag))
            {
                if (tag.ContainsKey("DownedMutant"))
                {
                    worldIcon.RemoveAllChildren();

                    Asset<Texture2D> MutantBorder = mutantBorder;
                    UIImage border = new UIImage(MutantBorder)
                    {
                        HAlign = 0.5f,
                        VAlign = 0.5f,
                        Top = new StyleDimension(-10f, 0),
                        Left = new StyleDimension(-3f, 0f),
                        IgnoresMouseInteraction = true
                    };
                    worldIcon.Append(border);
                }
                    
            }

        }

        public override void SaveWorldHeader(TagCompound tag)
        {
            if (WorldSavingSystem.DownedMutant)
                tag["DownedMutant"] = true;
        }
    }
}
