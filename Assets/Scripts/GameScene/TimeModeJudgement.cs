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
		currentModeSetting = GameSettingManager.GetCurrentTimeModeSetting();
		gameTime = currentModeSetting.gameTime;
		gameMainView.completeOneRound = NextRound;
		gameMainView.cardMatch = CardMatch;
		timeModeGameView = (TimeModeGameView)modeView;
		timeModeGameView.onClickPause = PauseGame;
		timeModeGameView.SetTimeBar(1f);
		timeModeGameView.onCountDownFinished = StartGame;
		feverTimeOn = false;
		yield return gameMainView.StartCoroutine(gameMainView.DealCard());
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
					timeModeGameView.SetFeverTimeBar(1f - feverTimePassTime/ currentModeSetting.feverTimeDuration);

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
		
	void CardMatch(bool match, params Card_Normal[] cards)
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
					timeModeGameView.SetFeverTimeBar((float)currentCombo / currentModeSetting.feverTimeThreshold);
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
					timeModeGameView.SetFeverTimeBar((float)currentCombo / currentModeSetting.feverTimeThreshold);
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

					foreach(Card_Normal matchCard in cards)
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

		Debug.LogFormat("gamePassTime: {0}", gamePassTime);

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

		int score = values[0];
		int grade = 1;
		if(currentRound >= currentModeSetting.targetRound)
			grade += 1;

		if(activeFeverTimeCount >= currentModeSetting.targetFeverTimeCount)
			grade += 1;

		if(score >= currentModeSetting.targetScore)
			grade += 1;
		
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

		if(record.playTimes % 3 == 1)
			UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.GameRecord, ModelManager.Instance.GetGameRecordForSendEvent(GameMode.LimitTime));

		ModelManager.Instance.SaveGameRecord(record);

		gameSettingView.ShowSinglePlayerGameOver(grade, score, ".SCORE.", "TIME'S UP", recordBreak);
	}
	
	IEnumerator NextRoundRoutine()
	{
		gameMainView.ToggleMask(true);
		yield return new WaitForSeconds(0.3f);
		yield return gameMainView.StartCoroutine(gameMainView.DealCard());
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
