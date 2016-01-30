using UnityEngine;
using System.Collections;

public class TimeModeJudgement : GameModeJudgement
{
	float gamePassTime;
	float gameTime;
	float feverTimePassTime;
	int currentRound;
	int currentCombo;
	int score;
	int activeFeverTimeCount;
	bool feverTimeOn = false;
	TimeModeGameView timeModeGameView;
	TimeModeSetting currentModeSetting;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		gameMainView.LoadCard(currentCardArraySetting.row * currentCardArraySetting.column, 0);
		gameMainView.SetUsingCard(currentCardArraySetting.row * currentCardArraySetting.column, 0);
		currentModeSetting = GameSettingManager.GetCurrentTimeModeSetting();
		gameTime = currentModeSetting.gameTime;
		gameMainView.completeOneRound = NextRound;
		gameMainView.cardMatch = CardMatch;
		timeModeGameView = (TimeModeGameView)modeView;
		timeModeGameView.onClickPause = PauseGame;
		timeModeGameView.SetTimeBar(1f);
		timeModeGameView.onCountDownFinished = StartGame;
		feverTimeOn = false;
		yield return gameMainView.StartCoroutine(gameMainView.DealCard(currentCardArraySetting.edgeLength, GetCardPos()));
	}

	protected override IEnumerator StartGame()
	{
		activeFeverTimeCount = 0;
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


				if(feverTimeOn)
				{
					feverTimePassTime += Time.deltaTime;
					timeModeGameView.SetFeverTimeCircle(1f - feverTimePassTime/ currentModeSetting.feverTimeDuration);

					if(feverTimePassTime >= currentModeSetting.feverTimeDuration)
					{
						feverTimeOn = false;
						timeModeGameView.ToggleFeverTimeEffect(false);
						gameMainView.SetGoldCard(0);
						gameMainView.ToggleCardGlow(false);
                    }
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
			int scoreChangeAmount = 0;
			if(match)
			{
				scoreChangeAmount = currentModeSetting.matchAddScore * cards.Length;

				if(!feverTimeOn)
				{
					++currentCombo;
					timeModeGameView.SetFeverTimeCircle((float)currentCombo / currentModeSetting.feverTimeThreshold);
                }

				if(cards[0].IsGoldCard())
					scoreChangeAmount *= 2;

				if(cards[1].IsGoldCard())
					scoreChangeAmount *= 2;

				if(currentCombo >= currentModeSetting.feverTimeThreshold && !feverTimeOn)
				{
					++activeFeverTimeCount;
                    timeModeGameView.ShowFeverTime();
					feverTimePassTime = 0f;
					feverTimeOn = true;
					gameMainView.SetGoldCard(-1);
					gameMainView.ToggleCardGlow(true);

					currentCombo = 0;
				}
			} else
			{
				scoreChangeAmount = currentModeSetting.mismatchReduceScore * cards.Length;

				if(!feverTimeOn)
				{
					currentCombo = 0;
					timeModeGameView.SetFeverTimeCircle((float)currentCombo / currentModeSetting.feverTimeThreshold);
				}
			}
			if(scoreChangeAmount != 0)
			{
				int saveScore = score;
				score += scoreChangeAmount;
				if(score < 0)
					score = 0;

				if(saveScore != score)
				{
					timeModeGameView.SetScore(score);

					foreach(CardBase matchCard in cards)
					{
						Vector2 pos = matchCard.GetAnchorPosition();
						pos.x += currentCardArraySetting.edgeLength / 2 - 20f;
						gameMainView.ShowScoreText((score - saveScore) / cards.Length, pos);
					}
				}
			}
		}
	}

	void NextRound()
	{
		if(currentState == GameState.Playing)
			currentState = GameState.Waiting;
		++currentRound;

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

		if(grade == 3 && score >= currentModeSetting.targetScore)
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
		if(grade >= 3)
			conditionContent[3] = string.Format("REACH {0} SCORE", currentModeSetting.targetScore);
		else
			conditionContent[3] = "?????????";
		gameSettingView.ShowSinglePlayerGameOver(achieveCondition, "TIME'S UP", "TIME MODE", ".SCORE.", score.ToString(), conditionContent, recordBreak);
	}
	
	IEnumerator NextRoundRoutine()
	{
		gameMainView.ToggleMask(true);
		yield return new WaitForSeconds(0.3f);
		yield return gameMainView.StartCoroutine(gameMainView.DealCard(currentCardArraySetting.edgeLength, GetCardPos()));
		yield return new WaitForSeconds(0.3f);
		yield return gameMainView.StartCoroutine(DealCardRoutine());
	}

	IEnumerator DealCardRoutine()
	{
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentModeSetting.showCardTime);
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		gameMainView.ToggleMask(false);
		if(currentState == GameState.Waiting)
			currentState = GameState.Playing;

		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}
}
