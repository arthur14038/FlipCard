using UnityEngine;
using System.Collections;

public class ClassicModeJudgement : GameModeJudgement
{	
	ClassicModeGameView classicModeGameView;
	ClassicModeSetting currentModeSetting;
	int moveTimes;

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
		moveTimes = 0;
		gameMainView.ToggleMask(false);
		if(currentState == GameState.Waiting)
			currentState = GameState.Playing;
		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}

	void RoundComplete()
	{
		GameOver(moveTimes);
	}

	protected override void GameOver(params int[] values)
	{
		if(PlayerPrefsManager.ClassicModeProgress == (int)currentModeSetting.level)
			PlayerPrefsManager.ClassicModeProgress += 1;

		if(PlayerPrefsManager.UnlockMode == 0)
			PlayerPrefsManager.UnlockMode = 3;

		if(PlayerPrefsManager.TimeModeProgress < 0)
			PlayerPrefsManager.TimeModeProgress = 0;

		base.GameOver(values);
		int moveTimes = values[0];
		int grade = 1;
		if(moveTimes == currentModeSetting.excellentMove)
			grade = 4;

		if(moveTimes > currentModeSetting.excellentMove && moveTimes <= currentModeSetting.gradeGap)
			grade = 3;

		if(moveTimes > currentModeSetting.gradeGap && moveTimes <= currentModeSetting.gradeGap * 2)
			grade = 2;

		bool recordBreak = false;
		GameRecord record = ModelManager.Instance.GetGameRecord(currentModeSetting.level, GameMode.Classic);
		if(record.grade < grade)
		{
			record.grade = grade;
			recordBreak = true;
		}
		if(moveTimes < record.highScore || record.highScore == 0)
		{
			record.highScore = moveTimes;
			recordBreak = true;
		}
		record.playTimes += 1;
		
		if(record.playTimes % 3 == 1)
			UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.GameRecord, ModelManager.Instance.GetGameRecordForSendEvent(GameMode.Classic));

		ModelManager.Instance.SaveGameRecord(record);
		gameMainView.ToggleMask(true);
		gameSettingView.ShowSinglePlayerGameOver(grade, moveTimes, ".MOVE.", "LEVEL COMPLETE", recordBreak);
	}

	void CardMatch(bool match, params Card_Normal[] cards)
	{
		if(currentState != GameState.GameOver)
		{
			++moveTimes;
			classicModeGameView.SetFailTimes(moveTimes);
		}
	}
}
