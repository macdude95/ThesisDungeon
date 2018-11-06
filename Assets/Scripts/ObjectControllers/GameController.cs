using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public GameObject levelControllerPrefab;
    public LevelController levelController;
    public GameObject player;
    public int numberOfHeartsAtStart = 3;
    public bool sceneIsLevel = false;
    public Vector3 centerOfScreen {
        get {
            return levelController == null || !sceneIsLevel ? new Vector3(5, 5) : levelController.currentRoomController.room.PositionOfCenter();
        }
    }

    private void Start()
    {
        if (sceneIsLevel)
        {
            GameObject levelControllerObject = Instantiate(levelControllerPrefab);
            levelController = levelControllerObject.GetComponent<LevelController>();
            levelController.player = player;

            levelController.StartLevel();
        }
    }

    public void loadInBetweenLevel()
    {
        SceneManager.LoadScene("OtherScene");
    }

    public void loadNewLevel()
    {
        SceneManager.LoadScene("LevelScene");

    }
}
