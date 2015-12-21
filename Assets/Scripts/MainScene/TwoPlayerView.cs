using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TwoPlayerView : AbstractView
{
	public VoidNoneParameter onClickBack;
	public VoidCardArrayLevelGameMode onClickPlay;
	public RectTransform group_2P;
	public RectTransform[] waveItems;

	public override IEnumerator Init()
	{
		escapeEvent = OnClickBack;
		group_2P.gameObject.SetActive(true);
		yield return null;
	}

	public void OnClickBack()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		if(onClickBack != null)
			onClickBack();
	}

	public void OnClickCompetition(int level)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickPlay != null)
			onClickPlay((CardArrayLevel)level, GameMode.Competition);
	}

	public void OnClickCooperation(int level)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickPlay != null)
			onClickPlay((CardArrayLevel)level, GameMode.Cooperation);
	}

	IEnumerator WaveEffect(RectTransform waveItem, float enterDuration)
	{
		waveItem.rotation = Quaternion.Euler(Vector3.zero);
		yield return waveItem.DORotate(Vector3.forward * 10f, enterDuration).SetEase(Ease.OutBounce).WaitForCompletion();
		yield return waveItem.DORotate(Vector3.back * 7.5f, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
		yield return waveItem.DORotate(Vector3.forward * 5.0f, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
		yield return waveItem.DORotate(Vector3.back * 2.5f, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
		yield return waveItem.DORotate(Vector3.zero, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
	}

	protected override IEnumerator HideUIAnimation()
	{
		group_2P.anchoredPosition = Vector2.zero;
		yield return group_2P.DOAnchorPos(hideRight, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		base.HideUI(false);
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation()
	{
		foreach(RectTransform waveItem in waveItems)
			StartCoroutine(WaveEffect(waveItem, 0.5f));
		group_2P.anchoredPosition = hideRight;
		yield return group_2P.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		showCoroutine = null;
	}
}
