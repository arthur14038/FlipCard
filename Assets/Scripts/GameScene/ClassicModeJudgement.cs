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
		gameMainView.LoadCard(currentCardArraySetting.row * currentCardArraySetting.column, 0);
		gameMainView.SetUsingCard(currentCardArraySetting.row * currentCardArraySetting.column, 0);
		currentModeSetting = GameSettingManager.GetCurrentClassicModeSetting();
        gameMainView.completeOneRound = RoundComplete;
		gameMainView.cardMatch = CardMatch;
		classicModeGameView = (ClassicModeGameView)modeView;
		classicModeGameView.onClickPause = PauseGame;
		classicModeGameView.onGameStart = StartGame;
		yield return gameMainView.StartCoroutine(gameMainView.DealCard(currentCardArraySetting.edgeLength, GetCardPos()));
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

		bool[] achieveCondition = new bool[4];
		int moveTimes = values[0];
		int grade = 1;
		achieveCondition[0] = true;

		if(moveTimes <= currentModeSetting.targetMove)
		{
			grade += 1;
			achieveCondition[1] = true;
		}else
		{
			achieveCondition[1] = false;
		}

		if(Mathf.FloorToInt(gameTime) < currentModeSetting.targetTime)
		{
			grade += 1;
			achieveCondition[2] = true;
		} else
		{
			achieveCondition[2] = false;
		}

		if(moveTimes <= currentModeSetting.excellentMove)
		{
			grade += 1;
			achieveCondition[3] = true;
		} else
		{
			achieveCondition[3] = false;
		}

		bool recordBreak = false;
		GameRecord record = ModelManager.Instance.GetGameRecord(currentModeSetting.level, GameMode.Classic);
		if(record.grade < grade)
		{
			record.grade = grade;
			recordBreak = true;
		}
		if(record.highScore == 0 || gameTime < record.highScore)
		{
			record.highScore = (int)gameTime;
			recordBreak = true;
		}
		record.playTimes += 1;

		if(saveGameRecord != null)
			saveGameRecord(record);
		
		gameMainView.ToggleMask(true);

		string[] conditionContent = new string[4];
		string gameTimeContent = string.Format("{0}:{1:00}", (int)gameTime / 60, (int)gameTime % 60);
		conditionContent[0] = string.Format("COMPLETE {0} LEVEL", currentModeSetting.level.ToString());
		conditionContent[1] = string.Format("LESS THAN {0} MOVE", currentModeSetting.targetMove);
		conditionContent[2] = string.Format("COMPLETE IN {0} SECOND", currentModeSetting.targetTime+1);
		conditionContent[3] = string.Format("LESS THAN {0} MOVE", currentModeSetting.excellentMove);

		gameSettingView.ShowSinglePlayerGameOver(achieveCondition, "LEVEL COMPLETE", "CLASSIC MODE", ".GAME TIME.", gameTimeContent, conditionContent, recordBreak);
	}

	void CardMatch(bool match, params CardBase[] cards)
	{
		if(currentState != GameState.GameOver)
		{
			++moveTimes;
			classicModeGameView.SetMoveTimes(moveTimes);
		}
	}
}
