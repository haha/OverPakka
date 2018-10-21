using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score {
    int score;
    Text scoreText;

    public Score() {
        score = 0;
        scoreText = null;
    }

    public void SetScoreUI(Text txt) {
        scoreText = txt;
        scoreText.text = score.ToString();
    }

    public void SetScore(int amount) {
        score += amount;
        if (score < 0) score = 0;
        scoreText.text = score.ToString();
    }
    public int GetScore() {
        return score;
    }
    public int GetResult(int min, int max, float difficulty) {
        int stars = 0;
        int difference = max - min;
        if(score >= difference * difficulty) {
            stars = 3;
        } else if(score >= difference * (difficulty / 2)) {
            stars = 2;
        } else if(score >= min) {
            stars = 1;
        } else {
            stars = 0;
        }
        return stars;
    }
}
