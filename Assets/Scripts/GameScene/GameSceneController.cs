﻿using UnityEngine;
using System.Collections;

public class GameSceneController : AbstractController
{
    public GameSettingView gameSettingView;
	public GameMainView gameMainView;
	AbstractView modeView;
	GameModeJudgement judgement;

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
	
    void ExitGame()
	{
		AudioManager.Instance.StopMusic();
		int returnView = 1;
		if(CardArrayManager.currentMode == GameMode.Competition || CardArrayManager.currentMode == GameMode.Cooperation)
			returnView = 2;
		GameMainLoop.Instance.ChangeScene(SceneName.MainScene, returnView);
    }
	
	AbstractView GetModeView()
	{
		switch(CardArrayManager.currentMode)
		{
		case GameMode.LimitTime:
			GameObject timeModeView = Instantiate(Resources.Load("UI/TimeModeView")) as GameObject;
			Canvas timeModeViewCanvas = timeModeView.GetComponent<Canvas>();
			timeModeViewCanvas.worldCamera = Camera.main;
			return timeModeView.GetComponent<AbstractView>();
        default:
			return null;
		}
	}

	GameModeJudgement GetJudgement()
	{
		switch(CardArrayManager.currentMode)
		{
			case GameMode.LimitTime:
				return new TimeModeJudgement();
			case GameMode.Competition:
				return new CompetitionModeJudgement();
			default:
				Debug.LogErrorFormat("{0}'s judgement is not implement", CardArrayManager.currentMode);
				return null;
		}
	}

    void Update()
    {
		if(judgement != null)
			judgement.JudgementUpdate();
    }
}
