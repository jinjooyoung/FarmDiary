using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Pot;

public class PotionUIManager : MonoBehaviour
{
    public static PotionUIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //===============================================================

    [Header("���õ� ��")]
    public int currentPotID = -1;
    public Pot currentPot;

    [Header("�г� ���� ���")]
    public GameObject potionPanel;  // ���� �г�
    public GameObject materialListPanel;    // �Ϲ� ��� �г�
    public GameObject magicPanel;   // ���� ��� �г�
    public GameObject PotionAutoPanel;  // �ڵ� ���� ���� ���� �г�
    public Image magicCircleImage;  // ������ �̹���
    public Image potionImage;       // ���� �̹���
    public Text craftingTime;       // �����ϴµ� �ɸ��� �ð� or ���� �ð� or ���� ����
    [Header("��� ��ư")]
    public Button magic;            // ���� ���
    public Button material1;        // �Ϲ� ��� ù��° (��)
    public Button material2;        // �Ϲ� ��� �ι�° (��)
    public Button material3;        // �Ϲ� ��� ����° (��)
    public Button AutoButton;
    [Header("���� & ���� ��ư")]
    public Button startButton;      // ���� ��ư
    public Button endButton;        // ���� ��ư
    public Button ResetButton;      

    [Header("����")]
    public Storage storage;

    public int currentButton;      // ���õ� ��ư
    //private float remainingTime = -1.0f;

    private void Update()
    {
        if (currentPot != null)
        {
            if (currentPot.currentState == PotState.Crafting)   // �������̶��
            {
                craftingTime.text = "���� �ð� : " + Mathf.FloorToInt(currentPot.remainingTime).ToString("N0") + "��";
            }
        }
    }

    // currentPotID�� �����Ͽ� ������ҵ��� �ʱ�ȭ�ϴ� �޼���
    public void InitializePotionUI()
    {
        if (currentPotID != -1)
        {
            currentPot = PotionManager.instance.potList[currentPotID].GetComponent<Pot>();  // ������ ���� Pot ������Ʈ

            switch (currentPot.currentState)
            {
                case PotState.Empty:
                    HandleEmpty();
                    break;
                case PotState.Selecting:
                    HandleSelecting();
                    break;
                case PotState.ReadyToStart:
                    HandleReadyToStart();
                    break;
                case PotState.Crafting:
                    HandleCrafting();
                    break;
                case PotState.Completed:
                    HandleCompleted();
                    break;
            }
        }
    }


    //====================��ư�� ���� �Լ���=====================


    // �Ϲ� ��� �г��� �� ���� 1 ������ 2 �������� 3
    public void OpenBasicMaterialPanel(int ID)       // ������ ���� �ִ� ���׶�� ��ư�� ���̴� �Լ�
    {
        if (currentPot == null)
        {
            return;
        }

        currentButton = ID;
        materialListPanel.SetActive(true);
        PotionAutoPanel.SetActive(false);
        magicPanel.SetActive(false);

        // �г��� ���� �̹� ���õǾ��ٸ� false�� �ƴ϶�� â�� �����ϴ� �͸� true��
        if (currentPot.basicMaterial[currentButton - 1] != 0)
        {
            ButtonInteractBasic(false);
        }
        else
        {
            ButtonInteractionBasic();
        }
    }

    // ���� ��� �г��� ��
    public void OpenMagicMaterialPanel()       // ������ ���� �ִ� ���׶�� ��ư�� ���̴� �Լ�
    {
        if (currentPot == null)
        {
            return;
        }

        currentButton = 0;
        magicPanel.SetActive(true);
        materialListPanel.SetActive(false);
        PotionAutoPanel.SetActive(false);

        // �г��� ���� �̹� ���õǾ��ٸ� false�� �ƴ϶�� â�� �����ϴ� �͸� true��
        if (currentPot.magicID != -1)
        {
            ButtonInteractMagic(false);
        }
        else
        {
            ButtonInteractionMagic();
        }
    }

