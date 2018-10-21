using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerScript : MonoBehaviour {

    public GameObject hake;
    public GameObject meny;

	// Use this for initialization
	void Start () {
        StartCoroutine(StartTime());
	}


    IEnumerator StartTime()
    {
        hake.SetActive(false);
        yield return new WaitForSeconds(5);
        GameObject.Find("Produce").SetActive(false);
        hake.SetActive(true);
        yield return new WaitForSeconds(2);
        GameObject.Find("Loading").SetActive(false);
        meny.SetActive(true);
    }
}
