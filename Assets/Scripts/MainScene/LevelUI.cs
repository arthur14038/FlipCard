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
	public Text text_LockInstruction;
	public Button button_Play;
	public RectTransform image_Mask;
	public RectTransform image_ShakeCircle;
	public Image[] image_Stars;
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
			button_Play.gameObject.SetActive(false);
			text_LevelTitle.gameObject.SetActive(false);
			image_Mask.gameObject.SetActive(true);

			image_LevelIcon.sprite = getSpriteIcon(LevelDifficulty.Lock);
        } else
		{
			text_FirstInformationTitle.gameObject.SetActive(true);
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

		for(int i = 0 ; i < image_Stars.Length ; ++i)
			image_Stars[i].color = Color.gray;

		if(record == null)
			return;

		switch(record.mode)
		{
			case GameMode.LimitTime:
				if(record.highScore > 0)
					text_FirstInformation.text = record.highScore.ToString();
				break;
			case GameMode.Classic:
				if(record.highScore > 0)
					text_FirstInformation.text = string.Format("{0:00}:{1:00}", record.highScore / 60, record.highScore % 60); ;
				break;
		}

		for(int i = 0 ; i < image_Stars.Length ; ++i)
		{
			if(i < record.grade)
				image_Stars[i].color = Color.white;
		}
	}

	void OnClickPlay()
	{
		if(onClickPlay != null)
			onClickPlay((int)theLevel.gameLevel, (int)theLevel.gameMode);
	}
}
