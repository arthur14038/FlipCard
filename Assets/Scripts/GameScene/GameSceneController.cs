using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSceneController : AbstractController
{
    public GameSettingView gameSettingView;
	public GameMainView gameMainView;
	AbstractView modeView;
	GameModeJudgement judgement;
	GameRecord thisTimeRecord;

    protected override void Start()
    {
        if (GameMainLoop.Instance != null)
            GameMainLoop.Instance.RegisterController(this, LoadingComplete);
    }

    public override IEnumerator Init()
	{
		modeView = GetModeView();
		judgement = GetJudgement();
		judgement.exitGame = ExitGame;
		judgement.saveGameRecord = SaveTmpGameRecord;
		yield return StartCoroutine(gameSettingView.Init());
		yield return StartCoroutine(gameMainView.Init());
		yield return StartCoroutine(modeView.Init());
		yield return StartCoroutine(judgement.Init(gameMainView, gameSettingView, modeView));
		
		gameSettingView.HideUI(false);
		gameMainView.ShowUI(false);
		modeView.ShowUI(false);
    }
	
    void LoadingComplete()
    {
		modeView.ShowUI(true);
    }

	void SaveTmpGameRecord(GameRecord record)
	{
		thisTimeRecord = record;
	}

	void ExitGame()
	{
		AudioManager.Instance.StopMusic();
		int returnView = 0;
		switch(GameSettingManager.currentMode)
		{
			case GameMode.FlipCard:
				returnView = 1;
                break;
			case GameMode.Competition:
				returnView = 2;
				break;
		}

		if(thisTimeRecord != null)
		{
			if(thisTimeRecord.playTimes % 3 == 1)
			{
				Dictionary<string, object> eventData = new Dictionary<string, object>();
				int level = thisTimeRecord.highLevel / 1000;
				int round = thisTimeRecord.highLevel % 1000;
				string gameInfo = string.Format("playTimes: {0}, highScore: {1}, highLevel: {2}-{3}", thisTimeRecord.playTimes, thisTimeRecord.highScore, level, round);
				eventData.Add("Game Info", gameInfo);
				eventData.Add("High Score", thisTimeRecord.highScore);
				eventData.Add("Play Times", thisTimeRecord.playTimes);
				UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.InfiniteGameRecord, eventData);
            }

			ModelManager.Instance.SaveFlipCardGameRecord(thisTimeRecord);
		}

		GameMainLoop.Instance.ChangeScene(SceneName.MainScene, returnView);
    }

	AbstractView GetModeView()
	{
		switch(GameSettingManager.currentMode)
		{
			case GameMode.FlipCard:
				GameObject flipCardGameView = Instantiate(Resources.Load("UI/FlipCardGameView")) as GameObject;
				Canvas timeModeViewCanvas = flipCardGameView.GetComponent<Canvas>();
				timeModeViewCanvas.worldCamera = Camera.main;
				return flipCardGameView.GetComponent<AbstractView>();
			case GameMode.Competition:
				GameObject competitionModeView = Instantiate(Resources.Load("UI/CompetitionModeView")) as GameObject;
				Canvas competitionModeViewCanvas = competitionModeView.GetComponent<Canvas>();
				competitionModeViewCanvas.worldCamera = Camera.main;
				return competitionModeView.GetComponent<AbstractView>();
			default:
				return null;
		}
	}

	GameModeJudgement GetJudgement()
	{
		switch(GameSettingManager.currentMode)
		{
			case GameMode.FlipCard:
				return new FlipCardGameJudgement();
			case GameMode.Competition:
				return new CompetitionModeJudgement();
			default:
				Debug.LogErrorFormat("{0}'s judgement is not implement", GameSettingManager.currentMode);
				return null;
		}
	}

    void Update()
    {
		if(judgement != null)
			judgement.JudgementUpdate();
    }
}
