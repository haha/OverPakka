using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
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
    private Score score;

    public Text scoreText;

    public bool interact;
    public Item interact_item;

	private EContainerType currentContainerType;
	private EContainerType2 currentContainerType2;
    private GameObject currentContainer;
    private bool isCarryingContainer;

	void Start()
	{
		controller = new Controller(controllerType);
		navMeshAgent = GetComponent<NavMeshAgent>();
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		isCarryingContainer = false;
		Level.SetPlayer (this);
		interact = false;
        score = new Score(scoreText);
	}

	// reached navMesh destination?
    private bool ReachedDestination()
    {
        return !navMeshAgent.pathPending && 
			(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance &&
            (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f));
    }

	// pickup
	public void CarryContainer(EContainerType containerType, EContainerType2 containerType2, GameObject containerGameObject)
    {
        currentContainer = Instantiate(containerGameObject, transform);
        currentContainerType = containerType;
		currentContainerType2 = containerType2;
        isCarryingContainer = true;
    }

	// drop
    public void DropContainer()
    {
        Destroy(currentContainer);
        isCarryingContainer = false;
        score.SetScore(-10);
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
		//canvas.gameObject.SetActive (false);
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
			//prefab.transform.LookAt (destination);
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
	        Vector3 rot = interact_item.GetLookAt();
	        rot.y = transform.position.y;

	        navMeshAgent.updateRotation = false;

	        Quaternion temp_rot = transform.rotation;

	        transform.LookAt(rot);
	        interact_item.Interact();
	        transform.rotation = temp_rot;

	        transform.DOLookAt(rot, 0.1f);

	        interact = false;
	    }
	}

    private bool IsItemPressed(RaycastHit hit, out Vector3 point)
    {   
        point = new Vector3();

        if (hit.transform.gameObject.tag == "Item")
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
