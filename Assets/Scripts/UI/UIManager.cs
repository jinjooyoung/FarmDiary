using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

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

    [SerializeField] public Button[] SeedButtons;        // �۹� ���� ��ư �迭 (��Ȯ�� ������ ���� �ر��ҰŶ�)

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
    public Storage storage; // Storage ����

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
        UpdateButtons();

        // ��ư�� Ŭ�� �̺�Ʈ �߰�
        StorageButton.onClick.AddListener(() => TogglePanel(StoragePanel));
        seedButton.onClick.AddListener(() => TogglePanel(seedPanel));
        FieldButton.onClick.AddListener(() => TogglePanel(FieldPanel));
        AchievementsButton.onClick.AddListener(() => TogglePanel(AchievementsPanel));
        PotionButton.onClick.AddListener(() => TogglePanel(PotionPanel));
        DecoButton.onClick.AddListener(() => TogglePanel(DecoPanel));
        settingButton.onClick.AddListener(() => TogglePanel(settingPanel));

        // ������ �� ���� �ʱ�ȭ
        one.text = ObjectsDatabase.CurrentPrice(0).ToString("N0");
        two.text = ObjectsDatabase.CurrentPrice(1).ToString("N0");
        three.text = ObjectsDatabase.CurrentPrice(2).ToString("N0");
        four.text = ObjectsDatabase.CurrentPrice(3).ToString("N0");
        SubC.text = ObjectsDatabase.CurrentPrice(5).ToString("N0");
        WaterC.text = ObjectsDatabase.CurrentPrice(6).ToString("N0");
        HarvestC.text = ObjectsDatabase.CurrentPrice(7).ToString("N0");
        Pot.text = ObjectsDatabase.CurrentPrice(4).ToString("N0");

        // ID�� �ؽ�Ʈ �ʵ带 �����մϴ�.
        cropTextFields = new Dictionary<int, Text>
        {
            { 9, amaranth }, { 10, asparagus }, { 11, balloonFlower }, { 12, beat }, { 13, Blueberry },
            { 14, bokChoy }, { 15, bracken }, { 16, broccoli }, { 17, cabbage }, { 18, Carrot },
            { 19, chili }, { 20, Corn }, { 21, cotton }, { 22, Cucumber }, { 23, deodeok },
            { 24, Eggplant }, { 25, Garlic }, { 26, ginger }, { 27, Greenonion }, { 28, lettuce },
            { 29, Melon }, { 30, mugwort }, { 31, napaCabbage }, { 32, Onion }, { 33, Pea },
            { 34, peanut }, { 35, Pimento }, { 36, Pineapple }, { 37, Potato }, { 38, Pumpkin },
            { 39, Radish }, { 40, Rice }, { 41, sesameLeaf }, { 42, Strawberry }, { 43, sugerCarrot },
            { 44, sweetPotato }, { 45, swissChard }, { 46, Tomato }, { 47, Watermelon }, { 48, mandrake },
            { 49, rainbowFlower }, { 50, smileMushroom }, { 51, frostFlower }, { 52, sunFlower }, { 53, bellFruit },
            { 54, jewelryBush }, { 55, cloudFlower }, { 56, paletteFlower }, { 57, skeletonFruit }, { 58, pearlTree },
            { 59, scissorsFlower }, { 60, waterPear }, { 61, landCoralReef }, { 62, starFlower }
        };
    }

    void Update()
    {
        UpdateStoragePanel();
    }

    // �г��� Ȱ��ȭ ���¸� ����ϴ� �޼���
    public void TogglePanel(GameObject panel)
    {
        // ��� �г��� ���� �ݱ�
        CloseAllPanels();

        if (panel == PotionPanel)
        {
            PotionUIManager.instance.InitializePotionUI();
        }

        // Ŭ���� �гθ� ���� ������ �ݴ�� ����
        panel.SetActive(!panel.activeSelf);
    }

    // ��� �г��� �ݴ� �޼���
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
                cropText.text = cropStorage.cropCount.ToString("N0"); // ������ ������Ʈ
            }
        }
    }

    // ��� ���� ��ư�� �̹����� �����ϴ� �޼���
    private void ButtonSetting()
    {
        int unlockedIndex = PlayerPrefs.GetInt("UnlockPlant", 2);       // �⺻���� 2 (�� ��° �۹����� �ر�)

        for (int i = 0; i < SeedButtons.Length; i++)
        {
            Transform childTransform = SeedButtons[i].transform.GetChild(0);

            if (childTransform != null)
            {
                Image iconImage = childTransform.GetComponent<Image>();     // �ش� ��ư �۹��� �̹��� ������Ʈ�� �̹��� ������Ʈ

                if (unlockedIndex >= i)     // �رݵǾ��� ��
                {
                    iconImage.color = new Color(255, 255, 255, 255);
                }
                else                        // �رݵ��� �ʾ��� ��
                {
                    iconImage.color = new Color(0, 0, 0, 0.5f);
                }
            }
        }

        Transform magic = SeedButtons[39].transform.GetChild(0);        // ���� �۹� ù ��°
        Image magicImage = magic.GetComponent<Image>();
        magicImage.color = new Color(255, 255, 255, 255);
    }

    // ��� ���� ��ư�� ���¸� ������Ʈ�ϴ� �޼���
    private void UpdateButtons()
    {
        int unlockedIndex = PlayerPrefs.GetInt("UnlockPlant", 2);       // �⺻���� 2 (�� ��° �۹����� �ر�)

        for (int i = 0; i < SeedButtons.Length; i++)
        {
            if (i == 39)    // �ǵ巹��ũ�� �׻� true
            {
                SeedButtons[i].interactable = true;
            }
            else
            {
                SeedButtons[i].interactable = (i <= unlockedIndex);         // �رݵ� �ε��� ������ ��ư�� Ȱ��ȭ
            }
        }

        ButtonSetting();
    }

    // �۹��� ��Ȯ�ǰų� ������ ����� �� ȣ��Ǵ� �޼���
    public void CheckAndUnlockCrops()
    {
        int unlockedIndex = PlayerPrefs.GetInt("UnlockPlant", 2);       // ���� �رݵ� �ε��� �ҷ�����

        foreach (var cropStorage in storage.storedCropsByID)
        {
            int id = cropStorage.cropID;
            // �ش� �۹��� ���� Goal �� ���� ���״°� = Ŭ�����ߴ°�
            if (AchievementsDatabase.GetCleared(id))
            {
                Debug.LogWarning("==========�ش� �۹� ���� Ŭ����==========");
                // �ش� cropID�� ���� �ر� ���� Ȯ��
                string cropKey = "CropUnlocked_" + id;
                if (PlayerPrefs.GetInt(cropKey, 0) == 0) // �رݵ��� ���� ���
                {
                    Debug.LogWarning("==========�ر�ó�� �����==========");
                    // �ر� ó��
                    PlayerPrefs.SetInt(cropKey, 1); // �ش� �۹� �ر� ���� ����
                    unlockedIndex++; // �رݵ� �۹� �ε��� ����
                    PlayerPrefs.SetInt("UnlockPlant", unlockedIndex); // ��ü �ر� �ε��� ����
                    AchievementsDatabase.UnlockAchievement(id + 1);
                    AchievementManager.Instance.SafeUpdateAchievementProgress(id);
                    AchievementManager.Instance.SafeUpdateAchievementProgress(id + 1);

                    if (id >=47)    // �����۹� �ɱ� ����
                    {
                        AchievementsDatabase.UnlockAchievement(id + 15);        // �ش� �۹� ���� ���� �ر�
                        AchievementManager.Instance.SafeUpdateAchievementProgress(id + 15);     // �ش� ���� UI ������Ʈ
                    }

                    ButtonSetting();
                    SeedButtons[unlockedIndex].interactable = true; // �ش� ��ư Ȱ��ȭ
                    // ���߿� ���� ���� �����ϰ� �� ���� ��ư �ر� ��ư���� ������ ���� �װ� ������ UpdateButtons ȣ��ǵ���.
                }
            }
        }
    }
}
