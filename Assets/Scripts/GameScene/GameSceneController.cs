using UnityEngine;
using System.Collections;

public class GameSceneController : AbstractController
{
    public GameMenuView gameMenuView;
    public CardDealer dealer;
	CardArraySetting currentSetting;
	GameModeJudgement judgement;

    protected override void Start()
    {
        if (GameMainLoop.Instance != null)
            GameMainLoop.Instance.RegisterController(this, LoadingComplete);
    }

    public override IEnumerator Init()
    {
        yield return StartCoroutine(gameMenuView.Init());

		currentSetting = CardArrayManager.GetCurrentLevelSetting();
		judgement = GetJudgement();
		yield return StartCoroutine(judgement.Init(dealer, GameOver, currentSetting, gameMenuView));
		
		gameMenuView.onCountDownFinished = StartGame;
        gameMenuView.onClickExit = ExitGame;
		gameMenuView.SetMode(CardArrayManager.currentMode);
	}
	
    void LoadingComplete()
    {
		switch(CardArrayManager.currentMode)
		{
			case GameMode.LimitTime:
				AudioManager.Instance.PlayOneShot("StartGameCountDown");
				gameMenuView.ShowUI(true);
				break;
			case GameMode.Competition:
				gameMenuView.ShowUI(false);
				break;
		}
    }
	
    void ExitGame()
	{
		AudioManager.Instance.StopMusic();
		int returnView = 1;
		if(CardArrayManager.currentMode == GameMode.Competition || CardArrayManager.currentMode == GameMode.Cooperation)
			returnView = 2;
		GameMainLoop.Instance.ChangeScene(SceneName.MainScene, returnView);
    }

    void StartGame()
    {
        StartCoroutine(judgement.StartGame());
    }
	
    void GameOver(int score, int maxCombo)
    {
		if (PlayerPrefsManager.OnePlayerProgress == (int)currentSetting.level)
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
		
        gameMenuView.ShowGameOverWindow(score, maxCombo, newHighScore, newMaxCombo);
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
