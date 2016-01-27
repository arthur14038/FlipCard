using UnityEngine;
using System.Collections;

public class InfiniteModeJudgement : GameModeJudgement
{
	protected override IEnumerator StartGame()
	{
		yield return null;
	}
}
