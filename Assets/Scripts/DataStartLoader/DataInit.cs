using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataInit : MonoBehaviour
{
    public GameObject[] levels;
    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {

            if (PlayerPrefs.HasKey(i.ToString()))
            {
                int k = PlayerPrefs.GetInt(i.ToString());

                if (k == 3)
                {
                    levels[i].transform.GetChild(3).gameObject.GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
                    levels[i].transform.GetChild(2).gameObject.GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
                    levels[i].transform.GetChild(1).gameObject.GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
                }
                else if(k == 2)
                {
                    levels[i].transform.GetChild(2).gameObject.GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
                    levels[i].transform.GetChild(1).gameObject.GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
                }
                else if(k == 1)
                {
                    levels[i].transform.GetChild(1).gameObject.GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
                }
                levels[i].transform.GetChild(0).gameObject.GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
            }
        }
    }

    public void LevelClick(int level)
    {
        PlayerPrefs.SetInt("level", level);
        SceneManager.LoadScene("Game");
    }

    public void CloseApp()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
