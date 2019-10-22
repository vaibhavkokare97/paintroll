using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class LevelProgressFillArea : MonoBehaviour
{
    bool pushed95 = false;
    public PaintIn3D.Examples.P3dChannelCounterFill Fill; // script which contains fill counter
    public NewLevel newLevel;
    public TextMeshProUGUI currentLvlTxt;
    int levelValue;
    int _percentageValue;

    private void Start()
    {
        levelValue = PlayerPrefs.GetInt("level"); //get level value
        _percentageValue = newLevel.lc[InfiniteLevelAlgo(levelValue, 10)].percentageComplete;
        LevelNumbers();
    }

    // repeats level
    public static int InfiniteLevelAlgo(int level, int uniqueLevels)
    {
        int i = level % uniqueLevels;
        return i;
    }

    void LevelNumbers()
    {
        currentLvlTxt.text = "LEVEL " + (levelValue + 1).ToString();
    }

    private void LateUpdate()
    {
        // virtual 100percent
        GetComponent<Slider>().value = Fill.cachedImage.fillAmount * 100/_percentageValue;

        // end level when reached 95percent completion
        if (GetComponent<Slider>().value > 0.95 && !pushed95)
        {
            pushed95 = true;
            newLevel.LoadGameOverScreen();
        }
    }
}
