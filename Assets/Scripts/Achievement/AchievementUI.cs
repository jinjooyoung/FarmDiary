using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private Image achievementIcon;     // ���� ������
    [SerializeField] private Text nameText;             // ���� �̸� �ؽ�Ʈ
    [SerializeField] private Text descText;             // ���� ���� �ؽ�Ʈ
    [SerializeField] private Text progressText;         // ���൵ �ؽ�Ʈ (0/100 ����)
    [SerializeField] private Text rewardText;           // ���� ����
    [SerializeField] private Image rewardIcon;          // ���� ������
    [SerializeField] private Image clearIcon;           // Ŭ���� ������

    private int ID; // �ش� UI�� �����ϴ� ���� ID

    // UI �ʱ�ȭ
    public void Initialize(int id)
    {
        ID = id;

        // �����ͺ��̽����� ������ ������
        string name = AchievementsDatabase.GetName(ID);
        string description = AchievementsDatabase.GetDescription(ID);
        int progress = AchievementsDatabase.GetProgress(ID);
        int goal = AchievementsDatabase.GetGoal(ID);
        bool isCleared = AchievementsDatabase.GetCleared(ID);
        bool isUnlocked = AchievementsDatabase.GetUnlocked(ID);
        string rewardCount = AchievementsDatabase.GetRewardCount(ID);

        // ���� �̸�
        nameText.text = name;

        if (isUnlocked && !isCleared) // ������ �رݵǾ��� Ŭ���� ���� �ʾ�����
        {
            rewardText.text = rewardCount;
            // ���� ����� ���൵ ǥ��
            descText.text = description;
            progressText.text = $"({progress}/{goal})";

            // ���� ������ ������ ������� ����
            achievementIcon.color = Color.white;
            // ���� ������ ����
            rewardIcon.sprite = LoadRewardIcon(ID);
            if (ID >= 9 && ID <= 61)
            {
                rewardIcon.color = new Color(0f, 0f, 0f, 0.5f);
            }
            clearIcon.color = new Color(1f, 1f, 1f, 0f);
        }
        else if (!isUnlocked)       // ������ ��� ������ ���
        {
            rewardText.text = "";
            // ��� ������ ���: ???�� ?/? ǥ��
            descText.text = "???";
            progressText.text = "(?/?)";

            // ���� �������� ���������� �����ϰ� ���İ� 50%�� ����
            achievementIcon.color = new Color(0f, 0f, 0f, 0.5f);
            rewardIcon.sprite = Resources.Load<Sprite>("Achievements/Rewards/Reward_Q");
            clearIcon.color = new Color(1f, 1f, 1f, 0f);
        }
        else if (isUnlocked && isCleared)   // �رݵǾ��� Ŭ���� �Ǿ�����
        {
            rewardText.text = rewardCount;
            descText.text = description;
            progressText.text = $"({progress}/{goal})";
            achievementIcon.color = Color.white;
            rewardIcon.sprite = LoadRewardIcon(ID);

            nameText.color = Color.white;
            descText.color = Color.white;
            progressText.color = Color.white;
            clearIcon.color = Color.white;
            rewardIcon.color = Color.white;
        }

        // ���� ������ ����
        achievementIcon.sprite = LoadAchievementIcon(ID);
    }

    // ���� ������ �ε�
    private Sprite LoadAchievementIcon(int id)
    {
        // ���� ID�� �ش��ϴ� ������ �ε� (���ҽ� �̸��� ID�� ���缭 Achievement_id�� �� ������)
        return Resources.Load<Sprite>($"Achievements/Icons/Achievement_{id}");
    }

    // ���� ������ �ε�
    private Sprite LoadRewardIcon(int id)
    {
        // ���� ID�� �ش��ϴ� ���� ������ �ε�

        if (id <= 8 || id >= 62)
        {
            return Resources.Load<Sprite>($"Achievements/Rewards/Reward_Coin");
        }
        else
        {
            return Resources.Load<Sprite>($"Achievements/Rewards/Reward_{id}");
        }
    }

    /*// ���� ���� ó��
    private void GiveReward(int id)
    {
        Enum rewardType = AchievementsDatabase.GetRewardType(id);
        int rewardData = AchievementsDatabase.GetRewardAmount(id);

        // ���� ���� ����
        switch (rewardType)
        {
            case AchievementData.RewardType.Coin:
                // ���� ���� ����
                GameManager.AddCoins(rewardData);
                Debug.Log($"���� {id}: {rewardData}���� ������ ���޹޾ҽ��ϴ�.");
                break;

            case AchievementData.RewardType.NextCrop:
                // �۹� ���� ����
                //CropManager.UnlockCrop(rewardData);
                Debug.Log($"���� {id}: ID {rewardData}�� �۹��� �ر��߽��ϴ�.");
                break;

            default:
                Debug.LogError($"���� {id}: �� �� ���� ���� Ÿ�� {rewardType}�Դϴ�.");
                break;
        }
    }*/
}
