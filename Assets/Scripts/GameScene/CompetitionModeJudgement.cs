using UnityEngine;
using System.Collections;

public class CompetitionModeJudgement : GameModeJudgement
{
	public enum WhosTurn {WaitingReady, Player1Playing, Player2Playing, WaitingPlayer1, WaitingPlayer2}
	WhosTurn currentTurn;
	int player1Score;
	int player2Score;
	bool player1Ready = false;
	bool player2Ready = false;
	bool lastTimeHadMatch = false;
	CompetitionModeView competitionModeView;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		gameMainView.completeOneRound = RoundComplete;
		gameMainView.cardMatch = CardMatch;
		player1Score = 0;
		player2Score = 0;
		competitionModeView = (CompetitionModeView)modeView;
		competitionModeView.button_Player1.onLightUp = OnPlayerButtonLightUp;
		competitionModeView.button_Player2.onLightUp = OnPlayerButtonLightUp;
		competitionModeView.SetTwoPlayerScore(player1Score, player2Score);
		competitionModeView.onClickPause = PauseGame;
		competitionModeView.onClickGameOverExit = ExitGame;
		competitionModeView.onTwoPlayerReady = StartGame;
		currentTurn = WhosTurn.WaitingReady;
		competitionModeView.SetPlayerButton(currentTurn);
		yield return gameMainView.StartCoroutine(gameMainView.DealCard());
	}

	protected override IEnumerator StartGame()
	{
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentSetting.showCardTime);
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		gameMainView.ToggleMask(false);
		currentState = GameState.Playing;
	}

	void OnPlayerButtonLightUp(PressButtonTool pressButton)
	{
		switch(currentTurn)
		{
			case WhosTurn.WaitingReady:
				if(pressButton == competitionModeView.button_Player1)
					player1Ready = true;
				if(pressButton == competitionModeView.button_Player2)
					player2Ready = true;
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
		AddScore(1, (int)currentTurn, null);
		GameOver(player1Score, player2Score);
	}

	protected override void GameOver(params int[] values)
	{
		base.GameOver(values);
		competitionModeView.ShowGameOver(values[0], values[1]);
	}

	void AddScore(int addAmount, int playerNumber, Card[] cards)
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

			foreach(Card matchCard in cards)
			{
				Vector2 pos = matchCard.GetAnchorPosition();
				pos.x += currentSetting.edgeLength / 2 - 20f;
				gameMainView.ShowScoreText((score - saveScore) / cards.Length, pos);
			}
		}
	}

	void CardMatch(bool match, params Card[] cards)
	{
		if(currentState != GameState.GameOver)
		{
			bool takeTurn = false;
			int scoreChangeAmount = 0;
			if(match)
			{
				if(lastTimeHadMatch)
				{
					scoreChangeAmount = 12;
				}
				else
				{
					scoreChangeAmount = 8;
					lastTimeHadMatch = true;
					gameMainView.ToggleCardGlow(true);
				}
			}else
			{
				if(lastTimeHadMatch)
				{
					lastTimeHadMatch = false;
					gameMainView.ToggleCardGlow(false);
				}
				scoreChangeAmount = -2;
				takeTurn = true;
			}

			if(scoreChangeAmount != 0)
				AddScore(scoreChangeAmount, (int)currentTurn, cards);

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
