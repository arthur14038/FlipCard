using UnityEngine;
using System.Collections;

public class TimeModeJudgement : GameModeJudgement
{
	float gamePassTime;
	float gameTime;
	int currentRound;
	int maxCombo;
	int currentCombo;
	int score;
	int failedTimes;
	int complimentTimes;
	bool lastTimeHadMatch = false;
	bool unknownCardActive = false;
	TimeModeView timeModeView;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		gameTime = currentSetting.gameTime;
		gameMainView.completeOneRound = NextRound;
		gameMainView.cardMatch = CardMatch;
		timeModeView = (TimeModeView)modeView;
		timeModeView.onClickPause = PauseGame;
		timeModeView.onClickGameOverExit = ExitGame;
		timeModeView.SetTimeBar(1f);
		timeModeView.onCountDownFinished = StartGame;
        yield return gameMainView.StartCoroutine(gameMainView.DealCard());
	}

	protected override IEnumerator StartGame()
	{
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentSetting.showCardTime);
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		gameMainView.ToggleMask(false);

		failedTimes = 0;
		complimentTimes = 0;
		if(currentState == GameState.Waiting)
			currentState = GameState.Playing;
		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}
	
	public override void JudgementUpdate()
	{
		if(timeModeView != null)
		{
			if(currentState == GameState.Playing)
			{
				timeModeView.SetTimeBar(1f - gamePassTime / gameTime);
				gamePassTime += Time.deltaTime;
				if(gamePassTime >= gameTime)
				{
					GameOver(score, maxCombo);
					timeModeView.SetTimeBar(0f);
				}

				if(gameTime - gamePassTime < 5f)
					timeModeView.ToggleTimeIsRunning(true);
				else
					timeModeView.ToggleTimeIsRunning(false);
			} else
			{
				timeModeView.ToggleTimeIsRunning(false);
			}
		}
	}
		
	void CardMatch(bool match, params Card[] cards)
	{
		if(currentState != GameState.GameOver)
		{
			int scoreChangeAmount = 0;
			if(match)
			{
				scoreChangeAmount = currentSetting.matchAddScore * cards.Length;
				if(lastTimeHadMatch)
				{
					scoreChangeAmount += currentSetting.comboAddScore * cards.Length;

					++currentCombo;
					maxCombo = Mathf.Max(currentCombo, maxCombo);
				} else
				{
					lastTimeHadMatch = true;
					gameMainView.ToggleCardGlow(true);
				}
			} else
			{
				scoreChangeAmount = currentSetting.mismatchReduceScore * cards.Length;
				if(lastTimeHadMatch)
				{
					lastTimeHadMatch = false;
					gameMainView.ToggleCardGlow(false);
				}
				currentCombo = 0;
				++failedTimes;
			}
			if(scoreChangeAmount != 0)
			{
				int saveScore = score;
				score += scoreChangeAmount;
				if(score < 0)
					score = 0;

				if(saveScore != score)
				{
					timeModeView.SetScore(score);

					foreach(Card matchCard in cards)
					{
						Vector2 pos = matchCard.GetAnchorPosition();
						pos.x += currentSetting.edgeLength / 2 - 20f;
						gameMainView.ShowScoreText((score - saveScore) / cards.Length, pos);
					}
				}
			}
		}
	}

	void NextRound()
	{
		if(failedTimes == 0)
		{
			++complimentTimes;
			timeModeView.ShowCompliment(complimentTimes);
		}
		if(currentState == GameState.Playing)
			currentState = GameState.Waiting;
		++currentRound;

		float awardTime = currentSetting.awardTime;

		if(unknownCardActive)
			awardTime *= 1.5f;

		if(!unknownCardActive && currentRound >= currentSetting.unknownCardShowRound)
			unknownCardActive = true;

		timeModeView.SetRound(currentRound);
		AddGameTime(awardTime);
		gameMainView.StartCoroutine(NextRoundRoutine());
	}
	
	void AddGameTime(float addAmount)
	{
		gamePassTime -= addAmount;
		if(gamePassTime < 0f)
			gamePassTime = 0f;
		timeModeView.AddTimeEffect(1f - gamePassTime / gameTime);
	}

	protected override void GameOver(params int[] values)
	{
		base.GameOver(values);
		int score = values[0];
		int maxCombo = values[1];

		if(PlayerPrefsManager.OnePlayerProgress == (int)currentSetting.level)
			PlayerPrefsManager.OnePlayerProgress += 1;

		GameRecord record = ModelManager.Instance.GetGameRecord(currentSetting.level);
		bool newHighScore = false;
		bool newMaxCombo = false;
		if(score > record.highScore)
		{
			record.highScore = score;
			newHighScore = true;
		}
		if(maxCombo > record.maxCombo)
		{
			record.maxCombo = maxCombo;
			newMaxCombo = true;
		}
		record.playTimes += 1;

		if(record.playTimes % 3 == 1)
			UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.GameRecord, ModelManager.Instance.GetAllGameRecord());

		ModelManager.Instance.SaveGameRecord(record);

		timeModeView.ShowGameOverWindow(score, maxCombo, newHighScore, newMaxCombo);
	}
	
	IEnumerator NextRoundRoutine()
	{
		gameMainView.ToggleMask(true);
		yield return new WaitForSeconds(0.3f);
		yield return gameMainView.StartCoroutine(gameMainView.DealCard(unknownCardActive));
		yield return new WaitForSeconds(0.3f);
		yield return gameMainView.StartCoroutine(DealCardRoutine());
	}

	IEnumerator DealCardRoutine()
	{
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentSetting.showCardTime);
		if(unknownCardActive)
			yield return new WaitForSeconds(1f);
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		if(unknownCardActive)
			gameMainView.ResetUnknownCard();
		gameMainView.ToggleMask(false);
		failedTimes = 0;
		if(currentState == GameState.Waiting)
			currentState = GameState.Playing;

		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}
}
