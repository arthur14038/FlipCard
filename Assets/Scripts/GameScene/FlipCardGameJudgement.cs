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
	int mismatchTimes;
	float gamePassTime;
	float frozenTime;
	float additionTime;
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
		mismatchTimes = 0;
		comboAward = false;
		flipCardGameView = (FlipCardGameView)modeView;
		flipCardGameView.onCountDownFinished = StartGame;
		flipCardGameView.onClickPause = PauseGame;
		flipCardGameView.onClickNextLevel = NextLevel;
		flipCardGameView.SetCurrentLevel(currentLevel, currentRound);
		flipCardGameView.SetCurrentScore(score);
		flipCardGameView.SetTimeBar(1f);

		currentGameSetting = GameSettingManager.GetFlipCardGameSetting(currentLevel);
		flipCardArraySetting = GameSettingManager.GetFlipCardArraySetting(currentLevel);
		gameMainView.SetUsingCard(currentGameSetting.cardCount, 0);
		flipCardGameView.SetPauseButtonState(false);

		yield return gameMainView.StartCoroutine(gameMainView.DealCard(flipCardArraySetting.cardSize, flipCardArraySetting.realCardPosition));
	}

	protected override IEnumerator StartGame()
	{
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentGameSetting.showCardTime);
		gameMainView.FlipAllCard();
		gameMainView.ToggleMask(false);
		yield return new WaitForSeconds(0.35f);

		SetCurrentState(GameState.Playing);
		flipCardGameView.SetPauseButtonState(true);
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

				if(cards[0].IsBombCard && gameMainView.GetTableCardCount() > 0)
					gameMainView.StartCoroutine(BombCardEffect());

				if(cards[0].IsFrozenCard)
				{
					gameMainView.ShowFrozen();
					frozenTime = currentGameSetting.showCardTime;
				}

				if(cards[0].IsFlashbangCard && gameMainView.GetTableCardCount() > 0)
					gameMainView.ShowFlashbang();
			} else
			{
				++mismatchTimes;
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
		SetCurrentState(GameState.Waiting);
		gameMainView.ToggleMask(true);

		++currentRound;

		if(mismatchTimes == 0)
		{
			score += 8;
			flipCardGameView.SetCurrentScore(score);
			flipCardGameView.StartCoroutine(flipCardGameView.PerfectEffect());
		} else
		{
			mismatchTimes = 0;
		}
		
		if(currentRound > currentGameSetting.round)
		{
			if(currentLevel < 7)
			{
				additionTime = currentGameSetting.levelTime - gamePassTime;
				gamePassTime = 0;				
				++currentLevel;
				currentRound = 1;
				
				flipCardGameView.ToggleFeverTimeEffect(false);

				flipCardArraySetting = GameSettingManager.GetFlipCardArraySetting(currentLevel);
				currentGameSetting = GameSettingManager.GetFlipCardGameSetting(currentLevel);

				int questionCardCount = 0;
				int normalCardCount = currentGameSetting.cardCount;

				if(currentGameSetting.questionCardAppearRound.Length > 0)
				{
					if(currentRound == currentGameSetting.questionCardAppearRound[0])
					{
						questionCardCount = currentGameSetting.questionCardCount;
						normalCardCount -= questionCardCount;
					}
				}

				gameMainView.SetUsingCard(normalCardCount, questionCardCount);

				if(comboAward)
					gameMainView.ToggleCardGlow(true);
				else
					gameMainView.ToggleCardGlow(false);

				flipCardGameView.StartCoroutine(flipCardGameView.ShowNextLevelUI(string.Format("NEXT LEVEL: {0}-{1}", currentLevel, currentRound)));
			}
		} else
		{
			int goldCardCount = 0;
			if(currentRound == currentGameSetting.round)
			{
				goldCardCount = -1;
				flipCardGameView.StartCoroutine(flipCardGameView.FinalRoundEffect());
				flipCardGameView.ToggleFeverTimeEffect(true);
			}

			int questionCardCount = 0;
			int normalCardCount = currentGameSetting.cardCount;

			if(currentGameSetting.questionCardAppearRound.Length > 0)
			{
				if(currentGameSetting.questionCardAppearRound[0] == 0 || currentRound == currentGameSetting.questionCardAppearRound[0])
				{
					questionCardCount = currentGameSetting.questionCardCount;
					normalCardCount -= questionCardCount;
				}
			}

			gameMainView.SetUsingCard(normalCardCount, questionCardCount);

			flipCardGameView.SetCurrentLevel(currentLevel, currentRound);
			gameMainView.SetGoldCard(goldCardCount);
			gameMainView.StartCoroutine(NextRoundRoutine());
		}
	}

	void NextLevel()
	{
		flipCardGameView.AddTimeEffect(1f);
		gamePassTime = 0;

		int goldCardCount = 0;
		if(currentRound == currentGameSetting.round)
		{
			goldCardCount = -1;
			flipCardGameView.StartCoroutine(flipCardGameView.FinalRoundEffect());
			flipCardGameView.ToggleFeverTimeEffect(true);
		}
		flipCardGameView.SetCurrentLevel(currentLevel, currentRound);
		gameMainView.SetGoldCard(goldCardCount);
		gameMainView.StartCoroutine(NextRoundRoutine());
	}

	IEnumerator NextRoundRoutine()
	{
		yield return new WaitForSeconds(0.3f);  //等桌面清空
		int[] specialCardCount = new int[3];
		specialCardCount[0] = 0;
		specialCardCount[1] = 0;
		specialCardCount[2] = 0;
		if(currentGameSetting.specialCardAppearRound.Length > 0)
		{
			int specialCardType = 0;
			if(currentGameSetting.specialCardAppearRound[0] == 0)
			{
				specialCardType = currentGameSetting.specialCardType;
			} else
			{
				for(int i = 0 ; i < currentGameSetting.specialCardAppearRound.Length ; ++i)
				{
					if(currentGameSetting.specialCardAppearRound[i] == currentRound)
					{
						specialCardType = currentGameSetting.specialCardType;
						break;
					}
				}
			}
			switch(specialCardType)
			{
				case 1:
					specialCardCount[0] = 2;
					break;
				case 2:
					specialCardCount[1] = 2;
					break;
				case 3:
					specialCardCount[2] = 2;
					break;
				case 4:
					int randomIndex = Random.Range(0, 3);
					specialCardCount[randomIndex] = 2;
					break;
			}
		}
		yield return gameMainView.StartCoroutine(gameMainView.DealCard(flipCardArraySetting.cardSize, flipCardArraySetting.realCardPosition,
			specialCardCount[0], specialCardCount[1], specialCardCount[2]));
		yield return new WaitForSeconds(0.3f);  //等發卡動作結束

		while(currentState == GameState.Pausing)
			yield return null;
		gameMainView.FlipAllCard();
		float showCardTime = 0.35f + currentGameSetting.showCardTime;
		while(showCardTime > 0f)
		{
			if(currentState == GameState.Pausing)
				yield return null;
			else
			{
				showCardTime -= Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}
		gameMainView.FlipAllCard();
		while(currentState == GameState.Pausing)
			yield return null;
		gameMainView.ToggleMask(false);
		yield return new WaitForSeconds(0.35f);
		while(currentState == GameState.Pausing)
			yield return null;
		SetCurrentState(GameState.Playing);
	}

	IEnumerator BombCardEffect()
	{
		SetCurrentState(GameState.Waiting);

		gameMainView.ShowExplosion();
		gameMainView.ToggleMask(true);
		yield return new WaitForSeconds(0.3f);

		while(currentState == GameState.Pausing)
			yield return null;
		gameMainView.FlipAllCard();
		float showCardTime = 0.35f + currentGameSetting.showCardTime;
		while(showCardTime > 0f)
		{
			if(currentState == GameState.Pausing)
				yield return null;
			else
			{
				showCardTime -= Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}
		gameMainView.FlipAllCard();
		while(currentState == GameState.Pausing)
			yield return null;
		gameMainView.ToggleMask(false);
		yield return new WaitForSeconds(0.35f);

		SetCurrentState(GameState.Playing);
	}
}
