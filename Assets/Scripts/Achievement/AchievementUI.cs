using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private Image achievementIcon;     // 업적 아이콘
    [SerializeField] private Text nameText;             // 업적 이름 텍스트
    [SerializeField] private Text descText;             // 업적 설명 텍스트
    [SerializeField] private Text progressText;         // 진행도 텍스트 (0/100 형식)
    [SerializeField] private Text rewardText;           // 보상 수량
    [SerializeField] private Image rewardIcon;          // 보상 아이콘
    [SerializeField] private Image clearIcon;           // 클리어 아이콘

    private int ID; // 해당 UI가 관리하는 업적 ID

    // UI 초기화
    public void Initialize(int id)
    {
        ID = id;

        // 데이터베이스에서 정보를 가져옴
        string name = AchievementsDatabase.GetName(ID);
        string description = AchievementsDatabase.GetDescription(ID);
        int progress = AchievementsDatabase.GetProgress(ID);
        int goal = AchievementsDatabase.GetGoal(ID);
        bool isCleared = AchievementsDatabase.GetCleared(ID);
        bool isUnlocked = AchievementsDatabase.GetUnlocked(ID);
        string rewardCount = AchievementsDatabase.GetRewardCount(ID);

        // 업적 이름
        nameText.text = name;

        if (isUnlocked && !isCleared) // 업적이 해금되었고 클리어 되지 않았을때
        {
            rewardText.text = rewardCount;
            // 업적 설명과 진행도 표시
            descText.text = description;
            progressText.text = $"({progress}/{goal})";

            // 업적 아이콘 색상을 원래대로 복원
            achievementIcon.color = Color.white;
            // 보상 아이콘 설정
            rewardIcon.sprite = LoadRewardIcon(ID);
            if (ID >= 9 && ID <= 61)
            {
                rewardIcon.color = new Color(0f, 0f, 0f, 0.5f);
            }
            clearIcon.color = new Color(1f, 1f, 1f, 0f);
        }
        else if (!isUnlocked)       // 업적이 잠금 상태일 경우
        {
            rewardText.text = "";
            // 잠금 상태일 경우: ???와 ?/? 표시
            descText.text = "???";
            progressText.text = "(?/?)";

            // 업적 아이콘을 검은색으로 설정하고 알파값 50%로 변경
            achievementIcon.color = new Color(0f, 0f, 0f, 0.5f);
            rewardIcon.sprite = Resources.Load<Sprite>("Achievements/Rewards/Reward_Q");
            clearIcon.color = new Color(1f, 1f, 1f, 0f);
        }
        else if (isUnlocked && isCleared)   // 해금되었고 클리어 되었을때
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

        // 업적 아이콘 설정
        achievementIcon.sprite = LoadAchievementIcon(ID);
    }

    // 업적 아이콘 로드
    private Sprite LoadAchievementIcon(int id)
    {
        // 업적 ID에 해당하는 아이콘 로드 (리소스 이름은 ID에 맞춰서 Achievement_id로 각 폴더에)
        return Resources.Load<Sprite>($"Achievements/Icons/Achievement_{id}");
    }

    // 보상 아이콘 로드
    private Sprite LoadRewardIcon(int id)
    {
        // 업적 ID에 해당하는 보상 아이콘 로드

        if (id <= 8 || id >= 62)
        {
            return Resources.Load<Sprite>($"Achievements/Rewards/Reward_Coin");
        }
        else
        {
            return Resources.Load<Sprite>($"Achievements/Rewards/Reward_{id}");
        }
    }

    /*// 보상 지급 처리
    private void GiveReward(int id)
    {
        Enum rewardType = AchievementsDatabase.GetRewardType(id);
        int rewardData = AchievementsDatabase.GetRewardAmount(id);

        // 보상 지급 로직
        switch (rewardType)
        {
            case AchievementData.RewardType.Coin:
                // 코인 보상 지급
                GameManager.AddCoins(rewardData);
                Debug.Log($"업적 {id}: {rewardData}개의 코인을 지급받았습니다.");
                break;

            case AchievementData.RewardType.NextCrop:
                // 작물 보상 지급
                //CropManager.UnlockCrop(rewardData);
                Debug.Log($"업적 {id}: ID {rewardData}의 작물을 해금했습니다.");
                break;

            default:
                Debug.LogError($"업적 {id}: 알 수 없는 보상 타입 {rewardType}입니다.");
                break;
        }
    }*/
}
