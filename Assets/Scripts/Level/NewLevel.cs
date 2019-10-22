using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NewLevel : MonoBehaviour
{
    public static bool GameOver = false;
    public GameObject Celebration;
    public GameObject DrawingCanvas;
    public GameObject Fill;
    public GameObject PaintGO;
    public GameObject GameCompletePanel;
    public RawImage[] borders;
    public Button newLevelButton;
    //public GameObject background;
    public GameObject physicsBall, jellyBall, Brush;
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

        ChangeBackgroundColor();
        ChangeBallColor();

        GameCompletePanel.SetActive(false);
        Celebration.SetActive(false);
    }

    void LoadDistictLevel()
    {
        lc[LevelProgressFillArea.InfiniteLevelAlgo(PlayerPrefs.GetInt("level"), 10)].platform.SetActive(true);
    }

    void ChangeBackgroundColor() //border color
    {
        for (int i = 0; i < 4; i++) //4 borders
        {
            borders[i].color = lc[LevelProgressFillArea.InfiniteLevelAlgo(PlayerPrefs.GetInt("level"), 10)].borderColor;
        }
    }

    void ChangeBallColor()
    {
#if JELLY
        physicsBall.GetComponent<MeshRenderer>().material.color = lc[LevelProgressFillArea.InfiniteLevelAlgo(PlayerPrefs.GetInt("level"), 10)].ballColor;
        jellyBall.GetComponent<SkinnedMeshRenderer>().material.color = lc[LevelProgressFillArea.InfiniteLevelAlgo(PlayerPrefs.GetInt("level"), 10)].ballColor;
#elif BRUSH
        Brush.SetActive(true);
        jellyBall.GetComponent<SkinnedMeshRenderer>().enabled = false;
        physicsBall.GetComponent<MeshRenderer>().enabled = false;
#else
        jellyBall.GetComponent<SkinnedMeshRenderer>().enabled = false;
        physicsBall.GetComponent<MeshRenderer>().material.color = lc[LevelProgressFillArea.InfiniteLevelAlgo(PlayerPrefs.GetInt("level"), 10)].ballColor;
#endif
    }

    public void NewLevelButton()
    {
        GameOver = false;
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        SceneManager.LoadScene("Game");
    }

    public void LoadGameOverScreen()
    {
        GameCompletePanel.SetActive(true);
        Celebration.SetActive(true);
        PaintGO.GetComponent<PaintIn3D.P3dPaintSphere>().Radius = 20; // complete coloring
        GameOver = true;
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
