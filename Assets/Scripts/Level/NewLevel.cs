using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[Serializable]
public class LevelClass
{
    public string name;
    public GameObject platform;
    public int percentageComplete;
    public Vector2 ImageSizeRange; // x : FROM, y : TO
}

public class NewLevel : MonoBehaviour
{
    public GameObject DrawingCanvas;
    public GameObject Fill;
    public static GameObject GameCompletePanel;
    public Button newLevelButton;

    public List<LevelClass> lc = new List<LevelClass>();

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey("level"))
        {
            ApplyImage();
        }
        else //2
        {
            PlayerPrefs.SetInt("level", 0);
            ApplyImage();
        }

        Debug.Log("level start: " + (PlayerPrefs.GetInt("level") + 1).ToString());
        newLevelButton.onClick.AddListener(NewLevelButton);
        GameCompletePanel = GameObject.Find("GameComplete");
        GameCompletePanel.SetActive(false);
    }

    void ApplyImage()
    {
        //DrawingCanvas.GetComponent<PaintIn3D.P3dPaintableTexture>().Texture = levels[PlayerPrefs.GetInt("level")];

        //try
        //{
        //    lc[PlayerPrefs.GetInt("level") - 1].platform.SetActive(false);
        //}
        //catch (System.Exception)
        //{

        //}
        lc[LevelProgressFillArea.InfiniteLevelAlgo(PlayerPrefs.GetInt("level"))].platform.SetActive(true);
       
    }

    public void NewLevelButton()
    {
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        //if (PlayerPrefs.GetInt("level") <= 9)
        //{
        //    SceneManager.LoadScene("Game");
        //}
        //else
        //{
        //    SceneManager.LoadScene("MainScreen");
        //}

        SceneManager.LoadScene("Game");

    }

    public static void LoadGameOverScreen()
    {
        GameCompletePanel.SetActive(true);
        Transform camTransform = Camera.main.gameObject.transform;
        camTransform.position = new Vector3(100, -2, -1);
        camTransform.rotation = Quaternion.identity;
        Debug.Log("level complete: " + (PlayerPrefs.GetInt("level") + 1).ToString());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //SceneManager.LoadScene("MainScreen"); //1
            Application.Quit(); //2
        }
    }
}
