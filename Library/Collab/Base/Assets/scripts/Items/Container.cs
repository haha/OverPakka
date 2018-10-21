using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EContainerType
{
    None,
    Banana,
    Tomato
}   

public enum EContainerType2
{
	None,
	Fruit,
    Fruit_Ready,
    Fruit_Overwashed,
	Meat,
    Meat_Ready
}

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
            Level.GetPlayer().CarryContainer(containerType, containerType2, prefabRaw); // give this
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
