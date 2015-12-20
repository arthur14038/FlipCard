using UnityEngine;
using System.Collections;

public class GameSceneController : AbstractController
{
    public enum GameState { Waiting, Playing, GameOver, Pausing }
    public static GameState currentState;
    public GameMenuView gameMenuView;
    public CardDealer dealer;
	public GameObject timeIsRunning;
    float gamePassTime;
    float gameTime;
    int score;
    int currentCombo;
    int maxCombo;
    int currentRound;
    bool lastTimeHadMatch;
    CardArraySetting currentSetting;

    protected override void Start()
    {
        if (GameMainLoop.Instance != null)
            GameMainLoop.Instance.RegisterController(this, StartCounting);
    }

    public override IEnumerator Init()
    {
        yield return StartCoroutine(gameMenuView.Init());

        gameMenuView.onClickPause = PauseGame;
        gameMenuView.onClickReady = ShowCardFaceAndStart;
        gameMenuView.onClickExit = ExitGame;
        gameMenuView.onClickResume = ResumeGame;

        gameMenuView.ShowUI(false);
		timeIsRunning.SetActive(false);

		currentSetting = CardArrayManager.GetCurrentLevelSetting();
        gameTime = currentSetting.gameTime;
        dealer.Init(currentSetting, NextRound, CardMatch);

        score = 0;
        currentCombo = 0;
        maxCombo = 0;
        currentRound = 1;

        gamePassTime = 0;
        currentState = GameState.Waiting;
        gameMenuView.SetTimeBar(1f);
        lastTimeHadMatch = false;
        yield return StartCoroutine(dealer.DealCard());
    }

    void StartCounting()
    {
		AudioManager.Instance.PlayOneShot("StartGameCountDown");
        gameMenuView.ShowUI(true);
    }

    void PauseGame()
	{
		currentState = GameState.Pausing;
    }

    void ResumeGame()
	{
		currentState = GameState.Playing;
    }

    void ExitGame()
	{
		AudioManager.Instance.StopMusic();
		if (currentState == GameState.GameOver)
            SaveGameRecord();
        GameMainLoop.Instance.ChangeScene(SceneName.MainScene, 1);
    }

    void ShowCardFaceAndStart()
    {
        StartCoroutine(DealCardRoutine());
    }

    void NextRound()
    {
        currentState = GameState.Waiting;
        ++currentRound;
        gameMenuView.SetRound(currentRound);
        AddGameTime(currentSetting.awardTime);
        StartCoroutine(NextRoundRoutine());
    }

    void GameOver()
    {
		AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayOneShot("GameResult");
		if (PlayerPrefsManager.OnePlayerProgress == (int)currentSetting.level)
            PlayerPrefsManager.OnePlayerProgress += 1;

        GameRecord record = ModelManager.Instance.GetGameRecord(currentSetting.level);
        bool newHighScore = false;
        bool newMaxCombo = false;
        if(score > record.highScore)
            newHighScore = true;
        if(maxCombo > record.maxCombo)
            newMaxCombo = true;

        currentState = GameState.GameOver;
        gameMenuView.SetTimeBar(0f);
        gameMenuView.ShowGameOverWindow(score, maxCombo, newHighScore, newMaxCombo);
    }

    void CardMatch(bool match, params Card[] cards)
    {
        int scoreChangeAmount = 0;
        //float timeChangeAmount = 0f;
        if (match)
        {
            if (lastTimeHadMatch)
            {
                scoreChangeAmount = 12;

                ++currentCombo;
                maxCombo = Mathf.Max(currentCombo, maxCombo);
            }
            else
            {
                scoreChangeAmount = 8;
                lastTimeHadMatch = true;
                dealer.ToggleCardGlow(true);
            }
        }
        else
        {
            if (lastTimeHadMatch)
            {
                lastTimeHadMatch = false;
                dealer.ToggleCardGlow(false);
            }
            currentCombo = 0;

			switch(currentSetting.level)
			{
			case CardArrayLevel.TwoByThree:
				scoreChangeAmount = -4;
				break;
			case CardArrayLevel.ThreeByFour:
				scoreChangeAmount = -2;
				break;
			}			                
        }
        if (scoreChangeAmount != 0)
        {
            int saveScore = score;
            score += scoreChangeAmount;
            if (score < 0)
                score = 0;

            if (saveScore != score)
            {
                gameMenuView.SetScore(score);

                foreach (Card matchCard in cards)
                {
                    Vector2 pos = matchCard.GetAnchorPosition();
                    pos.x += currentSetting.edgeLength / 2 - 20f;
                    gameMenuView.ShowScoreText((score - saveScore) / cards.Length, pos);
                }
            }
        }
    }

    void AddGameTime(float addAmount)
    {
        gamePassTime -= addAmount;
        if(gamePassTime < 0f)
            gamePassTime = 0f;
        gameMenuView.AddTimeEffect(1f - gamePassTime / gameTime);
    }

    IEnumerator DealCardRoutine()
    {
        dealer.FlipAllCard();
        yield return new WaitForSeconds(0.35f + currentSetting.showCardTime);
        dealer.FlipAllCard();
        yield return new WaitForSeconds(0.35f);
        gameMenuView.ToggleMask(false);
        currentState = GameState.Playing;

		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}

    IEnumerator NextRoundRoutine()
    {
        gameMenuView.ToggleMask(true);
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(dealer.DealCard());
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(DealCardRoutine());
    }

    void SaveGameRecord()
    {
        GameRecord record = ModelManager.Instance.GetGameRecord(currentSetting.level);
        if (score > record.highScore)
            record.highScore = score;
        if (maxCombo > record.maxCombo)
            record.maxCombo = maxCombo;
        record.playTimes += 1;

		if(record.playTimes % 3 == 1)
		{
			UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.GameRecord, ModelManager.Instance.GetAllGameRecord());
		}

        ModelManager.Instance.SaveGameRecord(record);
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            gameMenuView.SetTimeBar(1f - gamePassTime / gameTime);
            gamePassTime += Time.deltaTime;
            if (gamePassTime >= gameTime)
                GameOver();

			if(gameTime - gamePassTime < 5f)
			{
				if(!timeIsRunning.activeSelf)
					timeIsRunning.SetActive(true);
			} else
			{
				if(timeIsRunning.activeSelf)
					timeIsRunning.SetActive(false);
			}
        }else
		{
			if(timeIsRunning.activeSelf)
				timeIsRunning.SetActive(false);
		}
    }
}
