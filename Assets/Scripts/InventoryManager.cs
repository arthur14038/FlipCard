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

	EquippableVG equipCardBack = null;
	EquippableVG equipCardFace = null;
	EquippableVG equipTheme = null;
	Dictionary<string, Sprite> spritesWithItemId = new Dictionary<string, Sprite>();
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
			StoreInventory.EquipVirtualGood(FlipCardStoreAsset.CARD_BACK_000_ITEM_ID);
		}
		equipCardBack = StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.CardBackCategory.Name);
		if(equipCardBack == null)
		{
			StoreInventory.EquipVirtualGood(FlipCardStoreAsset.CARD_BACK_000_ITEM_ID);
			equipCardBack = StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.CardBackCategory.Name);
		}

		if(cardBackBalance == 0)
		{
			StoreInventory.GiveItem(FlipCardStoreAsset.CARD_FACE_000_ITEM_ID, 1);
			StoreInventory.EquipVirtualGood(FlipCardStoreAsset.CARD_FACE_000_ITEM_ID);
		}
		equipCardFace = StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.CardFaceCategory.Name);
		if(equipCardFace == null)
		{
			StoreInventory.EquipVirtualGood(FlipCardStoreAsset.CARD_FACE_000_ITEM_ID);
			equipCardFace = StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.CardFaceCategory.Name);
		}

		if(cardBackBalance == 0)
		{
			StoreInventory.GiveItem(FlipCardStoreAsset.THEME_00_ITEM_ID, 1);
			StoreInventory.EquipVirtualGood(FlipCardStoreAsset.THEME_00_ITEM_ID);
		}
		equipTheme = StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.ThemeCategory.Name);
		if(equipTheme == null)
		{
			StoreInventory.EquipVirtualGood(FlipCardStoreAsset.THEME_00_ITEM_ID);
			equipTheme = StoreInventory.GetEquippedVirtualGood(FlipCardStoreAsset.ThemeCategory.Name);
		}
	}

	IEnumerator LoadInventoryTexture()
	{
		string jsonString = ((TextAsset)Resources.Load("InventoryInfo")).text;
		List<InventoryInfo> tmp = JsonConvert.DeserializeObject<List<InventoryInfo>>(jsonString);
		inventoryInfo = tmp[0];

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

	public Sprite GetSpriteById(string itemId)
	{
		if(spritesWithItemId.ContainsKey(itemId))
			return spritesWithItemId[itemId];
		else
			return null;
	}

	public Sprite GetCurrentThemeSprite()
	{
		return GetSpriteById(equipTheme.ItemId);
	}

	public Sprite GetCurrentCardBack()
	{
		return GetSpriteById(equipCardBack.ItemId);
	}

	public Sprite GetCurrentCardFace()
	{
		return GetSpriteById(equipTheme.ItemId);
	}
}
