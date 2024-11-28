using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    [SerializeField] private AchievementUI[] achievementUIs;  // 각 업적 UI 요소 배열
    [SerializeField] private AchievementsDatabaseSO achievementsDatabase; // 업적 데이터베이스

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

        // 각 업적 UI를 초기화
        InitializeAchievementUIs();
    }

    // 각 업적 UI 초기화
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

    // 업적 진행 상태 업데이트
    public void UpdateAchievementProgress(int achievementID)
    {
        // 해당 업적의 UI만 갱신
        if (achievementID < 0)
        {
            achievementUIs[(-achievementID) - 1].Initialize(achievementID);
        }
        else
        {
            achievementUIs[achievementID + 4].Initialize(achievementID);
        }
    }
}
