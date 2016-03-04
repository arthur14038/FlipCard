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
	int finalLevel = 6;
	float gamePassTime;
	bool comboAward;
	bool feverTimeOn;
	bool lockNextRound;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		gameMainView.LoadCard(30, 2);
        gameMainView.completeOneRound = RoundComplete;
		gameMainView.cardMatch = CardMatch;
		currentLevel = 1;
		currentRound = 0;
		score = 0;
		gamePassTime = 0;
		mismatchTimes = 0;
		feverTimeOn = false;
		comboAward = false;
		lockNextRound = false;
		flipCardGameView = (FlipCardGameView)modeView;
		flipCardGameView.onCountDownFinished = StartGame;
		flipCardGameView.onClickPause = PauseGame;
		flipCardGameView.onClickNextLevel = NextLevel;
		flipCardGameView.SetCurrentLevel(currentLevel, currentRound + 1);
		flipCardGameView.SetCurrentScore(score);
		flipCardGameView.SetTimeBar(1f);

		currentGameSetting = GameSettingManager.GetFlipCardGameSetting(currentLevel);
		flipCardArraySetting = GameSettingManager.GetFlipCardArraySetting(currentGameSetting.cardCount);
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
		if(PlayerPrefsManager.UnlockMode == 0)
			PlayerPrefsManager.UnlockMode += 1;

		base.GameOver(values);
		bool recordBreak = false;
		int thisTimeLevel = values[1] * 1000 + values[2];
		int thisTimeScore = values[0];

		GameRecord lastRecord = ModelManager.Instance.GetFlipCardGameRecord();

		if(thisTimeScore > lastRecord.highScore)
		{
			recordBreak = true;
			lastRecord.highScore = thisTimeScore;
        }

		if(thisTimeLevel > lastRecord.highLevel)
		{
			recordBreak = true;
			lastRecord.highLevel = thisTimeLevel;
        }

		lastRecord.lastLevel[2] = lastRecord.lastLevel[1];
		lastRecord.lastLevel[1] = lastRecord.lastLevel[0];
		lastRecord.lastLevel[0] = thisTimeLevel;

		lastRecord.lastScore[2] = lastRecord.lastScore[1];
		lastRecord.lastScore[1] = lastRecord.lastScore[0];
		lastRecord.lastScore[0] = thisTimeScore;

		lastRecord.playTimes += 1;
		
		if(saveGameRecord != null)
			saveGameRecord(lastRecord);

		gameSettingView.ShowSinglePlayerGameOver(values[0].ToString(), string.Format("{0}-{1}", values[1], values[2]), recordBreak);
	}

	public override void JudgementUpdate()
	{
		if(flipCardGameView != null)
		{
			if(currentState == GameState.Playing)
			{
				flipCardGameView.SetTimeBar(1f - gamePassTime / currentGameSetting.levelTime);

				gamePassTime += Time.deltaTime;

				if(gamePassTime >= currentGameSetting.levelTime)
				{
					if(feverTimeOn && currentLevel < finalLevel)
						ThisLevelEnd();
					else
						GameOver(score, currentLevel, currentRound + 1);
					flipCardGameView.SetTimeBar(0f);
				}

				if(currentGameSetting.levelTime - gamePassTime < 5f)
					flipCardGameView.ToggleTimeIsRunning(true);
				else
					flipCardGameView.ToggleTimeIsRunning(false);
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
		if(currentState == GameState.GameOver)
			return;

		if(lockNextRound)
			return;

		SetCurrentState(GameState.Waiting);
		gameMainView.ToggleMask(true);

		if(mismatchTimes == 0)
		{
			score += 8;
			flipCardGameView.SetCurrentScore(score);
			flipCardGameView.StartCoroutine(flipCardGameView.PerfectEffect());
		} else
		{
			mismatchTimes = 0;
		}

		++currentRound;

		if(currentRound < currentGameSetting.round)
		{
			int goldCardCount = 0;
			if(currentRound + 1 == currentGameSetting.round)
			{
				goldCardCount = -1;
				string content = "FINAL LEVEL FOR " + currentLevel;
				//flipCardGameView.StartCoroutine(flipCardGameView.FinalRoundEffect(content));
			}
			int questionCardCount = 0;
			int normalCardCount = currentGameSetting.cardCount;

			if(currentGameSetting.questionCardAppearRound.Length > currentRound)
			{
				questionCardCount = currentGameSetting.questionCardAppearRound[currentRound];
				normalCardCount -= questionCardCount;
			}

			gameMainView.SetUsingCard(normalCardCount, questionCardCount);

			if(comboAward)
				gameMainView.ToggleCardGlow(true);
			else
				gameMainView.ToggleCardGlow(false);

			flipCardGameView.SetCurrentLevel(currentLevel, currentRound + 1);
			gameMainView.SetGoldCard(goldCardCount);
			gameMainView.StartCoroutine(NextRoundRoutine());
		} else
		{
			if(currentLevel == finalLevel)
			{
				int questionCardCount = currentGameSetting.questionCardAppearRound[currentGameSetting.questionCardAppearRound.Length-1];
				int normalCardCount = currentGameSetting.cardCount - questionCardCount;
				
				gameMainView.SetUsingCard(normalCardCount, questionCardCount);

				if(comboAward)
					gameMainView.ToggleCardGlow(true);
				else
					gameMainView.ToggleCardGlow(false);

				flipCardGameView.SetCurrentLevel(currentLevel, currentRound + 1);
				gameMainView.SetGoldCard(-1);
				gameMainView.StartCoroutine(NextRoundRoutine());
			} else
			{
				if(!feverTimeOn)
				{
					feverTimeOn = true;
					flipCardGameView.StartCoroutine(flipCardGameView.FeverTimeEffect());
					flipCardGameView.ToggleFeverTimeEffect(true);
				}

				gameMainView.SetUsingCard(currentGameSetting.cardCount, 0);

				if(comboAward)
					gameMainView.ToggleCardGlow(true);
				else
					gameMainView.ToggleCardGlow(false);

				flipCardGameView.SetCurrentLevel(currentLevel, currentRound + 1);
				gameMainView.SetGoldCard(-1);
				gameMainView.StartCoroutine(NextRoundRoutine());
			}
		}
	}

	void ThisLevelEnd()
	{
		if(currentLevel == finalLevel)
		{
			GameOver(score, currentLevel, currentRound + 1);
		} else
		{
			lockNextRound = true;
			SetCurrentState(GameState.Waiting);
			gameMainView.ToggleMask(true);

			if(feverTimeOn)
			{
				feverTimeOn = false;
				flipCardGameView.ToggleFeverTimeEffect(false);
			}
			gamePassTime = 0;
			++currentLevel;
			currentRound = 0;

			currentGameSetting = GameSettingManager.GetFlipCardGameSetting(currentLevel);
			flipCardArraySetting = GameSettingManager.GetFlipCardArraySetting(currentGameSetting.cardCount);

			int questionCardCount = 0;
			int normalCardCount = currentGameSetting.cardCount;

			if(currentGameSetting.questionCardAppearRound.Length > currentRound)
			{
				questionCardCount = currentGameSetting.questionCardAppearRound[currentRound];
				normalCardCount -= questionCardCount;
			}

			gameMainView.SetUsingCard(normalCardCount, questionCardCount);

			if(comboAward)
				gameMainView.ToggleCardGlow(true);
			else
				gameMainView.ToggleCardGlow(false);

			gameMainView.StartCoroutine(gameMainView.ClearTable());

			flipCardGameView.StartCoroutine(flipCardGameView.ShowNextLevelUI(string.Format("NEXT LEVEL: {0}-{1}", currentLevel, currentRound + 1)));
		}
	}

	void NextLevel()
	{
		lockNextRound = false;
        flipCardGameView.AddTimeEffect(1f);
		gamePassTime = 0;
		
		flipCardGameView.SetCurrentLevel(currentLevel, currentRound + 1);
		gameMainView.SetGoldCard(0);
		gameMainView.StartCoroutine(NextRoundRoutine());
	}

	IEnumerator NextRoundRoutine()
	{
		yield return new WaitForSeconds(0.3f);  //等桌面清空

		int[] specialCardCount = new int[2];
		specialCardCount[0] = 0;
		specialCardCount[1] = 0;
		if(!feverTimeOn)
		{
			if(currentGameSetting.specialCardAppearRound.Length > 0)
			{
				int specialCardType = 0;
				if(currentRound < currentGameSetting.round)
				{
					if(currentGameSetting.specialCardAppearRound.Length > currentRound)
					{
						specialCardType = currentGameSetting.specialCardAppearRound[currentRound];
					}
				} else
				{
					specialCardType = 3;
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
						int randomIndex = Random.Range(0, 2);
						specialCardCount[randomIndex] = 2;
						break;
					case 4:
						specialCardCount[0] = 2;
						specialCardCount[1] = 2;
						break;
				}
			}
		}
		yield return gameMainView.StartCoroutine(gameMainView.DealCard(flipCardArraySetting.cardSize, flipCardArraySetting.realCardPosition,
			specialCardCount[0], specialCardCount[1], feverTimeOn));
		yield return new WaitForSeconds(0.3f);  //等發卡動作結束

		while(currentState == GameState.Pausing)
			yield return null;
		gameMainView.FlipAllCard();
		float showCardTime = 0.35f + currentGameSetting.showCardTime;
		if(feverTimeOn)
			showCardTime = 0.55f;
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
		float showCardTime = 0.35f + currentGameSetting.showCardTime * (1f - (float)gameMainView.GetTableCardCount()/(float)currentGameSetting.cardCount);
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
