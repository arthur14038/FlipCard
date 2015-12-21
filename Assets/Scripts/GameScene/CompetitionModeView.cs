using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class CompetitionModeView : AbstractView
{
	public VoidNoneParameter onTwoPlayerReady;
	public Text text_Player1Score;
	public Text text_Player2Score;
	public Text text_TakeTurn;
	public CanvasGroup group_Instruction;
	public RectTransform text_Instruction;
	public GameObject group_ChooseSide;
	public PressButtonTool button_Player1;
	public PressButtonTool button_Player2;
	public Button button_Pause;

	public override IEnumerator Init()
	{
		yield return null;
	}

	public void SetTwoPlayerScore(int player1Score, int player2Score)
	{
		text_Player1Score.text = player1Score.ToString();
		text_Player2Score.text = player2Score.ToString();
	}

	public IEnumerator DisableChooseSide(CompetitionModeJudgement.WhosTurn currentTurn)
	{
		text_Instruction.gameObject.SetActive(false);

		if(currentTurn == CompetitionModeJudgement.WhosTurn.Player1Playing)
			text_TakeTurn.text = "Player1 First!";
		else if(currentTurn == CompetitionModeJudgement.WhosTurn.Player2Playing)
			text_TakeTurn.text = "Player2 First!";

		text_TakeTurn.gameObject.SetActive(true);
		text_TakeTurn.rectTransform.localScale = Vector2.zero;
		yield return text_TakeTurn.rectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
		yield return new WaitForSeconds(0.5f);

		SetPlayerButton(currentTurn);
	}

	public void SetPlayerButton(CompetitionModeJudgement.WhosTurn currentTurn)
	{
		switch(currentTurn)
		{
		case CompetitionModeJudgement.WhosTurn.WaitingReady:
			button_Player1.UpdateButton(PressButtonTool.PressButtonState.Clickable);
			button_Player2.UpdateButton(PressButtonTool.PressButtonState.Clickable);
			break;
		case CompetitionModeJudgement.WhosTurn.WaitingPlayer1:
			ShowTakeTurn(currentTurn);
			button_Player1.UpdateButton(PressButtonTool.PressButtonState.Clickable);
			button_Player2.UpdateButton(PressButtonTool.PressButtonState.Disable);
			break;
		case CompetitionModeJudgement.WhosTurn.WaitingPlayer2:
			ShowTakeTurn(currentTurn);
			button_Player1.UpdateButton(PressButtonTool.PressButtonState.Disable);
			button_Player2.UpdateButton(PressButtonTool.PressButtonState.Clickable);
			break;
		case CompetitionModeJudgement.WhosTurn.Player1Playing:
			StartCoroutine(FadeOutInstruction());
			button_Player1.UpdateButton(PressButtonTool.PressButtonState.LightUp);
			button_Player2.UpdateButton(PressButtonTool.PressButtonState.Disable);
			break;
		case CompetitionModeJudgement.WhosTurn.Player2Playing:
			StartCoroutine(FadeOutInstruction());
			button_Player1.UpdateButton(PressButtonTool.PressButtonState.Disable);
			button_Player2.UpdateButton(PressButtonTool.PressButtonState.LightUp);
			break;
		}
	}

	public void ShowGameOver(int player1Score, int player2Score)
	{

	}

	void ShowTakeTurn(CompetitionModeJudgement.WhosTurn currentTurn)
	{
		group_Instruction.gameObject.SetActive(true);
		group_Instruction.alpha = 0f;
		group_Instruction.DOFade(1f, 0.3f);

		if(currentTurn == CompetitionModeJudgement.WhosTurn.WaitingPlayer1)
			text_TakeTurn.text = "Player1's Turn!";
		else if(currentTurn == CompetitionModeJudgement.WhosTurn.WaitingPlayer2)
			text_TakeTurn.text = "Player2's Turn!";

		text_TakeTurn.gameObject.SetActive(true);
		text_TakeTurn.rectTransform.localScale = Vector2.zero;
		text_TakeTurn.rectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
	}

	IEnumerator FadeOutInstruction()
	{
		group_Instruction.DOFade(0f, 0.3f).WaitForCompletion();
		yield return text_TakeTurn.rectTransform.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
		group_Instruction.gameObject.SetActive(false);
	}

	protected override IEnumerator HideUIAnimation()
	{
		yield return null;
	}

	protected override IEnumerator ShowUIAnimation()
	{
		yield return null;
	}
}
