using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject UIPanel;

    public GameObject StoragePanel;
    public GameObject seedPanel;
    public GameObject FieldPanel;
    public GameObject AchievementsPanel;
    public GameObject PotionPanel;
    public GameObject DecoPanel;
    public GameObject settingPanel;

    public Button StorageButton;
    public Button seedButton;
    public Button FieldButton;
    public Button AchievementsButton;
    public Button PotionButton;
    public Button DecoButton;
    public Button settingButton;

    [SerializeField] private Button[] SeedButtons;        // 작물 선택 버튼 배열 (수확한 갯수에 따라서 해금할거라서)

    [Header("Field Price Text")]
    public Text one;
    public Text two;
    public Text three;
    public Text four;

    [Header("Character & Pot Price Text")]
    public Text SubC;
    public Text WaterC;
    public Text HarvestC;
    public Text Pot;

    [Header("Storage")]
    public Storage storage; // Storage 참조

    public Text amaranth, asparagus, balloonFlower, beat, Blueberry, bokChoy, bracken, broccoli, cabbage, Carrot, chili, Corn, cotton, Cucumber,
                deodeok, Eggplant, Garlic, ginger, Greenonion, lettuce, Melon, mugwort, napaCabbage, Onion, Pea, peanut, Pimento, Pineapple, Potato,
                Pumpkin, Radish, Rice, sesameLeaf, Strawberry, sugerCarrot, sweetPotato, swissChard, Tomato, Watermelon, mandrake, rainbowFlower,
                smileMushroom, frostFlower, sunFlower, bellFruit, jewelryBush, cloudFlower, paletteFlower, skeletonFruit, pearlTree, scissorsFlower,
                waterPear, landCoralReef, starFlower;

    private Dictionary<int, Text> cropTextFields;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameManager.instance.ResetCropKeys();
        UpdateButtons();

        UIPanel.transform.position = new Vector3(0, 0.3778f, 0);

        // 버튼에 클릭 이벤트 추가
        StorageButton.onClick.AddListener(() => TogglePanel(StoragePanel));
        seedButton.onClick.AddListener(() => TogglePanel(seedPanel));
        FieldButton.onClick.AddListener(() => TogglePanel(FieldPanel));
        AchievementsButton.onClick.AddListener(() => TogglePanel(AchievementsPanel));
        PotionButton.onClick.AddListener(() => TogglePanel(PotionPanel));
        DecoButton.onClick.AddListener(() => TogglePanel(DecoPanel));
        settingButton.onClick.AddListener(() => TogglePanel(settingPanel));

        // 시작할 때 가격 초기화
        one.text = ObjectsDatabase.CurrentPrice(0).ToString();
        two.text = ObjectsDatabase.CurrentPrice(1).ToString();
        three.text = ObjectsDatabase.CurrentPrice(2).ToString();
        four.text = ObjectsDatabase.CurrentPrice(3).ToString();
        SubC.text = ObjectsDatabase.CurrentPrice(5).ToString();
        WaterC.text = ObjectsDatabase.CurrentPrice(6).ToString();
        HarvestC.text = ObjectsDatabase.CurrentPrice(7).ToString();
        Pot.text = ObjectsDatabase.CurrentPrice(4).ToString();

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

        if (panel == PotionPanel)
        {
            PotionUIManager.instance.InitializePotionUI();
        }

        // 클릭된 패널만 현재 상태의 반대로 설정
        panel.SetActive(!panel.activeSelf);
    }

    // 모든 패널을 닫는 메서드
    public void CloseAllPanels()
    {
        StoragePanel.SetActive(false);
        seedPanel.SetActive(false);
        FieldPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        PotionPanel.SetActive(false);
        DecoPanel.SetActive(false);
        settingPanel.SetActive(false);
    }

    void UpdateStoragePanel()
    {
        foreach (var cropStorage in storage.storedCropsByID)
        {
            if (cropTextFields.TryGetValue(cropStorage.cropID, out Text cropText))
            {
                cropText.text = cropStorage.cropCount.ToString(); // 개수만 업데이트
            }
        }
    }

    // 모든 씨앗 버튼의 이미지를 설정하는 메서드
    private void ButtonSetting()
    {
        int unlockedIndex = PlayerPrefs.GetInt("UnlockPlant", 0);       // 기본값은 0 (첫 번째 작물만 해금)

        for (int i = 0; i < SeedButtons.Length; i++)
        {
            Transform childTransform = SeedButtons[i].transform.GetChild(0);

            if (childTransform != null)
            {
                Image iconImage = childTransform.GetComponent<Image>();     // 해당 버튼 작물의 이미지 오브젝트의 이미지 컴포넌트

                if (unlockedIndex >= i)     // 해금되었을 때
                {
                    iconImage.color = new Color(255, 255, 255, 255);
                }
                else                        // 해금되지 않았을 때
                {
                    iconImage.color = new Color(0, 0, 0, 0.5f);
                }
            }
        }

        Transform magic = SeedButtons[39].transform.GetChild(0);        // 마법 작물 첫 번째
        Image magicImage = magic.GetComponent<Image>();
        magicImage.color = new Color(255, 255, 255, 255);
    }

    // 모든 씨앗 버튼의 상태를 업데이트하는 메서드
    private void UpdateButtons()
    {
        int unlockedIndex = PlayerPrefs.GetInt("UnlockPlant", 0);       // 기본값은 0 (첫 번째 작물만 해금)

        for (int i = 0; i < SeedButtons.Length; i++)
        {
            if (i == 39)
            {
                SeedButtons[i].interactable = true;
            }
            else
            {
                SeedButtons[i].interactable = (i <= unlockedIndex);         // 해금된 인덱스 이하의 버튼만 활성화
            }
        }

        ButtonSetting();
    }

    // 작물이 수확되거나 갯수가 변경될 때 호출되는 메서드
    public void CheckAndUnlockCrops()
    {
        int unlockedIndex = PlayerPrefs.GetInt("UnlockPlant", 0);       // 현재 해금된 인덱스 불러오기

        foreach (var cropStorage in storage.storedCropsByID)
        {
            int id = cropStorage.cropID;
            // 해당 작물의 업적 Goal 을 충족 시켰는가 = 클리어했는가
            if (AchievementsDatabase.GetCleared(id))
            {
                Debug.LogWarning("==========해당 작물 업적 클리어==========");
                // 해당 cropID에 대한 해금 여부 확인
                string cropKey = "CropUnlocked_" + id;
                if (PlayerPrefs.GetInt(cropKey, 0) == 0) // 해금되지 않은 경우
                {
                    Debug.LogWarning("==========해금처리 진행됨==========");
                    // 해금 처리
                    PlayerPrefs.SetInt(cropKey, 1); // 해당 작물 해금 상태 저장
                    unlockedIndex++; // 해금된 작물 인덱스 증가
                    PlayerPrefs.SetInt("UnlockPlant", unlockedIndex); // 전체 해금 인덱스 저장
                    AchievementsDatabase.UnlockAchievement(id + 1);
                    AchievementManager.Instance.SafeUpdateAchievementProgress(id);
                    AchievementManager.Instance.SafeUpdateAchievementProgress(id + 1);

                    if (id >=47)    // 마법작물 심기 가능
                    {
                        AchievementsDatabase.UnlockAchievement(id + 15);        // 해당 작물 포션 업적 해금
                        AchievementManager.Instance.SafeUpdateAchievementProgress(id + 15);     // 해당 업적 UI 업데이트
                    }

                    ButtonSetting();
                    SeedButtons[unlockedIndex].interactable = true; // 해당 버튼 활성화
                    // 나중에 위의 문장 삭제하고 새 씨앗 버튼 해금 버튼으로 수정한 다음 그거 누르면 UpdateButtons 호출되도록.
                }
            }
        }
    }
}
