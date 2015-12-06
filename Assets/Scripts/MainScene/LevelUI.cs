using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour {
	public CanvasGroup group_Unlock;
	public CanvasGroup group_Locked;
	public Text text_HighScore;
	public Text text_PlayTimes;

	public void SetLockState(bool unlock)
	{
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
