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
	
	public FlipCardGameResult flipCardGameResult;

	public RectTransform group_TwoPlayer;
	public Image image_WinnerBoard;
	public Image image_WinnerFace;
	public Text text_WinnerName;	
	public Text text_Player1ResultScore;
	public Text text_Player2ResultScore;
	public RectTransform button_CompetitionGameOverExit;
	public RectTransform image_Player1;
	public RectTransform image_Player2;
	string player1ColorCode = "#02A887FF";
	string player2ColorCode = "#FF4D88FF";
	Color player1Color;
	Color player2Color;

	public override IEnumerator Init ()
	{
		ColorUtility.TryParseHtmlString(player1ColorCode, out player1Color);
		ColorUtility.TryParseHtmlString(player2ColorCode, out player2Color);

		AudioManager.Instance.SetListenToToggle(false);
		toggle_Music.isOn = !PlayerPrefsManager.MusicSetting;
		toggle_Sound.isOn = !PlayerPrefsManager.SoundSetting;
		AudioManager.Instance.SetListenToToggle(true);
		yield return 0;
		escapeEvent = OnClickEscape;
		group_Pause.gameObject.SetActive(false);
		group_TwoPlayer.gameObject.SetActive(false);
		flipCardGameResult.Init();
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
		text_Player1ResultScore.text = player1Score.ToString();
		text_Player2ResultScore.text = player2Score.ToString();

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

		image_Player1.anchoredPosition = new Vector2(-760f, -256f);
		image_Player2.anchoredPosition = new Vector2(760f, -256f);
		image_Player1.gameObject.SetActive(true);
		image_Player2.gameObject.SetActive(true);
		image_Player1.DOAnchorPos(new Vector2(-213f, -256f), 0.2f).SetEase(Ease.OutCubic);
		yield return image_Player2.DOAnchorPos(new Vector2(213f, -256f), 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();

		button_CompetitionGameOverExit.gameObject.SetActive(true);
		button_CompetitionGameOverExit.localScale = new Vector3(1f, 0f, 1f);

		yield return button_CompetitionGameOverExit.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
	}

	public void ShowSinglePlayerGameOver(int score, string level, bool recordBreak, bool[] thisTimeTask)
	{
		base.ShowUI(false);
		AudioManager.Instance.StopMusic();
		StartCoroutine(ToggleMask(true, 0.7f));
		flipCardGameResult.SetResult(score, level, recordBreak, thisTimeTask);
		StartCoroutine(FlipCardGameResultRoutine());
    }

	IEnumerator FlipCardGameResultRoutine()
	{
		yield return StartCoroutine(flipCardGameResult.ShowTimesUp());
		yield return StartCoroutine(flipCardGameResult.ShowText());
		yield return StartCoroutine(flipCardGameResult.ShowScoreBoard());
		yield return StartCoroutine(flipCardGameResult.ShowTaskAndButton());
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

	//void OnGUI()
	//{
	//	if(GUI.Button(new Rect(10, 10, 150, 50), "Init"))
	//	{
	//		flipCardGameResult.Init();
	//	}

	//	if(GUI.Button(new Rect(200, 10, 150, 50), "SetResult"))
	//	{
	//		flipCardGameResult.SetResult(600, "6-2", true, new bool[] { true, true, true, true, true, true });
	//	}

	//	if(GUI.Button(new Rect(10, 100, 150, 50), "FlipCardGameResultRoutine"))
	//	{
	//		StartCoroutine(FlipCardGameResultRoutine());
	//	}

	//	if(GUI.Button(new Rect(10, 100, 150, 50), "ShowText"))
	//	{
	//		StartCoroutine(flipCardGameResult.ShowText());
	//	}

	//	if(GUI.Button(new Rect(200, 100, 150, 50), "ShowScoreBoard"))
	//	{
	//		StartCoroutine(flipCardGameResult.ShowScoreBoard());
	//	}

	//	if(GUI.Button(new Rect(10, 200, 150, 50), "ShowTaskAndButton"))
	//	{
	//		StartCoroutine(flipCardGameResult.ShowTaskAndButton());
	//	}

	//	if(GUI.Button(new Rect(200, 200, 150, 50), "ShootStar"))
	//	{
	//		flipCardGameResult.ShootStar();
	//	}
	//}
}
