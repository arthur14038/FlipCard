using UnityEngine;
using System.Collections;

public class PickModeJudgement : GameModeJudgement
{
	PickGameView pickGameView;
	PickGameMainView pickGameMainView;
    PickGameSetting pickGameSetting;
	PickCardArraySetting pickCardArraySetting;
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
		pickCardArraySetting = GameSettingManager.GetPickCardArraySetting(pickGameSetting.cardCount);
		pickGameMainView = (PickGameMainView)gameMainView;
		pickGameMainView.LoadCard();

		pickGameView = (PickGameView)modeView;
		pickGameView.onClickPause = PauseGame;
		pickGameView.onClickReadyButton = StartGame;
		pickGameView.SetHeart(heart);
		pickGameView.SetCurrentScore(currentScore);
		pickGameView.SetPauseButtonState(false);
	}

	protected override IEnumerator StartGame()
	{
		Debug.Log("Before DealCard");
		//pickGameMainView.SetUsingCard(pickGameSetting.cardCount);
		yield return pickGameMainView.StartCoroutine(pickGameMainView.DealCard(pickCardArraySetting.cardSize, pickCardArraySetting.realCardPosition, pickGameSetting.targetCardCount));
		Debug.Log("After DealCard");
		yield return new WaitForSeconds(0.2f);

		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		yield return new WaitForSeconds(pickGameSetting.showCardTime);
		gameMainView.FlipAllCard();
		gameMainView.ToggleMask(false);
		yield return new WaitForSeconds(0.35f);

		SetCurrentState(GameState.Playing);
		pickGameView.SetPauseButtonState(true);
		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}
}
