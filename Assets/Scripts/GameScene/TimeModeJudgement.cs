using UnityEngine;
using System.Collections;

public class TimeModeJudgement : GameModeJudgement
{
	float gamePassTime;
	float gameTime;
	float frozenTime;
	int currentRound;
	int score;
	int activeFeverTimeCount;
	int mismatchTimes;
	int bombCardCount;
	int frozenCardCount;
	bool feverTimeOn = false;
	bool comboAward = false;
	TimeModeGameView timeModeGameView;
	TimeModeSetting currentModeSetting;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		currentModeSetting = GameSettingManager.GetCurrentTimeModeSetting();
		switch(currentModeSetting.level)
		{
			case LevelDifficulty.EASY:
				gameMainView.LoadCard(currentCardArraySetting.row * currentCardArraySetting.column, 0, false, false);
				break;
			case LevelDifficulty.NORMAL:

				gameMainView.LoadCard(currentCardArraySetting.row * currentCardArraySetting.column, 0, true, true);
				break;
			case LevelDifficulty.HARD:
				gameMainView.LoadCard(currentCardArraySetting.row * currentCardArraySetting.column, 0, true, true);
				break;
			case LevelDifficulty.CRAZY:
				gameMainView.LoadCard(currentCardArraySetting.row * currentCardArraySetting.column, 0, true, true);
				break;
		}
		gameTime = currentModeSetting.gameTime;
		gameMainView.completeOneRound = NextRound;
		gameMainView.cardMatch = CardMatch;
		timeModeGameView = (TimeModeGameView)modeView;
		timeModeGameView.onClickPause = PauseGame;
		timeModeGameView.SetTimeBar(1f);
		timeModeGameView.onCountDownFinished = StartGame;
		feverTimeOn = false;
		comboAward = false;
		RollSpecialCardCount();
		yield return gameMainView.StartCoroutine(gameMainView.DealCard(currentCardArraySetting.edgeLength, GetCardPos(), bombCardCount, frozenCardCount));
	}

	protected override IEnumerator StartGame()
	{
		score = 0;
		currentRound = 0;
		activeFeverTimeCount = 0;
		gamePassTime = 0;
		mismatchTimes = 0;
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentModeSetting.showCardTime);
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		gameMainView.ToggleMask(false);
		
        if(currentState == GameState.Waiting)
			currentState = GameState.Playing;
		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}
	
	public override void JudgementUpdate()
	{
		if(timeModeGameView != null)
		{
			if(currentState == GameState.Playing)
			{
				timeModeGameView.SetTimeBar(1f - gamePassTime / gameTime);
				if(frozenTime > 0)
				{
					frozenTime -= Time.deltaTime;
				} else
				{
					gamePassTime += Time.deltaTime;

					if(gamePassTime >= gameTime)
					{
						GameOver(score);
						timeModeGameView.SetTimeBar(0f);
					}

					if(gameTime - gamePassTime < 5f)
						timeModeGameView.ToggleTimeIsRunning(true);
					else
						timeModeGameView.ToggleTimeIsRunning(false);
				}
			} else
			{
				timeModeGameView.ToggleTimeIsRunning(false);
			}
		}
	}
		
	void CardMatch(bool match, params CardBase[] cards)
	{
		if(currentState != GameState.GameOver)
		{
			if(match)
			{
				int scoreChangeAmount = currentModeSetting.matchAddScore * cards.Length;

				foreach(CardBase matchCard in cards)
				{
					Vector2 pos = matchCard.GetAnchorPosition();
					pos.x += currentCardArraySetting.edgeLength / 2 - 20f;
					gameMainView.ShowScoreText(pos, comboAward, matchCard.IsGoldCard());
				}

				if(!comboAward)
				{
					comboAward = true;
					gameMainView.ToggleCardGlow(true);
				}else
				{
					scoreChangeAmount += 2 * cards.Length;
				}

				if(cards[0].IsGoldCard())
					scoreChangeAmount += 4;

				if(cards[1].IsGoldCard())
					scoreChangeAmount += 4;
				
				if(scoreChangeAmount != 0)
				{
					score += scoreChangeAmount;
					timeModeGameView.SetScore(score);
				}

				if(cards[0].IsBombCard && gameMainView.GetTableCardCount() > 0)
					gameMainView.StartCoroutine(BombCardEffect());

				if(cards[0].IsFrozenCard)
				{
					gameMainView.ShowFrozen();
					frozenTime = currentModeSetting.awardTime;
				}
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

	void NextRound()
	{
		if(currentState == GameState.Playing)
			currentState = GameState.Waiting;
		++currentRound;

		if(mismatchTimes > 0)
		{			
			feverTimeOn = false;
			mismatchTimes = 0;
			timeModeGameView.ToggleFeverTimeEffect(false);
			gameMainView.SetGoldCard(0);
		}else
		{
			feverTimeOn = true;
			++activeFeverTimeCount;
			timeModeGameView.ShowFeverTime();
			gameMainView.SetGoldCard(-1);
		}

		float awardTime = currentModeSetting.awardTime;
		
		timeModeGameView.SetRound(currentRound);
		AddGameTime(awardTime);
		gameMainView.StartCoroutine(NextRoundRoutine());
	}
	
	void AddGameTime(float addAmount)
	{
		gamePassTime -= addAmount;
		if(gamePassTime < 0f)
			gamePassTime = 0f;
		timeModeGameView.AddTimeEffect(1f - gamePassTime / gameTime);
	}

	void RollSpecialCardCount()
	{
		switch(currentModeSetting.level)
		{
			case LevelDifficulty.NORMAL:
				int normalDice = Random.Range(0, 5);
				switch(normalDice)
				{
					case 3:
						bombCardCount = 2;
						frozenCardCount = 0;
						break;
					case 4:
						bombCardCount = 0;
						frozenCardCount = 2;
						break;
					default:
						bombCardCount = 0;
						frozenCardCount = 0;
						break;
				}
				break;
			case LevelDifficulty.HARD:
				int hardDice = Random.Range(0, 1);
				switch(hardDice)
				{
					case 0:
						bombCardCount = 2;
						frozenCardCount = 0;
						break;
					case 1:
						bombCardCount = 0;
						frozenCardCount = 2;
						break;
				}
				break;
			case LevelDifficulty.CRAZY:
				bombCardCount = 2;
				frozenCardCount = 2;
				break;
			default:
				bombCardCount = 0;
				frozenCardCount = 0;
				break;
		}
	}

	protected override void GameOver(params int[] values)
	{
		base.GameOver(values);
		
		if(PlayerPrefsManager.TimeModeProgress == (int)currentModeSetting.level)
			PlayerPrefsManager.TimeModeProgress += 1;

		bool[] achieveCondition = new bool[4];
		int score = values[0];
		int grade = 1;
		achieveCondition[0] = true;

        if(currentRound >= currentModeSetting.targetRound)
		{
			grade += 1;
			achieveCondition[1] = true;
		} else
		{
			achieveCondition[1] = false;
		}

		if(activeFeverTimeCount >= currentModeSetting.targetFeverTimeCount)
		{
			grade += 1;
			achieveCondition[2] = true;
		} else
		{
			achieveCondition[2] = false;
		}

		if(score >= currentModeSetting.targetScore)
		{
			grade += 1;
			achieveCondition[3] = true;
		} else
		{
			achieveCondition[3] = false;
		}

		GameRecord record = ModelManager.Instance.GetGameRecord(currentModeSetting.level, GameMode.LimitTime);
		bool recordBreak = false;
		if(score > record.highScore)
		{
			record.highScore = score;
			recordBreak = true;
		}
		if(grade > record.grade)
		{
			record.grade = grade;
			recordBreak = true;
		}
		record.playTimes += 1;

		if(saveGameRecord != null)
			saveGameRecord(record);

		string[] conditionContent = new string[4];
		conditionContent[0] = string.Format("COMPLETE {0} LEVEL", currentModeSetting.level.ToString());
		conditionContent[1] = string.Format("OVER {0} ROUND", currentModeSetting.targetRound);
		conditionContent[2] = string.Format("FEVER {0} TIMES", currentModeSetting.targetFeverTimeCount);
		conditionContent[3] = string.Format("REACH {0} SCORE", currentModeSetting.targetScore);

		gameSettingView.ShowSinglePlayerGameOver(achieveCondition, "TIME'S UP", "TIME MODE", ".SCORE.", score.ToString(), conditionContent, recordBreak);
	}
	
	IEnumerator NextRoundRoutine()
	{
		gameMainView.ToggleMask(true);
		yield return new WaitForSeconds(0.3f);
		RollSpecialCardCount();
		yield return gameMainView.StartCoroutine(gameMainView.DealCard(currentCardArraySetting.edgeLength, GetCardPos(), bombCardCount, frozenCardCount));
		yield return new WaitForSeconds(0.3f);
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentModeSetting.showCardTime);
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		gameMainView.ToggleMask(false);
		if(currentState == GameState.Waiting)
			currentState = GameState.Playing;
	}

	IEnumerator BombCardEffect()
	{
		if(currentState == GameState.Playing)
			currentState = GameState.Waiting;

		gameMainView.ShowExplosion();
        gameMainView.ToggleMask(true);
		yield return new WaitForSeconds(0.3f);

		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentModeSetting.showCardTime);
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f);

		gameMainView.ToggleMask(false);

		if(currentState == GameState.Waiting)
			currentState = GameState.Playing;
	}
}
