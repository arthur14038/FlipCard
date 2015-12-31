using UnityEngine;
using System.Collections;

public class ClassicModeJudgement : GameModeJudgement
{	
	ClassicModeView classicModeView;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		gameMainView.completeOneRound = RoundComplete;
		gameMainView.cardMatch = CardMatch;
		classicModeView = (ClassicModeView)modeView;
		classicModeView.onClickPause = PauseGame;
		classicModeView.onClickGameOverExit = ExitGame;
		classicModeView.onGameStart = StartGame;
	}

	protected override IEnumerator StartGame()
	{
		yield return null;
		gameMainView.ToggleMask(false);
		if(currentState == GameState.Waiting)
			currentState = GameState.Playing;
		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}

	void RoundComplete()
	{
	}

	void CardMatch(bool match, params Card[] cards)
	{
	}
}
