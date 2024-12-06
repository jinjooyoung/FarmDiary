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
        one.text = ObjectsDatabase.CurrentPrice(0).ToString();
        two.text = ObjectsDatabase.CurrentPrice(1).ToString();
        three.text = ObjectsDatabase.CurrentPrice(2).ToString();
        four.text = ObjectsDatabase.CurrentPrice(3).ToString();
        SubC.text = ObjectsDatabase.CurrentPrice(5).ToString();
        WaterC.text = ObjectsDatabase.CurrentPrice(6).ToString();
        HarvestC.text = ObjectsDatabase.CurrentPrice(7).ToString();
        Pot.text = ObjectsDatabase.CurrentPrice(4).ToString();

        // ID�� �ؽ�Ʈ �ʵ带 �����մϴ�.
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
                cropText.text = cropStorage.cropCount.ToString(); // ������ ������Ʈ
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
