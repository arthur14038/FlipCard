using UnityEngine;
using System.Collections;

public class ClassicModeJudgement : GameModeJudgement
{	
	ClassicModeGameView classicModeGameView;
	ClassicModeSetting currentModeSetting;
	int moveTimes;
	float gameTime;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		currentModeSetting = GameSettingManager.GetCurrentClassicModeSetting();
        gameMainView.completeOneRound = RoundComplete;
		gameMainView.cardMatch = CardMatch;
		classicModeGameView = (ClassicModeGameView)modeView;
		classicModeGameView.onClickPause = PauseGame;
		classicModeGameView.onGameStart = StartGame;
		yield return gameMainView.StartCoroutine(gameMainView.DealCard());
		//gameMainView.SetAllCardBackUnknown();
    }

	protected override IEnumerator StartGame()
	{
		yield return null;
		gameMainView.ToggleMask(false);
		moveTimes = 0;
		gameTime = 0f;
		if(currentState == GameState.Waiting)
			currentState = GameState.Playing;
		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}

	public override void JudgementUpdate()
	{
		if(classicModeGameView != null)
		{
			if(currentState == GameState.Playing)
			{
				gameTime += Time.deltaTime;
				classicModeGameView.SetGameTime(gameTime);
            }
		}
	}

	void RoundComplete()
	{
		GameOver(moveTimes);
	}

	protected override void GameOver(params int[] values)
	{
		base.GameOver(values);

		if(PlayerPrefsManager.ClassicModeProgress == (int)currentModeSetting.level)
			PlayerPrefsManager.ClassicModeProgress += 1;

		if(PlayerPrefsManager.UnlockMode == 0)
			PlayerPrefsManager.UnlockMode = 3;

		if(PlayerPrefsManager.TimeModeProgress < 0)
			PlayerPrefsManager.TimeModeProgress = 0;

		int moveTimes = values[0];
		int grade = 1;
		int additionScore = 0;

		if(moveTimes < currentModeSetting.targetMove*2)
			additionScore += currentModeSetting.targetMove - moveTimes;

		if(moveTimes <= currentModeSetting.targetMove)
			grade += 1;

		if(gameTime <= currentModeSetting.targetTime)
			grade += 1;

		if(moveTimes == currentModeSetting.excellentMove)
			grade += 1;

		Debug.LogFormat("moveTimes: {0}, gameTime: {1}:{2:00}", moveTimes, (int)gameTime / 60, (int)gameTime % 60);

		bool recordBreak = false;
		GameRecord record = ModelManager.Instance.GetGameRecord(currentModeSetting.level, GameMode.Classic);
		if(record.grade < grade)
		{
			record.grade = grade;
			recordBreak = true;
		}
		record.playTimes += 1;
		
		if(record.playTimes % 3 == 1)
			UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.GameRecord, ModelManager.Instance.GetGameRecordForSendEvent(GameMode.Classic));

		ModelManager.Instance.SaveGameRecord(record);
		gameMainView.ToggleMask(true);
		gameSettingView.ShowSinglePlayerGameOver(grade, 0, ".SCORE.", "LEVEL COMPLETE", recordBreak);
	}

	void CardMatch(bool match, params Card_Normal[] cards)
	{
		if(currentState != GameState.GameOver)
		{
			++moveTimes;
			classicModeGameView.SetMoveTimes(moveTimes);

			//if(match)
			//{
			//	int scoreChangeAmount = currentModeSetting.matchAddScore * cards.Length;

			//	if(scoreChangeAmount != 0)
			//	{
			//		score += scoreChangeAmount;
			//		classicModeGameView.SetScore(score);

			//		foreach(Card_Normal matchCard in cards)
			//		{
			//			Vector2 pos = matchCard.GetAnchorPosition();
			//			pos.x += currentCardArraySetting.edgeLength / 2 - 20f;
			//			gameMainView.ShowScoreText(currentModeSetting.matchAddScore, pos);
			//		}
			//	}
			//}
		}
	}
}