    // �۹� ID
    public void SelectMaterial(int ID)    // ��� �гο� �ִ� �� ��� ��ư�� ���̴� �Լ�
    {
        if (ID <= 47)   // �Ϲ� �۹��̶��
        {
            // pot �迭�� �Ű��ֱ�, ��ư ���ҽ� ����
            switch(currentButton)
            {
                case 1:
                    material1.image.sprite = LoadIcon(ID);
                    material1.image.color = Color.white;
                    currentPot.basicMaterial[0] = ID;
                    break;
                case 2:
                    material2.image.sprite = LoadIcon(ID);
                    material2.image.color = Color.white;
                    currentPot.basicMaterial[1] = ID;
                    break;
                case 3:
                    material3.image.sprite = LoadIcon(ID);
                    material3.image.color = Color.white;
                    currentPot.basicMaterial[2] = ID;
                    break;
            }

            // �۹� ���������� false��
            ButtonInteractBasic(false);
        }
        else
        {
            // pot�� �Ű��ֱ�
            if (currentButton == 0)
            {
                currentPot.magicID = ID;

                magic.image.sprite = Resources.Load<Sprite>($"Achievements/Rewards/Reward_{currentPot.magicID - 1}");
                magic.image.color = Color.white;
                
                currentPot.totalCraftingTime = PotionDatabase.GetCraftingTime(currentPot.magicID);

                potionImage.sprite = Resources.Load<Sprite>($"Achievements/Icons/Achievement_{currentPot.magicID + 14}");

                if (AchievementsDatabase.GetCleared(currentPot.magicID + 14))
                {
                    potionImage.color = Color.white;
                }
                else
                {
                    potionImage.color = new Color(1f, 1f, 1f, 0f);
                }

                // �۹� ���������� false��
                ButtonInteractMagic(false);
            }
        }

        if (NotSelectedMaterial())  // �� ���õǾ��ٸ�
        {
            currentPot.ChangeState(PotState.ReadyToStart);
            magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_2");
            magicCircleImage.color = Color.yellow;       // ���� �غ� �Ϸ�Ǹ� �����\
            startButton.interactable = true;
        }
        else
        {
            currentPot.ChangeState(PotState.Selecting);
            magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_1");
            startButton.interactable = false;
        }
    }

    // ���� �ʱ�ȭ ��ư
    public void ClearSelectMaterial()
    {
        // UI �ʱ�ȭ
        magic.image.color = new Color(1f, 1f, 1f, 0f);
        material1.image.color = new Color(1f, 1f, 1f, 0f);
        material2.image.color = new Color(1f, 1f, 1f, 0f);
        material3.image.color = new Color(1f, 1f, 1f, 0f);

        // �� �ʱ�ȭ
        currentPot.magicID = -1;
        currentPot.basicMaterial[0] = 0;
        currentPot.basicMaterial[1] = 0;
        currentPot.basicMaterial[2] = 0;
        currentPot.totalCraftingTime = 0;
        currentPot.remainingTime = 0;

        currentPot.currentState = PotState.Empty;

        magic.interactable = true;
        material1.interactable = true;
        material2.interactable = true;
        material3.interactable = true;
        ResetButton.interactable = true;
        AutoButton.interactable = true;

        // ���� �ð� UI �ʱ�ȭ
        craftingTime.text = null;
        potionImage.sprite = Resources.Load<Sprite>("Achievements/Rewards/Reward_Q");

        magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_0");
        magicCircleImage.color = Color.white;

        startButton.interactable = false;
        endButton.interactable = false;
    }

