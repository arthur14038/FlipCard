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
	public Image group_Counting;
	public Text text_CurrentLevel;
	public Text text_CurrentScore;
	public Text text_NextLevelTitle;
	public Text text_AddScore;
	public RectTransform image_Counting3;
	public RectTransform image_Counting2;
	public RectTransform image_Counting1;
	public RectTransform image_CountingGo;
	public RectTransform group_FeverTime;
	public RectTransform group_Perfect;
	public RectTransform image_WindowBG;
	public Button button_Pause;
	public Button button_BonusTime;
    public Slider timeBar;
	public GameObject timeIsRunning;
	public GameObject feverTimeEffect;
	private Vector2 feverTimePos = new Vector2(0f, -832f);

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
		yield return null;
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
		StartCoroutine(GoNextLevel());
	}

	public void OnClickBonusTime()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
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

	public IEnumerator ShowNextLevelUI(string nextLevelTitle, int originalScore, int addScore)
	{
		button_BonusTime.interactable = false;
		yield return button_BonusTime.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
		button_BonusTime.gameObject.SetActive(false);

		yield return text_AddScore.rectTransform.DOScale(1.5f, 0.25f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return text_AddScore.rectTransform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return new WaitForSeconds(0.2f);

		float changeAmount = (float)addScore / (0.5f/Time.deltaTime);
		float scoreForShow = originalScore;
		float addScoreForShow = addScore;
		float forCountSound = scoreForShow;
        while(addScoreForShow > 0)
		{
			scoreForShow += changeAmount;
			addScoreForShow -= changeAmount;
			SetCurrentScore((int)scoreForShow);
			SetBonusScore((int)addScoreForShow);
			if(scoreForShow - forCountSound >= 1f)
			{
				AudioManager.Instance.PlayOneShot("GameResultScoreCount");
				forCountSound = scoreForShow;
			}
			yield return new WaitForEndOfFrame();
		}

		SetCurrentScore(originalScore + addScore);
		SetBonusScore(0);

		yield return text_CurrentScore.rectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return text_CurrentScore.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return new WaitForSeconds(0.2f);

		text_AddScore.gameObject.SetActive(false);

		text_NextLevelTitle.text = nextLevelTitle;
		image_WindowBG.anchoredPosition = hideDown;
		if(!image_WindowBG.gameObject.activeSelf)
			image_WindowBG.gameObject.SetActive(true);
		image_WindowBG.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack);
	}

	public IEnumerator ShowBonusButton()
	{
		yield return new WaitForSeconds(0.3f);  //等桌面清空
		text_AddScore.text = "";
		text_AddScore.gameObject.SetActive(true);
        button_BonusTime.gameObject.SetActive(true);
		button_BonusTime.interactable = true;

		button_BonusTime.transform.localScale = Vector3.zero;
		yield return button_BonusTime.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
	}
	
	public void SetBonusScore(int value)
	{
		text_AddScore.text = "+" + value;		
    }
	
	IEnumerator GoNextLevel()
	{
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

		group_Counting.color = Color.black * 0.7f;
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
		group_Counting.DOFade(0f, 0.3f).SetDelay(delayTime + 0.5f);
		yield return image_CountingGo.DOScale(flipDown, outTime).SetDelay(delayTime + 0.15f).SetEase(Ease.OutQuad).WaitForCompletion();
		image_CountingGo.gameObject.SetActive(false);
		group_Counting.gameObject.SetActive(false);

		if(onCountDownFinished != null)
			StartCoroutine(onCountDownFinished());

		showCoroutine = null;
	}
}
