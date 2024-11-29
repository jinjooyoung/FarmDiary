using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private Image achievementIcon;     // 업적 아이콘
    [SerializeField] private Text nameText;             // 업적 이름 텍스트
    [SerializeField] private Text descText;             // 업적 설명 텍스트
    [SerializeField] private Text progressText;         // 진행도 텍스트 (0/100 형식)
    [SerializeField] private Image rewardIcon;          // 보상 아이콘
    [SerializeField] private Button rewardButton;       // "보상 받기" 버튼

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

        // UI 설정
        nameText.text = name;
        descText.text = description;
        progressText.text = $"({progress}/{goal})";
        rewardButton.interactable = isCleared; // 클리어된 업적만 보상 버튼 활성화

        // 업적 아이콘과 보상 아이콘 로드
        /*if (ID > 0)    // 리소스가 없어서 임시로 이렇게 해둠 나중에 리소스 다 생기면 if 문 삭제
        {
            achievementIcon.sprite = LoadAchievementIcon(ID);
            rewardIcon.sprite = LoadRewardIcon(ID);
        }*/

        /*// 잠금 상태라면 UI 요소 숨기기
        if (!isUnlocked)
        {
            SetLockedUI();
        }
        else
        {
            // 잠금이 해제되었을 때는 모든 UI 요소 보이기
            ShowUnlockedUI();
        }*/
    }

    // 보상 받기 버튼 클릭 시 호출
    public void ClickRewardButton()
    {
        if (AchievementsDatabase.GetCleared(ID))
        {
            GiveReward(ID);

            // UI 업데이트
            rewardButton.interactable = false; // 보상을 이미 받았으므로 버튼 비활성화
        }
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
        return Resources.Load<Sprite>($"Achievements/Rewards/Reward_{id}");
    }

    // 잠금 상태 UI 설정 (잠금된 상태일 때 설명, 진행도, 보상 버튼 숨기기)
    private void SetLockedUI()
    {
        // 아이콘과 이름은 그대로 보이게 하고, 나머지 UI 요소들은 숨김
        achievementIcon.color = new Color(1f, 1f, 1f, 0.5f); // 반투명
        nameText.color = Color.white; // 이름은 그대로 보이게

        // 나머지 UI 요소들 숨기기
        descText.gameObject.SetActive(false);
        progressText.gameObject.SetActive(false);
        rewardIcon.gameObject.SetActive(false);
        rewardButton.gameObject.SetActive(false);
    }

    // 잠금 해제 후 UI 요소 보이기
    private void ShowUnlockedUI()
    {
        // 모든 UI 요소를 다시 보이게 설정
        descText.gameObject.SetActive(true);
        progressText.gameObject.SetActive(true);
        rewardIcon.gameObject.SetActive(true);
        rewardButton.gameObject.SetActive(true);
    }

    // 보상 지급 처리
    private void GiveReward(int id)
    {
        // 보상 지급 로직 구현
    }
}
