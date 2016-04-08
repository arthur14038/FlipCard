using UnityEngine;
using System.Collections;

public class CompetitionModeJudgement : GameModeJudgement
{
	public enum WhosTurn {WaitingReady, Player1Playing, Player2Playing, WaitingPlayer1, WaitingPlayer2}
	WhosTurn currentTurn;
	int player1Score;
	int player2Score;
	int comboCount;
	bool player1Ready = false;
	bool player2Ready = false;
	CompetitionModeView competitionModeView;
	CompetitionModeSetting currentModeSetting;
	FlipCardArraySetting flipCardArraySetting;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		currentModeSetting = GameSettingManager.GetCurrentCompetitionModeSetting();
		flipCardArraySetting = GameSettingManager.GetFlipCardArraySetting(currentModeSetting.cardCount);
        gameMainView.LoadCard(currentModeSetting.cardCount, 0);
		gameMainView.SetUsingCard(currentModeSetting.cardCount, 0);
		gameMainView.completeOneRound = RoundComplete;
		gameMainView.cardMatch = CardMatch;
		player1Score = 0;
		player2Score = 0;
		comboCount = 0;
		competitionModeView = (CompetitionModeView)modeView;
		competitionModeView.button_Player1.onLightUp = OnPlayerButtonLightUp;
		competitionModeView.button_Player2.onLightUp = OnPlayerButtonLightUp;
		competitionModeView.SetTwoPlayerScore(player1Score, player2Score);
		competitionModeView.onClickPause = PauseGame;
		competitionModeView.onTwoPlayerReady = StartGame;
		currentTurn = WhosTurn.WaitingReady;
		competitionModeView.SetPlayerButton(currentTurn);
	}

	protected override IEnumerator StartGame()
	{
		gameMainView.SetGoldCard(currentModeSetting.goldCardCount, false);
		yield return gameMainView.StartCoroutine(gameMainView.DealCard(flipCardArraySetting.cardSize, flipCardArraySetting.realCardPosition));
		yield return new WaitForSeconds(0.2f);
		yield return competitionModeView.StartCoroutine(competitionModeView.FadeOutInstruction());
		gameMainView.ToggleMask(false);
		currentState = GameState.Playing;
		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}

	void OnPlayerButtonLightUp(PressButtonTool pressButton)
	{
		AudioManager.Instance.PlayOneShot("GameResultScoreCount");
		switch(currentTurn)
		{
			case WhosTurn.WaitingReady:
				if(!player1Ready && pressButton == competitionModeView.button_Player1)
				{
					player1Ready = true;
					competitionModeView.button_Player1.SetChargeTime(0f);
                }
				if(!player2Ready && pressButton == competitionModeView.button_Player2)
				{
					player2Ready = true;
					competitionModeView.button_Player2.SetChargeTime(0f);
				}
				if(player1Ready && player2Ready)
				{
					currentTurn = (WhosTurn)Random.Range((int)WhosTurn.Player1Playing, (int)WhosTurn.Player2Playing + 1);
                    competitionModeView.StartCoroutine(competitionModeView.PlayerReadyEffect(currentTurn));
				}
				break;
			case WhosTurn.WaitingPlayer1:
				if(pressButton == competitionModeView.button_Player1)
				{
					currentTurn = WhosTurn.Player1Playing;
					competitionModeView.SetPlayerButton(currentTurn);
					gameMainView.ToggleMask(false);
				}
				break;
			case WhosTurn.WaitingPlayer2:
				if(pressButton == competitionModeView.button_Player2)
				{
					currentTurn = WhosTurn.Player2Playing;
					competitionModeView.SetPlayerButton(currentTurn);
					gameMainView.ToggleMask(false);
				}
				break;
		}
	}

	void RoundComplete()
	{
		AddScore(1, (int)currentTurn);
		GameOver(player1Score, player2Score);
	}

	protected override void GameOver(params int[] values)
	{
		base.GameOver(values);
		gameSettingView.ShowTwoPlayersGameOver(values[0], values[1]);
	}

	void AddScore(int addAmount, int playerNumber)
	{
		int saveScore = 0;
		int score = 0;
		switch(playerNumber)
		{
			case 1:
				score = player1Score;
				break;
			case 2:
				score = player2Score;
				break;
		}
		saveScore = score;
		score += addAmount;
		if(score < 0)
			score = 0;

		if(saveScore != score)
		{
			switch(playerNumber)
			{
				case 1:
					player1Score = score;
					break;
				case 2:
					player2Score = score;
					break;
			}
			competitionModeView.SetTwoPlayerScore(player1Score, player2Score);
		}
	}

	void CardMatch(bool match, params CardBase[] cards)
	{
		if(currentState != GameState.GameOver)
		{
			bool takeTurn = false;
			if(match)
			{
				int cardAScore = 1;
				int cardBScore = 1;
				
				if(comboCount == 0)
					gameMainView.ToggleCardGlow(true);
				
				if(comboCount > 0)
				{
					cardAScore += 2;
					cardBScore += 2;
                }

				++comboCount;

				if(cards[0].IsGoldCard())
					cardAScore += 4;

				if(cards[1].IsGoldCard())
					cardBScore += 4;

				int scoreChangeAmount = cardAScore + cardBScore;
                if(scoreChangeAmount != 0)
				{
					AddScore(scoreChangeAmount, (int)currentTurn);

					Vector2 pos = cards[0].GetAnchorPosition();
					pos.x += flipCardArraySetting.cardSize / 2 - 20f;
					gameMainView.ShowScoreText(pos, cardAScore);

					pos = cards[1].GetAnchorPosition();
					pos.x += flipCardArraySetting.cardSize / 2 - 20f;
					gameMainView.ShowScoreText(pos, cardBScore);
				}
			} else
			{
				if(comboCount > 0)
				{
					comboCount = 0;
					gameMainView.ToggleCardGlow(false);
				}
				takeTurn = true;
			}

			if(takeTurn)
			{
				if(currentTurn == WhosTurn.Player1Playing)
				{
					currentTurn = WhosTurn.WaitingPlayer2;
					competitionModeView.SetPlayerButton(currentTurn);
					gameMainView.ToggleMask(true);
				}
				if(currentTurn == WhosTurn.Player2Playing)
				{
					currentTurn = WhosTurn.WaitingPlayer1;
					competitionModeView.SetPlayerButton(currentTurn);
					gameMainView.ToggleMask(true);
				}
			}
		}
	}
}
