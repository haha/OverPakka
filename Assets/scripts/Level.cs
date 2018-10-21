using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ERoom {
	Workspace, Garage, FruitStorage, MeatStorage
}
	

public static class Level {
	public static ERoom currentRoom;
	public static Player player;
    public static GameController gameController;


    //Liste over alle bestillinger som kan forekomme
    public static List<Order> orders = new List<Order>()
    {
        new Order("Kyllingsalat", 45, new List<BagItem>()
        {
            new BagItem {t1 = EContainerType.Chicken, t2 = EContainerType2.Meat_Ready},
            new BagItem {t1 = EContainerType.Broccoli, t2 = EContainerType2.Fruit_Ready}, // må fixes
            new BagItem {t1 = EContainerType.Tomato, t2 = EContainerType2.Fruit_Ready}
        }),
        new Order("Svinestek", 45, new List<BagItem>()
        {
            new BagItem {t1 = EContainerType.Beef, t2 = EContainerType2.Meat_Ready},
            new BagItem {t1 = EContainerType.Potato, t2 = EContainerType2.Fruit_Ready},
            new BagItem {t1 = EContainerType.Tomato, t2 = EContainerType2.Fruit_Ready},
            new BagItem {t1 = EContainerType.Broccoli, t2 = EContainerType2.Fruit_Ready}
        }),
        new Order("Bananklase", 45, new List<BagItem>()
        {
            new BagItem {t1 = EContainerType.Banana, t2 = EContainerType2.Fruit_Ready},
            new BagItem {t1 = EContainerType.Banana, t2 = EContainerType2.Fruit_Ready},
            new BagItem {t1 = EContainerType.Banana, t2 = EContainerType2.Fruit_Ready},
            new BagItem {t1 = EContainerType.Banana, t2 = EContainerType2.Fruit_Ready}
        })
    };

    public static void SetRoom(ERoom room){
		currentRoom = room;
	}

	public static void SetPlayer(Player _player) {
		Level.player = _player;  
	}

	public static Player GetPlayer() {
		return player;
	}

    public static void SetGameController(GameController controller) {
        gameController = controller;
    }

    public static Bounds CalculateBounds(GameObject go)
	{
		Bounds b = new Bounds(go.transform.position, Vector3.zero);
		foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
		{
			b.Encapsulate(r.bounds);
		}
		return b;
	}
}
