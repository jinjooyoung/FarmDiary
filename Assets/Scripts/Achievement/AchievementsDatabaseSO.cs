using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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
        }
        UnlockAchievement(9);
    }

    // ID�� ���� ��������
    public static AchievementData GetAchievementByID(int id)
    {
        return database.achievementsData.Find(achievement => achievement.ID == id);
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

    // ���� ���൵ �߰�
    public static void AddProgressToAchievement(int achievementID, int progressAmount)
    {
        AchievementData achievement = GetAchievementByID(achievementID);
        if (achievement != null)
        {
            achievement.AddProgress(progressAmount);  // ���൵ �߰�
        }
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