using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour {
    public Image repositionButtonImage;

    public Sprite repositionIcon;
    public Sprite cancelIcon;

    public Text hintText;

    public Text scoreText;
    private int winCondition;

    private bool reposition = false;

    public void SetWindCondition(int newWinCondition) {
        Debug.Log("New win condition: " + newWinCondition);
        winCondition = newWinCondition;
        scoreText.text = "Score: 0 / " + winCondition;
    }

    public void UpdateScoreText(int newScore) {
        scoreText.text = "Score: " + newScore + " / " + winCondition;
    }

    public void ToggleRepositionButton() {
        reposition = !reposition;
        hintText.gameObject.SetActive(reposition);
        if (reposition) {
            repositionButtonImage.sprite = cancelIcon;
        } else {
            repositionButtonImage.sprite = repositionIcon;
        }
    }
}
