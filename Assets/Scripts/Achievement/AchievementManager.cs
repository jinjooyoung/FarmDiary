using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;          // �̱��� ȭ
    [SerializeField] private AchievementsDatabaseSO achievementsDatabase; // SO�� �����Ϳ��� �Ҵ�
    [SerializeField] private Storage storage;                             // �۹� ��Ȯ�� üũ�� ���ؼ�

    // UI ��ҵ��� ���� ���� ������Ʈ �迭
    public GameObject[] achievementUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);              // �ٸ� �������� �����ϱ� ���ؼ� �ı����� �ʰ� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // AchievementsDatabase �ʱ�ȭ
        AchievementsDatabase.Initialize(achievementsDatabase);
    }

    // ���ӿ��� �� �� �� Ư���� ��Ȳ���� ����Ǵ� ���� (Ʃ�丮�� �Ϸ�)
    public void CompleteTutorial()
    {
        int achievementID = 1;  // Ʃ�丮�� �Ϸ� ���� ID

        // ������ �޼��Ϸ��� �� �޼��尡 �� ���� ȣ��ǵ��� ����
        AchievementsDatabase.AddProgressToAchievement(achievementID, 1); // ��ǥ�� �´� ��ô�� �߰�
    }

    /*// ��Ȯ�� �۹��� ���� ���� ����
    public void OnCropHarvested(int harvestedAmount)
    {
        harvestedCropCount += harvestedAmount;

        int achievementID = 3;  // �۹� ��Ȯ ���� ID
        int goal = 1000;  // ��ǥ ��Ȯ�� (��: 1000��)

        // ��ǥ ��Ȯ���� �޼��ϸ� ���൵ �߰�
        if (harvestedCropCount >= goal)
        {
            AddProgressToAchievement(achievementID, harvestedCropCount);
            UnlockAchievement(achievementID);
        }
    }*/
}
