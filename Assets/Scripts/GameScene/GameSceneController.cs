﻿using UnityEngine;
using System.Collections;

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
		if((float)Screen.width / Screen.height > 1200f / 1848f)
		{
			foreach(UnityEngine.UI.CanvasScaler scalerA in canvasScaler)
				scalerA.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;

			UnityEngine.UI.CanvasScaler scaler = modeView.GetComponent<UnityEngine.UI.CanvasScaler>();
			scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
		}
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
	
	void SaveTmpGameRecord(GameRecord record)
	{
		thisTimeRecord = record;
    }

    void LoadingComplete()
    {
		modeView.ShowUI(true);
    }
	
    void ExitGame()
	{
		AudioManager.Instance.StopMusic();
		int returnView = 0;
		switch(GameSettingManager.currentMode)
		{
			case GameMode.Classic:
				returnView = 1;
                break;
			case GameMode.LimitTime:
				returnView = 3;
				break;
			case GameMode.Competition:
				returnView = 2;
				break;
		}

		if(thisTimeRecord != null)
		{
			if(thisTimeRecord.playTimes % 3 == 1)
				UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.GameRecord, ModelManager.Instance.GetGameRecordForSendEvent(GameMode.Classic));

			ModelManager.Instance.SaveGameRecord(thisTimeRecord);
		}

		//GameMainLoop.Instance.ChangeScene(SceneName.TestMain, returnView);
		GameMainLoop.Instance.ChangeScene(SceneName.MainScene, returnView);
    }
	
	AbstractView GetModeView()
	{
		switch(GameSettingManager.currentMode)
		{
			case GameMode.LimitTime:
				GameObject timeModeView = Instantiate(Resources.Load("UI/TimeModeView")) as GameObject;
				//GameObject timeModeView = Instantiate(Resources.Load("UI/TimeModeView_NoM")) as GameObject;
				Canvas timeModeViewCanvas = timeModeView.GetComponent<Canvas>();
				timeModeViewCanvas.worldCamera = Camera.main;
				return timeModeView.GetComponent<AbstractView>();
			case GameMode.Competition:
				GameObject competitionModeView = Instantiate(Resources.Load("UI/CompetitionModeView")) as GameObject;
				//GameObject competitionModeView = Instantiate(Resources.Load("UI/CompetitionModeView_NoM")) as GameObject;
				Canvas competitionModeViewCanvas = competitionModeView.GetComponent<Canvas>();
				competitionModeViewCanvas.worldCamera = Camera.main;
				return competitionModeView.GetComponent<AbstractView>();
			case GameMode.Classic:
				GameObject classicModeView = Instantiate(Resources.Load("UI/ClassicModeView")) as GameObject;
				//GameObject classicModeView = Instantiate(Resources.Load("UI/ClassicModeView_NoM")) as GameObject;
				Canvas classicModeViewCanvas = classicModeView.GetComponent<Canvas>();
				classicModeViewCanvas.worldCamera = Camera.main;
				return classicModeView.GetComponent<AbstractView>();
			default:
				return null;
		}
	}

	GameModeJudgement GetJudgement()
	{
		switch(GameSettingManager.currentMode)
		{
			case GameMode.LimitTime:
				return new TimeModeJudgement();
			case GameMode.Competition:
				return new CompetitionModeJudgement();
			case GameMode.Classic:
				return new ClassicModeJudgement();
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
