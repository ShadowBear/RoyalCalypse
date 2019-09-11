using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int gold;
    public int exp;
    public int lvl;
    public int maxExp;
    public int attack;
    public int maxHealth;
    public int hearths;
    public int gems;

    public string firstWeaponID;
    public string playerName;

    public bool[] unlockedweapons = new bool[18];

    public PlayerData(GameManager game)
    {
        gold = game.gold;
        exp = game.exp;
        lvl = game.lvl;
        maxExp = game.maxExp;
        attack = game.attack;
        maxHealth = game.maxHealth;
        hearths = game.hearths;
        gems = game.gems;

        firstWeaponID = game.firstWeaponID;
        playerName = game.playerName;

        unlockedweapons = game.unlockedWeapons;
    }
}
