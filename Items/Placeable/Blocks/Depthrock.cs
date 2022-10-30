﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Wisplantern.Items.Placeable.Blocks
{
    class Depthrock : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Depthrock Block");
            Tooltip.SetDefault("'It's lukewarm'");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Depthrock>();
        }
    }
}
