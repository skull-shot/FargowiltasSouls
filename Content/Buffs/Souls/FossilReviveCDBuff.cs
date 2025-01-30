using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class FossilReviveCDBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Revived");
            // Description.SetDefault("You cannot revive again");
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.debuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "已复活");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "最近经历过复活");
        }
        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            if (Main.LocalPlayer.HasEffect<SpectreEffect>())
            {
                Texture2D tex = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Buffs/Souls/SpectreReviveCDBuff").Value;
                drawParams.Texture = tex;
            }
            return base.PreDraw(spriteBatch, buffIndex, ref drawParams);
        }
    }
}