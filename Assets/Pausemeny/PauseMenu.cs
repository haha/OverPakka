using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

	public GameObject pauseMenu;

	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (Time.timeScale == 1)
			{
				Time.timeScale = 0;
				pauseMenu.SetActive (true);
			}
			else
			{
				Time.timeScale = 1;
				pauseMenu.SetActive (false);
			}
		}


	}
}
