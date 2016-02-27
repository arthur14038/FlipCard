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
			{
				Dictionary<string, object> eventData = new Dictionary<string, object>();
				eventData.Add("PlayTimes", thisTimeRecord.playTimes);
				eventData.Add("Grade", thisTimeRecord.grade.ToString());
				switch(thisTimeRecord.mode)
				{
					case GameMode.Classic:
					switch(thisTimeRecord.level)
					{
					case LevelDifficulty.EASY:
						UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.ClassicModeEasy, eventData);
						break;
					case LevelDifficulty.NORMAL:
						UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.ClassicModeNormal, eventData);
						break;
					case LevelDifficulty.HARD:
						UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.ClassicModeHard, eventData);
						break;
					case LevelDifficulty.CRAZY:
						UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.ClassicModeCrazy, eventData);
						break;
					}
					break;
				case GameMode.LimitTime:
					switch(thisTimeRecord.level)
					{
					case LevelDifficulty.EASY:
						UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.TimeModeEasy, eventData);
						break;
					case LevelDifficulty.NORMAL:
						UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.TimeModeNormal, eventData);
						break;
					case LevelDifficulty.HARD:
						UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.TimeModeHard, eventData);
						break;
					case LevelDifficulty.CRAZY:
						UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.TimeModeCrazy, eventData);
						break;
					}
						break;
				}
            }

			ModelManager.Instance.SaveGameRecord(thisTimeRecord);
		}

		GameMainLoop.Instance.ChangeScene(SceneName.MainScene, returnView);
    }
	
	AbstractView GetModeView()
	{
		switch(GameSettingManager.currentMode)
		{
			case GameMode.LimitTime:
				GameObject timeModeView = Instantiate(Resources.Load("UI/TimeModeView")) as GameObject;
				Canvas timeModeViewCanvas = timeModeView.GetComponent<Canvas>();
				timeModeViewCanvas.worldCamera = Camera.main;
				return timeModeView.GetComponent<AbstractView>();
			case GameMode.Competition:
				GameObject competitionModeView = Instantiate(Resources.Load("UI/CompetitionModeView")) as GameObject;
				Canvas competitionModeViewCanvas = competitionModeView.GetComponent<Canvas>();
				competitionModeViewCanvas.worldCamera = Camera.main;
				return competitionModeView.GetComponent<AbstractView>();
			case GameMode.Classic:
				GameObject classicModeView = Instantiate(Resources.Load("UI/ClassicModeView")) as GameObject;
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
