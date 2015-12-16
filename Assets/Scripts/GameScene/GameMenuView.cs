using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class GameMenuView : AbstractView {
	public Slider timeBar;
	public Text text_Score;
    public Text text_Round;
    public Text text_GameOverScore;
	public Text text_MaxCombo;
    public Text text_NewRecord;
    public GameObject image_Mask;
	public VoidNoneParameter onClickPause;
	public VoidNoneParameter onClickResume;
	public VoidNoneParameter onClickExit;
	public VoidNoneParameter onClickReady;
	public CanvasGroup group_Game;
	public CanvasGroup group_Pause;
    public CanvasGroup image_ScoreBoard;
    public Image group_GameOver;
    public Image group_Counting;
    public RectTransform image_Counting3;
    public RectTransform image_Counting2;
    public RectTransform image_Counting1;
    public RectTransform image_CountingGo;
    public RectTransform image_PauseWindow;
	public RectTransform image_GameOverWindow;
    public RectTransform button_Exit;
    public RectTransform image_CharacterRight;
    public RectTransform image_CharacterLeft;
    public RectTransform text_ScoreTitle;
    public RectTransform text_MaxComboTitle;
    public RectTransform image_NewHighScoreHeader;
    public Transform scoreTextParent;
    public GameObject scoreTextPrefab;
    public GameObject newHighScoreEffect;
    Queue<ScoreText> scoreTextQueue = new Queue<ScoreText>();

    public override IEnumerator Init ()
	{
		yield return 0;
		escapeEvent = OnClickEscape;
		ToggleMask(true);
        group_Counting.gameObject.SetActive(true);
        group_Game.gameObject.SetActive(true);
		group_Pause.gameObject.SetActive(false);
		group_GameOver.gameObject.SetActive(false);
        image_Counting3.gameObject.SetActive(false);
        image_Counting2.gameObject.SetActive(false);
        image_Counting1.gameObject.SetActive(false);
        image_CountingGo.gameObject.SetActive(false);
        image_NewHighScoreHeader.gameObject.SetActive(false);
        newHighScoreEffect.SetActive(false);
        SetScore(0);
        SetRound(1);
        for (int i = 0; i < 8; ++i)
        {
            GameObject tmp = Instantiate(scoreTextPrefab) as GameObject;
            tmp.name = scoreTextPrefab.name;
            tmp.transform.SetParent(scoreTextParent);
            tmp.transform.localScale = Vector3.one;
            ScoreText st = tmp.GetComponent<ScoreText>();
            st.Init(SaveScoreText);
            SaveScoreText(st);
        }
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

    public void SetRound(int round)
    {
        text_Round.text = string.Format("Round: {0}", round);
        text_Round.rectTransform.DOScale(1.2f, 0.15f).SetEase(Ease.InOutQuad).OnComplete(
            delegate () {
                text_Round.rectTransform.DOScale(1f, 0.15f).SetEase(Ease.InOutQuad);
            }
        );
    }

	public void ShowGameOverWindow(int score, int maxCombo, bool newHighScore, bool newMaxCombo)
	{
        if(newHighScore || newMaxCombo)
        {
			newHighScoreEffect.SetActive(true);
			image_NewHighScoreHeader.gameObject.SetActive(true);
		}

        StartCoroutine(GameOverEffect(score, maxCombo, newHighScore, newMaxCombo));
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
    
	public void ToggleMask(bool value)
	{
		if(image_Mask.activeSelf != value)
			image_Mask.SetActive(value);
	}

    public void ShowScoreText(int score, Vector2 pos)
    {
        ScoreText st = scoreTextQueue.Dequeue();
        st.ShowScoreText(score, pos);
    }
    
    void SaveScoreText(ScoreText st)
    {
        scoreTextQueue.Enqueue(st);
    }

	void OnClickEscape()
	{
		switch(GameSceneController.currentState)
		{
		case GameSceneController.GameState.Playing:
			OnClickPause();
			break;
		case GameSceneController.GameState.Pausing:
			OnClickResume();
			break;
		}
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

    IEnumerator PauseEffect()
	{
		group_Pause.gameObject.SetActive(true);
		group_Pause.alpha = 0f;
		group_Pause.DOFade(1f, 0.3f);
		image_PauseWindow.localScale = Vector3.zero;
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

    IEnumerator GameOverEffect(int score, int maxCombo, bool newHighScore, bool newMaxCombo)
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

		if(newHighScore)
			StartCoroutine(TextCelebrateEffect(text_GameOverScore));
		if(newMaxCombo)
			StartCoroutine(TextCelebrateEffect(text_MaxCombo));

		yield return button_Exit.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
	}

	protected override IEnumerator HideUIAnimation ()
	{
		yield return 0;
	}

	protected override IEnumerator ShowUIAnimation ()
	{
        Vector3 flipDown = new Vector3(0f, 0.9f, 1f);
        float delayTime = 0.25f;

        image_Counting3.localScale = flipDown;
        image_Counting3.gameObject.SetActive(true);
        yield return image_Counting3.DOScale(Vector3.one, 0.15f).WaitForCompletion();
        yield return image_Counting3.DOScale(flipDown, 0.2f).SetDelay(delayTime).SetEase(Ease.OutQuad).WaitForCompletion();
        image_Counting3.gameObject.SetActive(false);

        image_Counting2.localScale = flipDown;
        image_Counting2.gameObject.SetActive(true);
        yield return image_Counting2.DOScale(Vector3.one, 0.15f).WaitForCompletion();
        yield return image_Counting2.DOScale(flipDown, 0.2f).SetDelay(delayTime).SetEase(Ease.OutQuad).WaitForCompletion();
        image_Counting2.gameObject.SetActive(false);

        image_Counting1.localScale = flipDown;
        image_Counting1.gameObject.SetActive(true);
        yield return image_Counting1.DOScale(Vector3.one, 0.15f).WaitForCompletion();
        yield return image_Counting1.DOScale(flipDown, 0.2f).SetDelay(delayTime).SetEase(Ease.OutQuad).WaitForCompletion();
        image_Counting1.gameObject.SetActive(false);

        image_CountingGo.localScale = flipDown;
        image_CountingGo.gameObject.SetActive(true);
        yield return image_CountingGo.DOScale(Vector3.one, 0.15f).WaitForCompletion();
        group_Counting.DOFade(0f, 0.3f).SetDelay(delayTime + 0.05f);
        yield return image_CountingGo.DOScale(flipDown, 0.2f).SetDelay(delayTime + 0.15f).SetEase(Ease.OutQuad).WaitForCompletion();
        image_CountingGo.gameObject.SetActive(false);
        group_Counting.gameObject.SetActive(false);

        if (onClickReady != null)
            onClickReady();
    }

	//void OnGUI()
	//{
	//	if(GUI.Button(new Rect(10, 10, 150, 50), "Test"))
	//	{
	//		StartCoroutine(TextCelebrateEffect(text_GameOverScore));
	//	}
	//}
}
