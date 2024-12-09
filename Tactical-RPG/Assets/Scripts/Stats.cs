using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats
{
    [Header("Base Stats")]
    public int maxHp;
    public int maxMp;
    public int Atk;
    public int Spd;
    public int Def;
    public int Res;
    public int Skill;
    public int Eva;
    public int Mov;
    public int Range;

    [Header("Current Stats")]
    public int currentHp;
    public int currentAtk;
    public int currentSpd;
    public int currentDef;
    public int currentRes;
    public int currentSkill;
    public int currentEva;
    public int currentMov;
    public int currentMp;
}
