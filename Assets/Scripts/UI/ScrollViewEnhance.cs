using UnityEngine;
using UnityEngine.UI;

public class ScrollViewEnhance : MonoBehaviour
{
	public ScrollRect scrollRect;
	public float gap;
	CanvasGroup[] cells;

	void Start()
	{
		cells = scrollRect.content.GetComponentsInChildren<CanvasGroup>();
    }

	void Update()
	{
		float x = -scrollRect.content.anchoredPosition.x;
		for(int i = 0 ; i < cells.Length ; ++i)
		{
			float a = 1f - Mathf.Abs(cells[i].gameObject.transform.localPosition.x - x)/(gap*1.5f);
			if(a > 0.95f)
				a = 1f;
			if(a < 0f)
				a = 0f;
			cells[i].alpha = a;
        }
	}
}
