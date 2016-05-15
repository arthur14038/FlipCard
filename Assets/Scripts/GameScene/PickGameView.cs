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
	Image[] image_Hearts;
	[SerializeField]
	Text text_CurrentScore;
	[SerializeField]
	CanvasGroup group_Ready;
	[SerializeField]
	Button button_Ready;
	[SerializeField]
	Text text_CurrentLevel;
	Vector2 group_HeartPos = new Vector2(168f, 44f);
	CardBase pickCard;

	public override IEnumerator Init()
	{
		group_Ready.gameObject.SetActive(true);
		group_Ready.alpha = 1f;
        button_Ready.gameObject.SetActive(true);
		button_Ready.interactable = true;
        foreach(Image image_Heart in image_Hearts)
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
	
	public void SetCurrentLevel(int level)
	{
		text_CurrentLevel.text = level.ToString();
    }

	public void SetCurrentScore(int score)
	{
		text_CurrentScore.text = score.ToString();
	}

	public void SetHeart(int count, bool needShake = false)
	{
		for(int i = 0 ; i < image_Hearts.Length ; ++i)
		{
			if(i < count)
			{
				image_Hearts[i].color = Color.white;
			}else
			{
				image_Hearts[i].color = Color.gray;
			}
		}
		if(needShake)
			StartCoroutine(HeartShakeEffect());
	}

	public void OnClickReadyButton()
	{
		if(AudioManager.Instance)
			AudioManager.Instance.PlayOneShot("Button_Click");
		StartCoroutine(PlayerReadyEffect());
	}

	public void OnClickPauseButton()
	{
		if(AudioManager.Instance)
			AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickPause != null)
			onClickPause();
	}

	public void SetPauseButtonState(bool value)
	{
		button_Pause.interactable = value;
	}

	IEnumerator PlayerReadyEffect()
	{
		button_Ready.interactable = false;		
		yield return group_Ready.DOFade(0f, 0.25f).SetDelay(0.3f).WaitForCompletion();
		group_Ready.gameObject.SetActive(false);

		if(onClickReadyButton != null)
			StartCoroutine(onClickReadyButton());
	}

	IEnumerator HeartShakeEffect()
	{
		yield return group_Heart.DOAnchorPosY(18f, 0.1f).WaitForCompletion();
		yield return group_Heart.DOAnchorPosY(10f, 0.1f).WaitForCompletion();
		yield return group_Heart.DOAnchorPosY(18f, 0.1f).WaitForCompletion();
		yield return group_Heart.DOAnchorPosY(14f, 0.1f).WaitForCompletion();
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
}
