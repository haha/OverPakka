using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Sink : Item
{
    public GameObject timer;
    public GameObject timerMask;
    public GameObject alert;

    public float washTime = 5f; 
	public float alertTime = 5f;
	private float alertTimeLeft = 0;
    private bool washing = false;
    private bool hasItem = false;
    private bool timerDone = true;
    private bool alertDone = true;

	private EContainerType itemType1 = EContainerType.None;
    private EContainerType2 itemType2 = EContainerType2.None;
    private Container container;

    public override bool CanInteract()
    {
		EContainerType t1;
		EContainerType2 t2;
		bool carryingItem = Level.player.PeekContainer (out t1, out t2);
        return (hasItem && !carryingItem && timerDone) || (!hasItem && carryingItem && t2 == EContainerType2.Fruit);

        //(carryingItem && t2==EContainerType2.Fruit && !hasItem) || (hasItem && timerDone && !carryingItem);
    }

    public override Vector3 GetLookAt()
    {
        return base.GetLookAt();
    }

    public override void Interact()
    {
        if (!hasItem)
        {
            EContainerType t1;
            EContainerType2 t2;
            bool carryingItem = Level.player.PeekContainer(out t1, out t2);
            Debug.Log("Interact: put " + t1.ToString() + " in the sink");
            if (!carryingItem) Debug.Log("ERROR: PLAYER NOT CARRYING ITEM. Sink.Interact()");   
            container = Level.player.container;
            itemType1 = t1;
            itemType2 = t2;
            Level.player.DropContainer();

            washing = true;
            hasItem = true;

            timer.SetActive(true);
            timer.transform.forward = Camera.main.transform.forward;
            alert.transform.forward = Camera.main.transform.forward;

            timerDone = false;
			timerMask.transform.DOLocalMoveX(timerMask.transform.localPosition.x - 5f, washTime)
				.From()
				.SetEase(Ease.Linear)
                .OnComplete(OnTimerDone);
        }
        else
        {
            hasItem = false;
            washing = false;

            Vector3 rot = Level.player.interact_item.GetLookAt();
            rot.y = transform.position.y;
            Quaternion temp_rot = Level.player.transform.rotation;
            Level.player.transform.LookAt(rot);
            Debug.Log("Interact: " + itemType1 + " is " + (alertDone?"overwashed" : "ready") );
            itemType2 = alertDone ? EContainerType2.Fruit_Overwashed : EContainerType2.Fruit_Ready;
            Level.player.CarryContainer(itemType1, itemType2, 
                alertDone ? container.prefabDestroyed : container.prefabReady, container);
            
            Level.player.transform.rotation = temp_rot;
            Level.player.transform.DOLookAt(rot, 0.1f);

            alertDone = true;
            alert.SetActive(false);
        }
    }

    private void OnTimerDone()
    {
        timer.SetActive(false);
        if (hasItem)
        {
            alertDone = false;
            alert.SetActive(true); 
			alertTimeLeft = alertTime;
            //alert.transform.DOMove(alert.transform.position, 3f).OnComplete(OnAlertDone);
        }
        timerDone = true;
    }
/*
    private void OnAlertDone()
    {
        if (hasItem) Debug.Log("Overwashed");
        alert.SetActive(false);
        alertDone = true;
    }
*/
    void Start()
    {
        timer.SetActive(false);
    }

    void FixedUpdate()
    {
		if (alertDone == false) {
			alertTimeLeft -= Time.fixedDeltaTime;
			if (alertTimeLeft <= 0) {
				alertDone = true;
				alert.SetActive (false);
				Debug.Log("Overwashed");
			}
		}
        if (washing && timer)
        {
            //timer.transform.LookAt(Camera.main.transform.position, -Vector3.up);
            timer.transform.forward = Camera.main.transform.forward;

        }
    }
}
