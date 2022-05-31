using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelInfo
{
    public int xp;
    public int level;
    public LevelInfo(int _xp, int _level)
    {
        xp = _xp;
        level = _level;
    }
}