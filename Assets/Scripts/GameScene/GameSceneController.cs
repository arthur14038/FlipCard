using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

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

		bool showAd = false;

		if(thisTimeRecord != null)
		{
			if(thisTimeRecord.lastScore[0] > 500)
				showAd = (Random.Range(0, 3) == 0);

			if(thisTimeRecord.playTimes % 3 == 1)
			{
				int level = thisTimeRecord.highLevel / 1000;
				int round = thisTimeRecord.highLevel % 1000;
				string reachLevel = string.Format("{0}-{1}", level, round);
				long score = thisTimeRecord.lastScore[0];
				GoogleAnalyticsManager.LogEvent(GoogleAnalyticsManager.EventCategory.GameRecord,
					reachLevel, score);
            }

			ModelManager.Instance.SaveFlipCardGameRecord(thisTimeRecord);
		}else
		{
			showAd = true;
			switch(GameSettingManager.currentMode)
			{
				case GameMode.FlipCard:
					string currentLevel = ((FlipCardGameJudgement)judgement).GetCurrentLevel();
					GoogleAnalyticsManager.LogEvent(GoogleAnalyticsManager.EventCategory.QuitGame, currentLevel);
					break;
				case GameMode.Competition:
                    break;
			}
		}

		if(showAd)
			StartCoroutine(WaitAdAndExit(returnView));
		else
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
			case GameMode.PickCard:
				GameObject pickGameView = Instantiate(Resources.Load("UI/PickGameView")) as GameObject;
				Canvas pickModeViewCanvas = pickGameView.GetComponent<Canvas>();
				pickModeViewCanvas.worldCamera = Camera.main;
				return pickGameView.GetComponent<AbstractView>();
			default:
				return null;
		}
	}

	GameModeJudgement GetJudgement()
	{
		switch(GameSettingManager.currentMode)
		{
			case GameMode.FlipCard:
				GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.GameSceneInfiniteMode);
				return new FlipCardGameJudgement();
			case GameMode.Competition:
				GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.GameSceneTwoPlayer);
				return new CompetitionModeJudgement();
			case GameMode.PickCard:
				GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.GameScenePickMode);
				return new PickModeJudgement();
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

	IEnumerator WaitAdAndExit(int returnView)
	{
		while(!Advertisement.IsReady("video"))
			yield return null;

		Advertisement.Show("video");

		GameMainLoop.Instance.ChangeScene(SceneName.MainScene, returnView);
	}

	void OnDestroy()
	{
		GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.LeaveGameScene);
	}
}
