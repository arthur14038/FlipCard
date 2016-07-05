using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class FlipCardView : AbstractView
{
	public VoidNoneParameter onClickBack;
	public VoidNoneParameter onClickPlay;
	[SerializeField]
	RectTransform group_FlipCard;
	[SerializeField]
	RectTransform image_ShakeCircle;
	[SerializeField]
	CanvasGroup group_Achievement;
	[SerializeField]
	GameObject image_ScoreGet;
	[SerializeField]
	GameObject image_ScoreNotGet;
	[SerializeField]
	GameObject[] image_Check;
	[SerializeField]
	GameObject[] image_Uncheck;
	[SerializeField]
	Image button_Play;
	[SerializeField]
	GameObject button_Skip;

	[SerializeField]
	Text text_Score;
	[SerializeField]
	Text text_HighScoreTitle;
	[SerializeField]
	Text text_HighScore;
	[SerializeField]
	Text text_HighLevelTitle;
	[SerializeField]
	Text text_HighLevel;
	[SerializeField]
	Text text_LastScoreTitle;
	[SerializeField]
	Text text_LastScore;
	[SerializeField]
	Text text_LastLevelTitle;
	[SerializeField]
	Text text_LastLevel;
	[SerializeField]
	Text text_Task;

	[SerializeField]
	Text text_InfiniteModeTitle;
	[SerializeField]
	Text text_Play;

	[SerializeField]
	Text text_FirstRewardName;
	[SerializeField]
	Text text_FirstRewardGoal;

	[SerializeField]
	Text text_SecondRewardName;
	[SerializeField]
	Text text_Task1;
	[SerializeField]
	Text text_Task1Explain;
	[SerializeField]
	Text text_Task2;
	[SerializeField]
	Text text_Task2Explain;
	[SerializeField]
	Text text_Task3;
	[SerializeField]
	Text text_Task3Explain;

	[SerializeField]
	Text text_ThirdRewardName;
	[SerializeField]
	Text text_Task4;
	[SerializeField]
	Text text_Task4Explain;
	[SerializeField]
	Text text_Task5;
	[SerializeField]
	Text text_Task5Explain;
	[SerializeField]
	Text text_Task6;
	[SerializeField]
	Text text_Task6Explain;

	int canSkipToLevel = 0;

	public override IEnumerator Init()
	{
		NormalGameRecord record = ModelManager.Instance.GetFlipCardGameRecord();
		if(record.highScore > 0)
			text_HighScore.text = record.highScore.ToString();
		else
			text_HighScore.text = "- -";

		if(record.highLevel > 0)
		{
			int level = record.highLevel / 1000;
			int round = record.highLevel % 1000;
			text_HighLevel.text = string.Format("{0}-{1}", level, round);
			//button_Skip.SetActive(true);
			//button_Play.rectTransform.sizeDelta = new Vector2(600f, 181.5f);
			canSkipToLevel = level;
        } else
		{
			canSkipToLevel = 0;
            text_HighLevel.text = "- -";
		}
		button_Skip.SetActive(false);
		button_Play.rectTransform.sizeDelta = new Vector2(848f, 181.5f);
		string lastLevel = "";
		for(int i = 0 ; i < record.lastLevel.Length ; ++i)
		{
			if(record.lastLevel[i] > 0)
			{
				int level = record.lastLevel[i] / 1000;
				int round = record.lastLevel[i] % 1000;
				lastLevel += string.Format("{0}-{1}\n", level, round);
			}
		}
		if(string.IsNullOrEmpty(lastLevel))
			text_LastLevel.text = "- -";
		else
			text_LastLevel.text = lastLevel;

		string lastScore = "";
		for(int i = 0 ; i < record.lastScore.Length ; ++i)
		{
			if(record.lastScore[i] > 0)
			{
				lastScore += record.lastScore[i] + "\n";
			}
		}
		if(string.IsNullOrEmpty(lastScore))
			text_LastScore.text = "- -";
		else
			text_LastScore.text = lastScore;

		escapeEvent = OnClickBack;
		yield return null;
		group_Achievement.gameObject.SetActive(false);

		int targetCount = 0;

		int targetScore = 40000;
		int currentScore = ModelManager.Instance.GetInfiniteScore();
		if(currentScore < targetScore)
		{
			image_ScoreGet.gameObject.SetActive(false);
			image_ScoreNotGet.gameObject.SetActive(true);
		} else
		{
			++targetCount;
            image_ScoreGet.gameObject.SetActive(true);
			image_ScoreNotGet.gameObject.SetActive(false);
		}
		text_Score.text = string.Format("{0}/<color=#099393FF>{1}</color>", currentScore, targetScore);

		for(int i = 0 ; i < record.achievement.Length ; ++i)
		{
			if(record.achievement[i])
			{
				++targetCount;
				image_Check[i].SetActive(true);
				image_Uncheck[i].SetActive(false);
			} else
			{
				image_Check[i].SetActive(false);
				image_Uncheck[i].SetActive(true);
			}
		}
		
		text_Task.text = string.Format(Localization.Get("InfiniteView/Task"), targetCount);
		UpdateText();
		Localization.Event_ChangeLocaliztion += UpdateText;
	}

	void OnDestroy()
	{
		Localization.Event_ChangeLocaliztion -= UpdateText;
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

	public void OnClickSkip()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");

	}

	public void OnClickTask()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");

		group_Achievement.gameObject.SetActive(true);
		group_Achievement.alpha = 0f;
		group_Achievement.DOFade(1f, 0.3f).SetEase(Ease.OutQuad);
    }

	public void OnClickLeaveTask()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		group_Achievement.DOFade(0f, 0.3f).SetEase(Ease.InQuad).OnComplete(
			delegate () {
				group_Achievement.gameObject.SetActive(false);
			}
		);
	}

	void UpdateText()
	{
		text_InfiniteModeTitle.text = Localization.Get("InfiniteView/Title");
		text_Play.text = Localization.Get("InfiniteView/Play");
		text_HighScoreTitle.text = Localization.Get("HighScoreTitle");
		text_HighLevelTitle.text = Localization.Get("HighLevelTitle");
		text_LastLevelTitle.text = Localization.Get("LastLevelTitle");
		text_LastScoreTitle.text = Localization.Get("LastScoreTitle");
		text_FirstRewardName.text = string.Format("\"{0}\"", Localization.Get("Theme_08/Name"));
		text_FirstRewardGoal.text = Localization.Get("FirstRewardGoal");
		text_SecondRewardName.text = Localization.Get("InfiniteView/SecondRewardName");
		text_Task1.text = Localization.Get("Task1Name");
		text_Task1Explain.text = Localization.Get("Task1Explain");
		text_Task2.text = Localization.Get("Task2Name");
		text_Task2Explain.text = Localization.Get("Task2Explain");
		text_Task3.text = Localization.Get("Task3Name");
		text_Task3Explain.text = Localization.Get("Task3Explain");
		text_ThirdRewardName.text = string.Format("\"{0}\"", Localization.Get("Theme_02/Name"));
		text_Task4.text = Localization.Get("Task4Name");
		text_Task4Explain.text = Localization.Get("Task4Explain");
		text_Task5.text = Localization.Get("Task5Name");
		text_Task5Explain.text = Localization.Get("Task5Explain");
		text_Task6.text = Localization.Get("Task6Name");
		text_Task6Explain.text = Localization.Get("Task6Explain");
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
