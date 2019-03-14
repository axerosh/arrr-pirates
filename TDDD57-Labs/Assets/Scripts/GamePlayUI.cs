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

    public Text selectedText;

    public float hintTimer;
    public readonly string REPOSITION_STRING = "Reposition the board by touching somewhere on a detected plane!";
    public readonly string CREWMAN_HINT = "Touch a treasure chest to have your diver collect it!";
    public readonly string HELMSMAN_HINT = "While the helmsman is selected, the ship will move. Steer by tilting your phone!";

    private readonly Color HELMSMAN_GREEN = new Color(0.0f, 0.6705883f, 0.0f);

    private int winCondition;

    private bool reposition = false;

    public void SetWinCondition(int newWinCondition) {
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

    public void SetSelected(Selectable selected) {
        if (selected) {
            if (selected.GetComponent<Crewman>()) {
                selectedText.color = Color.red;
                selectedText.text = "Selected: Diver";
            } else if (selected.GetComponent<Helmsman>()){
                selectedText.color = HELMSMAN_GREEN;
                selectedText.text = "Selected: Helmsman";
            } else {
                //Display this if something not meant to be selectable is selected.
                selectedText.text = "Unknown selected: " + selected.gameObject.name;
            }
        } else {
            selectedText.text = "";
        }
    }

    private void HideHint() {
        hintText.gameObject.SetActive(false);
    }
}
