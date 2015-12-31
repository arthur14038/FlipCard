using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class ClassicModeView : AbstractView
{
	public IEnumeratorNoneParameter onGameStart;
	public VoidNoneParameter onClickPause;
	public VoidNoneParameter onClickGameOverExit;
	public Image group_Counting;
	public RectTransform image_Counting3;
	public RectTransform image_Counting2;
	public RectTransform image_Counting1;
	public RectTransform image_CountingGo;

	public override IEnumerator Init()
	{
		yield return null;
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
