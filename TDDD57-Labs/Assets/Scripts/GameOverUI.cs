using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour {

    public void GameOver() {
        //Restart the main scene, thus restarting the game.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
