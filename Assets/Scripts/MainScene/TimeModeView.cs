using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TimeModeView : AbstractView {
	public VoidNoneParameter onClickBack;
	public VoidCardArrayLevelGameMode onClickPlay;
	public RectTransform group_TimeMode;
	public Sprite[] levelIconSprites;
	public List<LevelUI> levelUIList;

	public override IEnumerator Init()
	{
		escapeEvent = OnClickBack;

		List<SinglePlayerLevel> levelList = GameSettingManager.GetSinglePlayerLevel(GameMode.LimitTime);

		//for(int i = 0 ; i < levelList.Count ; ++i)
		//{
		//	GameObject tmp = Instantiate(levelUIPrefab) as GameObject;
		//	tmp.transform.SetParent(levelParent);
		//	tmp.transform.localScale = Vector3.one;
		//	tmp.name = levelUIPrefab.name + i.ToString();
		//	LevelUI levelUI = tmp.GetComponent<LevelUI>();
		//	levelUI.Init(OnClickLevelPlay, GetLevelIcon, levelList[i]);
		//	levelUIList.Add(levelUI);
		//}
		//levelUIPrefab = null;
		for(int i = 0 ; i < levelList.Count ; ++i)
		{
			levelUIList[i].Init(OnClickLevelPlay, GetLevelIcon, levelList[i]);
		}
		yield return null;
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
			onClickPlay((CardArrayLevel)level, (GameMode)mode);
	}

	Sprite GetLevelIcon(CardArrayLevel level)
	{
		return levelIconSprites[(int)level];
	}

	protected override IEnumerator HideUIAnimation()
	{
		group_TimeMode.anchoredPosition = Vector2.zero;
		yield return group_TimeMode.DOAnchorPos(hideRight, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		base.HideUI(false);
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation()
	{
		foreach(LevelUI level in levelUIList)
			StartCoroutine(level.EnterEffect(0.5f));
		group_TimeMode.anchoredPosition = hideRight;
		yield return group_TimeMode.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		showCoroutine = null;
	}
}
