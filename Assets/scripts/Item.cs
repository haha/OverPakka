using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using UnityEngine;

/* Item
 * 
 */

public enum EItemType {
    NonClickable,
    Clickable
}

public struct BagItem
{
	public EContainerType t1;
	public EContainerType2 t2;
}

public enum EContainerType
{
	None,
	Bag,
	Banana,
	Tomato,
    Broccoli,
    Potato,
    Beef,
    Chicken
}   

public enum EContainerType2
{
	None,
	Fruit,
	Meat,
	Bag,
	Fruit_Ready,
	Fruit_Overwashed,
	Meat_Ready
}

public class Item : MonoBehaviour
{
    public EItemType itemType;
    public GameObject movePosition;
	public GameObject lookatPosition;

    void Awake()
    {
        gameObject.tag = "Item";

        if (itemType != EItemType.NonClickable)
        {
            foreach (Transform T in gameObject.GetComponentsInChildren<Transform>(false))
            {
                T.gameObject.tag = "Item";
            }
        }
    }

    public virtual void Interact()
    {
       // Debug.Log("Interact: not implemented for this item");
    }

	public virtual bool CanInteract() {
		return false;
	}

	public virtual Vector3 GetLookAt() {
		return lookatPosition.transform.position;
	}

}
