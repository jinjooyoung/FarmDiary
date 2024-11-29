using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private Image achievementIcon;     // ���� ������
    [SerializeField] private Text nameText;             // ���� �̸� �ؽ�Ʈ
    [SerializeField] private Text descText;             // ���� ���� �ؽ�Ʈ
    [SerializeField] private Text progressText;         // ���൵ �ؽ�Ʈ (0/100 ����)
    [SerializeField] private Image rewardIcon;          // ���� ������
    [SerializeField] private Button rewardButton;       // "���� �ޱ�" ��ư

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

        // UI ����
        nameText.text = name;
        descText.text = description;
        progressText.text = $"({progress}/{goal})";
        rewardButton.interactable = isCleared; // Ŭ����� ������ ���� ��ư Ȱ��ȭ

        // ���� �����ܰ� ���� ������ �ε�
        /*if (ID > 0)    // ���ҽ��� ��� �ӽ÷� �̷��� �ص� ���߿� ���ҽ� �� ����� if �� ����
        {
            achievementIcon.sprite = LoadAchievementIcon(ID);
            rewardIcon.sprite = LoadRewardIcon(ID);
        }*/

        /*// ��� ���¶�� UI ��� �����
        if (!isUnlocked)
        {
            SetLockedUI();
        }
        else
        {
            // ����� �����Ǿ��� ���� ��� UI ��� ���̱�
            ShowUnlockedUI();
        }*/
    }

    // ���� �ޱ� ��ư Ŭ�� �� ȣ��
    public void ClickRewardButton()
    {
        if (AchievementsDatabase.GetCleared(ID))
        {
            GiveReward(ID);

            // UI ������Ʈ
            rewardButton.interactable = false; // ������ �̹� �޾����Ƿ� ��ư ��Ȱ��ȭ
        }
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
        return Resources.Load<Sprite>($"Achievements/Rewards/Reward_{id}");
    }

    // ��� ���� UI ���� (��ݵ� ������ �� ����, ���൵, ���� ��ư �����)
    private void SetLockedUI()
    {
        // �����ܰ� �̸��� �״�� ���̰� �ϰ�, ������ UI ��ҵ��� ����
        achievementIcon.color = new Color(1f, 1f, 1f, 0.5f); // ������
        nameText.color = Color.white; // �̸��� �״�� ���̰�

        // ������ UI ��ҵ� �����
        descText.gameObject.SetActive(false);
        progressText.gameObject.SetActive(false);
        rewardIcon.gameObject.SetActive(false);
        rewardButton.gameObject.SetActive(false);
    }

    // ��� ���� �� UI ��� ���̱�
    private void ShowUnlockedUI()
    {
        // ��� UI ��Ҹ� �ٽ� ���̰� ����
        descText.gameObject.SetActive(true);
        progressText.gameObject.SetActive(true);
        rewardIcon.gameObject.SetActive(true);
        rewardButton.gameObject.SetActive(true);
    }

    // ���� ���� ó��
    private void GiveReward(int id)
    {
        // ���� ���� ���� ����
    }
}