    // ���� ��ư�� ���̴� �Լ�
    public void StartCrafting()
    {
        currentPot.remainingTime = currentPot.totalCraftingTime;

        startButton.interactable = false;
        magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_2");
        magicCircleImage.color = Color.green;       // ���� �߿��� �ʷϻ�

        magic.interactable = false;
        material1.interactable = false;
        material2.interactable = false;
        material3.interactable = false;
        AutoButton.interactable = false;

        SubtractCrop(currentPot.magicID);
        SubtractCrop(currentPot.basicMaterial[0]);
        SubtractCrop(currentPot.basicMaterial[1]);
        SubtractCrop(currentPot.basicMaterial[2]);

        currentPot.ChangeState(PotState.Crafting);
        ResetButton.interactable = false;
        currentPot.animator.SetBool("IsCrafting", true);
        currentPot.check = true;
    }

    // ����=��Ȯ ��ư�� ���̴� �Լ�
    public void HarvestingPotion()
    {
        int potionPrice = PotionDatabase.GetPotionPrice(currentPot.magicID);

        int selectedCropIDs = currentPot.basicMaterial[0] + currentPot.basicMaterial[1] + currentPot.basicMaterial[2];

        if (selectedCropIDs < 39)
        {
            GameManager.AddCoins(potionPrice + 100);
        }
        else if (selectedCropIDs >= 39 && selectedCropIDs < 48)
        {
            GameManager.AddCoins(potionPrice + 800);
        }
        else if (selectedCropIDs >= 48 && selectedCropIDs < 57)
        {
            GameManager.AddCoins(potionPrice + 2500);
        }
        else if (selectedCropIDs >= 57 && selectedCropIDs < 66)
        {
            GameManager.AddCoins(potionPrice + 7000);
        }
        else if (selectedCropIDs >= 66 && selectedCropIDs < 75)
        {
            GameManager.AddCoins(potionPrice + 16000);
        }
        else if (selectedCropIDs >= 75 && selectedCropIDs < 84)
        {
            GameManager.AddCoins(potionPrice + 30000);
        }
        else if (selectedCropIDs >= 84 && selectedCropIDs < 93)
        {
            GameManager.AddCoins(potionPrice + 42000);
        }
        else if (selectedCropIDs >= 93 && selectedCropIDs < 102)
        {
            GameManager.AddCoins(potionPrice + 59000);
        }
        else if (selectedCropIDs >= 102 && selectedCropIDs < 111)
        {
            GameManager.AddCoins(potionPrice + 76000);
        }
        else if (selectedCropIDs >= 111 && selectedCropIDs < 120)
        {
            GameManager.AddCoins(potionPrice + 100000);
        }
        else if (selectedCropIDs >= 120 && selectedCropIDs < 129)
        {
            GameManager.AddCoins(potionPrice + 130000);
        }
        else if (selectedCropIDs >= 129 && selectedCropIDs < 138)
        {
            GameManager.AddCoins(potionPrice + 180000);
        }
        else if (selectedCropIDs == 138)
        {
            GameManager.AddCoins(potionPrice + 250000);
        }

        AchievementsDatabase.AddProgressToAchievement(currentPot.magicID + 14, 1);
        AchievementsDatabase.UnlockAchievement(currentPot.magicID + 15);
        
        ClearSelectMaterial();
        Debug.Log("���� ���� ��Ȯ�Ϸ�");
    }

    // �ڵ� ���� �г� ���� ��ư
    public void OpenAutoSelectPanel()
    {
        if (currentPot == null)
        {
            return;
        }

        PotionAutoPanel.SetActive(true);
        magicPanel.SetActive(false);
        materialListPanel.SetActive(false);

        ButtonInteractPotion();
    }

