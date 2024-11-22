using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

[CreateAssetMenu(menuName = "Scriptable/AchievementsDatabaseSO", fileName = "AchievementsDatabase")]
public class AchievementsDatabaseSO : ScriptableObject
{
    public List<AchievementData> achievementsData;    // 업적 리스트
}

[Serializable]
public class AchievementData
{
    [field: SerializeField]
    public int ID { get; private set; }               // 업적 고유 ID

    [field: SerializeField]
    public string Name { get; private set; }          // 업적 이름

    [field: SerializeField]
    public string Description { get; private set; }   // 업적 설명

    [field: SerializeField]
    public int Goal { get; private set; }             // 업적 목표 값

    [field: SerializeField]
    public int Progress { get; private set; }         // 업적 진행도

    [field: SerializeField]
    public bool IsUnlocked { get; private set; }      // 업적 잠김 (true면 진행 가능)

    [field: SerializeField]
    public bool Clear { get; private set; }           // 업적 달성 유무

    // 업적 진행도 증가
    public void AddProgress(int amount)
    {
        if (Clear)
        {
            Debug.LogWarning("이미 업적 클리어 되어 리턴됨");
            return;
        }
        else
        {
            if (IsUnlocked)  // 잠금 해제된 경우에만 진행도 추가
            {
                Progress += amount;
                if (Progress >= Goal) // 목표에 도달하면 Clear 처리
                {
                    Progress = Goal;  // 목표 이상은 목표 값으로 고정
                    Clear = true;
                }
            }
        }
    }

    // 업적 잠금 해제
    public void Unlock()
    {
        if (!IsUnlocked)
        {
            IsUnlocked = true;
        }
    }
}

public static class AchievementsDatabase
{
    private static AchievementsDatabaseSO database;

    // 초기화
    public static void Initialize(AchievementsDatabaseSO db)
    {
        database = db;

        for (int i = 1; i <= 5; i++)    // 초기에 진행할 수 있는 (잠금해제(클리어와 다름)되어있는) 업적
        {
            UnlockAchievement(i);
        }
        UnlockAchievement(9);
    }

    // ID로 업적 가져오기
    public static AchievementData GetAchievementByID(int id)
    {
        return database.achievementsData.Find(achievement => achievement.ID == id);
    }

    // 업적 해금 bool (업적을 진행할 수 있는가 없는가)
    public static bool GetUnlocked(int id)
    {
        AchievementData achievement = GetAchievementByID(id);
        return achievement.IsUnlocked;
    }

    // 업적 클리어 bool (업적을 클리어 하였는가 아닌가)
    public static bool GetCleared(int id)
    {
        AchievementData achievement = GetAchievementByID(id);
        return achievement.Clear;
    }

    // 업적 진행도 추가
    public static void AddProgressToAchievement(int achievementID, int progressAmount)
    {
        AchievementData achievement = GetAchievementByID(achievementID);
        if (achievement != null)
        {
            achievement.AddProgress(progressAmount);  // 진행도 추가
        }
    }

    // 업적 잠금 해제
    public static void UnlockAchievement(int achievementID)
    {
        AchievementData achievement = GetAchievementByID(achievementID);
        if (achievement != null)
        {
            achievement.Unlock(); // 업적 잠금 해제
        }
    }
}