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

    public float hintTimer;
    public readonly string REPOSITION_STRING = "Reposition the board by touching somewhere on a detected plane!";
    public readonly string CREWMAN_HINT = "Touch a treasure chest to have your crewman go get it!";
    public readonly string HELMSMAN_HINT = "While the helmsman is selected, the ship will move. Steer by tilting your phone!";

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

    public void DisplayCrewmanHint() {
        hintText.text = CREWMAN_HINT;
        hintText.gameObject.SetActive(true);
        Invoke("HideHint", hintTimer);
    }

    public void DisplayHelmsmanHint() {
        hintText.text = HELMSMAN_HINT;
        hintText.gameObject.SetActive(true);
        Invoke("HideHint", hintTimer);
    }

    public void ToggleRepositionButton() {
        hintText.text = REPOSITION_STRING;
        CancelInvoke("HideHint"); //Make sure the hint isn't hidden by previous character selects.
        reposition = !reposition;
        hintText.gameObject.SetActive(reposition);
        if (reposition) {
            repositionButtonImage.sprite = cancelIcon;
        } else {
            repositionButtonImage.sprite = repositionIcon;
        }
    }

    private void HideHint() {
        hintText.gameObject.SetActive(false);
    }
}
