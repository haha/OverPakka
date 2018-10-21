
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class StartLevel : MonoBehaviour {

    public string levelToLoad;

	void Start () {
		SceneManager.LoadScene(levelToLoad);
	}
		
}