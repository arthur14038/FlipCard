using UnityEngine;
using System.Collections;

public class FlipCardGameJudgement : GameModeJudgement
{
	FlipCardGameSetting currentGameSetting;
	FlipCardArraySetting flipCardArraySetting;
	FlipCardGameView flipCardGameView;
    int currentLevel;
	int currentRound;
	int score;
	float gamePassTime;
	float frozenTime;
	bool comboAward;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		gameMainView.LoadCard(30, 1);
        gameMainView.completeOneRound = RoundComplete;
		gameMainView.cardMatch = CardMatch;
		currentLevel = 1;
		currentRound = 1;
		score = 0;
		gamePassTime = 0;
		comboAward = false;
		flipCardGameView = (FlipCardGameView)modeView;
		flipCardGameView.onCountDownFinished = StartGame;
		flipCardGameView.onClickPause = PauseGame;
		flipCardGameView.SetCurrentLevel(currentLevel, currentRound);
		flipCardGameView.SetCurrentScore(score);
		flipCardGameView.SetTimeBar(1f);

		currentGameSetting = GameSettingManager.GetFlipCardGameSetting(currentLevel);
		flipCardArraySetting = GameSettingManager.GetFlipCardArraySetting(currentLevel);
		gameMainView.SetUsingCard(currentGameSetting.cardCount, 0);

		yield return gameMainView.StartCoroutine(gameMainView.DealCard(flipCardArraySetting.cardSize, flipCardArraySetting.realCardPosition));
	}

	protected override IEnumerator StartGame()
	{
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentGameSetting.showCardTime);
		gameMainView.FlipAllCard();
		gameMainView.ToggleMask(false);
		yield return new WaitForSeconds(0.35f);
		
		if(currentState == GameState.Waiting)
			currentState = GameState.Playing;
		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}

	protected override void GameOver(params int[] values)
	{
		base.GameOver(values);
	}

	public override void JudgementUpdate()
	{
		if(flipCardGameView != null)
		{
			if(currentState == GameState.Playing)
			{
				flipCardGameView.SetTimeBar(1f - gamePassTime / currentGameSetting.levelTime);
				if(frozenTime > 0)
				{
					frozenTime -= Time.deltaTime;
				} else
				{
					gamePassTime += Time.deltaTime;

					if(gamePassTime >= currentGameSetting.levelTime)
					{
						GameOver(score, currentLevel, currentRound);
						flipCardGameView.SetTimeBar(0f);
					}

					if(currentGameSetting.levelTime - gamePassTime < 5f)
						flipCardGameView.ToggleTimeIsRunning(true);
					else
						flipCardGameView.ToggleTimeIsRunning(false);
				}
			} else
			{
				flipCardGameView.ToggleTimeIsRunning(false);
			}
		}		
	}

	void CardMatch(bool match, params CardBase[] cards)
	{
		if(currentState != GameState.GameOver)
		{
			if(match)
			{
				int scoreChangeAmount = 1 * cards.Length;

				foreach(CardBase matchCard in cards)
				{
					Vector2 pos = matchCard.GetAnchorPosition();
					pos.x += flipCardArraySetting.cardSize / 2 - 20f;
					gameMainView.ShowScoreText(pos, comboAward, matchCard.IsGoldCard());
				}

				if(comboAward)
				{
					scoreChangeAmount += 2 * cards.Length;
				} else
				{
					comboAward = true;
					gameMainView.ToggleCardGlow(true);
				}
				
				if(cards[0].IsGoldCard())
					scoreChangeAmount += 4;

				if(cards[1].IsGoldCard())
					scoreChangeAmount += 4;

				if(scoreChangeAmount != 0)
				{
					score += scoreChangeAmount;
					flipCardGameView.SetCurrentScore(score);
				}

			} else
			{
				if(comboAward)
				{
					comboAward = false;
					gameMainView.ToggleCardGlow(false);
				}
			}
		}
	}

	void RoundComplete()
	{
		if(currentState == GameState.Playing)
			currentState = GameState.Waiting;
		
		++currentRound;

		if(currentRound < currentGameSetting.round)
		{
			gameMainView.SetGoldCard(0);
			gameMainView.StartCoroutine(NextRoundRoutine(false));
		} else if(currentRound == currentGameSetting.round)
		{
			gameMainView.SetGoldCard(-1);
			flipCardGameView.ShowFinalRound();
			gameMainView.StartCoroutine(NextRoundRoutine(true));
		} else
		{
			if(currentGameSetting.round > 0)
			{
				gamePassTime = 0;
				flipCardGameView.AddTimeEffect(1f);
                gameMainView.SetGoldCard(0);

				++currentLevel;
				if(currentLevel > 7)
					currentLevel = 7;

				currentRound = 1;

				flipCardGameView.ToggleFeverTimeEffect(false);

				flipCardArraySetting = GameSettingManager.GetFlipCardArraySetting(currentLevel);
				currentGameSetting = GameSettingManager.GetFlipCardGameSetting(currentLevel);

				gameMainView.SetUsingCard(currentGameSetting.cardCount, 0);
				
				if(comboAward)
					gameMainView.ToggleCardGlow(true);
				else
					gameMainView.ToggleCardGlow(false);

			}
			gameMainView.StartCoroutine(NextRoundRoutine(false));
		}
    }

	IEnumerator NextRoundRoutine(bool finalRound)
	{
		flipCardGameView.SetCurrentLevel(currentLevel, currentRound);
		gameMainView.ToggleMask(true);
		yield return new WaitForSeconds(0.3f);
		yield return gameMainView.StartCoroutine(gameMainView.DealCard(flipCardArraySetting.cardSize, flipCardArraySetting.realCardPosition));
		yield return new WaitForSeconds(0.3f);
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentGameSetting.showCardTime);
		gameMainView.FlipAllCard();
		gameMainView.ToggleMask(false);
		yield return new WaitForSeconds(0.35f);
		if(currentState == GameState.Waiting)
			currentState = GameState.Playing;
	}
}
