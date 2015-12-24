﻿using UnityEngine;
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

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
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
		competitionModeView.onClickGameOverExit = ExitGame;
		competitionModeView.onTwoPlayerReady = StartGame;
		currentTurn = WhosTurn.WaitingReady;
		competitionModeView.SetPlayerButton(currentTurn);
	}

	protected override IEnumerator StartGame()
	{
		yield return gameMainView.StartCoroutine(gameMainView.DealCard());
		yield return new WaitForSeconds(0.2f);
		yield return competitionModeView.StartCoroutine(competitionModeView.FadeOutInstruction());
		gameMainView.ToggleMask(false);
		//gameMainView.SetLuckyCard(1);
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

			if(cards != null)
			{
				foreach(Card matchCard in cards)
				{
					Vector2 pos = matchCard.GetAnchorPosition();
					pos.x += currentSetting.edgeLength / 2 - 20f;
					gameMainView.ShowScoreText((score - saveScore) / cards.Length, pos);
				}
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
				if(comboCount == 0)
					gameMainView.ToggleCardGlow(true);

				scoreChangeAmount = 2 + 2 * comboCount;

				++comboCount;

				if(cards[0].GetCardType() == Card.CardType.Gold || cards[1].GetCardType() == Card.CardType.Gold)
				{
					scoreChangeAmount *= 2;
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
