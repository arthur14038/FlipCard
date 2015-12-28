using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ShopView : AbstractView
{
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

	protected override IEnumerator HideUIAnimation()
	{
		yield return group_Shop.DOAnchorPos(hideDown, 0.3f).SetEase(Ease.InQuad).WaitForCompletion();
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation()
	{
		group_Shop.gameObject.SetActive(true);
		group_Shop.anchoredPosition = hideDown;
		yield return group_Shop.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
		showCoroutine = null;
	}
	
}