    // �ڵ����� �Ϲ� ��� �ְ� ���� �����ϴ� ��ư
    public void AutoSelectAndCrafting(int magicID)
    {
        currentPot.magicID = magicID;   // �ܿ� ���� �̵�

        // �� UI �гο� ������Ʈ
        magic.image.sprite = Resources.Load<Sprite>($"Achievements/Rewards/Reward_{currentPot.magicID - 1}");
        magic.image.color = Color.white;

        // ���۽ð� UI
        currentPot.totalCraftingTime = PotionDatabase.GetCraftingTime(currentPot.magicID);

        // ���� �̹��� UI
        potionImage.sprite = Resources.Load<Sprite>($"Achievements/Icons/Achievement_{currentPot.magicID + 14}");



        if (AchievementsDatabase.GetCleared(currentPot.magicID + 14))
        {
            potionImage.color = Color.white;
        }
        else
        {
            potionImage.color = new Color(1f, 1f, 1f, 0f);
        }

        // �Ϲ� �۹� �ڵ� ����
        int selectCount = 0;

        // â�� �ڿ������� ���鼭 ��� �۹����� �ڵ� �Ҵ�
        for(int i = storage.storedCropsByID.Count - 1; i >= 0; i--)
        {
            if (storage.storedCropsByID[i].cropCount > 0 && storage.storedCropsByID[i].cropID < 48)
            {
                if (selectCount == 0)
                {
                    currentPot.basicMaterial[0] = storage.storedCropsByID[i].cropID;
                    material1.image.sprite = LoadIcon(storage.storedCropsByID[i].cropID);
                    material1.image.color = Color.white;
                    selectCount++;
                }
                else if (selectCount == 1)
                {
                    currentPot.basicMaterial[1] = storage.storedCropsByID[i].cropID;
                    material2.image.sprite = LoadIcon(storage.storedCropsByID[i].cropID);
                    material2.image.color = Color.white;
                    selectCount++;
                }
                else if (selectCount == 2)
                {
                    currentPot.basicMaterial[2] = storage.storedCropsByID[i].cropID;
                    material3.image.sprite = LoadIcon(storage.storedCropsByID[i].cropID);
                    material3.image.color = Color.white;
                    selectCount++;
                }
                else if (selectCount >= 3)
                {
                    continue;
                }
            }
        }

        if (NotSelectedMaterial())  // �� ���õǾ��ٸ�
        {
            // �ٷ� ���� ����
            StartCrafting();
        }
        else
        {
            currentPot.ChangeState(PotState.Selecting);
            magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_1");
            startButton.interactable = false;
        }

        PotionAutoPanel.SetActive(false);
    }

    //====================State �� �ʱ�ȭ �޼���==========

    public void HandleEmpty()
    {
        PotionAutoPanel.SetActive(false);
        materialListPanel.SetActive(false);
        magicPanel.SetActive(false);
        magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_0");
        magicCircleImage.color = Color.white;
        potionImage.sprite = Resources.Load<Sprite>("Achievements/Rewards/Reward_Q");
        craftingTime.text = null;
        magic.image.color = new Color(1f, 1f, 1f, 0f);
        material1.image.color = new Color(1f, 1f, 1f, 0f);
        material2.image.color = new Color(1f, 1f, 1f, 0f);
        material3.image.color = new Color(1f, 1f, 1f, 0f);

        magic.interactable = true;
        material1.interactable = true;
        material2.interactable = true;
        material3.interactable = true;
        AutoButton.interactable = true;
        ResetButton.interactable = true;
        startButton.interactable = false;
        endButton.interactable = false;
        currentButton = -1;
    }

