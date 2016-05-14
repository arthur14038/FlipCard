using UnityEngine;
using System.Collections;

public class PickModeJudgement : GameModeJudgement
{
	PickGameView pickGameView;
	PickGameSetting pickGameSetting;
	int heart;
	int currentScore;
	int currentLevel;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		heart = 3;
		currentScore = 0;
		currentLevel = 1;

		pickGameSetting = GameSettingManager.GetPickGameSetting(currentLevel);

		pickGameView = (PickGameView)modeView;
		pickGameView.onClickPause = PauseGame;
		pickGameView.onClickReadyButton = StartGame;
		pickGameView.SetHeart(heart);
		pickGameView.SetCurrentScore(currentScore);
		pickGameView.SetPauseButtonState(false);
	}

	protected override IEnumerator StartGame()
	{
		yield return null;
		pickGameView.SetPauseButtonState(true);
	}
}
