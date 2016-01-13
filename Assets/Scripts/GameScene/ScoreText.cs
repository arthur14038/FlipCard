using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreText : MonoBehaviour {
	public Sprite[] scoreSprite;
	Image thisImage;
    VoidScoreText recycle;

    public void Init(VoidScoreText recycle)
    {
        this.gameObject.SetActive(false);
        this.recycle = recycle;
		thisImage = this.GetComponent<Image>();
    }

    public void ShowScoreText(int score, Vector2 pos)
    {
        if (!this.gameObject.activeSelf)
            this.gameObject.SetActive(true);
		thisImage.rectTransform.anchoredPosition = pos;
		thisImage.color = Color.white;
		
		switch(score)
		{
			case -2:
				thisImage.sprite = scoreSprite[0];
                break;
			case -1:
				thisImage.sprite = scoreSprite[1];
				break;
			case 1:
				thisImage.sprite = scoreSprite[2];
				break;
			case 2:
				thisImage.sprite = scoreSprite[3];
				break;
			case 4:
				thisImage.sprite = scoreSprite[4];
				break;
			case 6:
				thisImage.sprite = scoreSprite[5];
				break;
			case 8:
				thisImage.sprite = scoreSprite[6];
				break;
			default:
				Debug.LogErrorFormat("Score {0} need to be handled.", score);
				break;
		}

		thisImage.rectTransform.DOAnchorPosY(pos.y + 50f, 0.8f).SetEase(Ease.OutQuad);
		thisImage.DOFade(0f, 0.2f).SetDelay(0.6f).OnComplete(
            delegate () {
                this.gameObject.SetActive(false);
                if (recycle != null)
                    recycle(this);
            }
        );
    }
}
