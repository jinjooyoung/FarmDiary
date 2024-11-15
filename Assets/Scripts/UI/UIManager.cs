using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject StoragePanel;
    public GameObject seedPanel;
    public GameObject DecoPanel;
    public GameObject AchievementsPanel;
    public GameObject CollectionPanel;
    public GameObject settingPanel;

    public Button StorageButton;
    public Button seedButton;
    public Button DecoButton;
    public Button AchievementsButton;
    public Button CollectionButton;
    public Button settingButton;

    [SerializeField] private Button[] SeedButtons;        // 작물 선택 버튼 배열 (수확한 갯수에 따라서 해금할거라서)

    public Storage storage; // Storage 참조

    public Text beat, Blueberry, Carrot, Corn, Cucumber, Eggplant, Garlic, Greenonion, Melon, Onion, Pea,
                Pimento, Pineapple, Potato, Pumpkin, Radish, Rice, Strawberry, Tomato, Watermelon;

    private Dictionary<int, Text> cropTextFields;

    void Start()
    {
        GameManager.instance.ResetCropKeys();
        UpdateButtons();

        // 버튼에 클릭 이벤트 추가
        StorageButton.onClick.AddListener(() => TogglePanel(StoragePanel));
        seedButton.onClick.AddListener(() => TogglePanel(seedPanel));
        DecoButton.onClick.AddListener(() => TogglePanel(DecoPanel));
        AchievementsButton.onClick.AddListener(() => TogglePanel(AchievementsPanel));
        CollectionButton.onClick.AddListener(() => TogglePanel(CollectionPanel));
        settingButton.onClick.AddListener(() => TogglePanel(settingPanel));

        // ID와 텍스트 필드를 매핑합니다.
        cropTextFields = new Dictionary<int, Text>
        {
            { 9, beat }, { 10, Blueberry }, { 11, Carrot }, { 12, Corn }, { 13, Cucumber },
            { 14, Eggplant }, { 15, Garlic }, { 16, Greenonion }, { 17, Melon }, { 18, Onion },
            { 19, Pea }, { 21, Pimento }, { 22, Pineapple }, { 23, Potato }, { 24, Pumpkin },
            { 25, Radish }, { 26, Rice }, { 27, Strawberry }, { 28, Tomato }, { 29, Watermelon }
        };
    }

    void Update()
    {
        UpdateStoragePanel();
    }

    // 패널의 활성화 상태를 토글하는 메서드
    public void TogglePanel(GameObject panel)
    {
        // 모든 패널을 먼저 닫기
        CloseAllPanels();

        // 클릭된 패널만 현재 상태의 반대로 설정
        panel.SetActive(!panel.activeSelf);
    }

    // 모든 패널을 닫는 메서드
    public void CloseAllPanels()
    {
        StoragePanel.SetActive(false);
        seedPanel.SetActive(false);
        DecoPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        CollectionPanel.SetActive(false);
        settingPanel.SetActive(false);
    }

    void UpdateStoragePanel()
    {
        foreach (var cropStorage in storage.storedCropsByID)
        {
            if (cropTextFields.TryGetValue(cropStorage.cropID, out Text cropText))
            {
                cropText.text = cropStorage.crops.Count.ToString();
            }
        }
    }

    // 모든 씨앗 버튼의 상태를 업데이트하는 메서드
    private void UpdateButtons()
    {
        int unlockedIndex = PlayerPrefs.GetInt("UnlockPlant", 0);       // 기본값은 0 (첫 번째 작물만 해금)

        for (int i = 0; i < SeedButtons.Length; i++)
        {
            SeedButtons[i].interactable = (i <= unlockedIndex);         // 해금된 인덱스 이하의 버튼만 활성화
        }
    }

    // 작물이 수확되거나 갯수가 변경될 때 호출되는 메서드
    public void CheckAndUnlockCrops()
    {
        int unlockedIndex = PlayerPrefs.GetInt("UnlockPlant", 0);       // 현재 해금된 인덱스 불러오기

        foreach (var cropStorage in storage.storedCropsByID)
        {
            int cropCount = cropStorage.crops.Count;                    // 해당 ID의 작물 수확 수

            // 현재 해금된 인덱스의 작물이 10개 수확되었는지 확인
            if (cropCount >= 3 && unlockedIndex < SeedButtons.Length - 1)
            {
                // 해당 cropID에 대한 해금 여부 확인
                Debug.Log("==========3개 이상 수확됨==========");
                string cropKey = "CropUnlocked_" + cropStorage.cropID;
                if (PlayerPrefs.GetInt(cropKey, 0) == 0) // 해금되지 않은 경우
                {
                    Debug.Log("==========해금처리 진행됨==========");
                    // 해금 처리
                    PlayerPrefs.SetInt(cropKey, 1); // 해당 작물 해금 상태 저장
                    unlockedIndex++; // 해금된 작물 인덱스 증가
                    PlayerPrefs.SetInt("UnlockPlant", unlockedIndex); // 전체 해금 인덱스 저장
                    SeedButtons[unlockedIndex].interactable = true; // 해당 버튼 활성화
                }
            }
        }
    }
}
