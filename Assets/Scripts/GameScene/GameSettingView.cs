using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class GameSettingView : AbstractView {
	public VoidNoneParameter onClickResume;
	public VoidNoneParameter onClickExit;
	public Sprite[] complimentSprite;
	public Sprite player1Sprite;
	public Sprite player2Sprite;
	public Image image_Mask;

	public CanvasGroup group_Pause;
	public Toggle toggle_Music;
	public Toggle toggle_Sound;
	public RectTransform image_PauseWindow;

	public RectTransform group_SinglePlayer;
	public RectTransform image_CharacterRight;
	public RectTransform image_CharacterLeft;
	public RectTransform button_SinglePlayerGameOverExit;
	public CanvasGroup image_SinglePlayerScoreBoard;
	public Text text_SinglePlayerTitle;
	public Text text_ScoreTitle;
	public Text text_Score;
    public Image image_Grade;
    public GameObject newHighScoreEffect;
	public GameObject image_NewHighScoreHeader;

	public RectTransform group_TwoPlayer;
	public Image image_WinnerBoard;
	public Image image_WinnerFace;
	public Text text_WinnerName;	
	public Text text_Player1ResultScore;
	public Text text_Player2ResultScore;
	public RectTransform button_CompetitionGameOverExit;
	public RectTransform image_Player1;
	public RectTransform image_Player2;

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
		group_SinglePlayer.gameObject.SetActive(false);
		group_TwoPlayer.gameObject.SetActive(false);
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
	
	public void ShowTwoPlayersGameOver(int player1Score, int player2Score)
	{
		ToggleMask(true, 0.3f);
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

	IEnumerator CompetitionGameOverEffect(int player1Score, int player2Score)
	{
		image_WinnerBoard.gameObject.SetActive(false);
		image_Player1.gameObject.SetActive(false);
		image_Player2.gameObject.SetActive(false);
		button_CompetitionGameOverExit.gameObject.SetActive(false);
		group_TwoPlayer.gameObject.SetActive(true);
		group_TwoPlayer.anchoredPosition = hideUp;
		yield return group_TwoPlayer.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();

		image_WinnerBoard.rectTransform.localScale = Vector3.zero;
		image_WinnerBoard.gameObject.SetActive(true);
		yield return image_WinnerBoard.rectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();

		image_Player1.anchoredPosition = new Vector2(750f, -392f);
		image_Player2.anchoredPosition = new Vector2(-750f, -392f);
		image_Player1.gameObject.SetActive(true);
		image_Player2.gameObject.SetActive(true);
		image_Player1.DOAnchorPos(new Vector2(217f, -392f), 0.2f).SetEase(Ease.OutCubic);
		yield return image_Player2.DOAnchorPos(new Vector2(-217f, -392f), 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();

		button_CompetitionGameOverExit.gameObject.SetActive(true);
		button_CompetitionGameOverExit.localScale = new Vector3(1f, 0f, 1f);

		yield return button_CompetitionGameOverExit.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
	}

	public void ShowSinglePlayerGameOver(int grade, int score, string scoreTitle, string headerTitle, bool recordBreak)
	{
		ToggleMask(true, 0.3f);
		base.ShowUI(false);
		AudioManager.Instance.StopMusic();
		AudioManager.Instance.PlayOneShot("GameResult");

		if(grade < 1)
			grade = 1;
		if(grade > complimentSprite.Length)
			grade = complimentSprite.Length;
        image_Grade.sprite = complimentSprite[grade - 1];
		text_ScoreTitle.text = scoreTitle;
		text_SinglePlayerTitle.text = headerTitle;

		if(recordBreak)
		{
			AudioManager.Instance.PlayOneShot("NewHighScore2");
			newHighScoreEffect.SetActive(true);
			image_NewHighScoreHeader.gameObject.SetActive(true);
		} else
		{
			newHighScoreEffect.SetActive(false);
			image_NewHighScoreHeader.gameObject.SetActive(false);
		}

		StartCoroutine(SinglePlayerGameOverEffect(score, recordBreak));
	}

	IEnumerator SinglePlayerGameOverEffect(int score, bool recordBreak)
	{
		image_CharacterLeft.gameObject.SetActive(false);
		image_CharacterRight.gameObject.SetActive(false);
		image_SinglePlayerScoreBoard.gameObject.SetActive(false);
		image_Grade.gameObject.SetActive(false);
		button_SinglePlayerGameOverExit.gameObject.SetActive(false);
		text_Score.gameObject.SetActive(false);
		group_SinglePlayer.gameObject.SetActive(true);
		group_SinglePlayer.anchoredPosition = hideUp;
		yield return group_SinglePlayer.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();

		image_Grade.rectTransform.localScale = Vector3.zero;
		image_Grade.gameObject.SetActive(true);
		yield return image_Grade.rectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();

		image_CharacterRight.anchoredPosition = new Vector2(750f, -117.5f);
		image_CharacterLeft.anchoredPosition = new Vector2(-750f, -117.5f);
		image_CharacterRight.gameObject.SetActive(true);
		image_CharacterLeft.gameObject.SetActive(true);
		image_CharacterRight.DOAnchorPos(new Vector2(250f, -93f), 0.2f).SetEase(Ease.OutCubic);
		image_CharacterLeft.DOAnchorPos(new Vector2(-240f, -93f), 0.2f).SetEase(Ease.OutCubic);

		image_SinglePlayerScoreBoard.alpha = 0f;
		image_SinglePlayerScoreBoard.gameObject.SetActive(true);
		yield return image_SinglePlayerScoreBoard.DOFade(1f, 0.4f).WaitForCompletion();

		text_Score.text = "";
		text_Score.gameObject.SetActive(true);
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
				text_Score.text = ((int)tmpScore).ToString();
				tmpScore += scoreChangeAmount;
				yield return new WaitForEndOfFrame();
				changeTime -= Time.deltaTime;
			}
		}

		text_Score.text = score.ToString();

		yield return text_Score.rectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return text_Score.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();

		if(recordBreak)
			StartCoroutine(TextCelebrateEffect(text_Score));

		button_SinglePlayerGameOverExit.localScale = new Vector3(1f, 0f, 1f);
		button_SinglePlayerGameOverExit.gameObject.SetActive(true);

		yield return button_SinglePlayerGameOverExit.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
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

	void OnClickEscape()
	{
		OnClickResume();
	}

	void ToggleMask(bool turnOn, float fadeDuration)
	{
		if(turnOn)
		{
			image_Mask.gameObject.SetActive(true);
			image_Mask.color = Color.clear;
			image_Mask.DOColor(Color.black * 0.7f, fadeDuration);
		} else
		{
			image_Mask.DOFade(0f, fadeDuration).OnComplete(
			delegate () {
				image_Mask.gameObject.SetActive(false);
			}
		);
		}
	}
	
	protected override IEnumerator HideUIAnimation ()
	{
		ToggleMask(false, 0.3f);
		group_Pause.DOFade(0f, 0.3f);
		yield return image_PauseWindow.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
		if(onClickResume != null)
			onClickResume();
		base.HideUI(false);
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation ()
	{
		ToggleMask(true, 0.3f);
        group_Pause.gameObject.SetActive(true);
        group_Pause.alpha = 0f;
		group_Pause.DOFade(1f, 0.3f);
		image_PauseWindow.localScale = Vector3.zero;
		yield return image_PauseWindow.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
		showCoroutine = null;
	}
}
