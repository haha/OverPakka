using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : Item {
	
	public override bool CanInteract()
	{
		EContainerType t1;
		EContainerType2 t2;
		bool carryingItem = Level.player.PeekContainer(out t1, out t2);
		return carryingItem;
	}

	public override Vector3 GetLookAt()
	{
		return base.GetLookAt();
	}

	public override void Interact()
	{
		Debug.Log("Interact: threw item in the truck");

		List<BagItem> bagItems;
		Level.player.PeekBag (out bagItems);
		foreach (var item in bagItems) {
			Debug.Log ("Item: " + item.t1.ToString() + " (" + item.t2.ToString() + ")");
		}
		Level.player.DropContainer();
	}
}
