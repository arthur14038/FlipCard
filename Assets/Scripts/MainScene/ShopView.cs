using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using Soomla.Store;

public class ShopView : AbstractView
{
	enum ShopGroup {Theme, Shop, Moni, None}
	ShopGroup currentGroup = ShopGroup.None;
	public VoidNoneParameter onClickBack;
	public VoidString onClickEquipTheme;
	public VoidTwoString onClickEquipCard;
	public GameObject themePackUIPrefab;
	public Text text_CurrentMoni;
	public RectTransform group_Shop;
	public RectTransform content_ThemeUI;
	public RectTransform group_Theme;
	public RectTransform group_Moni;
	List<ThemePackUI> themePackUIList = new List<ThemePackUI>();

	public override IEnumerator Init()
	{
		text_CurrentMoni.text = StoreInventory.GetItemBalance(FlipCardStoreAsset.MONI_ITEM_ID).ToString();

		List<ThemePack> themePackList = InventoryManager.Instance.GetAllThemePack();

		for(int i = 0 ; i < themePackList.Count ; ++i)
		{
			GameObject tmp = Instantiate(themePackUIPrefab) as GameObject;
			tmp.transform.SetParent(content_ThemeUI);
			tmp.transform.localScale = Vector3.one;
			tmp.name = themePackUIPrefab.name + i.ToString();
			ThemePackUI themePackUI = tmp.GetComponent<ThemePackUI>();
			themePackUI.Init(themePackList[i], OnClickEquipTheme, OnClickEquipCard);
			themePackUIList.Add(themePackUI);
		}

		SetCurrentGroup(ShopGroup.Theme);
		yield return null;
	}

	public void OnClickBack()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		if(onClickBack != null)
			onClickBack();
	}

	public void BuyMoniPack(int tier)
	{
		Debug.LogFormat("BuyMoniPack Tier {0}", tier);
	}

	public void ToggleGroup(int groupIndex)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		SetCurrentGroup((ShopGroup)groupIndex);
	}

	void SetCurrentGroup(ShopGroup value)
	{
		currentGroup = value;
		switch(currentGroup)
		{
			case ShopGroup.Theme:
				foreach(ThemePackUI themePackUI in themePackUIList)
				{
					if(themePackUI.IsInBag)
						themePackUI.gameObject.SetActive(true);
					else
						themePackUI.gameObject.SetActive(false);
				}
				group_Moni.gameObject.SetActive(false);
				group_Theme.gameObject.SetActive(true);
				break;
			case ShopGroup.Shop:
				foreach(ThemePackUI themePackUI in themePackUIList)
				{
					if(themePackUI.IsInBag)
						themePackUI.gameObject.SetActive(false);
					else
						themePackUI.gameObject.SetActive(true);
				}
				group_Moni.gameObject.SetActive(false);
				group_Theme.gameObject.SetActive(true);
				break;
			case ShopGroup.Moni:
				group_Theme.gameObject.SetActive(false);
				group_Moni.gameObject.SetActive(true);
				break;
		}
	}

	void OnClickEquipTheme(string themeItemId)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickEquipTheme != null)
			onClickEquipTheme(themeItemId);
	}

	void OnClickEquipCard(string cardFaceItemId, string cardBackItemId)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickEquipCard != null)
			onClickEquipCard(cardFaceItemId, cardBackItemId);
	}

	protected override IEnumerator HideUIAnimation()
	{
		group_Shop.anchoredPosition = Vector2.zero;
		yield return group_Shop.DOAnchorPos(hideRight, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		base.HideUI(false);
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation()
	{
		group_Shop.gameObject.SetActive(true);
		group_Shop.anchoredPosition = hideRight;
		yield return group_Shop.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		showCoroutine = null;
	}	
}
