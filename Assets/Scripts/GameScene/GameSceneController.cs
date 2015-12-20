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
            GameMainLoop.Instance.RegisterController(this, StartCounting);
    }

    public override IEnumerator Init()
    {
        yield return StartCoroutine(gameMenuView.Init());

		currentSetting = CardArrayManager.GetCurrentLevelSetting();
		judgement = new TimeModeJudgement();
		yield return StartCoroutine(judgement.Init(dealer, GameOver, currentSetting, gameMenuView));
		
		gameMenuView.onCountDownFinished = StartGame;
        gameMenuView.onClickExit = ExitGame;
		gameMenuView.onClickPause = judgement.PauseGame;
		gameMenuView.onClickResume = judgement.ResumeGame;
        gameMenuView.ShowUI(false);	
    }
	
    void StartCounting()
    {
		AudioManager.Instance.PlayOneShot("StartGameCountDown");
        gameMenuView.ShowUI(true);
    }
	
    void ExitGame()
	{
		AudioManager.Instance.StopMusic();
        GameMainLoop.Instance.ChangeScene(SceneName.MainScene, 1);
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

    void Update()
    {
		if(judgement != null)
			judgement.JudgementUpdate();
    }
}
