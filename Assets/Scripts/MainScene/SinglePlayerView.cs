using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SinglePlayerView : AbstractView {
	public VoidNoneParameter onClickBack;
	public VoidCardArrayLevelGameMode onClickPlay;
	public RectTransform group_1P;
	public GameObject levelUIPrefab;
	public Transform levelParent;
	List<LevelUI> levelUIList = new List<LevelUI>();
	List<SinglePlayerLevel> levelList;

	public override IEnumerator Init ()
	{
		escapeEvent = OnClickBack;
		group_1P.gameObject.SetActive(true);

		levelList = GameSettingManager.GetAllSinglePlayerLevel();
		for(int i = 0 ; i < levelList.Count ; ++i)
		{
			GameObject tmp = Instantiate(levelUIPrefab) as GameObject;
			tmp.transform.SetParent(levelParent);
			tmp.transform.localScale = Vector3.one;
            tmp.name = levelUIPrefab.name + i.ToString();
			LevelUI levelUI = tmp.GetComponent<LevelUI>();
			levelUI.Init(OnClickLevelPlay, levelList[i]);
            levelUIList.Add(levelUI);
        }
        yield return 0;
	}

	public void SetProgress(int progress)
	{
		for(int i = 0 ; i < levelUIList.Count ; ++i)
		{
			if(levelList[i].requireProgress > progress)
				levelUIList[i].SetLockState(false);
			else
				levelUIList[i].SetLockState(true);
		}
	}

	public void OnClickBack()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		if(onClickBack != null)
			onClickBack();
	}

	void OnClickLevelPlay(int level, int mode)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickPlay != null)
			onClickPlay((CardArrayLevel)level, (GameMode)mode);
	}
	
	protected override IEnumerator HideUIAnimation ()
	{
		group_1P.anchoredPosition = Vector2.zero;
		yield return group_1P.DOAnchorPos(hideRight, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		base.HideUI(false);
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation ()
	{
		foreach(LevelUI level in levelUIList)
			StartCoroutine(level.EnterEffect(0.5f));
		group_1P.gameObject.SetActive(true);
		group_1P.anchoredPosition = hideRight;
		yield return group_1P.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		showCoroutine = null;
	}
}
