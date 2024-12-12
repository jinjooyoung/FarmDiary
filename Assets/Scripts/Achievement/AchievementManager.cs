using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public AchievementUI[] achievementUIs;  // �� ���� UI ��� �迭
    [SerializeField] private AchievementsDatabaseSO achievementsDatabase; // ���� �����ͺ��̽�

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        AchievementsDatabase.Initialize(achievementsDatabase);
        InitializeAchievementUIs();
    }

    // �� ���� UI �ʱ�ȭ
    public void InitializeAchievementUIs()
    {
        for (int a = 1; a <= 5; a++)
        {
            achievementUIs[a - 1].Initialize(-a);
        }

        for (int i = 1; i <= 76; i++)
        {
            achievementUIs[i + 4].Initialize(i);
        }
    }

    // ���� ���� ���� ������Ʈ
    public void SafeUpdateAchievementProgress(int achievementID)
    {
        int index = -99;

        if (achievementID < 0)
        {
            index = (-achievementID) - 1;
        }
        else
        {
            index = achievementID + 4;
        }


        // achievementUIs �迭�� ���� Ȯ��
        if (achievementID >= achievementUIs.Length)
        {
            Debug.LogError($"�߸��� achievementID: {achievementID}, UI�� ������Ʈ�� �� �����ϴ�.");
            return;
        }

        AchievementData achievement = AchievementsDatabase.GetAchievementByID(achievementID);

        AchievementUI achievementUI = achievementUIs[index];
        if (achievementUI != null)
        {
            achievementUI.Initialize(achievementID);
            //Debug.LogWarning("UI �ʱ�ȭ ������Ʈ ȣ���");
        }
        else
        {
            Debug.LogError($"�ε��� {achievementID}�� Achievement UI�� null�Դϴ�.");
        }
    }
}
