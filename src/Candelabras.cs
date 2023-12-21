using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Config;
using Vintagestory.API.Util;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using Vintagestory.GameContent.Mechanics;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Candlelight
{

	public class BlockCandelabra : Block
    {
	
		private int numCandles;
	
        public bool Empty
        {
            get { return Variant["state"] == "empty"; }
        }
		
		public bool Lit
        {
            get { return Variant["state"] == "lit"; }
        }
		
        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
			numCandles = Block.Attributes?["numCandles"]?.AsInt() ?? 1;
			
            if (Empty)
            {
                ItemStack heldStack = byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack;
                if (heldStack != null && heldStack.Collectible.Code.Path.Equals("candle") && heldStack.StackSize >= numCandles)
                {
                    byPlayer.InventoryManager.ActiveHotbarSlot.TakeOut(numCandles);
                    byPlayer.InventoryManager.ActiveHotbarSlot.MarkDirty();

                    Block filledBlock = world.GetBlock(CodeWithVariant("state", "unlit"));
                    world.BlockAccessor.ExchangeBlock(filledBlock.BlockId, blockSel.Position);

                    if (Sounds?.Place != null)
                    {
                        world.PlaySoundAt(Sounds.Place, blockSel.Position.X, blockSel.Position.Y, blockSel.Position.Z, byPlayer);
                    }

                    return true;
                }
            } 
			else
			{
				if (!byEntity.Controls.ShiftKey)
				{
					if (!Lit)
					{
						Block filledBlock = world.GetBlock(CodeWithVariant("state", "lit"));
						world.BlockAccessor.ExchangeBlock(filledBlock.BlockId, blockSel.Position);
						return false;
					} 
					else 
					{
						Block filledBlock = world.GetBlock(CodeWithVariant("state", "unlit"));
						world.BlockAccessor.ExchangeBlock(filledBlock.BlockId, blockSel.Position);
						return true;
					}
				}
				else
				{
					ItemStack stack = new ItemStack(world.GetItem(new AssetLocation("candle")));
					stack.StackSize = numCandles;
					if (byPlayer.InventoryManager.TryGiveItemstack(stack, true))
					{
						Block filledBlock = world.GetBlock(CodeWithVariant("state", "empty"));
						world.BlockAccessor.ExchangeBlock(filledBlock.BlockId, blockSel.Position);
	
						if (Sounds?.Place != null)
						{
							world.PlaySoundAt(Sounds.Place, blockSel.Position.X, blockSel.Position.Y, blockSel.Position.Z, byPlayer);
						}

						return true;
					}
				}
			}

            return base.OnBlockInteractStart(world, byPlayer, blockSel);
        }


        public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
        {
            if (Empty)
            {
                return new WorldInteraction[]
                {
                    new WorldInteraction()
                    {
                        ActionLangCode = "blockhelp-torchholder-addcandle",
                        MouseButton = EnumMouseButton.Right,
                        Itemstacks = new ItemStack[] { new ItemStack(world.GetItem(new AssetLocation("candle"))) }
                    }
                }.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
            } 
			else
            {
				if (!Lit)
				{
					return new WorldInteraction[]
					{
						new WorldInteraction()
						{
							ActionLangCode = "blockhelp-torchholder-lightcandelabra",
							MouseButton = EnumMouseButton.Right,
							Itemstacks = null
						}
					}.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
				}
				else
				{
					return new WorldInteraction[]
					{
						new WorldInteraction()
						{
							ActionLangCode = "blockhelp-torchholder-snuffcandelabra",
							MouseButton = EnumMouseButton.Right,
							Itemstacks = null
						}
					}.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
				}
            }
            
        }
    }

}