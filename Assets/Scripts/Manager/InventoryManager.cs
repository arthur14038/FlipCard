using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
	Dictionary<string, ThemeInfo> themeInfoDict = new Dictionary<string, ThemeInfo>();
	InventoryInfo inventoryInfo;
	bool storeInitialized;

	public IEnumerator Init()
	{
		LoadThemeInfo();
		storeInitialized = true;
		yield return StartCoroutine(LoadInventoryTexture());
	}


	public ThemeInfo GetThemeInfo(string themeItemId)
	{
		if(themeInfoDict.ContainsKey(themeItemId))
			return themeInfoDict[themeItemId];
		else
			return null;
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
		return GetSpriteById("Theme_00");
	}

	public Sprite GetCurrentCardBack()
	{
		return GetSpriteById("CardBack_000");
	}

	public Sprite GetCurrentCardFace()
	{
		return GetSpriteById("CardFace_000");
	}

	public void EquipItem(string itemId)
	{
	}

	public void BuyCurrencyPack(string moniPackItemId, VoidString purchaseCurrencyCallback, VoidNoneParameter purchaseCancelCallBack)
	{
	}

	IEnumerator LoadInventoryTexture()
	{
		Debug.Log("InventoryManager LoadInventoryTexture");
		while (!storeInitialized)
		{
			yield return null;
		}

		string jsonString = ((TextAsset)Resources.Load("InventoryInfo")).text;
		List<InventoryInfo> tmp = JsonConvert.DeserializeObject<List<InventoryInfo>>(jsonString);
		inventoryInfo = tmp[0];

		//for(int i = 0 ; i < inventoryInfo.themeCount ; ++i)
		//{
		//	string themeItemId = string.Format("Theme_{0}", i.ToString("D2"));
		//	string cardBackItemId = string.Format("CardBack_{0}", i.ToString("D3"));
		//	string cardFaceItemId = string.Format("CardFace_{0}", i.ToString("D3"));

		//	ThemePack pack = new ThemePack();
		//	pack.theme = (VirtualGood)StoreInfo.GetItemByItemId(themeItemId);
		//	pack.cardBack = (VirtualGood)StoreInfo.GetItemByItemId(cardBackItemId);
		//	pack.cardFace = (VirtualGood)StoreInfo.GetItemByItemId(cardFaceItemId);

		//	themePackList.Add(pack);
		//}

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

		Debug.Log("InventoryManager LoadInventoryTexture complete");
	}

	void LoadThemeInfo()
	{
		string jsonString = ((TextAsset)Resources.Load("ThemeInfo")).text;

		List<ThemeInfo> tmpList = JsonConvert.DeserializeObject<List<ThemeInfo>>(jsonString);

		foreach(ThemeInfo info in tmpList)
		{
			if(!themeInfoDict.ContainsKey(info.themeItemId))
				themeInfoDict.Add(info.themeItemId, info);
		}
	}
}

public class ThemeInfo
{
	public string themeItemId;
	public string themeName;
	public string themeContent;
	public int requireMoni;
}