﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;
using Newtonsoft.Json;

public class InventoryManager : SingletonMonoBehavior<InventoryManager>
{
	class InventoryInfo
	{
		public int cardBackCount = 0;
		public int cardFaceCount = 0;
		public int themeCount = 0;
		public string cardBackPath = "";
		public string cardFacePath = "";
		public string themePath = "";
	}

	public VoidNoneParameter updateCurrency;
	Dictionary<string, Sprite> spritesWithItemId = new Dictionary<string, Sprite>();
	List<ThemePack> themePackList = new List<ThemePack>();
	InventoryInfo inventoryInfo;
	VoidBool lastPurchaseCallback;
	ThemePack buyingThemePack;
	VoidNoneParameter purchaseCancelCallBack;
	bool storeInitialized;

	public IEnumerator Init()
	{
		storeInitialized = false;
        StoreEvents.OnSoomlaStoreInitialized += OnSoomlaStoreInitialized;
		StoreEvents.OnCurrencyBalanceChanged += onCurrencyBalanceChanged;
		StoreEvents.OnMarketPurchase += OnMarketPurchase;
		StoreEvents.OnMarketPurchaseCancelled += OnMarketPurchaseCancelled;
		StoreEvents.OnItemPurchased += OnItemPurchase;
		StoreEvents.OnGoodEquipped += OnGoodEquipped;
		StoreEvents.OnUnexpectedStoreError += OnUnexpectedStoreError;

		SoomlaStore.Initialize(new FlipCardStoreAsset());
		yield return StartCoroutine(LoadInventoryTexture());
	}

	public void OnSoomlaStoreInitialized()
	{
		storeInitialized = true;
		SetEquipItem();
	}

	public void onCurrencyBalanceChanged(VirtualCurrency virtualCurrency, int balance, int amountAdded)
	{
		if(updateCurrency != null)
			updateCurrency();
	}
	
	public void OnMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
	{
		Debug.LogFormat("OnMarketPurchase Name: {0}, payload: {1}, extra.Count: {2}", pvi.Name, payload, extra.Count);
		if(lastPurchaseCallback != null)
		{
			lastPurchaseCallback(true);
			lastPurchaseCallback = null;
		}
	}

	public void OnMarketPurchaseCancelled(PurchasableVirtualItem pvi)
	{
		Debug.LogFormat("OnMarketPurchaseCancelled Name: {0}", pvi.Name);
		if(purchaseCancelCallBack != null)
			purchaseCancelCallBack();
	}

	public void OnItemPurchase(PurchasableVirtualItem pvi, string payload)
	{
		Debug.LogFormat("OnItemPurchase Name: {0}, payload: {1}", pvi.Name, payload);

		if(buyingThemePack != null)
		{
			if(StoreInventory.GetItemBalance(buyingThemePack.cardBack.ItemId) == 0)
				StoreInventory.GiveItem(buyingThemePack.cardBack.ItemId, 1);
			if(StoreInventory.GetItemBalance(buyingThemePack.cardFace.ItemId) == 0)
				StoreInventory.GiveItem(buyingThemePack.cardFace.ItemId, 1);
			buyingThemePack = null;
		}

		if(lastPurchaseCallback != null)
		{
			lastPurchaseCallback(true);
			lastPurchaseCallback = null;
		}
	}

	public void OnGoodEquipped(EquippableVG good)
	{
		Debug.LogFormat("OnGoodEquipped Name: {0}", good.Name);
	}

	public void OnUnexpectedStoreError(int errorCode)
	{
		string errorContent = "";
		switch(errorCode)
		{
			case 0:
				errorContent = "GENERAL";
				break;
			case 1:
				errorContent = "VERIFICATION_TIMEOUT";
				break;
			case 2:
				errorContent = "VERIFICATION_FAIL";
				break;
			case 3:
				errorContent = "PURCHASE_FAIL";
				break;
		}
		Debug.LogErrorFormat("OnUnexpectedStoreError errorContent: {0}", errorContent);

		if(lastPurchaseCallback != null)
		{
			lastPurchaseCallback(false);
			lastPurchaseCallback = null;
		}

		if(buyingThemePack != null)
			buyingThemePack = null;
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
		EquippableVG equipTheme = GetEquippedVirtualGood(FlipCardStoreAsset.ThemeCategory.Name);
		return GetSpriteById(equipTheme.ItemId);
	}

