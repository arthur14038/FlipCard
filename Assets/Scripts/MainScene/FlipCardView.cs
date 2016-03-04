using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class FlipCardView : AbstractView
{
	public VoidNoneParameter onClickBack;
	public VoidNoneParameter onClickPlay;
	public RectTransform group_FlipCard;
	public RectTransform image_ShakeCircle;
	public Text text_HighScore;
	public Text text_HighLevel;
	public Text text_LastScore;
	public Text text_LastLevel;

	public override IEnumerator Init()
	{
		escapeEvent = OnClickBack;
		yield return null;
		GameRecord record = ModelManager.Instance.GetFlipCardGameRecord();
		text_HighScore.text = record.highScore.ToString();

		if(record.highLevel > 0)
		{
			int level = record.highLevel / 1000;
			int round = record.highLevel % 1000;
			text_HighLevel.text = string.Format("{0}-{1}", level, round);
		} else
		{
			text_HighLevel.text = "";
        }
		string lastLevel = "";
		for(int i = 0 ; i < record.lastLevel.Length ; ++i)
		{
			if(record.lastLevel[i] > 0)
			{
				int level = record.lastLevel[i] / 1000;
				int round = record.lastLevel[i] % 1000;
				lastLevel += string.Format("{0}-{1}\n", level, round) ;
			}
		}
		text_LastLevel.text = lastLevel;

		string lastScore = "";
		for(int i = 0 ; i < record.lastScore.Length ; ++i)
		{
			if(record.lastScore[i] > 0)
			{
				lastScore += record.lastScore[i] + "\n";
			}
		}
		text_LastScore.text = lastScore;
	}

	public void OnClickBack()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		if(onClickBack != null)
			onClickBack();
	}

	public void OnClickPlay()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickPlay != null)
			onClickPlay();
	}

	public IEnumerator ShakeEffect(RectTransform shakeItem, float enterDuration)
	{
		shakeItem.rotation = Quaternion.Euler(Vector3.zero);
		yield return shakeItem.DORotate(Vector3.forward * 10f, enterDuration).SetEase(Ease.OutBounce).WaitForCompletion();
		yield return shakeItem.DORotate(Vector3.back * 7.5f, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
		yield return shakeItem.DORotate(Vector3.forward * 5.0f, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
		yield return shakeItem.DORotate(Vector3.back * 2.5f, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
		yield return shakeItem.DORotate(Vector3.zero, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
	}

	protected override IEnumerator HideUIAnimation()
	{
		group_FlipCard.anchoredPosition = Vector2.zero;
		yield return group_FlipCard.DOAnchorPos(hideRight, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		base.HideUI(false);
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation()
	{
		StartCoroutine(ShakeEffect(image_ShakeCircle, 0.5f));
		group_FlipCard.anchoredPosition = hideRight;
		yield return group_FlipCard.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		showCoroutine = null;
	}
}
