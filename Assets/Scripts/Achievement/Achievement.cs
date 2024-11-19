using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]

public class Achievement                    // ���� Ŭ���� ���� (MonoBehaviour X)
{
    public string name;                     // ���� �̸�
    public string description;              // ���� ����
    public bool isUnlocked;                 // ��� ����
    public int currentProgress;             // ���� ����
    public int goal;                        // �Ϸ� ����

    public Achievement(string name, string description, int goal)       // Achievement ������ �Լ�
    {
        this.name = name;                       // AchievementŬ������ ���� �� �� �̸��� �μ��� �޾Ƽ� ����
        this.description = description;         // AchievementŬ������ ���� �� �� ������ �μ��� �޾Ƽ� ����
        this.isUnlocked = false;                // AchievementŬ������ ���� �� �� false
        this.currentProgress = 0;               // AchievementŬ������ ���� �� �� 0
        this.goal = goal;                       // AchievementŬ������ ���� �� �� �Ϸ� ����
    }

    public void AddProgress(int amount)         // ���� ���൵ �Լ�
    {
        if (!isUnlocked)                        // ����ִٸ�
        {
            currentProgress += amount;          // ���൵ �߰�
            if (currentProgress >= goal)        // ���൵���� �Ϸ� ���ڰ� �� ���� ��
            {
                isUnlocked = true;
                OnAchievementUnlocked();        // ���� �޼��� Debug.Log�� ���
            }
        }
    }

    protected virtual void OnAchievementUnlocked()
    {
        Debug.Log($"���� �޼� : {name}");           // $ǥ�ð� ����ִ� String���� {} ���� ���� ��� �� �� �ִ�.
    }
}
