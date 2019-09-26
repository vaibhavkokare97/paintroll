using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class LevelProgressFillArea : MonoBehaviour
{
    bool isEnd = false;
    bool pushed95 = false;
    public PaintIn3D.Examples.P3dChannelCounterFill Fill; // script which contains fill counter
    public NewLevel newLevel;
    public GameObject button95;
    public GameObject featureImage;
    public GameObject platformList;
    public RawImage[] stars = new RawImage[3];
    public TextMeshProUGUI currentLvlTxt, nextLevelTxt;
    int starValue = 0;
    int levelValue;
    int _percentageValue;

    private void Start()
    {
        levelValue = PlayerPrefs.GetInt("level"); //get level value
        _percentageValue = newLevel.lc[InfiniteLevelAlgo(levelValue)].percentageComplete;
        Invoke("AttachFeatureImage", 2f);
        LevelNumbers();
    }

    // repeats level
    public static int InfiniteLevelAlgo(int level)
    {
        int i = level % 10;
        return i;
    }

    void LevelNumbers()
    {
        currentLvlTxt.text = "LEVEL " + (levelValue + 1).ToString();
        //if(levelValue < 9)
        //{
        //    nextLevelTxt.text = (levelValue + 1).ToString();
        //}
        //else
        //{
        //    nextLevelTxt.text = "E";
        //}
    }

    // assign feature image
    void AttachFeatureImage()
    {
        featureImage.GetComponent<RawImage>().texture = platformList.GetComponentInChildren<PaintIn3D.P3dPaintableTexture>().Texture;
    }

    private void LateUpdate()
    {
        // virtual 100percent
        GetComponent<Slider>().value = Fill.cachedImage.fillAmount * 100/_percentageValue;
        if (GetComponent<Slider>().value >= 1 && !isEnd)
        {
            isEnd = true;
            starValue = 3; // if level ends by 100percent completition, give 3 stars
            OldPlayerPrefCheck();
            NewLevel.LoadGameOverScreen();
            //AnimateEndScreen();
        }

        // enable next level button on 95percent completion
        if (GetComponent<Slider>().value > 0.95 && !pushed95)
        {
            pushed95 = true;
            button95.SetActive(true);
            button95.GetComponent<Button>().onClick.AddListener(NextLevel);
        }
    }

    void StarValueCheck()
    {
        if (GetComponent<Slider>().value >= 1)
        {
            starValue = 3;
        }
        else if (GetComponent<Slider>().value > 0.975f)
        {
            starValue = 2;
            stars[2].color = new Color(1, 1, 1, 0.2f);
        }
        else
        {
            starValue = 1;
            stars[2].color = new Color(1, 1, 1, 0.2f);
            stars[1].color = new Color(1, 1, 1, 0.2f);
        }

        // check old values before setting playerprefs
        OldPlayerPrefCheck();
    }

    void OldPlayerPrefCheck()
    {
        if (PlayerPrefs.HasKey(levelValue.ToString()))
        {
            if (PlayerPrefs.GetInt(levelValue.ToString()) < starValue)
            {
                PlayerPrefs.SetInt(levelValue.ToString(), starValue);
            }
        }
        else
        {
            PlayerPrefs.SetInt(levelValue.ToString(), starValue);
        }
    }

    void NextLevel()
    {
        StarValueCheck();
        NewLevel.LoadGameOverScreen();
        //AnimateEndScreen();
    }

    void AnimateEndScreen()
    {
        // Using Coroutine For End Screen Animation
        StartCoroutine(ImageStampAnimation());
    }

    public IEnumerator ImageStampAnimation()
    {
        featureImage.GetComponent<RectTransform>().localScale = Vector3.one * newLevel.lc[InfiniteLevelAlgo(levelValue)].ImageSizeRange.x;
        while (featureImage.GetComponent<RectTransform>().lossyScale.x > newLevel.lc[InfiniteLevelAlgo(levelValue)].ImageSizeRange.y)
        {
            featureImage.GetComponent<RectTransform>().localScale -= Vector3.one * Time.deltaTime * 5f;
            yield return null;
        }
        // vibrate
        Taptic.tapticOn = true;
        Taptic.Success();

        yield return null;
    }


}
