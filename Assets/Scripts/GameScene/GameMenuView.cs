using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class GameMenuView : AbstractView {
	public Slider timeBar;
	public Button button_Ready;
	public Text text_Score;
    public Text text_Combo;
    public Text text_GameOverScore;
	public Text text_MaxCombo;
	public GameObject image_Mask;
	public VoidNoneParameter onClickPause;
	public VoidNoneParameter onClickResume;
	public VoidNoneParameter onClickExit;
	public VoidNoneParameter onClickReady;
	public CanvasGroup group_Game;
	public CanvasGroup group_Pause;
    public CanvasGroup image_ScoreBoard;
    public Image group_GameOver;
    public RectTransform image_PauseWindow;
	public RectTransform image_GameOverWindow;
    public RectTransform button_Exit;
    public RectTransform image_CharacterRight;
    public RectTransform image_CharacterLeft;
    public RectTransform text_ScoreTitle;
    public RectTransform text_MaxComboTitle;

    public override IEnumerator Init ()
	{
		yield return 0;
		ToggleMask(true);		
		button_Ready.gameObject.SetActive(true);
		group_Game.gameObject.SetActive(true);
		group_Pause.gameObject.SetActive(false);
		group_GameOver.gameObject.SetActive(false);
        SetScore(0);
        SetCombo(0);
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
        text_Score.rectTransform.DOScale(1.2f, 0.15f).SetEase(Ease.InOutQuad).OnComplete(
            delegate () {
                text_Score.rectTransform.DOScale(1f, 0.15f).SetEase(Ease.InOutQuad);
            }
        );
    }

    public void SetCombo(int combo)
    {
        text_Combo.text = string.Format("Combo: {0}", combo);
        text_Combo.rectTransform.DOScale(1.2f, 0.15f).SetEase(Ease.InOutQuad).OnComplete(
            delegate () {
                text_Combo.rectTransform.DOScale(1f, 0.15f).SetEase(Ease.InOutQuad);
            }
        );
    }

	public void ShowGameOverWindow(int score, int maxCombo)
	{
        StartCoroutine(GameOverEffect(score, maxCombo));
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

    IEnumerator GameOverEffect(int score, int maxCombo)
    {
        image_CharacterRight.gameObject.SetActive(false);
        image_CharacterLeft.gameObject.SetActive(false);
        image_ScoreBoard.gameObject.SetActive(false);
        button_Exit.gameObject.SetActive(false);
        group_GameOver.gameObject.SetActive(true);
        group_GameOver.color = Color.clear;
        group_GameOver.DOColor(Color.black * 0.5f, 0.3f);
        image_GameOverWindow.anchoredPosition = new Vector2(0f, 1572f);
        yield return image_GameOverWindow.DOAnchorPos(new Vector2(0f, 72f), 0.5f).SetEase(Ease.OutBack).WaitForCompletion();

        image_CharacterRight.anchoredPosition = new Vector2(750f, -72f);
        image_CharacterLeft.anchoredPosition = new Vector2(-750f, -72f);
        image_CharacterRight.gameObject.SetActive(true);
        image_CharacterLeft.gameObject.SetActive(true);
        image_CharacterRight.DOAnchorPos(new Vector2(250f, -72f), 0.2f).SetEase(Ease.OutCubic);
        image_CharacterLeft.DOAnchorPos(new Vector2(-240f, -72f), 0.2f).SetEase(Ease.OutCubic);

        text_MaxCombo.text = "";
        text_GameOverScore.text = "";
        image_ScoreBoard.gameObject.SetActive(true);
        image_ScoreBoard.alpha = 0f;
        yield return image_ScoreBoard.DOFade(1f, 0.4f).WaitForCompletion();
        
        float changeTime = 0.35f;
        float scoreChangeAmount = (score / (changeTime / Time.deltaTime));
        float tmpScore = 0;
        while(changeTime > 0f)
        {
            text_GameOverScore.text = ((int)tmpScore).ToString();
            tmpScore += scoreChangeAmount;
            yield return new WaitForEndOfFrame();
            changeTime -= Time.deltaTime;
        }
        text_GameOverScore.text = score.ToString();
        yield return text_GameOverScore.rectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
        yield return text_GameOverScore.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
        
        changeTime = 0.35f;
        float comboChangeAmount = (maxCombo/(changeTime / Time.deltaTime));
        float tmpCombo = 0;
        while (changeTime > 0f)
        {
            text_MaxCombo.text = ((int)tmpCombo).ToString();
            tmpCombo += comboChangeAmount;
            yield return new WaitForEndOfFrame();
            changeTime -= Time.deltaTime;
        }
        text_MaxCombo.text = maxCombo.ToString();
        yield return text_MaxCombo.rectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
        yield return text_MaxCombo.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();

        button_Exit.gameObject.SetActive(true);
        button_Exit.localScale = new Vector3(1f, 0f, 1f);
        yield return button_Exit.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
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
