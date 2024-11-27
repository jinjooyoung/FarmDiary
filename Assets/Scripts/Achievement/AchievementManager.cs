using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;          // 싱글톤 화
    [SerializeField] private AchievementsDatabaseSO achievementsDatabase; // SO를 에디터에서 할당
    [SerializeField] private Storage storage;                             // 작물 수확량 체크를 위해서

    // UI 요소들을 담을 게임 오브젝트 배열
    public GameObject[] achievementUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);              // 다른 씬에서도 적용하기 위해서 파괴되지 않게 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // AchievementsDatabase 초기화
        AchievementsDatabase.Initialize(achievementsDatabase);
    }

    // 게임에서 단 한 번 특수한 상황에서 진행되는 업적 (튜토리얼 완료)
    public void CompleteTutorial()
    {
        int achievementID = 1;  // 튜토리얼 완료 업적 ID

        // 업적을 달성하려면 이 메서드가 한 번만 호출되도록 관리
        AchievementsDatabase.AddProgressToAchievement(achievementID, 1); // 목표에 맞는 진척도 추가
    }

    /*// 수확된 작물에 따른 업적 진행
    public void OnCropHarvested(int harvestedAmount)
    {
        harvestedCropCount += harvestedAmount;

        int achievementID = 3;  // 작물 수확 업적 ID
        int goal = 1000;  // 목표 수확량 (예: 1000개)

        // 목표 수확량을 달성하면 진행도 추가
        if (harvestedCropCount >= goal)
        {
            AddProgressToAchievement(achievementID, harvestedCropCount);
            UnlockAchievement(achievementID);
        }
    }*/
}
