using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class ClassicModeGameView : AbstractView
{
	public IEnumeratorNoneParameter onGameStart;
	public VoidNoneParameter onClickPause;
	public Text text_MoveTimes;
	public Text text_CurrentScore;
	public Text text_GameTime;
	public Image group_Counting;
	public RectTransform image_Counting3;
	public RectTransform image_Counting2;
	public RectTransform image_Counting1;
	public RectTransform image_CountingGo;

	public override IEnumerator Init()
	{
		group_Counting.gameObject.SetActive(true);
		yield return null;
		SetMoveTimes(0);
		SetScore(0);
		SetGameTime(0);
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

	public void SetGameTime(float gameTime)
	{
		string minSec = string.Format("{0}:{1:00}", (int)gameTime / 60, (int)gameTime % 60);
		text_GameTime.text = minSec;
	}

	public void OnClickPause()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickPause != null)
			onClickPause();
	}

	public void SetMoveTimes(int value)
	{
		text_MoveTimes.text = string.Format("Move: {0}", value);
		text_MoveTimes.rectTransform.DOScale(1.2f, 0.15f).SetEase(Ease.InOutQuad).OnComplete(
			delegate () {
				text_MoveTimes.rectTransform.DOScale(1f, 0.15f).SetEase(Ease.InOutQuad);
			}
		);
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

		if(onGameStart != null)
			StartCoroutine(onGameStart());

		showCoroutine = null;
	}
}