	public Sprite GetCurrentCardBack()
	{
		EquippableVG equipCardBack = GetEquippedVirtualGood(FlipCardStoreAsset.CardBackCategory.Name);
		return GetSpriteById(equipCardBack.ItemId);
	}

	public Sprite GetCurrentCardFace()
	{
		EquippableVG equipCardFace = GetEquippedVirtualGood(FlipCardStoreAsset.CardFaceCategory.Name);
		return GetSpriteById(equipCardFace.ItemId);
	}

	public void EquipItem(string itemId)
	{
		StoreInventory.EquipVirtualGood(itemId);
	}

	public bool IsCardEquip(string cardBackItemId)
	{
		EquippableVG equipCardBack = GetEquippedVirtualGood(FlipCardStoreAsset.CardBackCategory.Name);
		if(cardBackItemId == equipCardBack.ItemId)
			return true;
		else
			return false;
	}

	public bool IsThemeEquiped(string themeItemId)
	{
		EquippableVG equipTheme = GetEquippedVirtualGood(FlipCardStoreAsset.ThemeCategory.Name);
		if(themeItemId == equipTheme.ItemId)
			return true;
		else
			return false;
	}

	public void BuyCurrencyPack(string moniPackItemId, VoidBool lastPurchaseCallback, VoidNoneParameter purchaseCancelCallBack)
	{
		this.purchaseCancelCallBack = purchaseCancelCallBack;
		this.lastPurchaseCallback = lastPurchaseCallback;
		StoreInventory.BuyItem(moniPackItemId);
	}

	public void BuyThemePack(ThemePack themePack, VoidBool lastPurchaseCallback)
	{
		this.lastPurchaseCallback = lastPurchaseCallback;
		buyingThemePack = themePack;
		StoreInventory.BuyItem(buyingThemePack.theme.ItemId);
	}

	/// <summary>
	/// Checks currently equipped good in given <c>category</c>
	/// </summary>
	/// <param name="string">Name of the category we want to check</param>
	/// <returns>EquippableVG otherwise null</returns>
	public static EquippableVG GetEquippedVirtualGood(string categoryName)
	{
		foreach(VirtualCategory category in StoreInfo.Categories)
		{
			if(category.Name == categoryName)
			{
				return StoreInventory.GetEquippedVirtualGood(category);
			}
		}

		Debug.LogError("There is no category named " + categoryName);
		return null;
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
		if(GetEquippedVirtualGood(FlipCardStoreAsset.CardBackCategory.Name) == null)
			EquipItem(FlipCardStoreAsset.CARD_BACK_000_ITEM_ID);

		if(cardFackBalance == 0)
		{
			StoreInventory.GiveItem(FlipCardStoreAsset.CARD_FACE_000_ITEM_ID, 1);
			EquipItem(FlipCardStoreAsset.CARD_FACE_000_ITEM_ID);
		}
		if(GetEquippedVirtualGood(FlipCardStoreAsset.CardFaceCategory.Name) == null)
			EquipItem(FlipCardStoreAsset.CARD_FACE_000_ITEM_ID);

		if(themeBalance == 0)
		{
			StoreInventory.GiveItem(FlipCardStoreAsset.THEME_00_ITEM_ID, 1);
			EquipItem(FlipCardStoreAsset.THEME_00_ITEM_ID);
		}
		if(GetEquippedVirtualGood(FlipCardStoreAsset.ThemeCategory.Name) == null)
			EquipItem(FlipCardStoreAsset.THEME_00_ITEM_ID);
	}

	IEnumerator LoadInventoryTexture()
	{
		while(!storeInitialized)
		{
			yield return null;
		}

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
}

public class ThemePack
{
	public VirtualGood theme;
	public VirtualGood cardBack;
	public VirtualGood cardFace;
}