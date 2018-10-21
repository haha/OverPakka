using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.UIElements;


public enum ECharacter
{
	CHARACTER_ONE, CHARACTER_TWO, CHARACTER_THREE
}

[System.Serializable] 
public class CharacterDefinition {
	public ECharacter character;
	public GameObject prefab;
	public float speed;
	public CharacterDefinition(ECharacter character, float speed) {
		this.character = character;
		this.speed = speed;
	}
}

public class Player : MonoBehaviour {
	public EControllerType controllerType;

	public ECharacter selectedCharacter;
	public CharacterDefinition[] characters = new CharacterDefinition[3]{
		new CharacterDefinition(character: ECharacter.CHARACTER_ONE,    speed: 10),
		new CharacterDefinition(character: ECharacter.CHARACTER_TWO,    speed: 20),
		new CharacterDefinition(character: ECharacter.CHARACTER_THREE,  speed: 30)
	};

	private GameObject prefab;
	private NavMeshAgent navMeshAgent;
	private Controller controller;
	private ECharacter lastCharacter;
	private Canvas canvas;

    public GameObject holdPosition;
    

    public bool interact;
    public Item interact_item;

    public Container container;

	private EContainerType currentContainerType;
	private EContainerType2 currentContainerType2;
    private GameObject currentContainer;
    private bool isCarryingContainer;
    private bool hasBag = false;
    private List<BagItem> bagItems;

	void Start()
	{
		controller = new Controller(controllerType);
		navMeshAgent = GetComponent<NavMeshAgent>();
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		isCarryingContainer = false;
		Level.SetPlayer (this);
		interact = false;
	}

	// reached navMesh destination?
    private bool ReachedDestination()
    {
        return !navMeshAgent.pathPending && 
			(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance &&
            (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f));
    }

    // pickup bag
    public void CarryBag(List<BagItem> _bagItems, GameObject containerGameObject, Container c)
    {
        container = c;
        bagItems = new List<BagItem>(_bagItems);
        currentContainer = Instantiate(containerGameObject, transform);
        currentContainerType = EContainerType.Bag;
        currentContainerType2 = EContainerType2.Bag;
        isCarryingContainer = true;
        hasBag = true; 
    }

    // peek bag
    public bool PeekBag(out List<BagItem> _bagItems)
    {
        _bagItems = new List<BagItem>(bagItems);
        return hasBag;
    }

	// pickup
	public void CarryContainer(EContainerType containerType, EContainerType2 containerType2, GameObject containerGameObject, Container c)
	{
	    container = c;
        currentContainer = Instantiate(containerGameObject, transform);
        currentContainerType = containerType;
		currentContainerType2 = containerType2;
        isCarryingContainer = true;
        hasBag = false;
    }

	// drop
    public void DropContainer()
    {
        Destroy(currentContainer);

        isCarryingContainer = false;
        if (hasBag) bagItems.Clear();
        hasBag = false;
    }

	// what is the player holding?
	public bool PeekContainer(out EContainerType containerType, out EContainerType2 containerType2)
    {
        containerType = currentContainerType;
		containerType2 = currentContainerType2;
        return isCarryingContainer;
    }

    // called when character select screen clicked
	public void SetCharacter(int character) {
		SetCharacter ((ECharacter)character);
	}

	public void SetCharacter(ECharacter character){
		selectedCharacter = character;

		navMeshAgent.speed = characters[(int)character].speed;
		navMeshAgent.angularSpeed = characters[(int)character].speed*40;
		navMeshAgent.acceleration = characters[(int)character].speed*40;

		Destroy (prefab);
		prefab = Instantiate(characters[(int)character].prefab, gameObject.transform.position, Quaternion.identity);
		prefab.transform.parent = gameObject.transform;
		prefab.transform.localScale *= 4;
		prefab.transform.Translate (new Vector3(0,-1,0));
		prefab.transform.Rotate(new Vector3(-90,0,0));
	}
		
	public void MoveTo(Vector3 destination)
	{
        /* // test characters
		if (selectedCharacter != lastCharacter) {
			SetCharacter (selectedCharacter);
			lastCharacter = selectedCharacter;
			//prefabRaw.transform.LookAt (destination);
		}
        */
		navMeshAgent.destination = destination;
	}

	void Update()
	{
	    RaycastHit hit;

	    if (controller.IsScreenPressed(EButton.LEFT_BUTTON, out hit))
	    {
	        Vector3 point;
	        Door door;
            if (IsItemPressed (hit, out point))
            {
				MoveTo (point);
			} else 
            if (IsDoorPressed(hit, out door))
	        {
				navMeshAgent.updateRotation = true;
                if (door) door.PressDoor();
	        }
	    }

	    if (interact && ReachedDestination())
	    {
	        interact = false;
            Vector3 rot = interact_item.GetLookAt();
	        rot.y = transform.position.y;

	        navMeshAgent.updateRotation = false;

	        Quaternion temp_rot = transform.rotation;

	        transform.LookAt(rot);
	        interact_item.Interact();
	        transform.rotation = temp_rot;

	        transform.DOLookAt(rot, 0.1f);

	    }
	}


    private bool IsItemPressed(RaycastHit hit, out Vector3 point)
    {   
        point = new Vector3();

        if (hit.transform.gameObject.CompareTag("Item"))
        {
            Item item = hit.transform.gameObject.GetComponent<Item>() ?? hit.transform.gameObject.GetComponentInParent<Item>();

            if (item == null)
            {
                Debug.Log("cannot find item!");
            }
            else if (item.itemType != EItemType.NonClickable)
            {
                Room room = hit.transform.gameObject.GetComponent<Room>() ?? hit.transform.gameObject.GetComponentInParent<Room>();
                if (room != null && room.room == Level.currentRoom)
                {
                    point = item.movePosition == null ? hit.point : item.movePosition.transform.position;
                    //Controller.FocusCameraOnGameObject(Camera.main, hit.transform.gameObject);
					if (item.CanInteract ()) {
						interact = true;
						interact_item = item;
                       
					    //if (item is Container) container = item as Container;
                        //    else Debug.Log("ERROR isItemPresed, not Container");

                        navMeshAgent.updateRotation = true;
					} else {
						navMeshAgent.updateRotation = true;
						interact = false;
					}
                    return true;
                }
            }
        }

        return false;
    }

	private bool IsDoorPressed(RaycastHit hit, out Door outDoor)
	{   
		//outDoor = new Door ();
	    outDoor = null;

		if (hit.transform.gameObject.tag == "Door")
		{
			Door door = hit.transform.gameObject.GetComponent<Door>() ?? hit.transform.gameObject.GetComponentInParent<Door>();

			if (door == null)
			{
				Debug.Log("cannot find door!");
			}
			else 
			{
				outDoor = door;
				interact = false;
				return true;
			}
		}

		return false;
	}
}
