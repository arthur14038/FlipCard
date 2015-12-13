using UnityEngine;
using System.Collections;

public class GameSceneController : AbstractController {
    public enum GameState { Waiting, Playing, GameOver, Pausing }
    public static GameState currentState;
    public GameMenuView gameMenuView;
    public CardDealer dealer;
    float gamePassTime;
    float gameTime;
    int score;
    int currentCombo;
    int maxCombo;
    int currentRound;
    bool lastTimeHadMatch;
    CardArraySetting currentSetting;

    public override IEnumerator Init()
    {
        yield return StartCoroutine(gameMenuView.Init());

        gameMenuView.onClickPause = PauseGame;
        gameMenuView.onClickReady = ShowCardFaceAndStart;
        gameMenuView.onClickExit = ExitGame;
        gameMenuView.onClickResume = ResumeGame;

        gameMenuView.ShowUI(false);

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
        if (PlayerPrefsManager.OnePlayerProgress == (int)currentSetting.level)
            PlayerPrefsManager.OnePlayerProgress += 1;
        
        currentState = GameState.GameOver;
        gameMenuView.SetTimeBar(0f);
        gameMenuView.ShowGameOverWindow(score, maxCombo);
    }

    void CardMatch(bool match, params Card[] cards)
    {
        int scoreChangeAmount = 0;
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
        }else
        {
            if (lastTimeHadMatch)
            {
                lastTimeHadMatch = false;
                dealer.ToggleCardGlow(false);
            }
            currentCombo = 0;
            if(currentSetting.level < CardArrayLevel.FourByFive)
                scoreChangeAmount = -2;
        }
        if(scoreChangeAmount != 0)
        {
            int saveScore = score;
            score += scoreChangeAmount;
            if (score < 0)
                score = 0;

            if(saveScore != score)
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
        gameTime += addAmount;
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
		if(score > record.highScore)
			record.highScore = score;
        if (maxCombo > record.maxCombo)
            record.maxCombo = maxCombo;
        record.playTimes += 1;

		ModelManager.Instance.SaveGameRecord(record);
	}

	void Update()
	{
		if(currentState == GameState.Playing)
		{
			gameMenuView.SetTimeBar(1f - gamePassTime/gameTime);
			gamePassTime += Time.deltaTime;
			if(gamePassTime >= gameTime)
				GameOver();
		}
	}
}
