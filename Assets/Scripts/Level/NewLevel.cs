using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NewLevel : MonoBehaviour
{
    public GameObject Celebration;
    public GameObject DrawingCanvas;
    public GameObject Fill;
    public GameObject PaintGO;
    public GameObject GameCompletePanel;
    public Button newLevelButton;

    public List<LevelClass> lc = new List<LevelClass>();

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey("level"))
        {
            LoadDistictLevel();
        }
        else
        {
            PlayerPrefs.SetInt("level", 0);
            LoadDistictLevel();
        }

        Debug.Log("level start: " + (PlayerPrefs.GetInt("level") + 1).ToString());
        newLevelButton.onClick.AddListener(NewLevelButton);

        GameCompletePanel.SetActive(false);
        Celebration.SetActive(false);
    }

    void LoadDistictLevel()
    {
        lc[LevelProgressFillArea.InfiniteLevelAlgo(PlayerPrefs.GetInt("level"), 13)].platform.SetActive(true);
    }

    public void NewLevelButton()
    {
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        SceneManager.LoadScene("Game");
    }

    public void LoadGameOverScreen()
    {
        GameCompletePanel.SetActive(true);
        Celebration.SetActive(true);
        PaintGO.GetComponent<PaintIn3D.P3dPaintSphere>().Radius = 20;
        Debug.Log("level complete: " + (PlayerPrefs.GetInt("level") + 1).ToString());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
