using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEditor;

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

    // 보상 유형 Enum
    public enum RewardType
    {
        NextCrop,   // 다음 작물 보상
        Coin        // 코인 보상
    }

    [field: SerializeField]
    public RewardType Reward { get; private set; }    // 보상 유형

    [field: SerializeField, Tooltip("보상이 코인일 경우 코인의 수량")]
    public int CoinAmount { get; private set; }       // 코인 보상의 수량

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
                    if (Reward == RewardType.Coin)      // 보상 타입이 코인이라면
                    {
                        GameManager.AddCoins(CoinAmount);   // 정해진 코인을 받음
                    }

                    if (ID < 0)
                    {
                        AchievementManager.Instance.achievementUIs[(-ID)-1].Initialize(ID);
                    }
                    else
                    {
                        AchievementManager.Instance.achievementUIs[ID + 4].Initialize(ID);
                    }
                    
                }
            }
        }
    }

    public void RemoveProgress(int amount)
    {
        if (Clear)
        {
            Debug.LogWarning("업적이 이미 클리어되어 리턴됨");
            return;
        }
        else
        {
            if (IsUnlocked)  // 잠금 해제된 경우에만 진행도 감소
            {
                Progress -= amount;
                if (Progress < 0)  // 진행도가 0보다 작아지지 않도록 방지
                {
                    Progress = 0;
                }
            }
        }
    }

    public void SetProgress(int amount)
    {
        if (Clear)
        {
            Debug.LogWarning("이미 업적 클리어 되어 리턴됨");
            return;
        }
        else
        {
            if (IsUnlocked)  // 잠금 해제된 경우에만 진행도 설정
            {
                Progress = Mathf.Min(amount, Goal);  // 진행도가 목표를 초과하지 않도록 설정
                if (Progress >= Goal)  // 목표에 도달하면 클리어 처리
                {
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
            UnlockAchievement(-i);
        }
        UnlockAchievement(9);
        UnlockAchievement(48);
        UnlockAchievement(62);
    }

    // ID로 업적 가져오기
    public static AchievementData GetAchievementByID(int id)
    {
        return database.achievementsData.Find(achievement => achievement.ID == id);
    }

    // 업적 이름
    public static string GetName(int id)
    {
        AchievementData achievement = GetAchievementByID(id);
        return achievement.Name;
    }

    // 업적 설명
    public static string GetDescription(int id)
    {
        AchievementData achievement = GetAchievementByID(id);
        return achievement.Description;
    }

    // 업적 목표치
    public static int GetGoal(int id)
    {
        AchievementData achievement = GetAchievementByID(id);
        return achievement.Goal;
    }

    // 업적 진행도
    public static int GetProgress(int id)
    {
        AchievementData achievement = GetAchievementByID(id);
        return achievement.Progress;
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

    // 업적 보상 리턴 (보상은 진행도 메서드에서 클리어하면 자동으로 주니까 UI용으로)
    public static string GetRewardCount(int id)
    {
        AchievementData achievement = GetAchievementByID(id);
        if (achievement.Reward == AchievementData.RewardType.Coin)
        {
            return achievement.CoinAmount.ToString("N0");
        }
        else
        {
            return "";
        }
    }

    // 업적 진행도 추가
    public static void AddProgressToAchievement(int achievementID, int progressAmount)
    {
        AchievementData achievement = GetAchievementByID(achievementID);
        if (achievement != null)
        {
            achievement.AddProgress(progressAmount);  // 진행도 추가
            AchievementManager.Instance.SafeUpdateAchievementProgress(achievementID);
        }
    }

    // 업적 진행도 감소
    public static void SubProgressToAchievement(int achievementID, int progressAmount)
    {
        AchievementData achievement = GetAchievementByID(achievementID);
        if (achievement != null)
        {
            achievement.RemoveProgress(progressAmount);  // 진행도 추가
            AchievementManager.Instance.SafeUpdateAchievementProgress(achievementID);
        }
    }

    // 진행도를 할당함
    public static void SetProgressToAchievement(int achievementID, int amount)
    {
        AchievementData achievement = GetAchievementByID(achievementID);
        if (achievement != null)
        {
            achievement.SetProgress(amount);  // 진행도 할당
            AchievementManager.Instance.SafeUpdateAchievementProgress(achievementID);
        }
    }

    public static void KeyboardProgress()
    {
        AddProgressToAchievement(6, 1);
        AddProgressToAchievement(7, 1);
        AddProgressToAchievement(8, 1);
    }

    public static void CoinProgress(int coin)
    {
        SetProgressToAchievement(-1, coin);
        SetProgressToAchievement(-2, coin);
        SetProgressToAchievement(-3, coin);
    }

    public static void TutorialAchievement()
    {
        AddProgressToAchievement(1, 1);
        AddProgressToAchievement(2, 1);
        AddProgressToAchievement(3, 1);
        AddProgressToAchievement(4, 1);
        AddProgressToAchievement(5, 1);
        AddProgressToAchievement(-4, 1);
        AddProgressToAchievement(-5, 1);
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