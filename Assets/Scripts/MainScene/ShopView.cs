using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ShopView : AbstractView
{
	enum ShopGroup {Theme, Shop, Moni, None}
	ShopGroup currentGroup = ShopGroup.None;
	public VoidNoneParameter onClickBack;
	public RectTransform group_Shop;

	public override IEnumerator Init()
	{
		yield return null;
	}

	public void OnClickBack()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		if(onClickBack != null)
			onClickBack();
	}

	public void ToggleGroup(int groupIndex)
	{
		currentGroup = (ShopGroup)groupIndex;
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
