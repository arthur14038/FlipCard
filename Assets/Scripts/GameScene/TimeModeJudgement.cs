using UnityEngine;
using System.Collections;

public class TimeModeJudgement : GameModeJudgement
{
	float gamePassTime;
	float gameTime;
	int currentRound;
	int maxCombo;
	int currentCombo;
	int score;
	int failedTimes;
	int complimentTimes;
	bool lastTimeHadMatch = false;

	public override IEnumerator Init(CardDealer dealer, VoidTwoInt gameOver, CardArraySetting currentSetting, GameMenuView gameMenuView)
	{
		yield return gameMenuView.StartCoroutine(base.Init(dealer, gameOver, currentSetting, gameMenuView));
		gameTime = currentSetting.gameTime;
		dealer.Init(currentSetting, NextRound, CardMatch);
		gameMenuView.SetTimeBar(1f);
		yield return gameMenuView.StartCoroutine(dealer.DealCard());
	}

	public override IEnumerator StartGame()
	{
        dealer.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentSetting.showCardTime);
		dealer.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		gameMenuView.ToggleMask(false);

		failedTimes = 0;
		complimentTimes = 0;
		currentState = GameState.Playing;
		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}

	public override void PauseGame()
	{
		currentState = GameState.Pausing;
	}

	public override void ResumeGame()
	{
		currentState = GameState.Playing;
	}

	public override void JudgementUpdate()
	{
		if(currentState == GameState.Playing)
		{
			gameMenuView.SetTimeBar(1f - gamePassTime / gameTime);
			gamePassTime += Time.deltaTime;
			if(gamePassTime >= gameTime)
			{
				currentState = GameState.GameOver;
				gameOver(score, maxCombo);
				gameMenuView.SetTimeBar(0f);
			}

			if(gameTime - gamePassTime < 5f)
				gameMenuView.ToggleTimeIsRunning(true);
			else
				gameMenuView.ToggleTimeIsRunning(false);
		} else
		{
			gameMenuView.ToggleTimeIsRunning(false);
		}
	}

	void CardMatch(bool match, params Card[] cards)
	{
		if(currentState == GameState.Playing)
		{
			int scoreChangeAmount = 0;
			if(match)
			{
				if(lastTimeHadMatch)
				{
					scoreChangeAmount = 12;

					++currentCombo;
					maxCombo = Mathf.Max(currentCombo, maxCombo);
				} else
				{
					scoreChangeAmount = 8;
					lastTimeHadMatch = true;
					dealer.ToggleCardGlow(true);
				}
			} else
			{
				if(lastTimeHadMatch)
				{
					lastTimeHadMatch = false;
					dealer.ToggleCardGlow(false);
				}
				currentCombo = 0;
				++failedTimes;

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
			if(scoreChangeAmount != 0)
			{
				int saveScore = score;
				score += scoreChangeAmount;
				if(score < 0)
					score = 0;

				if(saveScore != score)
				{
					gameMenuView.SetScore(score);

					foreach(Card matchCard in cards)
					{
						Vector2 pos = matchCard.GetAnchorPosition();
						pos.x += currentSetting.edgeLength / 2 - 20f;
						gameMenuView.ShowScoreText((score - saveScore) / cards.Length, pos);
					}
				}
			}
		}
	}

	void NextRound()
	{
		if(failedTimes == 0)
		{
			++complimentTimes;
			gameMenuView.ShowCompliment(complimentTimes);
		}
		currentState = GameState.Waiting;
		++currentRound;
		gameMenuView.SetRound(currentRound);
		AddGameTime(currentSetting.awardTime);
		gameMenuView.StartCoroutine(NextRoundRoutine());
	}
	
	void AddGameTime(float addAmount)
	{
		gamePassTime -= addAmount;
		if(gamePassTime < 0f)
			gamePassTime = 0f;
		gameMenuView.AddTimeEffect(1f - gamePassTime / gameTime);
	}
	
	IEnumerator NextRoundRoutine()
	{
		gameMenuView.ToggleMask(true);
		yield return new WaitForSeconds(0.3f);
		yield return gameMenuView.StartCoroutine(dealer.DealCard());
		yield return new WaitForSeconds(0.3f);
		yield return gameMenuView.StartCoroutine(DealCardRoutine());
	}

	IEnumerator DealCardRoutine()
	{
		dealer.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentSetting.showCardTime);
		dealer.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		gameMenuView.ToggleMask(false);
		failedTimes = 0;
		currentState = GameState.Playing;

		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}
}
