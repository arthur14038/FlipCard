using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

[System.Serializable]
public class FlipCardGameResult
{
	[SerializeField]
	RectTransform group_SinglePlayer;
	[SerializeField]
	RectTransform image_NewRecordHeader;
	[SerializeField]
	RectTransform image_CharacterRight;
	[SerializeField]
	RectTransform image_CharacterLeft;
	[SerializeField]
	RectTransform button_SinglePlayerGameOverExit;
	[SerializeField]
	RectTransform image_TimesUp;
	[SerializeField]
	RectTransform image_GameOverWindow;
	[SerializeField]
	RectTransform image_Header;
	[SerializeField]
	RectTransform group_GetMoni;
	[SerializeField]
	CanvasGroup image_SinglePlayerScoreBoard;
	[SerializeField]
	CanvasGroup image_TaskBG;
	[SerializeField]
	Text text_Score;
	[SerializeField]
	Text text_Level;
	[SerializeField]
	Text text_Title;
	[SerializeField]
	Text text_MoniCount;
	[SerializeField]
	GameObject newHighScoreEffect;
	[SerializeField]
	GameObject[] tasks;
	[SerializeField]
	Animator moniAnimator;
    bool recordBreak;
	int getMoniCount;
	bool[] thisTimeTask;

	public void Init()
	{
		group_SinglePlayer.gameObject.SetActive(false);
		image_NewRecordHeader.gameObject.SetActive(false);
		image_CharacterLeft.gameObject.SetActive(false);
		image_CharacterRight.gameObject.SetActive(false);
		image_SinglePlayerScoreBoard.gameObject.SetActive(false);
		button_SinglePlayerGameOverExit.gameObject.SetActive(false);
		text_Score.gameObject.SetActive(false);
		text_Level.gameObject.SetActive(false);
		image_GameOverWindow.gameObject.SetActive(false);
		newHighScoreEffect.SetActive(false);
		image_Header.gameObject.SetActive(false);
		image_TaskBG.gameObject.SetActive(false);
    }

	public void SetResult(int score, string level, bool recordBreak, bool[] thisTimeTask)
	{
		text_Score.text = score.ToString();
		text_Level.text = level;
		getMoniCount = score / 10;

		this.recordBreak = recordBreak;
		this.thisTimeTask = thisTimeTask;
        if(recordBreak)
		{
            text_Title.text = Localization.Get("GameResult/NewRecord");
		} else
		{
			string msg = Localization.Get("GameResult/TryAgain");
			if(score > 3000)
				msg = Localization.Get("GameResult/Incredible");
			else if(score > 2000)
				msg = Localization.Get("GameResult/Excellent");
			else if(score > 1000)
				msg = Localization.Get("GameResult/Awesome");
			else if(score > 500)
				msg = Localization.Get("GameResult/Great");
			text_Title.text = msg;
		}

		AudioManager.Instance.PlayOneShot("GameResult");
		group_SinglePlayer.gameObject.SetActive(true);
	}

	public IEnumerator ShowTimesUp()
	{
		AudioManager.Instance.PlayOneShot("Whistle");
		image_TimesUp.gameObject.SetActive(true);
		image_TimesUp.localScale = Vector3.zero;
		yield return image_TimesUp.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
		yield return new WaitForSeconds(0.4f);
		yield return image_TimesUp.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
		image_TimesUp.gameObject.SetActive(false);
	}

	public IEnumerator ShowText()
	{
		image_GameOverWindow.gameObject.SetActive(true);
		image_GameOverWindow.anchoredPosition = new Vector2(0f, (1080f * Screen.height) / Screen.width);
		image_GameOverWindow.sizeDelta = new Vector2(862f, 0f);
		yield return image_GameOverWindow.DOAnchorPos(new Vector2(0f, -121f), 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
		yield return new WaitForSeconds(0.2f);
		if(recordBreak)
		{
			newHighScoreEffect.SetActive(true);
			AudioManager.Instance.PlayOneShot("NewHighScore2");
			image_NewRecordHeader.gameObject.SetActive(true);
		}
		else
			image_Header.gameObject.SetActive(true);
		yield return image_GameOverWindow.DOSizeDelta(new Vector2(862f, 860f), 0.3f).WaitForCompletion();
    }

	public IEnumerator ShowScoreBoard()
	{
		image_CharacterRight.anchoredPosition = new Vector2(750f, -295f);
		image_CharacterLeft.anchoredPosition = new Vector2(-750f, -297f);
		image_CharacterRight.gameObject.SetActive(true);
		image_CharacterLeft.gameObject.SetActive(true);
		image_CharacterRight.DOAnchorPos(new Vector2(247f, -295f), 0.2f).SetEase(Ease.OutCubic);
		image_CharacterLeft.DOAnchorPos(new Vector2(-257f, -297f), 0.2f).SetEase(Ease.OutCubic);

		text_MoniCount.text = "0";
		image_SinglePlayerScoreBoard.alpha = 0f;
		image_SinglePlayerScoreBoard.gameObject.SetActive(true);
		yield return image_SinglePlayerScoreBoard.DOFade(1f, 0.4f).WaitForCompletion();

		text_Level.gameObject.SetActive(true);
		yield return text_Level.rectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return text_Level.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();

		text_Score.gameObject.SetActive(true);
		yield return text_Score.rectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		yield return text_Score.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCubic).WaitForCompletion();
		
		group_GetMoni.gameObject.SetActive(true);
		moniAnimator.SetTrigger("Play");
		yield return new WaitForSeconds(0.5f);

		float changeAmount = (float)getMoniCount / (0.5f / Time.deltaTime);
		float addMoniForShow = 0;
		float lastTimeSound = -0.03f;
		while(addMoniForShow < getMoniCount)
		{
			addMoniForShow += changeAmount;

			text_MoniCount.text = ((int)addMoniForShow).ToString();
            if(Time.time - lastTimeSound >= 0.03f)
			{
				AudioManager.Instance.PlayOneShot("GameResultScoreCount");
				lastTimeSound = Time.time;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	public IEnumerator ShowTaskAndButton()
	{
		int index = 0;
		for(int i = 0 ; i < thisTimeTask.Length ; ++i)
		{
			if(thisTimeTask[i])
			{
				++index;
				tasks[i].SetActive(true);
			} else
			{
				tasks[i].SetActive(false);
			}
		}

		if(index > 0)
		{
			yield return image_GameOverWindow.DOSizeDelta(new Vector2(862f, 860f + 70f*index), 0.3f).WaitForCompletion();
			image_TaskBG.alpha = 0f;
			image_TaskBG.gameObject.SetActive(true);
			yield return image_TaskBG.DOFade(1f, 0.4f).WaitForCompletion();
		}

		button_SinglePlayerGameOverExit.localScale = Vector3.one - Vector3.up;
		button_SinglePlayerGameOverExit.gameObject.SetActive(true);

		yield return button_SinglePlayerGameOverExit.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
	}
}
