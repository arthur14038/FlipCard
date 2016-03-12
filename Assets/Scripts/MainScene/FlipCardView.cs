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
	public CanvasGroup group_Achievement;
	public GameObject image_ScoreGet;
	public GameObject image_ScoreNotGet;
	public GameObject[] image_Check;
	public GameObject[] image_Uncheck;
	public Text text_Score;
	public Text text_HighScore;
	public Text text_HighLevel;
	public Text text_LastScore;
	public Text text_LastLevel;
	public Text text_Task;

	public override IEnumerator Init()
	{
		GameRecord record = ModelManager.Instance.GetFlipCardGameRecord();
		if(record.highScore > 0)
			text_HighScore.text = record.highScore.ToString();
		else
			text_HighScore.text = "- -";

		if(record.highLevel > 0)
		{
			int level = record.highLevel / 1000;
			int round = record.highLevel % 1000;
			text_HighLevel.text = string.Format("{0}-{1}", level, round);
		} else
		{
			text_HighLevel.text = "- -";
		}
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

		text_Task.text = string.Format("TASK {0}/7", targetCount);
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
