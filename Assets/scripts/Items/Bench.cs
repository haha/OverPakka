using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bench : Item
{
    public GameObject placePosition;
    public GameObject item1;
    public GameObject item2;
    public GameObject item3;
    public GameObject item4;
    public GameObject item5;

    public Sprite BananaSprite;
    public Sprite TomatoSprite;
    public Sprite BroccoliSprite;
    public Sprite PotatoSprite;
    public Sprite BeefSprite;
    public Sprite ChickenSprite;

    private bool hasItem = false;
    private bool hasBag = false;
    private GameObject placeItem;
    private List<BagItem> bagItems = new List<BagItem>();
    private EContainerType placeItemType;
    private EContainerType2 placeItemType2;

    private bool canPlaceDown = false;
    private bool canPickUp = false;

    private Container container;


    public override bool CanInteract()
    {
        EContainerType t1;
        EContainerType2 t2;
        bool carryingItem = Level.player.PeekContainer(out t1, out t2);
        canPlaceDown = (carryingItem && !hasItem) ||
                       (carryingItem && hasItem &&
                        placeItemType == EContainerType.Bag);
        canPickUp = (!carryingItem && hasItem);
        return canPickUp || canPlaceDown;
    }

    public override void Interact()
    {
        EContainerType t1;
        EContainerType2 t2;
        bool carryingItem = Level.player.PeekContainer(out t1, out t2);

        if (canPlaceDown && !hasItem)
        {
            Debug.Log("Interact: placed down " + t1.ToString());
            hasItem = true;
            placeItemType = t1;
            placeItemType2 = t2;
            hasBag = (t1 == EContainerType.Bag && t2 == EContainerType2.Bag);
            container = Level.player.container;
            if (hasBag)
            {
                Level.player.PeekBag(out bagItems);
            }

            bool ready = (t2 == EContainerType2.Fruit_Ready ||
                          t2 == EContainerType2.Meat_Ready);
            bool destroyed = (t2 == EContainerType2.Fruit_Overwashed);
            GameObject go;
            if (!ready && !destroyed)
                go = Level.player.container.prefabRaw;
            else
                go = ready
                    ? Level.player.container.prefabReady
                    : Level.player.container.prefabDestroyed;

            placeItem = Instantiate(go, placePosition.transform.position,
                placePosition.transform.rotation);
            Level.player.DropContainer();
        }
        else if ((canPlaceDown && hasItem && hasBag))
        {
            if ((t2 == EContainerType2.Fruit_Ready ||
                 t2 == EContainerType2.Meat_Ready))
            {
                if (bagItems.Count >= 5)
                {
                    Debug.Log("Bag full");
                }
                else
                {
                    Debug.Log("Interact: placed " + t1.ToString() + " into Bag");
                    BagItem bag = new BagItem
                    {
                        t1 = t1,
                        t2 = t2
                    };
                    bagItems.Add(bag);
                    Level.player.DropContainer();
                }
            }
            else
            {
                Debug.Log(
                    "Interact: wash the fruit before placing into Bag.");
            }
        }

        if (canPickUp)
        {
            Debug.Log("Interact: picked up " + placeItemType.ToString());
            placeItem.transform.position =
                Level.player.holdPosition.transform.localPosition;
            //placeItem.transform.position = new Vector3(Level.player.holdPosition.transform.position.x, Level.player.holdPosition.transform.position.y, Level.player.holdPosition.transform.position.z);
            if (hasBag)
                Level.player.CarryBag(bagItems, placeItem, container);
            else
                Level.player.CarryContainer(placeItemType, placeItemType2,
                    placeItem, container);

            hasItem = false;
            bagItems.Clear();
            Destroy(placeItem);
        }

        RenderBagItems();
    }

    private void RenderBagItems()
    {
        item1.SetActive(false);
        item2.SetActive(false);
        item3.SetActive(false);
        item4.SetActive(false);
        item5.SetActive(false);
        for (int i = 0; i < bagItems.Count; i++)
        {
            GameObject item = item1;

            switch (i)
            {
                case 0:
                    item = item1;
                    break;
                case 1:
                    item = item2;
                    break;
                case 2:
                    item = item3;
                    break;
                case 3:
                    item = item4;
                    break;
                case 4:
                    item = item5;
                    break;
            }

            Sprite sprite = BananaSprite;
            switch (bagItems[i].t1)
            {
                case EContainerType.None:
                    sprite = BananaSprite;
                    break;
                case EContainerType.Bag:
                    sprite = BananaSprite;
                    break;
                case EContainerType.Banana:
                    sprite = BananaSprite;
                    break;
                case EContainerType.Tomato:
                    sprite = TomatoSprite;
                    break;
                case EContainerType.Broccoli:
                    sprite = BroccoliSprite;
                    break;
                case EContainerType.Potato:
                    sprite = PotatoSprite;
                    break;
                case EContainerType.Beef:
                    sprite = BeefSprite;
                    break;
                case EContainerType.Chicken:
                    sprite = ChickenSprite;
                    break;
            }


            SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
                item.transform.forward = Camera.main.transform.forward;
                item.SetActive(true);
            }
        }
    }

    public override Vector3 GetLookAt()
    {
        return base.GetLookAt();
    }
}