    public void HandleSelecting()
    {
        PotionAutoPanel.SetActive(false);
        materialListPanel.SetActive(false);
        magicPanel.SetActive(false);
        magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_1");
        magicCircleImage.color = Color.white;

        magic.interactable = true;
        material1.interactable = true;
        material2.interactable = true;
        material3.interactable = true;
        AutoButton.interactable = true;

        SelectedMagic();

        if (currentPot.magicID != -1)
        {
            magic.image.sprite = Resources.Load<Sprite>($"Achievements/Rewards/Reward_{currentPot.magicID - 1}");
            magic.image.color = Color.white;
        }

        material1.image.color = new Color(1f, 1f, 1f, 0f);
        material2.image.color = new Color(1f, 1f, 1f, 0f);
        material3.image.color = new Color(1f, 1f, 1f, 0f);

        if (currentPot.basicMaterial[0] != 0)   // ù��° �Ϲ� �۹� ���õ�
        {
            material1.image.sprite = LoadIcon(currentPot.basicMaterial[0]);
            material1.image.color = Color.white;
        }
        if (currentPot.basicMaterial[1] != 0)   // �ι�° �Ϲ� �۹� ���õ�
        {
            material2.image.sprite = LoadIcon(currentPot.basicMaterial[1]);
            material2.image.color = Color.white;
        }
        if (currentPot.basicMaterial[2] != 0)   // ����° �Ϲ� �۹� ���õ�
        {
            material3.image.sprite = LoadIcon(currentPot.basicMaterial[2]);
            material3.image.color = Color.white;
        }

        startButton.interactable = false;
        ResetButton.interactable = true;
        endButton.interactable = false;
        currentButton = -1;
    }

    public void HandleReadyToStart()
    {
        PotionAutoPanel.SetActive(false);
        materialListPanel.SetActive(false);
        magicPanel.SetActive(false);
        magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_2");
        magicCircleImage.color = Color.yellow;

        AutoButton.interactable = true;
        ResetButton.interactable = true;
        magic.interactable = true;
        material1.interactable = true;
        material2.interactable = true;
        material3.interactable = true;
        SelectedMagic();

        magic.image.color = Color.white;
        material1.image.color = Color.white;
        material2.image.color = Color.white;
        material3.image.color = Color.white;

        magic.image.sprite = Resources.Load<Sprite>($"Achievements/Rewards/Reward_{currentPot.magicID - 1}");
        material1.image.sprite = LoadIcon(currentPot.basicMaterial[0]);
        material2.image.sprite = LoadIcon(currentPot.basicMaterial[1]);
        material3.image.sprite = LoadIcon(currentPot.basicMaterial[2]);
        startButton.interactable = true;
        endButton.interactable = false;
        currentButton = -1;
    }

    public void HandleCrafting()
    {
        PotionAutoPanel.SetActive(false);
        materialListPanel.SetActive(false);
        magicPanel.SetActive(false);
        magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_2");
        magicCircleImage.color = Color.green;

        potionImage.sprite = Resources.Load<Sprite>($"Achievements/Icons/Achievement_{currentPot.magicID + 14}");
        if (AchievementsDatabase.GetCleared(currentPot.magicID + 14))
        {
            potionImage.color = Color.white;
        }
        else
        {
            potionImage.color = new Color(0f, 0f, 0f, 0.5f);
        }
        magic.image.sprite = Resources.Load<Sprite>($"Achievements/Rewards/Reward_{currentPot.magicID-1}");

        craftingTime.text = "���� �ð� : " + Mathf.FloorToInt(currentPot.remainingTime).ToString("N0") + "��";

        material1.image.sprite = LoadIcon(currentPot.basicMaterial[0]);
        material2.image.sprite = LoadIcon(currentPot.basicMaterial[1]);
        material3.image.sprite = LoadIcon(currentPot.basicMaterial[2]);

        magic.image.color = Color.white;
        material1.image.color = Color.white;
        material2.image.color = Color.white;
        material3.image.color = Color.white;

        AutoButton.interactable = false;
        magic.interactable = false;
        material1.interactable = false;
        material2.interactable = false;
        material3.interactable = false;
        startButton.interactable = false;
        endButton.interactable = false;
        ResetButton.interactable = false;
        currentButton = -1;
    }

