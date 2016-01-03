using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class LevelUI : MonoBehaviour {
	public Image image_LevelIcon;
	public Image image_Header;
    public Text text_LevelTitle;
	public Text text_FirstInformation;
	public Text text_FirstInformationTitle;
	public Text text_SecondInformation;
	public Text text_SecondInformationTitle;
	public Text text_LockInstruction;
	public Button button_Play;
	public RectTransform image_Mask;
	public RectTransform image_ShakeCircle;
	VoidTwoInt onClickPlay;
	SinglePlayerLevel theLevel;
	SpriteCardArrayLevel getSpriteIcon;

	public void Init(VoidTwoInt onClickPlay, SpriteCardArrayLevel getSpriteIcon, SinglePlayerLevel theLevel)
	{
		this.onClickPlay = onClickPlay;
		this.getSpriteIcon = getSpriteIcon;
		button_Play.onClick.AddListener(OnClickPlay);
		this.theLevel = theLevel;
		
		text_LockInstruction.text = theLevel.lockInstruction;
		text_LevelTitle.text = theLevel.gameLevel.ToString();
		text_FirstInformationTitle.text = theLevel.firstInformationTitle;
		text_SecondInformationTitle.text = theLevel.secondInformationTitle;

		Color headerColor;
		ColorUtility.TryParseHtmlString(theLevel.headerColor, out headerColor);
		image_Header.color = headerColor;
    }

	public IEnumerator EnterEffect(float enterDuration)
	{
		image_ShakeCircle.rotation = Quaternion.Euler(Vector3.zero);
		yield return image_ShakeCircle.DORotate(Vector3.forward*10f, enterDuration).SetEase(Ease.OutBounce).WaitForCompletion();
		yield return image_ShakeCircle.DORotate(Vector3.back*7.5f, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
		yield return image_ShakeCircle.DORotate(Vector3.forward*5.0f, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
		yield return image_ShakeCircle.DORotate(Vector3.back*2.5f, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
		yield return image_ShakeCircle.DORotate(Vector3.zero, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
	}

	public void SetLockState(bool isLock)
	{
		if(isLock)
		{
			text_FirstInformationTitle.gameObject.SetActive(false);
			text_SecondInformationTitle.gameObject.SetActive(false);
			button_Play.gameObject.SetActive(false);
			text_LevelTitle.gameObject.SetActive(false);
			image_Mask.gameObject.SetActive(true);

			image_LevelIcon.sprite = getSpriteIcon(CardArrayLevel.Lock);
        } else
		{
			text_FirstInformationTitle.gameObject.SetActive(true);
			text_SecondInformationTitle.gameObject.SetActive(true);
			button_Play.gameObject.SetActive(true);
			text_LevelTitle.gameObject.SetActive(true);
			image_Mask.gameObject.SetActive(false);

			image_LevelIcon.sprite = getSpriteIcon(theLevel.gameLevel);

			GameRecord record = ModelManager.Instance.GetGameRecord(theLevel.gameLevel, theLevel.gameMode);
			SetGameRecord(record);
		}
	}

	void SetGameRecord(GameRecord record)
	{
		text_FirstInformation.text = "-- --";
		text_SecondInformation.text = "-- --";
		if(record == null)
			return;

		if(record.highScore > 0)
			text_FirstInformation.text = record.highScore.ToString();
		if(record.secondInformation > 0)
			text_SecondInformation.text = record.secondInformation.ToString();
	}

	void OnClickPlay()
	{
		if(onClickPlay != null)
			onClickPlay((int)theLevel.gameLevel, (int)theLevel.gameMode);
	}
}
