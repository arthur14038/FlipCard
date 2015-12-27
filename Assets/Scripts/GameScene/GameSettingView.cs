using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class GameSettingView : AbstractView {
	public VoidNoneParameter onClickResume;
	public VoidNoneParameter onClickExit;
	public Sprite[] complimentSprite;
	public Text text_TimeModeResultScore;
	public Text text_TimeModeResultCombo;
	public Text text_Player1ResultScore;
	public Text text_Player2ResultScore;
	public Text text_WinnerName;
    public CanvasGroup image_TimeModeScoreBoard;
	public CanvasGroup group_Pause;
	public Toggle toggle_Music;
	public Toggle toggle_Sound;
	public Image group_CompetitionGameOver;
	public Image group_TimeModeGameOver;
	public Image image_grade;
	public Image image_WinnerBoard;
	public Image image_WinnerFace;
	public RectTransform image_CompetitionGameOverWindow;
	public RectTransform button_CompetitionGameOverExit;
	public RectTransform image_CompetitionCharacterRight;
	public RectTransform image_CompetitionCharacterLeft;
	public RectTransform image_PauseWindow;
	public RectTransform image_TimeModeGameOverWindow;
	public RectTransform button_TimeModeGameOverExit;
	public RectTransform image_TimeModeCharacterRight;
	public RectTransform image_TimeModeCharacterLeft;
	public RectTransform text_TimeModeScoreTitle;
	public RectTransform text_TimeModeMaxComboTitle;
	public RectTransform image_TimeModeNewHighScoreHeader;
    public GameObject timeModeNewHighScoreEffect;
	public Sprite player1Sprite;
	public Sprite player2Sprite;
	float countSoundTime = 0.05f;
	Color player1Color = new Color(9f/255f, 147f / 255f, 147f / 255f, 178f / 255f);
	Color player2Color = new Color(255f / 255f, 73f / 255f, 73f / 255f, 178f / 255f);

	public override IEnumerator Init ()
	{
		AudioManager.Instance.SetListenToToggle(false);
		toggle_Music.isOn = !PlayerPrefsManager.MusicSetting;
		toggle_Sound.isOn = !PlayerPrefsManager.SoundSetting;
		AudioManager.Instance.SetListenToToggle(true);
		yield return 0;
		escapeEvent = OnClickEscape;
		group_Pause.gameObject.SetActive(false);
		group_TimeModeGameOver.gameObject.SetActive(false);
		group_CompetitionGameOver.gameObject.SetActive(false);
	}
	
	public void OnClickResume()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		HideUI(true);
	}

	public void OnClickExit()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickExit != null)
			onClickExit();
	}
    
	public void OnMusicValueChange(bool value)
	{
		AudioManager.Instance.MusicChangeValue(!value);
	}

	public void OnSoundValueChange(bool value)
	{
		AudioManager.Instance.SoundChangeValue(!value);
	}

	public void ShowCompetitionGameOver(int player1Score, int player2Score)
	{
		base.ShowUI(false);
		AudioManager.Instance.StopMusic();
		AudioManager.Instance.PlayOneShot("GameResult");
		text_Player1ResultScore.text = string.Format("SCORE: {0}", player1Score);
		text_Player2ResultScore.text = string.Format("SCORE: {0}", player2Score);

		if(player1Score > player2Score)
		{
			image_WinnerFace.sprite = player1Sprite;
            text_WinnerName.text = "PLAYER 1";
			image_WinnerBoard.color = player1Color;
        }
		else
		{
			image_WinnerFace.sprite = player2Sprite;
			text_WinnerName.text = "PLAYER 2";
			image_WinnerBoard.color = player2Color;
		}

		StartCoroutine(CompetitionGameOverEffect(player1Score, player2Score));
	}

	public void ShowTimeModeGameOverWindow(int score, int maxStar, int grade, bool newHighScore, bool newMaxStar)
	{
		base.ShowUI(false);
		AudioManager.Instance.StopMusic();
		AudioManager.Instance.PlayOneShot("GameResult");
		if(newHighScore || newMaxStar)
		{
			AudioManager.Instance.PlayOneShot("NewHighScore2");
			timeModeNewHighScoreEffect.SetActive(true);
			image_TimeModeNewHighScoreHeader.gameObject.SetActive(true);
		}else
		{
			image_TimeModeNewHighScoreHeader.gameObject.SetActive(false);
		}

		StartCoroutine(TimeModeGameOverEffect(score, maxStar, grade, newHighScore, newMaxStar));
	}

	void OnClickEscape()
	{
		OnClickResume();
	}

	IEnumerator CompetitionGameOverEffect(int player1Score, int player2Score)
	{
		image_WinnerBoard.gameObject.SetActive(false);
        image_CompetitionCharacterRight.gameObject.SetActive(false);
		image_CompetitionCharacterLeft.gameObject.SetActive(false);
		button_CompetitionGameOverExit.gameObject.SetActive(false);
		group_CompetitionGameOver.gameObject.SetActive(true);
		group_CompetitionGameOver.color = Color.clear;
		group_CompetitionGameOver.DOColor(Color.black * 0.7f, 0.3f);
		image_CompetitionGameOverWindow.anchoredPosition = new Vector2(0f, 1775f);
		yield return image_CompetitionGameOverWindow.DOAnchorPos(new Vector2(0f, 5f), 0.5f).SetEase(Ease.OutBack).WaitForCompletion();

		image_WinnerBoard.rectTransform.localScale = Vector3.zero;
		image_WinnerBoard.gameObject.SetActive(true);
		yield return image_WinnerBoard.rectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();

		image_CompetitionCharacterRight.anchoredPosition = new Vector2(750f, -392f);
		image_CompetitionCharacterLeft.anchoredPosition = new Vector2(-750f, -392f);
		image_CompetitionCharacterRight.gameObject.SetActive(true);
		image_CompetitionCharacterLeft.gameObject.SetActive(true);
		image_CompetitionCharacterRight.DOAnchorPos(new Vector2(217f, -392f), 0.2f).SetEase(Ease.OutCubic);
		yield return image_CompetitionCharacterLeft.DOAnchorPos(new Vector2(-217f, -392f), 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		
		button_CompetitionGameOverExit.gameObject.SetActive(true);
		button_CompetitionGameOverExit.localScale = new Vector3(1f, 0f, 1f);

		yield return button_CompetitionGameOverExit.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
	}

	IEnumerator TimeModeGameOverEffect(int score, int maxStar, int grade, bool newHighScore, bool newMaxStar)
	{
		image_TimeModeCharacterRight.gameObject.SetActive(false);
		image_TimeModeCharacterLeft.gameObject.SetActive(false);
		image_TimeModeScoreBoard.gameObject.SetActive(false);
		image_grade.gameObject.SetActive(false);
		button_TimeModeGameOverExit.gameObject.SetActive(false);
		group_TimeModeGameOver.gameObject.SetActive(true);
		group_TimeModeGameOver.color = Color.clear;
		group_TimeModeGameOver.DOColor(Color.black * 0.7f, 0.3f);
		image_TimeModeGameOverWindow.anchoredPosition = new Vector2(0f, 1572f);
		yield return image_TimeModeGameOverWindow.DOAnchorPos(new Vector2(0f, 13f), 0.5f).SetEase(Ease.OutBack).WaitForCompletion();

		image_grade.sprite = complimentSprite[grade - 1];
		image_grade.rectTransform.localScale = Vector3.zero;
		image_grade.gameObject.SetActive(true);
		yield return image_grade.rectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();

		image_TimeModeCharacterRight.anchoredPosition = new Vector2(750f, -117.5f);
		image_TimeModeCharacterLeft.anchoredPosition = new Vector2(-750f, -117.5f);
		image_TimeModeCharacterRight.gameObject.SetActive(true);
		image_TimeModeCharacterLeft.gameObject.SetActive(true);
		image_TimeModeCharacterRight.DOAnchorPos(new Vector2(250f, -117.5f), 0.2f).SetEase(Ease.OutCubic);
		image_TimeModeCharacterLeft.DOAnchorPos(new Vector2(-240f, -117.5f), 0.2f).SetEase(Ease.OutCubic);

		text_TimeModeResultCombo.text = "";
		text_TimeModeResultScore.text = "";
		image_TimeModeScoreBoard.gameObject.SetActive(true);
		image_TimeModeScoreBoard.alpha = 0f;
		yield return image_TimeModeScoreBoard.DOFade(1f, 0.4f).WaitForCompletion();

		if(score > 0)
		{
			float changeTime = 0.35f;
			float scoreChangeAmount = (score / (changeTime / Time.deltaTime));
			float tmpScore = 0;
			float forCountSound = changeTime;
			while(changeTime > 0f)
			{
				if(forCountSound - changeTime > countSoundTime)
				{
					forCountSound = changeTime;
					AudioManager.Instance.PlayOneShot("GameResultScoreCount");
				}
				text_TimeModeResultScore.text = ((int)tmpScore).ToString();
				tmpScore += scoreChangeAmount;
				yield return new WaitForEndOfFrame();
				changeTime -= Time.deltaTime;
			}
		}

		text_TimeModeResultScore.text = score.ToString();
		yield return text_TimeModeResultScore.rectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return text_TimeModeResultScore.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();

		if(maxStar > 0)
		{
			float changeTime = 0.35f;
			float comboChangeAmount = (maxStar / (changeTime / Time.deltaTime));
			float tmpCombo = 0;
			float forCountSound = changeTime;
			while(changeTime > 0f)
			{
				if(forCountSound - changeTime > countSoundTime)
				{
					forCountSound = changeTime;
					AudioManager.Instance.PlayOneShot("GameResultScoreCount");
				}
				text_TimeModeResultCombo.text = ((int)tmpCombo).ToString();
				tmpCombo += comboChangeAmount;
				yield return new WaitForEndOfFrame();
				changeTime -= Time.deltaTime;
			}
		}

		text_TimeModeResultCombo.text = maxStar.ToString();
		yield return text_TimeModeResultCombo.rectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return text_TimeModeResultCombo.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();

		button_TimeModeGameOverExit.gameObject.SetActive(true);
		button_TimeModeGameOverExit.localScale = new Vector3(1f, 0f, 1f);

		if(newHighScore)
			StartCoroutine(TextCelebrateEffect(text_TimeModeResultScore));
		if(newMaxStar)
			StartCoroutine(TextCelebrateEffect(text_TimeModeResultCombo));

		yield return button_TimeModeGameOverExit.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
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

	protected override IEnumerator HideUIAnimation ()
	{
		group_Pause.DOFade(0f, 0.3f);
		yield return image_PauseWindow.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
		if(onClickResume != null)
			onClickResume();
		base.HideUI(false);
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation ()
	{
		group_Pause.gameObject.SetActive(true);
        group_Pause.alpha = 0f;
		group_Pause.DOFade(1f, 0.3f);
		image_PauseWindow.localScale = Vector3.zero;
		yield return image_PauseWindow.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
		showCoroutine = null;
	}
}
