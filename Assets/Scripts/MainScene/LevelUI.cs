using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class LevelUI : MonoBehaviour {
	public CanvasGroup group_Unlock;
	public CanvasGroup group_Locked;
	public Text text_HighScore;
	public Text text_PlayTimes;
	public RectTransform levelIcon;
	public RectTransform lockedLevelIcon;
	bool levelUnlock;

	public IEnumerator EnterEffect(float enterDuration)
	{
		RectTransform waveItem = null;
		if(levelUnlock)
			waveItem = levelIcon;
		else
			waveItem = lockedLevelIcon;

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
		}else
		{
			group_Unlock.gameObject.SetActive(false);
			group_Locked.gameObject.SetActive(true);
		}
	}

	public void SetGameRecord(GameRecord record)
	{
		if(record.highScore > 0)
			text_HighScore.text = record.highScore.ToString();
		else
			text_HighScore.text = "- -";
		
		if(record.playTimes > 0)
			text_PlayTimes.text = record.playTimes.ToString();
		else
			text_PlayTimes.text = "- -";
	}
}
