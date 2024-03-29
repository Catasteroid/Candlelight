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
	
		private int maxCandles;
	
        public bool Empty
        {
            get { return Variant["state"] == "empty"; }
        }
		
		public bool Lit
        {
            get { return Variant["state"] == "lit"; }
        }
		
		public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
			if (Attributes["maxCandles"].Exists)
            {
                maxCandles = Attributes["maxCandles"].AsInt(1);		
			}
		}
		
        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
			
            if (Empty)
            {
                ItemStack heldStack = byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack;
                if (heldStack != null && heldStack.Collectible.Code.Path.Equals("candle") && heldStack.StackSize >= maxCandles)
                {
                    byPlayer.InventoryManager.ActiveHotbarSlot.TakeOut(maxCandles);
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
				if (!byPlayer.Entity.Controls.ShiftKey)
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
					stack.StackSize = maxCandles;
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
	
		public bool debugMessages;
	
		public int maxCandles;
		
		ICoreClientAPI capi;
		
		public string maxCandlesString
        {
            get
            {
				return "candle" + maxCandles;
			}
		}
	
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
		
		public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
			capi = api as ICoreClientAPI;
			if (Attributes["debugMessages"].Exists)
			{
				debugMessages = Attributes["debugMessages"].AsBool(false);
				api.World.Logger.Error("Loaded candelabra {0}, maxCandles: {1}",api.World.GetBlock(CodeWithParts("candlenumber","candle0")),maxCandles);
			}
			if (Attributes["maxCandles"].Exists)
            {
                maxCandles = Attributes["maxCandles"].AsInt(1);		
				if (debugMessages == true)
				{
					api.World.Logger.Error("Loaded candelabra {0}, maxCandles: {1}",api.World.GetBlock(CodeWithParts("candlenumber","candle"+maxCandles)),maxCandles);
				}
			}
		}
	
		public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            int candlecount = CandleCount;
            ItemStack itemstack = byPlayer.InventoryManager.ActiveHotbarSlot?.Itemstack;

			// If they're not holding shift down place a candle if they're holding one or toggle lit/unlit if they're not
			if (!byPlayer.Entity.Controls.ShiftKey)
			{
				// Attempt to add a candle to the candelabra
				if (itemstack != null && itemstack.Collectible.Code.Path == "candle")
				{
					if (byPlayer != null && byPlayer.WorldData.CurrentGameMode == EnumGameMode.Survival)
					{
						if (CandleCount < maxCandles)
						{
							byPlayer.InventoryManager.ActiveHotbarSlot.TakeOut(1);
							Block block = world.GetBlock(CodeWithParts("candlenumber",GetNextCandleCount()));
							world.BlockAccessor.ExchangeBlock(block.BlockId, blockSel.Position);
							world.BlockAccessor.MarkBlockDirty(blockSel.Position);
							if (debugMessages == true)
							{
								world.Logger.Error("Player has added a candle to the candelabra ({0}), it now has {1} candles of {2}",world.GetBlock(CodeWithParts("candlenumber","candle" + CandleCount)),CandleCount,maxCandles);
							}
							return true;
						}
						else
						{
							(world.Api as ICoreClientAPI)?.TriggerIngameError(this, "candelabrafull", Lang.Get("candelabrafull"));
							if (debugMessages == true)
							{
								world.Logger.Error("Player attempted to add a candle to the candelabra ({0}), but it had {1} of {2} candles so it was full",world.GetBlock(CodeWithParts("candlenumber","candle" + CandleCount)),CandleCount,maxCandles);
							}
							return false;
						}
					}
				}
				// Attempt to toggle between lit and unlit
				else if (itemstack == null || itemstack.Collectible.Code.Path != "candle")
				{
					if (CandleCount > 0)
					{	
						if (debugMessages == true)
						{
							world.Logger.Error("Player attempting to toggle a candelabra ({0})'s lit status, current candles: {1}, max candles: {2}, currently lit: {3}",world.GetBlock(CodeWithParts("candlenumber","candle" + CandleCount)),CandleCount,maxCandles,Lit);
						}
						if (!Lit)
						{ 
							Block filledBlock = world.GetBlock(CodeWithVariant("state", "lit"));
							world.BlockAccessor.ExchangeBlock(filledBlock.BlockId, blockSel.Position);
							if (debugMessages == true)
							{
								world.Logger.Error("Player has lit the candelabra");
							}
							return true;
						} 
						else 
						{
							Block filledBlock = world.GetBlock(CodeWithVariant("state", "unlit"));
							world.BlockAccessor.ExchangeBlock(filledBlock.BlockId, blockSel.Position);
							if (debugMessages == true)
							{
								world.Logger.Error("Player has snuffed the candelabra");
							}
							return true;
						}
					}
					else if (world.Api is ICoreClientAPI capi && world.Api.Side == EnumAppSide.Client)
					{
						capi.TriggerIngameError(this, "notenoughcandles", Lang.Get("needcandlestolight"));
						return false;
					}
				}
			}
			else
			{
				// Attempt to remove a candle from the candelabra if it has any
				if (CandleCount > 0)
				{
					if (debugMessages == true)
					{
						world.Logger.Error("Player attempting to remove a candle from {0}, candleCount: {1} maxCandles: {2}",world.GetBlock(CodeWithParts("candlenumber","candle"+CandleCount)),maxCandles);
					}
					if (byPlayer != null && byPlayer.WorldData.CurrentGameMode == EnumGameMode.Survival)
					{
						ItemStack stack = new ItemStack(world.GetItem(new AssetLocation("candle")));
						if (byPlayer.InventoryManager.TryGiveItemstack(stack, true))
						{
							Block block = world.GetBlock(CodeWithParts("candlenumber",GetPreviousCandleCount()));
							world.BlockAccessor.ExchangeBlock(block.BlockId, blockSel.Position);
							world.BlockAccessor.MarkBlockDirty(blockSel.Position);
							if (debugMessages == true)
							{                                                                                                                
								world.Logger.Error("Player removed a candle to the candelabra ({0}), it now has {1} of {2} candles",world.GetBlock(CodeWithParts("candlenumber","candle"+CandleCount)),CandleCount,maxCandles);
							}
							return true;
						}
					}
				}
				else 
				{
					// Report that the candelabra is empty and thus candles cannot be removed
					if (world.Api is ICoreClientAPI capi && world.Api.Side == EnumAppSide.Client)
					{
						capi.TriggerIngameError(this, "candelabraempty", Lang.Get("candelabraempty"));
					}
					return false;
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