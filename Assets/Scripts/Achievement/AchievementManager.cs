using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public AchievementUI[] achievementUIs;  // 각 업적 UI 요소 배열
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
        InitializeAchievementUIs();
    }

    // 각 업적 UI 초기화
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

    // 업적 진행 상태 업데이트
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


        // achievementUIs 배열의 범위 확인
        if (achievementID >= achievementUIs.Length)
        {
            Debug.LogError($"잘못된 achievementID: {achievementID}, UI를 업데이트할 수 없습니다.");
            return;
        }

        AchievementData achievement = AchievementsDatabase.GetAchievementByID(achievementID);

        AchievementUI achievementUI = achievementUIs[index];
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
