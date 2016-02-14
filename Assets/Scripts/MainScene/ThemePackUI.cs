﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Soomla.Store;

public class ThemePackUI : MonoBehaviour {
	enum ThemePackUIState {EquipedCard, EquipedTheme, EquipedBoth, CanBeEquiped }
	ThemePackUIState currentState;
	public Text text_ThemePrice;
	public Image image_Theme;
	public Image image_CardFace;
	public Image image_CardBack;
	public GameObject image_CardEquiped;
	public GameObject image_ThemeEquiped;
	public GameObject group_Theme;
	public GameObject group_Shop;
	public Toggle toggle_Scene;
	public Toggle toggle_Card;
	VoidString onClickThemeInfo;
	VoidString onEquipTheme;
	VoidTwoString onEquipCard;
	VoidThemePack onClickThemePrice;
	ThemePack themePack;
	bool inBag;
	public bool IsInBag {
		get
		{
			return inBag;
		}
		set
		{
			inBag = value;
			if(value)
			{
				group_Theme.SetActive(true);
				group_Shop.SetActive(false);
			}
			else
			{
				group_Shop.SetActive(true);
				group_Theme.SetActive(false);
			}
		}
	}

	public void Init(ThemePack themePack, VoidString onEquipTheme, VoidTwoString onEquipCard, VoidThemePack onClickThemePrice, VoidString onClickThemeInfo)
	{
		this.themePack = themePack;
		this.onEquipTheme = onEquipTheme;
		this.onEquipCard = onEquipCard;
		this.onClickThemePrice = onClickThemePrice;
		this.onClickThemeInfo = onClickThemeInfo;
		image_Theme.sprite = InventoryManager.Instance.GetSpriteById(themePack.theme.ItemId);
		image_CardFace.sprite = InventoryManager.Instance.GetSpriteById(themePack.cardFace.ItemId);
		image_CardBack.sprite = InventoryManager.Instance.GetSpriteById(themePack.cardBack.ItemId);
		text_ThemePrice.text = themePack.theme.PurchaseType.GetPrice();

		CheckUIState();
	}

	public void OnClickInfo()
	{
		if(onClickThemeInfo != null)
			onClickThemeInfo(themePack.theme.ItemId);
	}

	public void OnClickEquipTheme()
	{
		if(currentState == ThemePackUIState.CanBeEquiped || currentState == ThemePackUIState.EquipedCard)
		{
			onEquipTheme(themePack.theme.ItemId);
		}
	}

	public void OnClickEquipCard()
	{
		if(currentState == ThemePackUIState.CanBeEquiped || currentState == ThemePackUIState.EquipedTheme)
		{
			onEquipCard(themePack.cardFace.ItemId, themePack.cardBack.ItemId);
		}
	}

	public void OnClickBuy()
	{
		if(!IsInBag)
		{
			if(onClickThemePrice != null)
				onClickThemePrice(themePack);
		}
	}

	public void CheckUIState()
	{
		int themeBalance = StoreInventory.GetItemBalance(themePack.theme.ItemId);
		if(themeBalance == 0)
		{
			IsInBag = false;
			image_CardEquiped.SetActive(false);
			image_ThemeEquiped.SetActive(false);
		}
		else
		{
			IsInBag = true;
			ThemePackUIState stateShouldBe = ThemePackUIState.CanBeEquiped;

			if(InventoryManager.Instance.IsCardEquip(themePack.cardBack.ItemId))
				stateShouldBe = ThemePackUIState.EquipedCard;

			if(InventoryManager.Instance.IsThemeEquiped(themePack.theme.ItemId))
			{
				if(stateShouldBe == ThemePackUIState.EquipedCard)
					stateShouldBe = ThemePackUIState.EquipedBoth;
				else
					stateShouldBe = ThemePackUIState.EquipedTheme;
			}

			SetUIState(stateShouldBe);
		}
	}

	public IEnumerator EnterEffect(float enterDuration)
	{
		StartCoroutine(WaveEffect(image_CardBack.rectTransform, enterDuration));
		yield return StartCoroutine(WaveEffect(image_CardFace.rectTransform, enterDuration));
	}

	IEnumerator WaveEffect(RectTransform waveItem, float duration)
	{
		waveItem.rotation = Quaternion.Euler(Vector3.zero);
		yield return waveItem.DORotate(Vector3.back * (10f + Random.Range(-2f, 2f)), duration).SetEase(Ease.OutBounce).WaitForCompletion();
		float noise = Random.Range(0f, 0.125f);
		yield return waveItem.DORotate(Vector3.forward * 7.5f, 0.125f + noise).SetEase(Ease.OutQuad).WaitForCompletion();
		noise = Random.Range(0f, 0.125f);
		yield return waveItem.DORotate(Vector3.back * 5.0f, 0.125f + noise).SetEase(Ease.OutQuad).WaitForCompletion();
		noise = Random.Range(0f, 0.125f);
		yield return waveItem.DORotate(Vector3.forward * 2.5f, 0.125f + noise).SetEase(Ease.OutQuad).WaitForCompletion();
		noise = Random.Range(0f, 0.125f);
		yield return waveItem.DORotate(Vector3.zero, 0.125f + noise).SetEase(Ease.OutQuad).WaitForCompletion();
	}

	void SetUIState(ThemePackUIState state)
	{
		currentState = state;
		switch(currentState)
		{
			case ThemePackUIState.EquipedCard:
				image_CardEquiped.SetActive(true);
				image_ThemeEquiped.SetActive(false);
				toggle_Scene.isOn = false;
				toggle_Card.isOn = true;
				break;
			case ThemePackUIState.EquipedTheme:
				image_CardEquiped.SetActive(false);
				image_ThemeEquiped.SetActive(true);
				toggle_Card.isOn = false;
				toggle_Scene.isOn = true;
				break;
			case ThemePackUIState.EquipedBoth:
				image_CardEquiped.SetActive(true);
				image_ThemeEquiped.SetActive(true);
				toggle_Card.isOn = true;
				toggle_Scene.isOn = true;
				break;
			case ThemePackUIState.CanBeEquiped:
				image_CardEquiped.SetActive(false);
				image_ThemeEquiped.SetActive(false);
				toggle_Card.isOn = false;
				toggle_Scene.isOn = false;
				break;
		}
	}
}
