﻿using UnityEngine;
using System;

[Serializable]
public class LevelClass
{
    public string name;
    public GameObject platform;
    public int percentageComplete;
    public Color ballColor;
    public Color skyboxOut, skyboxIn;

    public LevelClass()
    {

    }
}
