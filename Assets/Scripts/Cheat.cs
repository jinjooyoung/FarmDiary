using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cheat : MonoBehaviour
{
    public GameObject CheatPanel;
    public Button[] SeedButtons;
    public Image[] StorageImages;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            CheatPanel.SetActive(true);
        }
    }

    public void ShowMeTheMoney()
    {
        GameManager.AddCoins(int.MaxValue);
    }

    public void AllSeedUnlockButton()
    {
        AllUnlockAchievement();

        for (int i = 0; i < SeedButtons.Length; i++)
        {
            SeedButtons[i].interactable = true;

            Transform childTransform = SeedButtons[i].transform.GetChild(0);

            if (childTransform != null)
            {
                Image iconImage = childTransform.GetComponent<Image>();     // 해당 버튼 작물의 이미지 오브젝트의 이미지 컴포넌트

                iconImage.color = new Color(255, 255, 255, 255);
                StorageImages[i].color = new Color(255, 255, 255, 255);
            }
        }

        for (int id = 9; id <= 62; id++)
        {
            PlayerPrefs.SetInt("CropUnlocked_" + id, 1);

            if (id != 62)
            {
                AchievementsDatabase.AddProgressToAchievement(id, 100);
                AchievementManager.Instance.SafeUpdateAchievementProgress(id);
            }
        }
    }

    public void PotionTimeOne()
    {
        for (int i = 48; i <= 62; i++)
        {
            PotionDatabase.CheatCraftingTime(i);
        }
    }

    public void GrowthTimeOne()
    {
        CropGrowthManager.Instance.CheatOn = true;
        Time.timeScale = 5f;
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
