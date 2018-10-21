using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Transform endPanel;
    public Transform activePanel;
    public GameObject buttonPrefab;

    // Use this for initialization
    void Start () {
        Level.SetGameController(this);
        //score.SetScoreUI(scoreText);
        SetActiveOrders(Level.orders, 1);
        SetMinMax();
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
        scoreText.text = timeLeft.ToString();
        foreach (Order o in activeOrders) {
            o.SetTimer();
        }
    }

    //todo
    public void CurrentInfo() {
       // Text t = EventSystem.current.currentSelectedGameObject.GetChild(0).gameObject.GetComponent<Text>();
        string info = currentOrder.name + "\n";
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
            
            GameObject newButton = GameObject.Instantiate(buttonPrefab);
            newButton.transform.SetParent(activePanel);

            Button btn = newButton.GetComponent<Button>();

            Text newText = newButton.transform.GetChild(0).gameObject.GetComponent<Text>();
            newText.text = activeOrders[i].GetName();

            index++;
        }
    }

    public void UpdateScore(List<BagItem> bagItems) {
        int bestIndex = -1;
        int best = 0;
        for (int i = 0; i < activeOrders.Count; i++) {
            if (activeOrders[i].items != null) {
                int matches = activeOrders[i].compareDelivery(bagItems);
                
                if (matches > best) { 
                    best = matches;
                    bestIndex = i;
                }
            }
        }
        if (bestIndex >= 0 && best == (activeOrders[bestIndex].items.Count)) { // hva om de har ekstra items i posen?
            int s = best * pointsPerItem + (int)activeOrders[bestIndex].GetTimer() * pointsPerTime;
            Debug.Log("Bag items matches with an order for: " + s + "points!");
            score.SetScore(s);
            activeOrders.RemoveAt(bestIndex);
            Destroy(activePanel.transform.GetChild(bestIndex).gameObject);
        } else {
            Debug.Log("No matches, bag thrown away");
            Debug.Log(3 / 2);
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
        Text endText = endPanel.transform.GetChild(1).GetComponent<Text>();
        endText.text = score.GetResult(minScore, maxScore, 0.8f).ToString();
    }
}