    public void HandleCompleted()
    {
        PotionAutoPanel.SetActive(false);
        materialListPanel.SetActive(false);
        magicPanel.SetActive(false);
        magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_2");
        magicCircleImage.color = Color.blue;

        potionImage.sprite = Resources.Load<Sprite>($"Achievements/Icons/Achievement_{currentPot.magicID + 14}");
        if (AchievementsDatabase.GetCleared(currentPot.magicID + 14))
        {
            potionImage.color = Color.white;
        }
        else
        {
            potionImage.color = new Color(0f, 0f, 0f, 0.5f);
        }
        magic.image.sprite = Resources.Load<Sprite>($"Achievements/Rewards/Reward_{currentPot.magicID-1}");

        craftingTime.text = "���� �Ϸ�";

        currentPot.totalCraftingTime = 0;
        currentPot.remainingTime = 0;

        material1.image.sprite = LoadIcon(currentPot.basicMaterial[0]);
        material2.image.sprite = LoadIcon(currentPot.basicMaterial[1]);
        material3.image.sprite = LoadIcon(currentPot.basicMaterial[2]);

        magic.image.color = Color.white;
        material1.image.color = Color.white;
        material2.image.color = Color.white;
        material3.image.color = Color.white;

        magic.interactable = false;
        material1.interactable = false;
        material2.interactable = false;
        material3.interactable = false;
        AutoButton.interactable = false;
        startButton.interactable = false;
        endButton.interactable = true;
        ResetButton.interactable = false;
        currentButton = -1;
    }

    //====================��Ÿ �޼���=====================

    // �Ϲ� �۹� ���� ��ư���� ��ȣ�ۿ� ������ �ʱ�ȭ
    public void ButtonInteractionBasic()
    {
        // ���õ� �۹��� false��
        // �θ� ������Ʈ�� ��� �ڽĵ��� ��������
        Button[] buttons = materialListPanel.GetComponentsInChildren<Button>();

        // �� ��ư�� ���� ��ȣ�ۿ��� false�� ����
        for (int i = 0; i < buttons.Length; i++)
        {
            if (DoesCropExist(i + 9) || i == buttons.Length - 1)
            {
                if (i + 9 == currentPot.basicMaterial[0] || i + 9 == currentPot.basicMaterial[1] || i + 9 == currentPot.basicMaterial[2])
                {
                    buttons[i].interactable = false;
                }
                else
                {
                    buttons[i].interactable = true;
                }
            }
            else
            {
                buttons[i].interactable = false;
            }
        }
    }

