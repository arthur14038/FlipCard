using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;
using Newtonsoft.Json;

public class InventoryManager : SingletonMonoBehavior<InventoryManager>
{
	class InventoryInfo
	{
		public int cardBackCount;
		public int cardFaceCount;
		public int themeCount;
		public string cardBackPath;
		public string cardFacePath;
		public string themePath;
	}
	
	Dictionary<string, Sprite> spritesWithItemId = new Dictionary<string, Sprite>();
	List<ThemePack> themePackList = new List<ThemePack>();
	InventoryInfo inventoryInfo;

	public IEnumerator Init()
	{
		SetEquipItem();
		yield return StartCoroutine(LoadInventoryTexture());
	}

	void SetEquipItem()
	{
		int cardBackBalance = StoreInventory.GetItemBalance(FlipCardStoreAsset.CARD_BACK_000_ITEM_ID);
		int cardFackBalance = StoreInventory.GetItemBalance(FlipCardStoreAsset.CARD_FACE_000_ITEM_ID);
		int themeBalance = StoreInventory.GetItemBalance(FlipCardStoreAsset.THEME_00_ITEM_ID);

		if(cardBackBalance == 0)
		{
			StoreInventory.GiveItem(FlipCardStoreAsset.CARD_BACK_000_ITEM_ID, 1);
			EquipItem(FlipCardStoreAsset.CARD_BACK_000_ITEM_ID);
		}
		if(StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.CardBackCategory.Name) == null)
			EquipItem(FlipCardStoreAsset.CARD_BACK_000_ITEM_ID);

		if(cardBackBalance == 0)
		{
			StoreInventory.GiveItem(FlipCardStoreAsset.CARD_FACE_000_ITEM_ID, 1);
			EquipItem(FlipCardStoreAsset.CARD_FACE_000_ITEM_ID);
		}
		if(StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.CardFaceCategory.Name) == null)
			EquipItem(FlipCardStoreAsset.CARD_FACE_000_ITEM_ID);

		if(cardBackBalance == 0)
		{
			StoreInventory.GiveItem(FlipCardStoreAsset.THEME_00_ITEM_ID, 1);
			EquipItem(FlipCardStoreAsset.THEME_00_ITEM_ID);
		}
		if(StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.ThemeCategory.Name) == null)
			EquipItem(FlipCardStoreAsset.THEME_00_ITEM_ID);
	}

	IEnumerator LoadInventoryTexture()
	{
		string jsonString = ((TextAsset)Resources.Load("InventoryInfo")).text;
		List<InventoryInfo> tmp = JsonConvert.DeserializeObject<List<InventoryInfo>>(jsonString);
		inventoryInfo = tmp[0];

		for(int i = 0 ; i < inventoryInfo.themeCount ; ++i)
		{
			string themeItemId = string.Format("Theme_{0}", i.ToString("D2"));
			string cardBackItemId = string.Format("CardBack_{0}", i.ToString("D3"));
			string cardFaceItemId = string.Format("CardFace_{0}", i.ToString("D3"));

			ThemePack pack = new ThemePack();
			pack.theme = (VirtualGood)StoreInfo.GetItemByItemId(themeItemId);
			pack.cardBack = (VirtualGood)StoreInfo.GetItemByItemId(cardBackItemId);
			pack.cardFace = (VirtualGood)StoreInfo.GetItemByItemId(cardFaceItemId);

			themePackList.Add(pack);
		}

		for(int i = 0 ; i < inventoryInfo.cardBackCount ; ++i)
		{
			ResourceRequest request = Resources.LoadAsync<Sprite>(string.Format(inventoryInfo.cardBackPath, i.ToString("D3")));
			yield return request;
			spritesWithItemId.Add(request.asset.name, (Sprite)request.asset);
		}

		for(int i = 0 ; i < inventoryInfo.cardFaceCount ; ++i)
		{
			ResourceRequest request = Resources.LoadAsync<Sprite>(string.Format(inventoryInfo.cardFacePath, i.ToString("D3")));
			yield return request;
			spritesWithItemId.Add(request.asset.name, (Sprite)request.asset);
		}

		for(int i = 0 ; i < inventoryInfo.themeCount ; ++i)
		{
			ResourceRequest request = Resources.LoadAsync<Sprite>(string.Format(inventoryInfo.themePath, i.ToString("D2")));
			yield return request;
			spritesWithItemId.Add(request.asset.name, (Sprite)request.asset);
		}
	}
	
	public List<ThemePack> GetAllThemePack()
	{
		return themePackList;
	}

	public VirtualGood GetVirtualGood(string itemId)
	{
		return (VirtualGood)StoreInfo.GetItemByItemId(itemId); ;
	}

	public Sprite GetSpriteById(string itemId)
	{
		if(spritesWithItemId.ContainsKey(itemId))
			return spritesWithItemId[itemId];
		else
			return null;
	}

	public Sprite GetCurrentThemeSprite()
	{
		EquippableVG equipTheme = StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.ThemeCategory.Name);
		return GetSpriteById(equipTheme.ItemId);
	}

	public Sprite GetCurrentCardBack()
	{
		EquippableVG equipCardBack = StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.CardBackCategory.Name);
		return GetSpriteById(equipCardBack.ItemId);
	}

	public Sprite GetCurrentCardFace()
	{
		EquippableVG equipCardFace = StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.CardFaceCategory.Name);
		return GetSpriteById(equipCardFace.ItemId);
	}

	public void EquipItem(string itemId)
	{
		StoreInventory.EquipVirtualGood(itemId);
	}

	public bool IsCardEquip(string cardBackItemId)
	{
		EquippableVG equipCardBack = StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.CardBackCategory.Name);
		if(cardBackItemId == equipCardBack.ItemId)
			return true;
		else
			return false;
	}

	public bool IsThemeEquiped(string themeItemId)
	{
		EquippableVG equipTheme = StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.ThemeCategory.Name);
		if(themeItemId == equipTheme.ItemId)
			return true;
		else
			return false;
	}

	public bool CanAfford(string themeItemId)
	{
		VirtualGood good = (VirtualGood)StoreInfo.GetItemByItemId(themeItemId);
		if(good == null)
		{
			Debug.LogError("theTheme == null");
			return false;
		}
		return good.CanAfford();
	}
}

public class ThemePack
{
	public VirtualGood theme;
	public VirtualGood cardBack;
	public VirtualGood cardFace;
}