﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SinglePlayerView : AbstractView {
	public VoidNoneParameter onClickBack;
	public VoidCardArrayLevelGameMode onClickPlay;
	public RectTransform group_1P;
	public LevelUI[] levels;

	public override IEnumerator Init ()
	{
		escapeEvent = OnClickBack;
		group_1P.gameObject.SetActive(true);
		yield return 0;
	}

	public void SetProgress(int progress)
	{
		for(int i = 0 ; i < levels.Length ; ++i)
		{
			if(i > progress)
			{
				levels[i].SetLockState(false);
			}else
			{
				levels[i].SetLockState(true);
				levels[i].SetGameRecord(ModelManager.Instance.GetGameRecord((CardArrayLevel)i));
			}
		}
	}

	public void OnClickBack()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		if(onClickBack != null)
			onClickBack();
	}

	public void OnClickPlay(int level)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickPlay != null)
			onClickPlay((CardArrayLevel)level, GameMode.LimitTime);
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
		foreach(LevelUI level in levels)
			StartCoroutine(level.EnterEffect(0.5f));
		group_1P.gameObject.SetActive(true);
		group_1P.anchoredPosition = hideRight;
		yield return group_1P.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		showCoroutine = null;
	}
}