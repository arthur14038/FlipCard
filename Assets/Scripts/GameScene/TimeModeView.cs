using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class TimeModeView : AbstractView
{
	public IEnumeratorNoneParameter onCountDownFinished;
	public VoidNoneParameter onClickPause;
	public VoidNoneParameter onClickGameOverExit;
    public Slider timeBar;
	public Text text_CurrentScore;
	public Text text_CurrentRound;
	public Text text_ResultScore;
	public Text text_ResultCombo;
	public CanvasGroup image_ScoreBoard;
	public Image group_Counting;
	public Image group_GameOver;
	public RectTransform image_Counting3;
	public RectTransform image_Counting2;
	public RectTransform image_Counting1;
	public RectTransform image_CountingGo;
	public RectTransform image_GameOverWindow;
	public RectTransform button_GameOverExit;
	public RectTransform image_CharacterRight;
	public RectTransform image_CharacterLeft;
	public RectTransform text_ScoreTitle;
	public RectTransform text_MaxComboTitle;
	public RectTransform image_NewHighScoreHeader;
	public GameObject newHighScoreEffect;
	public GameObject timeIsRunning;
	public ShowComplimentTool group_Compliment;

	public override IEnumerator Init()
	{
		yield return null;
		SetScore(0);
		SetRound(0);
		group_Compliment.Init(hideLeft, hideRight);
		group_Counting.gameObject.SetActive(true);
		image_Counting3.gameObject.SetActive(false);
		image_Counting2.gameObject.SetActive(false);
		image_Counting1.gameObject.SetActive(false);
		image_CountingGo.gameObject.SetActive(false);
	}
	
	public void OnClickPause()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickPause != null)
			onClickPause();
	}

	public void OnClickGameOverExit()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickGameOverExit != null)
			onClickGameOverExit();
	}

	public void SetTimeBar(float value)
	{
		timeBar.value = value;
	}

	public void AddTimeEffect(float endValue)
	{
		timeBar.DOValue(endValue, 0.5f).SetDelay(0.5f);
	}

	public void SetScore(int score)
	{
		text_CurrentScore.text = string.Format("Score: {0}", score);
		text_CurrentScore.rectTransform.DOScale(1.2f, 0.15f).SetEase(Ease.InOutQuad).OnComplete(
			delegate () {
				text_CurrentScore.rectTransform.DOScale(1f, 0.15f).SetEase(Ease.InOutQuad);
			}
		);
	}

	public void SetRound(int round)
	{
		text_CurrentRound.text = string.Format("Round: {0}", round);
		text_CurrentRound.rectTransform.DOScale(1.2f, 0.15f).SetEase(Ease.InOutQuad).OnComplete(
			delegate () {
				text_CurrentRound.rectTransform.DOScale(1f, 0.15f).SetEase(Ease.InOutQuad);
			}
		);
	}

	public void ShowGameOverWindow(int score, int maxCombo, bool newHighScore, bool newMaxCombo)
	{
		AudioManager.Instance.StopMusic();
		AudioManager.Instance.PlayOneShot("GameResult");
		if(newHighScore || newMaxCombo)
		{
			AudioManager.Instance.PlayOneShot("NewHighScore2");
			newHighScoreEffect.SetActive(true);
			image_NewHighScoreHeader.gameObject.SetActive(true);
		}

		StartCoroutine(GameOverEffect(score, maxCombo, newHighScore, newMaxCombo));
	}

	public void ToggleTimeIsRunning(bool value)
	{
		if(timeIsRunning.activeSelf != value)
			timeIsRunning.SetActive(value);
	}

	public void ShowCompliment(int value)
	{
		group_Compliment.ShowCompliment(value);
	}

	IEnumerator GameOverEffect(int score, int maxCombo, bool newHighScore, bool newMaxCombo)
	{
		image_CharacterRight.gameObject.SetActive(false);
		image_CharacterLeft.gameObject.SetActive(false);
		image_ScoreBoard.gameObject.SetActive(false);
		button_GameOverExit.gameObject.SetActive(false);
		group_GameOver.gameObject.SetActive(true);
		group_GameOver.color = Color.clear;
		group_GameOver.DOColor(Color.black * 0.7f, 0.3f);
		image_GameOverWindow.anchoredPosition = new Vector2(0f, 1572f);
		yield return image_GameOverWindow.DOAnchorPos(new Vector2(0f, 72f), 0.5f).SetEase(Ease.OutBack).WaitForCompletion();

		image_CharacterRight.anchoredPosition = new Vector2(750f, -72f);
		image_CharacterLeft.anchoredPosition = new Vector2(-750f, -72f);
		image_CharacterRight.gameObject.SetActive(true);
		image_CharacterLeft.gameObject.SetActive(true);
		image_CharacterRight.DOAnchorPos(new Vector2(250f, -72f), 0.2f).SetEase(Ease.OutCubic);
		image_CharacterLeft.DOAnchorPos(new Vector2(-240f, -72f), 0.2f).SetEase(Ease.OutCubic);

		text_ResultCombo.text = "";
		text_ResultScore.text = "";
		image_ScoreBoard.gameObject.SetActive(true);
		image_ScoreBoard.alpha = 0f;
		yield return image_ScoreBoard.DOFade(1f, 0.4f).WaitForCompletion();

		if(score > 0)
		{
			float changeTime = 0.35f;
			float scoreChangeAmount = (score / (changeTime / Time.deltaTime));
			float tmpScore = 0;
			while(changeTime > 0f)
			{
				AudioManager.Instance.PlayOneShot("GameResultScoreCount");
				text_ResultScore.text = ((int)tmpScore).ToString();
				tmpScore += scoreChangeAmount;
				yield return new WaitForEndOfFrame();
				changeTime -= Time.deltaTime;
			}
		}

		text_ResultScore.text = score.ToString();
		yield return text_ResultScore.rectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return text_ResultScore.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();

		if(maxCombo > 0)
		{
			float changeTime = 0.35f;
			float comboChangeAmount = (maxCombo / (changeTime / Time.deltaTime));
			float tmpCombo = 0;
			while(changeTime > 0f)
			{
				AudioManager.Instance.PlayOneShot("GameResultScoreCount");
				text_ResultCombo.text = ((int)tmpCombo).ToString();
				tmpCombo += comboChangeAmount;
				yield return new WaitForEndOfFrame();
				changeTime -= Time.deltaTime;
			}
		}

		text_ResultCombo.text = maxCombo.ToString();
		yield return text_ResultCombo.rectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return text_ResultCombo.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();

		button_GameOverExit.gameObject.SetActive(true);
		button_GameOverExit.localScale = new Vector3(1f, 0f, 1f);

		if(newHighScore)
			StartCoroutine(TextCelebrateEffect(text_ResultScore));
		if(newMaxCombo)
			StartCoroutine(TextCelebrateEffect(text_ResultCombo));

		yield return button_GameOverExit.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
	}

	IEnumerator TextCelebrateEffect(Text theText)
	{
		while(this.gameObject.activeInHierarchy)
		{
			yield return theText.rectTransform.DOScale(1.3f, 0.5f).SetEase(Ease.OutQuart).WaitForCompletion();
			yield return theText.rectTransform.DOScale(1f, 1.5f).SetEase(Ease.OutBounce).WaitForCompletion();
			yield return new WaitForSeconds(0.2f);
		}
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
