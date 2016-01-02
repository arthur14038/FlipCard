using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class LevelUI : MonoBehaviour {
	public Image image_Grade;
	public Image image_LevelIcon;
	public Image image_Header;
    public Text text_LevelTitle;
	public Text text_ShowWordsTitle;
	public Text text_ShowWordsContent;
	public Text text_LockInstruction;
	public Button button_Play;
	public RectTransform group_Unlock;
	public RectTransform group_Locked;
	public RectTransform levelIcon;
	public RectTransform lockedLevelIcon;
	public Sprite[] gradeSprites;
	public Sprite[] levelIconSprites;
	bool levelUnlock;
	VoidTwoInt onClickPlay;
	SinglePlayerLevel theLevel;

	public void Init(VoidTwoInt onClickPlay, SinglePlayerLevel theLevel)
	{
		this.onClickPlay = onClickPlay;
        button_Play.onClick.AddListener(OnClickPlay);
		this.theLevel = theLevel;

		if(string.IsNullOrEmpty(theLevel.showContent))
			text_ShowWordsTitle.gameObject.SetActive(false);
		else
		{
			text_ShowWordsTitle.gameObject.SetActive(true);
			text_ShowWordsTitle.text = theLevel.showContent;
        }
		text_LevelTitle.text = theLevel.levelTitle;
		text_LockInstruction.text = theLevel.lockInstruction;
		image_LevelIcon.sprite = levelIconSprites[(int)theLevel.gameLevel];
		Color headerColor;
		ColorUtility.TryParseHtmlString(theLevel.headerColor, out headerColor);
		image_Header.color = headerColor;
    }

	public IEnumerator EnterEffect(float enterDuration)
	{
		RectTransform waveItem = null;
		if(levelUnlock)
			waveItem = levelIcon;
		else
			waveItem = lockedLevelIcon;

		waveItem.rotation = Quaternion.Euler(Vector3.zero);
		yield return waveItem.DORotate(Vector3.forward*10f, enterDuration).SetEase(Ease.OutBounce).WaitForCompletion();
		yield return waveItem.DORotate(Vector3.back*7.5f, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
		yield return waveItem.DORotate(Vector3.forward*5.0f, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
		yield return waveItem.DORotate(Vector3.back*2.5f, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
		yield return waveItem.DORotate(Vector3.zero, 0.125f).SetEase(Ease.OutQuad).WaitForCompletion();
	}

	public void SetLockState(bool unlock)
	{
		levelUnlock = unlock;
		if(unlock)
		{
			group_Unlock.gameObject.SetActive(true);
			group_Locked.gameObject.SetActive(false);

			GameRecord thisRecord = ModelManager.Instance.GetGameRecord(theLevel.gameLevel, theLevel.gameMode);
			SetGameRecord(thisRecord);
		}else
		{
			group_Unlock.gameObject.SetActive(false);
			group_Locked.gameObject.SetActive(true);
		}
	}

	void SetGameRecord(GameRecord record)
	{
		if(record == null)
		{
			text_ShowWordsContent.text = "-- --";
			image_Grade.gameObject.SetActive(false);
			return;
		}
		
		if(record.grade > 0)
		{
			if(record.grade > gradeSprites.Length)
				image_Grade.sprite = gradeSprites[gradeSprites.Length - 1];
			else
				image_Grade.sprite = gradeSprites[record.grade - 1];
		}
		else
			image_Grade.gameObject.SetActive(false);

		if(record.highScore > 0)
			text_ShowWordsContent.text = record.highScore.ToString();
		else
			text_ShowWordsContent.text = "-- --";
    }

	void OnClickPlay()
	{
		if(onClickPlay != null)
			onClickPlay((int)theLevel.gameLevel, (int)theLevel.gameMode);
	}
}
