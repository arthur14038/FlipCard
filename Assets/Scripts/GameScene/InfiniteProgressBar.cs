using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InfiniteProgressBar : MonoBehaviour {
	public RectTransform image_Icon;
	public RectTransform[] image_CircleGreen;
	public RectTransform[] image_CircleOrange;
	public Image[] image_Line_Orange;
	
	public void SetProgress(int value, bool needTween = false)
	{
		if(value > 0 && needTween)
		{
			SetProgressBar(value - 1);
			Vector2 destination = image_CircleGreen[value].anchoredPosition;
			image_Icon.DOAnchorPos(destination, 0.5f);
			image_Line_Orange[value - 1].fillAmount = 0f;
			image_Line_Orange[value - 1].gameObject.SetActive(true);
			image_Line_Orange[value - 1].DOFillAmount(1f, 0.5f);
		} else
		{
			SetProgressBar(value);
        }
	}

	void SetProgressBar(int value)
	{
        image_Icon.anchoredPosition = image_CircleGreen[value].anchoredPosition;
		for(int i = 0 ; i < image_CircleGreen.Length ; ++i)
		{
			if(i > value)
				image_CircleGreen[i].gameObject.SetActive(true);
			else
				image_CircleGreen[i].gameObject.SetActive(false);
		}

		for(int i = 0 ; i < image_CircleOrange.Length ; ++i)
		{
			if(i > value)
				image_CircleOrange[i].gameObject.SetActive(false);
			else
				image_CircleOrange[i].gameObject.SetActive(true);
		}

		for(int i = 0 ; i < image_Line_Orange.Length ; ++i)
		{
			if(i < value)
				image_Line_Orange[i].gameObject.SetActive(true);
			else
				image_Line_Orange[i].gameObject.SetActive(false);
		}
	}
}
