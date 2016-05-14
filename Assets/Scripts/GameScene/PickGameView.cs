using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class PickGameView : AbstractView
{
	public IEnumeratorNoneParameter onClickReadyButton;
	public VoidNoneParameter onClickPause;
	[SerializeField]
	Button button_Pause;
	[SerializeField]
	RectTransform group_Heart;
	[SerializeField]
	RectTransform[] image_Hearts;
	[SerializeField]
	Image image_Cardback;
	[SerializeField]
	Image image_CardImage;
	[SerializeField]
	Text text_CurrentScore;
	[SerializeField]
	RectTransform group_Ready;
	[SerializeField]
	RectTransform button_Ready;
	Vector2 group_HeartPos = new Vector2(168f, 44f);

	public override IEnumerator Init()
	{
		group_Ready.gameObject.SetActive(true);
		button_Ready.gameObject.SetActive(true);
		foreach(RectTransform image_Heart in image_Hearts)
		{
			image_Heart.gameObject.SetActive(true);
        }
		yield return null;
		UpdateText();
		Localization.Event_ChangeLocaliztion += UpdateText;
	}

	void OnDestroy()
	{
		Localization.Event_ChangeLocaliztion -= UpdateText;
	}

	void UpdateText()
	{

	}

	public void SetCurrentScore(int score)
	{
		text_CurrentScore.text = score.ToString();
	}

	public void SetHeart(int count, bool needShake = false)
	{
		for(int i = 0 ; i < image_Hearts.Length ; ++i)
		{
			if(i > count)
			{
				image_Hearts[i].gameObject.SetActive(true);
			}else
			{
				image_Hearts[i].gameObject.SetActive(false);
			}
		}
		if(needShake)
			StartCoroutine(HeartShakeEffect());
	}

	public void OnClickReadyButton()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickReadyButton != null)
			onClickReadyButton();
	}

	public void OnClickPauseButton()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickPause != null)
			onClickPause();
	}

	public void SetPauseButtonState(bool value)
	{
		button_Pause.interactable = value;
	}

	IEnumerator HeartShakeEffect()
	{
		yield return group_Heart.DOAnchorPosY(48f, 0.1f).WaitForCompletion();
		yield return group_Heart.DOAnchorPosY(40f, 0.1f).WaitForCompletion();
		yield return group_Heart.DOAnchorPosY(48f, 0.1f).WaitForCompletion();
		yield return group_Heart.DOAnchorPosY(44f, 0.1f).WaitForCompletion();
	}

	protected override IEnumerator HideUIAnimation()
	{
		yield return null;
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation()
	{
		yield return null;
		showCoroutine = null;
	}

	//void OnGUI()
	//{
	//	if(GUI.Button(new Rect(10, 10, 150, 50), "HeartShakeEffect"))
	//	{
	//		StartCoroutine(HeartShakeEffect());
	//	}
	//}
}
