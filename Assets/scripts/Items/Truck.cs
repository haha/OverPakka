using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Truck : Item {

	public GameObject fromPosition;
	public GameObject fromLookAt;
	public GameObject toCamera;
	public GameObject fromCamera;
	private bool interacted = false;


	public override bool CanInteract()
	{
		EContainerType t1;
		EContainerType2 t2;
		bool carryingItem = Level.player.PeekContainer(out t1, out t2);

		bool canInteract = carryingItem && t1 == EContainerType.Bag && t2 == EContainerType2.Bag;

		if (canInteract)
		{
			Camera.main.transform.DOMove(toCamera.transform.position, 0.6f, false).SetEase(Ease.InOutQuad);
			Camera.main.transform.DORotate(toCamera.transform.rotation.eulerAngles, 0.8f, RotateMode.Fast);
		}

		interacted = true;
		return canInteract;
	}

	public override Vector3 GetLookAt()
	{
		return !interacted ? base.GetLookAt() : fromLookAt.transform.position;
	}

	public override void Interact()
	{
		interacted = false;
		Debug.Log("Interact: threw bag in the truck");

		List<BagItem> bagItems;
		Level.player.PeekBag (out bagItems);
		foreach (var item in bagItems) {
			Debug.Log ("Item: " + item.t1.ToString() /* + " (" + item.t2.ToString() + ")" */ );
		}

        Level.gameController.UpdateScore(bagItems);
		Level.player.DropContainer();

		Camera.main.transform.DOMove(fromCamera.transform.position, 0.6f, false).SetEase(Ease.InOutQuad);
		Camera.main.transform.DORotate(fromCamera.transform.rotation.eulerAngles, 0.8f, RotateMode.Fast);
		Level.GetPlayer ().MoveTo (fromPosition.transform.position);


	}
}
