using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheat : MonoBehaviour
{
    public void ShowMeTheMoney()
    {
        GameManager.AddCoins(int.MaxValue);
    }

    public void AllSeedUnlockButton()
    {
        for (int id = 9; id <= 62; id++)
        {
            PlayerPrefs.SetInt("CropUnlocked_" + id, 1);
            AchievementsDatabase.AddProgressToAchievement(id, 10);
        }
    }

    public void AllUnlockAchievement()
    {
        for (int i = 1; i <= 76; i++)
        {
            AchievementsDatabase.UnlockAchievement(i);
        }

        for (int a = 1; a <= 5; a++)
        {
            AchievementsDatabase.UnlockAchievement(a);
        }
    }
}
