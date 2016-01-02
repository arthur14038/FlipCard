using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class TimeModeView : AbstractView
{
	public IEnumeratorNoneParameter onCountDownFinished;
	public VoidNoneParameter onClickPause;
    public Slider timeBar;
	public Text text_CurrentScore;
	public Text text_CurrentRound;
	public Image group_Counting;
	public RectTransform image_Counting3;
	public RectTransform image_Counting2;
	public RectTransform image_Counting1;
	public RectTransform image_CountingGo;
	public RectTransform group_FeverTime;
	public GameObject timeIsRunning;
	public GameObject feverTimeEffect;

	public override IEnumerator Init()
	{
		yield return null;
		SetScore(0);
		SetRound(0);
		group_Counting.gameObject.SetActive(true);
		image_Counting3.gameObject.SetActive(false);
		image_Counting2.gameObject.SetActive(false);
		image_Counting1.gameObject.SetActive(false);
		image_CountingGo.gameObject.SetActive(false);
		group_FeverTime.gameObject.SetActive(false);
		ToggleFeverTimeEffect(false);
    }
	
	public void OnClickPause()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickPause != null)
			onClickPause();
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
	
	public void ToggleTimeIsRunning(bool value)
	{
		if(timeIsRunning.activeSelf != value)
			timeIsRunning.SetActive(value);
	}

	public void ShowFeverTime()
	{
		ToggleFeverTimeEffect(true);
		StartCoroutine(FeverTimeEffect());
	}
	
	public void ToggleFeverTimeEffect(bool value)
	{
		if(feverTimeEffect != null && feverTimeEffect.activeSelf != value)
			feverTimeEffect.SetActive(value);
	}

	IEnumerator FeverTimeEffect()
	{
		group_FeverTime.gameObject.SetActive(true);
        group_FeverTime.anchoredPosition = hideRight;
		yield return group_FeverTime.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
		yield return group_FeverTime.DOAnchorPos(hideLeft, 0.5f).SetDelay(0.3f).SetEase(Ease.InBack).WaitForCompletion();
		group_FeverTime.gameObject.SetActive(false);
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
