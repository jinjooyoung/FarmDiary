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
    public void SafeUpdateAchievementProgress(int achievementID)
    {
        // achievementUIs 배열의 범위 확인
        if (achievementID < 0 || achievementID >= achievementUIs.Length)
        {
            Debug.LogError($"잘못된 achievementID: {achievementID}, UI를 업데이트할 수 없습니다.");
            return;
        }

        AchievementData achievement = AchievementsDatabase.GetAchievementByID(achievementID);

        // 업적이 클리어된 상태이면 업데이트하지 않음
        if (achievement != null && achievement.Clear)
        {
            return;  // 클리어된 업적은 UI 갱신을 하지 않음
        }

        AchievementUI achievementUI = achievementUIs[achievementID];
        if (achievementUI != null)
        {
            achievementUI.Initialize(achievementID);
            //Debug.LogWarning("UI 초기화 업데이트 호출됨");
        }
        else
        {
            Debug.LogError($"인덱스 {achievementID}의 Achievement UI가 null입니다.");
        }
    }
}
