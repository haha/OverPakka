using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order {

    string name;
    float timer;
    public List<BagItem> items = new List<BagItem>();

    bool isActive = false;

    public Order(string name, float timer, List<BagItem> items) {
        this.name = name;
        this.timer = timer;
        this.items = items;
    }

    public bool GetIfActive() {
        return isActive;
    }
    public void SetActive() {
        isActive = true;
    }

    public string GetName() {
        return name;
    }

    public float GetTimer() {
        return timer;
    }
    public void SetTimer() {
        if (isActive) {
            timer -= Time.deltaTime;
            if (timer < 0) {
                isActive = false;
            }
        }
    }

    public int CompareDelivery(List<BagItem> list) {
        int count = 0;
        if (items == null || items.Count <= 0) return count;
        
        foreach(BagItem item in list) {
            foreach(BagItem orderItem in items) {
                if(item.Equals(orderItem)) {
                    count++;
                    break;
                }
            }

        }

        return count;
    }
}

