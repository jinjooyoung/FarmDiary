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

    public Storage storage; // Storage 참조

    public Text beat, Blueberry, Carrot, Corn, Cucumber, Eggplant, Garlic, Greenonion, Melon, Onion, Pea,
                Pimento, Pineapple, Potato, Pumpkin, Radish, Rice, Strawberry, Tomato, Watermelon;

    private Dictionary<int, Text> cropTextFields;

    void Start()
    {
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
}
