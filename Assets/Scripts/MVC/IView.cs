using UnityEngine;
using System.Collections;

public interface IView  {
	IEnumerator Init();

	void ShowUI(bool needAnimation);

	void HideUI(bool needAnimation);
}
