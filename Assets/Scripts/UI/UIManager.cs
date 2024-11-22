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

    [SerializeField] private Button[] SeedButtons;        // �۹� ���� ��ư �迭 (��Ȯ�� ������ ���� �ر��ҰŶ�)

    public Storage storage; // Storage ����

    public Text beat, Blueberry, Carrot, Corn, Cucumber, Eggplant, Garlic, Greenonion, Melon, Onion, Pea,
                Pimento, Pineapple, Potato, Pumpkin, Radish, Rice, Strawberry, Tomato, Watermelon;

    private Dictionary<int, Text> cropTextFields;

    void Start()
    {
        GameManager.instance.ResetCropKeys();
        UpdateButtons();

        // ��ư�� Ŭ�� �̺�Ʈ �߰�
        StorageButton.onClick.AddListener(() => TogglePanel(StoragePanel));
        seedButton.onClick.AddListener(() => TogglePanel(seedPanel));
        DecoButton.onClick.AddListener(() => TogglePanel(DecoPanel));
        AchievementsButton.onClick.AddListener(() => TogglePanel(AchievementsPanel));
        CollectionButton.onClick.AddListener(() => TogglePanel(CollectionPanel));
        settingButton.onClick.AddListener(() => TogglePanel(settingPanel));

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

        // Ŭ���� �гθ� ���� ������ �ݴ�� ����
        panel.SetActive(!panel.activeSelf);
    }

    // ��� �г��� �ݴ� �޼���
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

    // ��� ���� ��ư�� �̹����� �����ϴ� �޼���
    private void ButtonSetting()
    {
        int unlockedIndex = PlayerPrefs.GetInt("UnlockPlant", 0);       // �⺻���� 0 (ù ��° �۹��� �ر�)

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
    }

    // ��� ���� ��ư�� ���¸� ������Ʈ�ϴ� �޼���
    private void UpdateButtons()
    {
        int unlockedIndex = PlayerPrefs.GetInt("UnlockPlant", 0);       // �⺻���� 0 (ù ��° �۹��� �ر�)

        for (int i = 0; i < SeedButtons.Length; i++)
        {
            SeedButtons[i].interactable = (i <= unlockedIndex);         // �رݵ� �ε��� ������ ��ư�� Ȱ��ȭ
        }

        ButtonSetting();
    }

    // �۹��� ��Ȯ�ǰų� ������ ����� �� ȣ��Ǵ� �޼���
    public void CheckAndUnlockCrops()
    {
        int unlockedIndex = PlayerPrefs.GetInt("UnlockPlant", 0);       // ���� �رݵ� �ε��� �ҷ�����

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
                    SeedButtons[unlockedIndex].interactable = true; // �ش� ��ư Ȱ��ȭ
                    // ���߿� ���� ���� �����ϰ� �� ���� ��ư �ر� ��ư���� ������ ���� �װ� ������ UpdateButtons ȣ��ǵ���.
                }
            }
        }
    }
}