    public void ButtonInteractBasic(bool a)
    {
        // �θ� ������Ʈ�� ��� �ڽĵ��� ��������
        Button[] buttons = materialListPanel.GetComponentsInChildren<Button>();

        // �� ��ư�� ���� ��ȣ�ۿ��� �μ��� ����
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = a;

            if (i == buttons.Length - 1)
            {
                buttons[i].interactable = true;
            }
        }
    }

    // ���� �۹� ���� ��ư���� ��ȣ�ۿ� ������ �ʱ�ȭ
    public void ButtonInteractionMagic()
    {
        // �θ� ������Ʈ�� ��� �ڽĵ��� ��������
        Button[] buttons = magicPanel.GetComponentsInChildren<Button>();

        // �� ��ư�� ���� ��ȣ�ۿ��� �μ��� ����
        for (int i = 0; i < buttons.Length; i++)
        {
            if (DoesCropExist(i + 48) || i == buttons.Length - 1)
            {
                buttons[i].interactable = true;
            }
            else
            {
                buttons[i].interactable = false;
            }
        }
    }

    public void ButtonInteractMagic(bool a)
    {
        // �θ� ������Ʈ�� ��� �ڽĵ��� ��������
        Button[] buttons = magicPanel.GetComponentsInChildren<Button>();

        // �� ��ư�� ���� ��ȣ�ۿ��� �ϴ� �� false�� ����
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = a;

            if (i == buttons.Length - 1)
            {
                buttons[i].interactable = true;
            }
        }
    }

    public void ButtonInteractPotion()
    {
        // �θ� ������Ʈ�� ��� �ڽĵ��� ��������
        Button[] buttons = PotionAutoPanel.GetComponentsInChildren<Button>();

        // �� ��ư�� ���� ��ȣ�ۿ��� false�� ����
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;

            Transform button = PotionAutoPanel.transform.GetChild(i);
            Transform ImageOBJ = button.transform.GetChild(0);

            // �ڽ� ������Ʈ�� ������ �ִ� Image ������Ʈ�� ��������
            Image image = ImageOBJ.GetComponent<Image>();

            if (image != null)
            {
                if (AchievementsDatabase.GetCleared(i + 62))
                {
                    image.color = Color.white;
                }
                else
                {
                    image.color = new Color(0f, 0f, 0f, 0.5f);
                }
            }
            else
            {
                Debug.Log($"��ư {i}���� �̹����� �����ϴ�.");
            }
        }

        foreach (var cropStorage in storage.storedCropsByID)
        {
            int cropID = cropStorage.cropID;

            // �����۹���
            if (cropID >= 48 && cropID <= 62)
            {
                // 1�� �̻� �ִٸ�
                if (cropStorage.cropCount > 0)
                {
                    // �ش� ��ư �ε��� ��� (0��° ��ư�� 48�� ��Ī��)
                    int buttonIndex = cropID - 48;

                    // ��ȿ�� ��ư �ε������� Ȯ��
                    if (buttonIndex >= 0 && buttonIndex < buttons.Length)
                    {
                        // �ش� ��ư�� ��ȣ�ۿ� �����ϰ� ����
                        buttons[buttonIndex].interactable = true;
                    }
                }
            }
        }
    }

    // ���� �۹� ���� ������ ���� �ʱ�ȭ
    public void SelectedMagic()
    {
        if (currentPot.magicID != -1)   // �����۹��� ���õǾ��ٸ�
        {
            potionImage.sprite = Resources.Load<Sprite>($"Achievements/Icons/Achievement_{currentPot.magicID + 14}");
            if (AchievementsDatabase.GetCleared(currentPot.magicID + 14))
            {
                potionImage.color = Color.white;
            }
            else
            {
                potionImage.color = new Color(0f, 0f, 0f, 0.5f);
            }
            magic.image.sprite = Resources.Load<Sprite>($"Achievements/Rewards/Reward_{currentPot.magicID-1}");
            currentPot.totalCraftingTime = PotionDatabase.GetCraftingTime(currentPot.magicID);
            craftingTime.text = "���� �ð� : " + PotionDatabase.GetCraftingTime(currentPot.magicID).ToString("N0") + "��";
        }
        else    // �����۹� ���� �� ��
        {
            craftingTime.text = null;
            potionImage.sprite = Resources.Load<Sprite>("Achievements/Rewards/Reward_Q");
            magic.image.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    // �ϳ��� ���� �� �� �׸��� �ִٸ� false �� ���õǾ��ٸ� true
    public bool NotSelectedMaterial()
    {
        // basicMaterial ����Ʈ���� 0�� �ϳ��� �����ϸ� false ��ȯ
        foreach (int material in currentPot.basicMaterial)
        {
            if (material == 0)
            {
                return false;
            }
        }

        // ���� �۹��� ���� �� �ƴٸ� false
        if (currentPot.magicID == -1)
        {
            return false;
        }

        return true; // 0�� ������ true ��ȯ
    }

    // Ư�� cropID�� â�� 1�� �̻� �����ϸ� true �׿ܿ��� false
    public bool DoesCropExist(int cropID)
    {
        CropStorage crop = storage.storedCropsByID.Find(crop => crop.cropID == cropID);

        return crop != null && crop.cropCount >= 1;
    }

    public void SubtractCrop(int ID)
    {
        CropStorage crop = storage.storedCropsByID.Find(crop => crop.cropID == ID);
        crop.cropCount--;
    }

    // �۹� ID �� ���ҽ� �ε�
    private Sprite LoadIcon(int id)
    {
        // ������ ��� �Լ� �׳� �״�� ���� ��
        return Resources.Load<Sprite>($"Achievements/Icons/Achievement_{id}");
    }
}
