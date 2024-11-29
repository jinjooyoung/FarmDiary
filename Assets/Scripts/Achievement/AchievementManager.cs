using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    [SerializeField] private AchievementUI[] achievementUIs;  // �� ���� UI ��� �迭
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

        // �� ���� UI�� �ʱ�ȭ
        InitializeAchievementUIs();
    }

    // �� ���� UI �ʱ�ȭ
    private void InitializeAchievementUIs()
    {
        for (int a = 1; a <= 5; a++)
        {
            achievementUIs[a - 1].Initialize(-a);
        }

        for (int i = 1; i <= 61; i++)
        {
            achievementUIs[i + 4].Initialize(i);
        }
    }

    // ���� ���� ���� ������Ʈ
    public void SafeUpdateAchievementProgress(int achievementID)
    {
        // achievementUIs �迭�� ���� Ȯ��
        if (achievementID < 0 || achievementID >= achievementUIs.Length)
        {
            Debug.LogError($"�߸��� achievementID: {achievementID}, UI�� ������Ʈ�� �� �����ϴ�.");
            return;
        }

        AchievementData achievement = AchievementsDatabase.GetAchievementByID(achievementID);

        // ������ Ŭ����� �����̸� ������Ʈ���� ����
        if (achievement != null && achievement.Clear)
        {
            return;  // Ŭ����� ������ UI ������ ���� ����
        }

        AchievementUI achievementUI = achievementUIs[achievementID];
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
