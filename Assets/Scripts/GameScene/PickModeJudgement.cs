using UnityEngine;
using System.Collections;

public class PickModeJudgement : GameModeJudgement
{
	PickGameView pickGameView;
	PickGameMainView pickGameMainView;
    PickGameSetting pickGameSetting;
	PickCardArraySetting pickCardArraySetting;
    float heart;
	int currentScore;
	int currentLevel;
	int flipCardTimes;
	int matchTimes;
	int failTimes;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		heart = 3f;
		currentScore = 0;
		currentLevel = 1;
		flipCardTimes = 0;
		matchTimes = 0;
		failTimes = 0;

		pickGameSetting = GameSettingManager.GetPickGameSetting(currentLevel);
		pickCardArraySetting = GameSettingManager.GetPickCardArraySetting(pickGameSetting.cardCount);
		pickGameMainView = (PickGameMainView)gameMainView;
		pickGameMainView.LoadCard(CanFlipCardNow);
		pickGameMainView.SetUsingCard(pickGameSetting.cardCount);
		pickGameMainView.SetHint(0, 0);
        pickGameMainView.cardMatch = CardMatch;

		pickGameView = (PickGameView)modeView;
		pickGameView.onClickPause = PauseGame;
		pickGameView.onClickReadyButton = StartGame;
		pickGameView.SetHeart(heart);
		pickGameView.SetCurrentScore(currentScore);
		pickGameView.SetCurrentLevel(currentLevel);
        pickGameView.SetPauseButtonState(false);
	}
	
	protected override IEnumerator StartGame()
	{
		yield return pickGameMainView.StartCoroutine(pickGameMainView.DealCard(pickCardArraySetting.cardSize, pickCardArraySetting.realCardPosition, pickGameSetting.targetCardCount));
		yield return new WaitForSeconds(0.2f);
		pickGameMainView.SetHint(pickGameSetting.targetCardCount, pickGameSetting.targetCardCount);

		pickGameMainView.FlipAllCard(true);
		yield return new WaitForSeconds(0.35f);
		yield return new WaitForSeconds(pickGameSetting.showCardTime);
		pickGameMainView.FlipAllCard(false);
		pickGameMainView.ToggleMask(false);
		yield return new WaitForSeconds(0.35f);

		SetCurrentState(GameState.Playing);
		pickGameView.SetPauseButtonState(true);
		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}

	bool CanFlipCardNow(CardBase card)
	{
		if(flipCardTimes >= pickGameSetting.targetCardCount)
		{
			return false;
		}else
		{
			++flipCardTimes;
			pickGameMainView.SetHint(pickGameSetting.targetCardCount, pickGameSetting.targetCardCount - flipCardTimes);
			return true;
		}
	}

	void CardMatch(bool match, params CardBase[] cards)
	{
		if(match)
		{
			++matchTimes;
			++currentScore;
			pickGameView.SetCurrentScore(currentScore);
		}else
		{
			++failTimes;
		}

		if(matchTimes + failTimes == pickGameSetting.targetCardCount)
			pickGameMainView.StartCoroutine(CompleteOneRound());
	}

	IEnumerator CompleteOneRound()
	{
		yield return new WaitForSeconds(0.2f);
		pickGameMainView.ToggleMask(true);
		if(pickGameMainView.GetTableCardCount() != 0)
		{
			yield return new WaitForSeconds(0.2f);
			pickGameMainView.FlipAllCard(false);
			float additionWaitTime = Mathf.Clamp(0.2f * pickGameMainView.GetTableCardCount(), 0.2f, 1f);
			yield return new WaitForSeconds(0.35f + additionWaitTime);
		}

		pickGameMainView.SetHint(0, 0);
		pickGameMainView.ClearAllCard();

		SetCurrentState(GameState.Waiting);

		float correctRate = (float)matchTimes / flipCardTimes;

		if(correctRate < 0.33f)
		{
			heart -= 1f;
		}else if(correctRate < 0.66f)
		{
			heart -= 0.5f;
        } else if(correctRate < 0.99f)
		{

		}else
		{
			heart += 1f;
		}

		if(heart > 5f)
			heart = 5f;
		if(heart < 0f)
			heart = 0f;
		
		pickGameView.SetHeart(heart);

		if(currentLevel >= 20)
		{
			//達到最後一關結束
			gameSettingView.ShowSinglePlayerGameOver(currentScore, currentLevel.ToString(), false, null);
		} else if(heart > 0)
		{
			//愛心足夠往下一關
			++currentLevel;
			pickGameSetting = GameSettingManager.GetPickGameSetting(currentLevel);
			pickCardArraySetting = GameSettingManager.GetPickCardArraySetting(pickGameSetting.cardCount);
			pickGameMainView.SetUsingCard(pickGameSetting.cardCount);
			pickGameView.SetCurrentLevel(currentLevel);
			flipCardTimes = 0;
			matchTimes = 0;
			failTimes = 0;
            pickGameMainView.StartCoroutine(NewRoundRoutine());
		} else
		{
			//愛心花完
			gameSettingView.ShowSinglePlayerGameOver(currentScore, currentLevel.ToString(), false, null);
		}
	}

	IEnumerator NewRoundRoutine()
	{
		yield return new WaitForSeconds(0.5f);
		yield return pickGameMainView.StartCoroutine(pickGameMainView.DealCard(pickCardArraySetting.cardSize, pickCardArraySetting.realCardPosition, pickGameSetting.targetCardCount));
		yield return new WaitForSeconds(0.2f);
		pickGameMainView.SetHint(pickGameSetting.targetCardCount, pickGameSetting.targetCardCount);

		pickGameMainView.FlipAllCard(true);
		yield return new WaitForSeconds(0.35f);
		yield return new WaitForSeconds(pickGameSetting.showCardTime);
		pickGameMainView.FlipAllCard(false);
		pickGameMainView.ToggleMask(false);
		yield return new WaitForSeconds(0.35f);

		SetCurrentState(GameState.Playing);
	}
}
