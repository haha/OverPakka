using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class GameController : MonoBehaviour {

    List<Order> activeOrders = new List<Order>();
    Score score = new Score();
    Order currentOrder = null;

    int index = 0;
    int minScore = 0;
    int maxScore = 0;

    public float timeLeft = 120f;
    public int pointsPerItem = 100;
    public int pointsPerTime = 3;

    public Text scoreText;
    public Text timeText;

    public PostProcessingProfile postProfile;
    public Transform endPanel;
    public Transform activePanel;
    public GameObject orderPanelPrefab;
    public GameObject UI;
    public GameObject cam;

    //sprites
    public Sprite BananaSprite;
    public Sprite TomatoSprite;
    public Sprite BroccoliSprite;
    public Sprite PotatoSprite;
    public Sprite BeefSprite;
    public Sprite ChickenSprite;

    // Use this for initialization
    void Start () {
        Level.SetGameController(this);
        score.SetScoreUI(scoreText);
        SetActiveOrders(Level.orders, 3);
        SetMinMax();
        Time.timeScale = 1;
    }
	
	// Update is called once per frame
	void Update () {
        if (activeOrders.Count == 0) { 
            if (index <= Level.orders.Count - 1) {
                SetActiveOrders(Level.orders, 1);
            } else {
                GameOver();
            }
        }
        if(timeLeft <= 0) { 
            GameOver();
        }

        SetTime();
        timeText.text = timeLeft.ToString("F0");
        foreach (Order o in activeOrders) {
            o.SetTimer();
        }
    }

    //todo
    public void CurrentInfo() {
        string info = currentOrder.GetName() + "\n";
        foreach (BagItem item in currentOrder.items) {
            info += item.t1 + "\n";
        }
    }
   
    public void SetActiveOrders(List<Order> orders, int amount) {
        activeOrders = new List<Order>();
        if (orders.Count < amount) amount = orders.Count;
        for (int i = 0; i < amount; i++) {
            activeOrders.Add(orders[index]);
            activeOrders[i].SetActive();
            
            GameObject newPanel = GameObject.Instantiate(orderPanelPrefab);
            newPanel.transform.SetParent(activePanel);

            Text newText = newPanel.transform.GetChild(0).gameObject.GetComponent<Text>();
            newText.text = activeOrders[i].GetName();
            
            foreach(BagItem item in activeOrders[i].items) {
                GameObject itm = new GameObject();
                itm.AddComponent<RectTransform>().sizeDelta = new Vector2(30, 30);
                itm.transform.SetParent(newPanel.transform.GetChild(1));
                Image img = itm.AddComponent<Image>();
                switch (item.t1) {
                    case EContainerType.Banana:
                        img.sprite = BananaSprite;
                        break;
                    case EContainerType.Tomato:
                        img.sprite = TomatoSprite;
                        break;
                    case EContainerType.Potato:
                        img.sprite = PotatoSprite;
                        break;
                    case EContainerType.Broccoli:
                        img.sprite = BroccoliSprite;
                        break;
                    case EContainerType.Beef:
                        img.sprite = BeefSprite;
                        break;
                    case EContainerType.Chicken:
                        img.sprite = ChickenSprite;
                        break;
                }
            }
            index++;
        }
    }

    public void UpdateScore(List<BagItem> bagItems) {
        int bestIndex = -1;
        int best = 0;
        for (int i = 0; i < activeOrders.Count; i++) {
            if (activeOrders[i].items != null) {
                int matches = activeOrders[i].CompareDelivery(bagItems);
                if (matches > best) { 
                    best = matches;
                    bestIndex = i;
                }
            }
        }
        if (bestIndex >= 0 && best == (activeOrders[bestIndex].items.Count)) {
            int s = best * pointsPerItem + (int)activeOrders[bestIndex].GetTimer() * pointsPerTime;
            Debug.Log("Bag items matches with an order for: " + s + "points!");
            score.SetScore(s);
            activeOrders.RemoveAt(bestIndex);
            Destroy(activePanel.transform.GetChild(bestIndex).gameObject);
        } else {
            Debug.Log("No matches, bag thrown away");
        }
    }
    
    public Score GetScore() {
        return score;
    }

    void SetMinMax() {
        foreach(Order o in Level.orders) {
            minScore += o.items.Count * pointsPerItem;
            maxScore += o.items.Count * pointsPerItem + (int)o.GetTimer() * pointsPerTime;
        }
    }

    void SetTime() {
        timeLeft -= Time.deltaTime;
    }
    void GameOver() {
        endPanel.gameObject.SetActive(true);
        UI.gameObject.SetActive(false);
        Text endText = endPanel.transform.GetChild(1).GetComponent<Text>();
        endText.text = score.GetResult(minScore, maxScore, 0.8f).ToString();
        Time.timeScale = 0;

        cam.GetComponent<PostProcessingBehaviour>().profile = postProfile;
    }
}