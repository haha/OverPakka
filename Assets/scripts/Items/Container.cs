using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Item
{
    public EContainerType containerType;
	public EContainerType2 containerType2;
    public GameObject prefabRaw;
    public GameObject prefabReady;
    public GameObject prefabDestroyed;

    public override void Interact()
    {
        EContainerType container;
		EContainerType2 container2;
		if (Level.GetPlayer().PeekContainer(out container, out container2)) // player holding something?
        {
            if (container == containerType) // player holding same as this container?
            {
                Debug.Log("Interact: dropped " + containerType.ToString());
                Level.GetPlayer().DropContainer(); // drop it
            }
        }
        else // player NOT holding anything?
        {
            Debug.Log("Interact: picked up " + containerType.ToString());
            // give this
	        prefabRaw.transform.position =
		        Level.player.holdPosition.transform.localPosition;
            if (containerType2 == EContainerType2.Bag) 
                Level.GetPlayer().CarryBag(new List<BagItem>(), prefabRaw, this);
            else
                Level.GetPlayer().CarryContainer(containerType, containerType2, prefabRaw, this); 
        }
    }

	public override bool CanInteract() {
	    EContainerType container;
		EContainerType2 container2;
		if (!Level.GetPlayer().PeekContainer(out container, out container2)) return true;
	    return container == containerType;
	}

    public override Vector3 GetLookAt() {
		return base.GetLookAt ();
	}
}
