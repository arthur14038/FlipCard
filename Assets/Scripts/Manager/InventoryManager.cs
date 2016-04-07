using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class InventoryManager : SingletonMonoBehavior<InventoryManager>
{
	public VoidNoneParameter updateCurrency;
	Dictionary<string, Sprite> spritesWithItemId = new Dictionary<string, Sprite>();
	Dictionary<string, ThemeInfo> themeInfoDict = new Dictionary<string, ThemeInfo>();
	int ownedTheme;

	public IEnumerator Init()
	{
		ownedTheme = PlayerPrefsManager.OwnedTheme;
		yield return StartCoroutine(LoadInventoryTexture());
	}
	
	public ThemeInfo GetThemeInfo(string themeItemId)
	{
		if(themeInfoDict.ContainsKey(themeItemId))
			return themeInfoDict[themeItemId];
		else
			return null;
    }

	public List<ThemeInfo> GetAllThemeInfo()
	{
		List<ThemeInfo> themeInfoList = new List<ThemeInfo>();
		foreach(KeyValuePair<string, ThemeInfo> kvp in themeInfoDict)
		{
			themeInfoList.Add(kvp.Value);
        }
		return themeInfoList;
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
		return GetSpriteById(PlayerPrefsManager.EquipedThemeId);
	}

	public Sprite GetCurrentCardBack()
	{
		return GetSpriteById(PlayerPrefsManager.EquipedCardBackId);
	}

	public Sprite GetCurrentCardFace()
	{
		return GetSpriteById(PlayerPrefsManager.EquipedCardFaceId);
	}
	
	public void EquipCardFace(string itemId)
	{
		PlayerPrefsManager.EquipedCardFaceId = itemId;
	}

	public void EquipCardBack(string itemId)
	{
		PlayerPrefsManager.EquipedCardBackId = itemId;
	}

	public void EquipTheme(string itemId)
	{
		PlayerPrefsManager.EquipedThemeId = itemId;
	}

	public bool IsCardEquip(string cardBackItemId)
	{
		if(cardBackItemId == PlayerPrefsManager.EquipedCardBackId)
			return true;
		else
			return false;
	}

	public bool IsThemeEquiped(string themeItemId)
	{
		if(themeItemId == PlayerPrefsManager.EquipedThemeId)
			return true;
		else
			return false;
	}
	
	public void BuyTheme(string themeItemId, VoidBool purchaseThemeCallback)
	{
		if(CanAfford(themeItemId))
		{
			ThemeInfo themeInfo = GetThemeInfo(themeItemId);
			themeInfo.isOwned = true;
			ownedTheme |= themeInfo.ownedFlag;
			PlayerPrefsManager.OwnedTheme = ownedTheme;

			AddMoni(-themeInfo.requireMoni);
			purchaseThemeCallback(true);
		} else
		{
			purchaseThemeCallback(false);
        }
	}

	public void AddMoni(int value)
	{
		PlayerPrefsManager.MoniCount += value;

		if(updateCurrency != null)
			updateCurrency();
	}

	public bool CanAfford(string themeItemId)
	{
		if(PlayerPrefsManager.MoniCount < GetThemeInfo(themeItemId).requireMoni)
			return false;
		else
			return true;
	}
	
	IEnumerator LoadInventoryTexture()
	{
		string jsonString = ((TextAsset)Resources.Load("ThemeInfo")).text;

		List<ThemeInfo> tmpList = JsonConvert.DeserializeObject<List<ThemeInfo>>(jsonString);

		for(int i = 0 ; i < tmpList.Count ; ++i)
		{
			if((ownedTheme & tmpList[i].ownedFlag) == tmpList[i].ownedFlag)
				tmpList[i].isOwned = true;
			else
				tmpList[i].isOwned = false;

			if(!themeInfoDict.ContainsKey(tmpList[i].themeItemId))
				themeInfoDict.Add(tmpList[i].themeItemId, tmpList[i]);
		}
		
		string cardFacePath = "CardFace/{0}";
		string cardBackPath = "CardBack/{0}";
		string themePath = "Theme/{0}";
		foreach(ThemeInfo info in tmpList)
		{
			ResourceRequest request = Resources.LoadAsync<Sprite>(string.Format(cardFacePath, info.cardFaceId));
			yield return request;
			spritesWithItemId.Add(request.asset.name, (Sprite)request.asset);

			request = Resources.LoadAsync<Sprite>(string.Format(cardBackPath, info.cardBackId));
			yield return request;
			spritesWithItemId.Add(request.asset.name, (Sprite)request.asset);

			request = Resources.LoadAsync<Sprite>(string.Format(themePath, info.themeItemId));
			yield return request;
			spritesWithItemId.Add(request.asset.name, (Sprite)request.asset);
		}
	}
}

public class ThemeInfo
{
	public string themeItemId;
	public string cardFaceId;
	public string cardBackId;
	public string themeName;
	public string themeContent;
	public int requireMoni;
	public int ownedFlag;
	public bool isOwned;
}