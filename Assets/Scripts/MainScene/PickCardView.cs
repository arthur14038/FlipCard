using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class PickCardView : AbstractView
{
	public VoidNoneParameter onClickBack;
	public VoidNoneParameter onClickPlay;
	[SerializeField]
	RectTransform group_PickCard;
	[SerializeField]
	RectTransform image_ShakeCircle;
	[SerializeField]
	Text text_PickModeTitle;
	[SerializeField]
	Text text_HighScore;
	[SerializeField]
	Text text_HighLevel;
	[SerializeField]
	Text text_LastLevel;
	[SerializeField]
	Text text_LastScore;
	[SerializeField]
	Text text_HighScoreTitle;
	[SerializeField]
	Text text_HighLevelTitle;
	[SerializeField]
	Text text_LastLevelTitle;
	[SerializeField]
	Text text_LastScoreTitle;
	[SerializeField]
	Text text_Play;

	public override IEnumerator Init()
	{
		PickGameRecord record = ModelManager.Instance.GetPickGameRecord();

		if(record.highScore > 0)
			text_HighScore.text = record.highScore.ToString();
		else
			text_HighScore.text = "- -";

		if(record.highLevel > 0)
			text_HighLevel.text = record.highLevel.ToString();
		else
			text_HighLevel.text = "- -";

		string lastLevel = "";
		for(int i = 0 ; i < record.lastLevels.Length ; ++i)
		{
			if(record.lastLevels[i] > 0)
				lastLevel += record.lastLevels[i] + "\n";
		}
		if(string.IsNullOrEmpty(lastLevel))
			text_LastLevel.text = "- -";
		else
			text_LastLevel.text = lastLevel;

		string lastScore = "";
		for(int i = 0 ; i < record.lastScores.Length ; ++i)
		{
			if(record.lastScores[i] > 0)
				lastScore += record.lastScores[i] + "\n";
		}
		if(string.IsNullOrEmpty(lastScore))
			text_LastScore.text = "- -";
		else
			text_LastScore.text = lastScore;

		escapeEvent = OnClickBack;
		yield return null;
		UpdateText();
		Localization.Event_ChangeLocaliztion += UpdateText;
	}

	void OnDestroy()
	{
		Localization.Event_ChangeLocaliztion -= UpdateText;
	}

	void UpdateText()
	{
		text_PickModeTitle.text = Localization.Get("PickCardView/Title");
		text_Play.text = Localization.Get("InfiniteView/Play");
		text_HighScoreTitle.text = Localization.Get("HighScoreTitle");
		text_HighLevelTitle.text = Localization.Get("HighLevelTitle");
		text_LastLevelTitle.text = Localization.Get("LastLevelTitle");
		text_LastScoreTitle.text = Localization.Get("LastScoreTitle");
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
		group_PickCard.anchoredPosition = Vector2.zero;
		yield return group_PickCard.DOAnchorPos(hideRight, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		base.HideUI(false);
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation()
	{
		StartCoroutine(ShakeEffect(image_ShakeCircle, 0.5f));
		group_PickCard.anchoredPosition = hideRight;
		yield return group_PickCard.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		showCoroutine = null;
	}
}
