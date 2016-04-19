using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class FlipCardGameView : AbstractView
{
	public IEnumeratorNoneParameter onCountDownFinished;
	public VoidNoneParameter onClickPause;
	public VoidNoneParameter onClickNextLevel;
	public VoidNoneParameter onClickBonusTime;
	[SerializeField]
	Text text_CurrentLevel;
	[SerializeField]
	Text text_CurrentScore;
	[SerializeField]
	Text text_Level;
	[SerializeField]
	Text text_Round;
	[SerializeField]
	Text text_AddScore;
	[SerializeField]
	Text text_Task;
	[SerializeField]
	RectTransform group_Counting;
	[SerializeField]
	RectTransform image_Counting3;
	[SerializeField]
	RectTransform image_Counting2;
	[SerializeField]
	RectTransform image_Counting1;
	[SerializeField]
	RectTransform image_CountingGo;
	[SerializeField]
	RectTransform group_FeverTime;
	[SerializeField]
	RectTransform group_Perfect;
	[SerializeField]
	RectTransform image_WindowBG;
	[SerializeField]
	RectTransform group_SpecialCard;
	[SerializeField]
	RectTransform image_BombCard;
	[SerializeField]
	RectTransform image_FlareCard;
	[SerializeField]
	RectTransform group_Task;
	[SerializeField]
	Button button_Pause;
	[SerializeField]
	Button button_BonusTime;
	[SerializeField]
	Button button_NextLevel;
	[SerializeField]
	Slider timeBar;
	[SerializeField]
	InfiniteProgressBar infiniteProgressBar;
	[SerializeField]
	GameObject timeIsRunning;
	[SerializeField]
	GameObject feverTimeEffect;

	[SerializeField]
	Text text_LevelTitle;
	[SerializeField]
	Text text_ScoreTitle;

	[SerializeField]
	Text text_Level1;
	[SerializeField]
	Text text_Level1Tip;
	[SerializeField]
	Text text_Perfect;
	[SerializeField]
	Text text_SpecialCard;

	Vector2 feverTimePos = new Vector2(0f, -832f);
	Vector2 nextLevelPos = new Vector2(0f, -112f);
	int nextLevel;

	public override IEnumerator Init()
	{
		group_Counting.gameObject.SetActive(true);
		image_Counting3.gameObject.SetActive(false);
		image_Counting2.gameObject.SetActive(false);
		image_Counting1.gameObject.SetActive(false);
		image_CountingGo.gameObject.SetActive(false);
		group_FeverTime.gameObject.SetActive(false);
		button_BonusTime.gameObject.SetActive(false);
		group_Perfect.gameObject.SetActive(false);
		image_WindowBG.gameObject.SetActive(false);
		text_AddScore.gameObject.SetActive(false);
		group_Task.gameObject.SetActive(false);
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
		text_LevelTitle.text = Localization.Get("InfiniteGameView/LevelTitle");
		text_ScoreTitle.text = Localization.Get("InfiniteGameView/ScoreTitle");
		text_Level1.text = Localization.Get("InfiniteGameView/Level1");
		text_Level1Tip.text = Localization.Get("InfiniteGameView/Level1Tip");
		text_Perfect.text = Localization.Get("InfiniteGameView/Perfect");
		text_SpecialCard.text = Localization.Get("InfiniteGameView/SpecialCardAppear");
    }

	public void OnClickPause()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickPause != null)
			onClickPause();
	}

	public void OnClickNextLevel()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		button_NextLevel.interactable = false;
		StartCoroutine(GoNextLevel());
	}

	public void OnClickBonusTime()
	{
		if(onClickBonusTime != null)
			onClickBonusTime();
	}

	public void SetTimeBar(float value)
	{
		timeBar.value = value;
	}

	public void AddTimeEffect(float endValue)
	{
		timeBar.DOValue(endValue, 0.5f).SetDelay(0.5f);
	}

	public void SetCurrentScore(int score)
	{
		text_CurrentScore.text = score.ToString();
	}

	public void SetCurrentLevel(int level, int round)
	{
		text_CurrentLevel.text = string.Format("{0}-{1}", level, round);
	}

	public void ToggleTimeIsRunning(bool value)
	{
		if(timeIsRunning.activeSelf != value)
			timeIsRunning.SetActive(value);
	}

	public void ToggleFeverTimeEffect(bool value)
	{
		if(feverTimeEffect != null && feverTimeEffect.activeSelf != value)
			feverTimeEffect.SetActive(value);
	}
	
	public void SetPauseButtonState(bool value)
	{
		button_Pause.interactable = value;
	}
	
	public IEnumerator FeverTimeEffect()
	{
		group_FeverTime.gameObject.SetActive(true);
		group_FeverTime.anchoredPosition = feverTimePos + hideRight;
		yield return group_FeverTime.DOAnchorPos(feverTimePos, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
		yield return group_FeverTime.DOAnchorPos(feverTimePos + hideLeft, 0.5f).SetDelay(0.3f).SetEase(Ease.InBack).WaitForCompletion();
		group_FeverTime.gameObject.SetActive(false);
	}

	public IEnumerator PerfectEffect()
	{
		group_Perfect.gameObject.SetActive(true);

		group_Perfect.localScale = Vector3.zero;
		yield return group_Perfect.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
		yield return new WaitForSeconds(0.2f);
		yield return group_Perfect.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();

		group_Perfect.gameObject.SetActive(false);
	}

	public IEnumerator ShowNextLevelUI(int level, int round, int originalScore, int addScore, int specialCardType, bool thisLevelTaskComplete)
	{
		nextLevel = level - 1;
        button_BonusTime.interactable = false;
		yield return button_BonusTime.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
		button_BonusTime.gameObject.SetActive(false);

		yield return text_AddScore.rectTransform.DOScale(1.5f, 0.25f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return text_AddScore.rectTransform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return new WaitForSeconds(0.2f);

		float changeAmount = (float)addScore / (0.5f/Time.deltaTime);
		float scoreForShow = originalScore;
		float addScoreForShow = addScore;
		float lastTimeSound = -0.03f;
		while(addScoreForShow > 0)
		{
			scoreForShow += changeAmount;
			addScoreForShow -= changeAmount;
			SetCurrentScore((int)scoreForShow);
			SetBonusScore((int)addScoreForShow);
			if(Time.time - lastTimeSound >= 0.03f)
			{
				AudioManager.Instance.PlayOneShot("GameResultScoreCount");
				lastTimeSound = Time.time;
			}
			yield return new WaitForEndOfFrame();
		}

		SetCurrentScore(originalScore + addScore);
		SetBonusScore(0);

		yield return text_CurrentScore.rectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return text_CurrentScore.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return new WaitForSeconds(0.2f);

		text_AddScore.gameObject.SetActive(false);

		if(thisLevelTaskComplete)
		{
			text_Task.text = string.Format(Localization.Get("InfiniteGameView/TaskComplete"), level - 1);
			group_Task.gameObject.SetActive(true);

			group_Task.localScale = Vector3.zero;
			yield return group_Task.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
			yield return new WaitForSeconds(0.2f);
			yield return group_Task.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();

			group_Task.gameObject.SetActive(false);
		}

		text_Level.text = string.Format(Localization.Get("InfiniteGameView/LevelTipTitle"), level);
		text_Round.text = string.Format(Localization.Get("InfiniteGameView/LevelTip"), round);
		infiniteProgressBar.SetProgress(level - 2);
		if(specialCardType > 0)
			group_SpecialCard.gameObject.SetActive(true);
		else
			group_SpecialCard.gameObject.SetActive(false);
		image_WindowBG.anchoredPosition = hideDown;
		if(!image_WindowBG.gameObject.activeSelf)
			image_WindowBG.gameObject.SetActive(true);
		image_WindowBG.DOAnchorPos(Vector2.zero + nextLevelPos, 0.5f).SetEase(Ease.OutBack);
		button_NextLevel.interactable = true;
	}

	public IEnumerator ShowBonusButton()
	{
		text_AddScore.text = "";
		text_AddScore.gameObject.SetActive(true);
        button_BonusTime.gameObject.SetActive(true);
		button_BonusTime.interactable = false;

		button_BonusTime.transform.localScale = Vector3.zero;
		yield return button_BonusTime.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
		button_BonusTime.interactable = true;
	}
	
	public void SetBonusScore(int value)
	{
		text_AddScore.text = "+" + value;		
    }
	
	IEnumerator GoNextLevel()
	{
		infiniteProgressBar.SetProgress(nextLevel, true);
		yield return new WaitForSeconds(0.5f);
		yield return image_WindowBG.DOAnchorPos(hideDown, 0.5f).SetEase(Ease.InBack).WaitForCompletion();
		image_WindowBG.gameObject.SetActive(false);
		if(onClickNextLevel != null)
			onClickNextLevel();
	}

	protected override IEnumerator HideUIAnimation()
	{
		yield return null;
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation()
	{
		AudioManager.Instance.PlayOneShot("StartGameCountDown");
		Vector3 flipDown = new Vector3(0f, 0.9f, 1f);
		float delayTime = 0.25f;
		float inTime = 0.245f;
		float outTime = 0.17f;

		//group_Counting.color = Color.black * 0.7f;
		group_Counting.gameObject.SetActive(true);

		image_Counting3.localScale = flipDown;
		image_Counting3.gameObject.SetActive(true);
		yield return image_Counting3.DOScale(Vector3.one, inTime).WaitForCompletion();
		yield return image_Counting3.DOScale(flipDown, outTime).SetDelay(delayTime).SetEase(Ease.OutQuad).WaitForCompletion();
		image_Counting3.gameObject.SetActive(false);

		image_Counting2.localScale = flipDown;
		image_Counting2.gameObject.SetActive(true);
		yield return image_Counting2.DOScale(Vector3.one, inTime).WaitForCompletion();
		yield return image_Counting2.DOScale(flipDown, outTime).SetDelay(delayTime).SetEase(Ease.OutQuad).WaitForCompletion();
		image_Counting2.gameObject.SetActive(false);

		image_Counting1.localScale = flipDown;
		image_Counting1.gameObject.SetActive(true);
		yield return image_Counting1.DOScale(Vector3.one, inTime).WaitForCompletion();
		yield return image_Counting1.DOScale(flipDown, outTime).SetDelay(delayTime).SetEase(Ease.OutQuad).WaitForCompletion();
		image_Counting1.gameObject.SetActive(false);

		image_CountingGo.localScale = flipDown;
		image_CountingGo.gameObject.SetActive(true);
		yield return image_CountingGo.DOScale(Vector3.one, inTime + 0.15f).WaitForCompletion();
		//group_Counting.DOFade(0f, 0.3f).SetDelay(delayTime + 0.5f);
		yield return group_Counting.DOScale(0f, 0.25f).SetDelay(delayTime + 0.07f).SetEase(Ease.InBack).WaitForCompletion();
		group_Counting.gameObject.SetActive(false);

		if(onCountDownFinished != null)
			StartCoroutine(onCountDownFinished());

		showCoroutine = null;
	}
}
