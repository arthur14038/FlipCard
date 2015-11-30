using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class GameMenuView : AbstractView {
	public Slider timeBar;
	public Button button_Ready;
	public Text text_Score;
	public Text text_GameOverScore;
	public GameObject image_Mask;
	public VoidNoneParameter onClickPause;
	public VoidNoneParameter onClickResume;
	public VoidNoneParameter onClickExit;
	public VoidNoneParameter onClickReady;
	public CanvasGroup group_Game;
	public CanvasGroup group_Pause;
	public CanvasGroup group_GameOver;
	public RectTransform image_PauseWindow;
	public RectTransform image_GameOverWindow;

	public override IEnumerator Init ()
	{
		yield return 0;
		ToggleMask(true);		
		button_Ready.gameObject.SetActive(true);
		group_Game.gameObject.SetActive(true);
		group_Pause.gameObject.SetActive(false);
		group_GameOver.gameObject.SetActive(false);
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
		text_Score.text = string.Format("Score: {0}", score);
	}

	public void ShowGameOverWindow(int score)
	{
		text_GameOverScore.text = string.Format("Score: {0}", score);
		group_GameOver.gameObject.SetActive(true);
		group_GameOver.alpha = 0f;
		group_GameOver.DOFade(1f, 0.3f);		
		image_GameOverWindow.localScale = Vector3.zero;
		image_GameOverWindow.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
	}

	public void OnClickPause()
	{
		if(onClickPause != null)
			onClickPause();
		StartCoroutine(PauseEffect());
	}

	public void OnClickResume()
	{
		StartCoroutine(ResumeEffect());
	}

	public void OnClickExit()
	{
		if(onClickExit != null)
			onClickExit();
	}

	public void OnClickReady()
	{
		button_Ready.interactable = false;
		button_Ready.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
		if(onClickReady != null)
			onClickReady();
	}

	public void ToggleMask(bool value)
	{
		if(image_Mask.activeSelf != value)
			image_Mask.SetActive(value);
	}

	IEnumerator PauseEffect()
	{
		group_Pause.gameObject.SetActive(true);
		group_Pause.alpha = 0f;
		image_PauseWindow.localScale = Vector3.zero;
		group_Pause.DOFade(1f, 0.3f);
		yield return image_PauseWindow.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
	}

	IEnumerator ResumeEffect()
	{
		group_Pause.DOFade(0f, 0.3f);
		yield return image_PauseWindow.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
		group_Pause.gameObject.SetActive(false);
		if(onClickResume != null)
			onClickResume();
	}

	protected override IEnumerator HideUIAnimation ()
	{
		yield return 0;
	}

	protected override IEnumerator ShowUIAnimation ()
	{
		yield return 0;
	}
}
