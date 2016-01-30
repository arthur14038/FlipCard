using UnityEngine;
using System.Collections;

public class InfiniteModeJudgement : GameModeJudgement
{
	int life;
	int score;
	int currentRound;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		gameMainView.completeOneRound = NexRound;
		gameMainView.cardMatch = CardMatch;
		yield return gameMainView.StartCoroutine(gameMainView.DealCard(GetCurrentCardSize(), GetCardPos()));
	}

	protected override IEnumerator StartGame()
	{
		life = 3;
		score = 0;
		currentRound = 1;
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f + GetCurrentShowCardTime());
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		gameMainView.ToggleMask(false);

		if(currentState == GameState.Waiting)
			currentState = GameState.Playing;
		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}

	protected override void GameOver(params int[] values)
	{
		base.GameOver(values);
	}

	void CardMatch(bool match, params CardBase[] cards)
	{
		if(currentState != GameState.GameOver)
		{
			if(match)
			{
				int addScore = 1 * cards.Length;

				if(cards[0].IsGoldCard())
					addScore *= 2;

				if(cards[1].IsGoldCard())
					addScore *= 2;
			} else
			{
				--life;
				if(life <= 0)
				{
					GameOver(score, currentRound);
				}
			}
		}
	}

	void NexRound()
	{
		if(currentState == GameState.Playing)
			currentState = GameState.Waiting;
		++currentRound;
		gameMainView.StartCoroutine(NextRoundRoutine());
	}

	protected override Vector2[] GetCardPos()
	{
		return base.GetCardPos();
	}

	float GetCurrentCardSize()
	{
		return 192f;
	}

	float GetCurrentShowCardTime()
	{
		return 1.5f;
	}

	IEnumerator NextRoundRoutine()
	{
		gameMainView.ToggleMask(true);
		yield return new WaitForSeconds(0.3f);
		yield return gameMainView.StartCoroutine(gameMainView.DealCard(GetCurrentCardSize(), GetCardPos()));
		yield return new WaitForSeconds(0.3f);
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f + GetCurrentShowCardTime());
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		gameMainView.ToggleMask(false);
		if(currentState == GameState.Waiting)
			currentState = GameState.Playing;
	}
}
