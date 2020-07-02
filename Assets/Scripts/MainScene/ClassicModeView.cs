using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ClassicModeView : AbstractView
{
	public VoidNoneParameter onClickBack;
	public VoidCardArrayLevelGameMode onClickPlay;
	public RectTransform group_ClassicMode;
	public Sprite[] levelIconSprites;
	public List<LevelUI> levelUIList;

	public override IEnumerator Init()
	{
		Debug.Log("ClassicModeView Init");
		escapeEvent = OnClickBack;

		List<SinglePlayerLevel> levelList = GameSettingManager.GetSinglePlayerLevel(GameMode.Classic);

		for(int i = 0 ; i < levelList.Count ; ++i)
		{
			levelUIList[i].Init(OnClickLevelPlay, GetLevelIcon, levelList[i]);
		}
		yield return null;
		Debug.Log("ClassicModeView Init complete");
	}

	public void OnClickBack()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		if(onClickBack != null)
			onClickBack();
	}

	public void SetProgress(int progress)
	{
		for(int i = 0 ; i < levelUIList.Count ; ++i)
		{
			if(i > progress)
				levelUIList[i].SetLockState(true);
			else
				levelUIList[i].SetLockState(false);
		}
	}

	void OnClickLevelPlay(int level, int mode)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickPlay != null)
			onClickPlay((LevelDifficulty)level, (GameMode)mode);
	}

	Sprite GetLevelIcon(LevelDifficulty level)
	{
		return levelIconSprites[(int)level];
    }

	protected override IEnumerator HideUIAnimation()
	{
		group_ClassicMode.anchoredPosition = Vector2.zero;
		yield return group_ClassicMode.DOAnchorPos(hideRight, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		base.HideUI(false);
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation()
	{
		foreach(LevelUI level in levelUIList)
			StartCoroutine(level.EnterEffect(0.5f));
		group_ClassicMode.anchoredPosition = hideRight;
		yield return group_ClassicMode.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		showCoroutine = null;
	}
}
