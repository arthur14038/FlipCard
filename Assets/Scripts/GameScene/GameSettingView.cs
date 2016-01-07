using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class GameSettingView : AbstractView {
	public VoidNoneParameter onClickResume;
	public VoidNoneParameter onClickExit;
	public Sprite player1Sprite;
	public Sprite player2Sprite;
	public Image image_Mask;

	public CanvasGroup group_Pause;
	public Toggle toggle_Music;
	public Toggle toggle_Sound;
	public RectTransform image_PauseWindow;

	public Text[] text_Condition;
	public Toggle[] toggle_Condition;
	public RectTransform[] image_Star;
    public RectTransform group_SinglePlayer;
	public RectTransform image_CharacterRight;
	public RectTransform image_CharacterLeft;
	public RectTransform button_SinglePlayerGameOverExit;
	public CanvasGroup image_SinglePlayerScoreBoard;
	public Text text_SinglePlayerTitle;
	public Text text_ScoreTitle;
	public Text text_Score;
	public Text text_Mode;
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
		yield return StartCoroutine(ToggleMask(true, 0.7f));

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

	public void ShowSinglePlayerGameOver(bool[] achieveCondition, string headerTitle, string mode, string scoreTitle, string score, string[] conditions, bool recordBreak)
	{
		base.ShowUI(false);
		AudioManager.Instance.StopMusic();
		AudioManager.Instance.PlayOneShot("GameResult");

		text_SinglePlayerTitle.text = headerTitle;
		text_Mode.text = mode;
		text_ScoreTitle.text = scoreTitle;
		text_Score.text = score;
		for(int i = 0 ; i < text_Condition.Length ; ++i)
			text_Condition[i].text = conditions[i];

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

		StartCoroutine(SinglePlayerGameOverEffect(achieveCondition));
	}

	IEnumerator SinglePlayerGameOverEffect(bool[] achieveCondition)
	{
		yield return StartCoroutine(ToggleMask(true, 0.7f));

		image_CharacterLeft.gameObject.SetActive(false);
		image_CharacterRight.gameObject.SetActive(false);
		image_SinglePlayerScoreBoard.gameObject.SetActive(false);
		button_SinglePlayerGameOverExit.gameObject.SetActive(false);
		text_Score.gameObject.SetActive(false);
		for(int i = 0 ; i < toggle_Condition.Length ; ++i)
		{
			toggle_Condition[i].gameObject.SetActive(false);
			toggle_Condition[i].isOn = false;
        }
        for(int i = 0 ; i < image_Star.Length ; ++i)
			image_Star[i].gameObject.SetActive(false);
		group_SinglePlayer.gameObject.SetActive(true);
		group_SinglePlayer.anchoredPosition = hideUp;
		yield return group_SinglePlayer.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
		
		image_CharacterRight.anchoredPosition = new Vector2(750f, 17f);
		image_CharacterLeft.anchoredPosition = new Vector2(-750f, 14.5f);
		image_CharacterRight.gameObject.SetActive(true);
		image_CharacterLeft.gameObject.SetActive(true);
		image_CharacterRight.DOAnchorPos(new Vector2(247f, 17f), 0.2f).SetEase(Ease.OutCubic);
		image_CharacterLeft.DOAnchorPos(new Vector2(-257f, 14.5f), 0.2f).SetEase(Ease.OutCubic);

		int[] activeCondition = new int[4];
		int starCount = 0;

		for(int i = 0 ; i < toggle_Condition.Length ; ++i)
		{
			toggle_Condition[i].gameObject.SetActive(true);
			if(achieveCondition[i])
			{
				++starCount;
				activeCondition[i] = i;
			}
		}

		image_SinglePlayerScoreBoard.alpha = 0f;
		image_SinglePlayerScoreBoard.gameObject.SetActive(true);
		yield return image_SinglePlayerScoreBoard.DOFade(1f, 0.4f).WaitForCompletion();

		text_Score.gameObject.SetActive(true);
		yield return text_Score.rectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return text_Score.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();

		for(int i = 0 ; i < starCount ; ++i)
		{
			toggle_Condition[activeCondition[i]].isOn = true;
			image_Star[i].gameObject.SetActive(true);
			image_Star[i].DOScale(1f, 0.3f).SetEase(Ease.OutBack);
			yield return toggle_Condition[activeCondition[i]].transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
			yield return toggle_Condition[activeCondition[i]].transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		}

		button_SinglePlayerGameOverExit.localScale = new Vector3(1f, 0f, 1f);
		button_SinglePlayerGameOverExit.gameObject.SetActive(true);

		yield return button_SinglePlayerGameOverExit.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
	}
	
	IEnumerator ToggleMask(bool turnOn, float fadeDuration)
	{
		if(turnOn)
		{
			image_Mask.gameObject.SetActive(true);
			image_Mask.color = Color.clear;
			yield return image_Mask.DOColor(Color.black * 0.7f, fadeDuration).WaitForCompletion();
		}
		else
		{
			yield return image_Mask.DOFade(0f, fadeDuration).WaitForCompletion();
			image_Mask.gameObject.SetActive(false);
		}
	}

	void OnClickEscape()
	{
		OnClickResume();
	}

	protected override IEnumerator HideUIAnimation ()
	{
		StartCoroutine(ToggleMask(false, 0.3f));
		group_Pause.DOFade(0f, 0.3f);
		yield return image_PauseWindow.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
		if(onClickResume != null)
			onClickResume();
		base.HideUI(false);
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation ()
	{
		StartCoroutine(ToggleMask(true, 0.3f));
        group_Pause.gameObject.SetActive(true);
        group_Pause.alpha = 0f;
		group_Pause.DOFade(1f, 0.3f);
		image_PauseWindow.localScale = Vector3.zero;
		yield return image_PauseWindow.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
		showCoroutine = null;
	}
}
