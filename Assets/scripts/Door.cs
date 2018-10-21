using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour {
	public ERoom room;
	public GameObject toPosition;
	public GameObject fromPosition;
    public GameObject toCamera;
    public GameObject fromCamera;

    void Awake()
    {
        gameObject.tag = "Door";
        /*
        foreach (Transform T in gameObject.GetComponentsInChildren<Transform>(false))
        {
            T.gameObject.tag = "Door";
        }
        */
    }

    public void PressDoor()
	{
		if (Level.currentRoom == this.room) {
            Level.SetRoom(ERoom.Workspace);
			Level.GetPlayer ().MoveTo (fromPosition.transform.position);
		    //Camera.main.transform.position = fromCamera.transform.position;
		    Camera.main.transform.DOMove(fromCamera.transform.position, 0.5f, false).SetEase(Ease.InOutQuad);
		    Camera.main.transform.DORotate(fromCamera.transform.rotation.eulerAngles, 0.6f, RotateMode.Fast);
		} else {
		    Level.SetRoom(room);
            Level.GetPlayer ().MoveTo (toPosition.transform.position);
            //Camera.main.transform.position = toCamera.transform.position;
		    Camera.main.transform.DOMove(toCamera.transform.position, 0.5f, false).SetEase(Ease.InOutQuad);
		    Camera.main.transform.DORotate(toCamera.transform.rotation.eulerAngles, 0.6f, RotateMode.Fast);
        }
	}
}
