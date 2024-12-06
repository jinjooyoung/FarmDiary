using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEditor;

[CreateAssetMenu(menuName = "Scriptable/AchievementsDatabaseSO", fileName = "AchievementsDatabase")]
public class AchievementsDatabaseSO : ScriptableObject
{
    public List<AchievementData> achievementsData;    // ���� ����Ʈ
}

[Serializable]
public class AchievementData
{
    [field: SerializeField]
    public int ID { get; private set; }               // ���� ���� ID

    [field: SerializeField]
    public string Name { get; private set; }          // ���� �̸�

    [field: SerializeField]
    public string Description { get; private set; }   // ���� ����

    [field: SerializeField]
    public int Goal { get; private set; }             // ���� ��ǥ ��

    [field: SerializeField]
    public int Progress { get; private set; }         // ���� ���൵

    [field: SerializeField]
    public bool IsUnlocked { get; private set; }      // ���� ��� (true�� ���� ����)

    [field: SerializeField]
    public bool Clear { get; private set; }           // ���� �޼� ����

    // ���� ���� Enum
    public enum RewardType
    {
        NextCrop,   // ���� �۹� ����
        Coin        // ���� ����
    }

    [field: SerializeField]
    public RewardType Reward { get; private set; }    // ���� ����

    [field: SerializeField, Tooltip("������ ������ ��� ������ ����")]
    public int CoinAmount { get; private set; }       // ���� ������ ����

    // ���� ���൵ ����
    public void AddProgress(int amount)
    {
        if (Clear)
        {
            Debug.LogWarning("�̹� ���� Ŭ���� �Ǿ� ���ϵ�");
            return;
        }
        else
        {
            if (IsUnlocked)  // ��� ������ ��쿡�� ���൵ �߰�
            {
                Progress += amount;
                if (Progress >= Goal) // ��ǥ�� �����ϸ� Clear ó��
                {
                    Progress = Goal;  // ��ǥ �̻��� ��ǥ ������ ����
                    Clear = true;
                    if (Reward == RewardType.Coin)      // ���� Ÿ���� �����̶��
                    {
                        GameManager.AddCoins(CoinAmount);   // ������ ������ ����
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
            Debug.LogWarning("������ �̹� Ŭ����Ǿ� ���ϵ�");
            return;
        }
        else
        {
            if (IsUnlocked)  // ��� ������ ��쿡�� ���൵ ����
            {
                Progress -= amount;
                if (Progress < 0)  // ���൵�� 0���� �۾����� �ʵ��� ����
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
            Debug.LogWarning("�̹� ���� Ŭ���� �Ǿ� ���ϵ�");
            return;
        }
        else
        {
            if (IsUnlocked)  // ��� ������ ��쿡�� ���൵ ����
            {
                Progress = Mathf.Min(amount, Goal);  // ���൵�� ��ǥ�� �ʰ����� �ʵ��� ����
                if (Progress >= Goal)  // ��ǥ�� �����ϸ� Ŭ���� ó��
                {
                    Clear = true;
                }
            }
        }
    }

    // ���� ��� ����
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

    // �ʱ�ȭ
    public static void Initialize(AchievementsDatabaseSO db)
    {
        database = db;

        for (int i = 1; i <= 5; i++)    // �ʱ⿡ ������ �� �ִ� (�������(Ŭ����� �ٸ�)�Ǿ��ִ�) ����
        {
            UnlockAchievement(i);
            UnlockAchievement(-i);
        }
        UnlockAchievement(9);
        UnlockAchievement(48);
        UnlockAchievement(62);
    }

    // ID�� ���� ��������
    public static AchievementData GetAchievementByID(int id)
    {
        return database.achievementsData.Find(achievement => achievement.ID == id);
    }

    // ���� �̸�
    public static string GetName(int id)
    {
        AchievementData achievement = GetAchievementByID(id);
        return achievement.Name;
    }

    // ���� ����
    public static string GetDescription(int id)
    {
        AchievementData achievement = GetAchievementByID(id);
        return achievement.Description;
    }

    // ���� ��ǥġ
    public static int GetGoal(int id)
    {
        AchievementData achievement = GetAchievementByID(id);
        return achievement.Goal;
    }

    // ���� ���൵
    public static int GetProgress(int id)
    {
        AchievementData achievement = GetAchievementByID(id);
        return achievement.Progress;
    }

    // ���� �ر� bool (������ ������ �� �ִ°� ���°�)
    public static bool GetUnlocked(int id)
    {
        AchievementData achievement = GetAchievementByID(id);
        return achievement.IsUnlocked;
    }

    // ���� Ŭ���� bool (������ Ŭ���� �Ͽ��°� �ƴѰ�)
    public static bool GetCleared(int id)
    {
        AchievementData achievement = GetAchievementByID(id);
        return achievement.Clear;
    }

    // ���� ���� ���� (������ ���൵ �޼��忡�� Ŭ�����ϸ� �ڵ����� �ִϱ� UI������)
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

    // ���� ���൵ �߰�
    public static void AddProgressToAchievement(int achievementID, int progressAmount)
    {
        AchievementData achievement = GetAchievementByID(achievementID);
        if (achievement != null)
        {
            achievement.AddProgress(progressAmount);  // ���൵ �߰�
            AchievementManager.Instance.SafeUpdateAchievementProgress(achievementID);
        }
    }

    // ���� ���൵ ����
    public static void SubProgressToAchievement(int achievementID, int progressAmount)
    {
        AchievementData achievement = GetAchievementByID(achievementID);
        if (achievement != null)
        {
            achievement.RemoveProgress(progressAmount);  // ���൵ �߰�
            AchievementManager.Instance.SafeUpdateAchievementProgress(achievementID);
        }
    }

    // ���൵�� �Ҵ���
    public static void SetProgressToAchievement(int achievementID, int amount)
    {
        AchievementData achievement = GetAchievementByID(achievementID);
        if (achievement != null)
        {
            achievement.SetProgress(amount);  // ���൵ �Ҵ�
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

    // ���� ��� ����
    public static void UnlockAchievement(int achievementID)
    {
        AchievementData achievement = GetAchievementByID(achievementID);
        if (achievement != null)
        {
            achievement.Unlock(); // ���� ��� ����
        }
    }
}