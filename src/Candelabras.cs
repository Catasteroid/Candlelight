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

	public class BlockSimpleCandelabra : Block
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

	public class BlockCandelabra : Block
	{
	
		private int maxCandles;
	
		public int CandleCount
        {
            get
            {
                switch (CodeWithParts("candlenumber"))
                {
                    case "candle0":
                        return 0;
                    case "candle1":
                        return 1;
                    case "candle2":
                        return 2;
                    case "candle3":
                        return 3;
                    case "candle4":
                        return 4;
                    case "candle5":
                        return 5;
                    case "candle6":
                        return 6;
                    case "candle7":
                        return 7;
                    case "candle8":
                        return 8;
					case "candle9":
                        return 9;
					case "candle10":
                        return 10;
					case "candle11":
                        return 11;
					case "candle12":
                        return 12;
                    case "candle13":
                        return 13;
                    case "candle14":
                        return 14;
                    case "candle15":
                        return 15;
                    case "candle16":
                        return 16;
                    case "candle17":
                        return 17;
                    case "candle18":
                        return 18;
					case "candle19":
                        return 19;
					case "candle20":
                        return 20;
					case "candle21":
                        return 21;
					case "candle22":
                        return 22;
                    case "candle23":
                        return 23;
                    case "candle24":
                        return 24;
                    case "candle25":
                        return 25;
                }
                return -1;
            }
        }
		
		public bool Lit
        {
            get { return Variant["state"] == "lit"; }
        }
	
		public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
			maxCandles = Block.Attributes?["maxCandles"]?.AsInt() ?? 1;
            int candlecount = CandleCount;
            ItemStack itemstack = byPlayer.InventoryManager.ActiveHotbarSlot?.Itemstack;

			// If they're not holding shift down place a candle if they're holding on or toggle lit/unlit if they're not
			if (!byEntity.Controls.ShiftKey)
			{
				// Attempt to add a candle to the candelabra
				if (itemstack != null && itemstack.Collectible.Code.Path == "candle" && CandleCount != 8)
				{
					if (byPlayer != null && byPlayer.WorldData.CurrentGameMode == EnumGameMode.Survival)
					{
						byPlayer.InventoryManager.ActiveHotbarSlot.TakeOut(1);
						Block block = world.GetBlock(CodeWithParts("candlenumber",GetNextCandleCount()));
						world.BlockAccessor.ExchangeBlock(block.BlockId, blockSel.Position);
						world.BlockAccessor.MarkBlockDirty(blockSel.Position);

						return true;
					}
				}
				// Attempt to toggle between lit and unlit
				else if (itemstack == null || itemstack.Collectible.Code.Path != "candle")
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
			}
			else
			{
				// Attempt to remove a candle from the candelabra if it has any
				if (CandleCount > 0)
				{
					if (byPlayer != null && byPlayer.WorldData.CurrentGameMode == EnumGameMode.Survival)
					{
						ItemStack stack = new ItemStack(world.GetItem(new AssetLocation("candle")));
						if (byPlayer.InventoryManager.TryGiveItemstack(stack, true))
						{
							Block block = world.GetBlock(CodeWithParts("candlenumber",GetPreviousCandleCount()));
							world.BlockAccessor.ExchangeBlock(block.BlockId, blockSel.Position);
							world.BlockAccessor.MarkBlockDirty(blockSel.Position);
							
							return true;
						}
					}
				}
			}
            return false;
        }

        string GetNextCandleCount()
        {
            if (CandleCount != maxCandles)
                return $"candle{CandleCount + 1}";
            else 
                return "";
        }

		string GetPreviousCandleCount()
        {
            if (CandleCount != 0)
                return $"candle{CandleCount - 1}";
            else 
                return "";
        }

        public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
        {
            if (CandleCount == maxCandles)
			{
				return new WorldInteraction[]
				{
					new WorldInteraction()
					{
						ActionLangCode = "blockhelp-chandelier-removecandle",
						HotKeyCode = "shift",
						MouseButton = EnumMouseButton.Right,
						Itemstacks = new ItemStack[] { new ItemStack(world.GetItem(new AssetLocation("candle"))) }
					}
				}.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
			}
			else if (CandleCount == 0)
			{
				return new WorldInteraction[]
				{
					new WorldInteraction()
					{
						ActionLangCode = "blockhelp-chandelier-addcandle",
						MouseButton = EnumMouseButton.Right,
						Itemstacks = new ItemStack[] { new ItemStack(world.GetItem(new AssetLocation("candle"))) }
					}
				}.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
			}
			else
			{
				return new WorldInteraction[]
				{
					new WorldInteraction()
					{
						ActionLangCode = "blockhelp-chandelier-addcandle",
						MouseButton = EnumMouseButton.Right,
						Itemstacks = new ItemStack[] { new ItemStack(world.GetItem(new AssetLocation("candle"))) }
					},
					new WorldInteraction()
					{
						ActionLangCode = "blockhelp-chandelier-removecandle",
						MouseButton = EnumMouseButton.Right,
						Itemstacks = new ItemStack[] { new ItemStack(world.GetItem(new AssetLocation("candle"))) }
					},
					new WorldInteraction()
					{
						ActionLangCode = "blockhelp-chandelier-togglecandles",
						HotKeyCode = "shift",
						MouseButton = EnumMouseButton.Right
					}
				}.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
			}
		}					
	}
}