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

    [Header("선택된 솥")]
    public int currentPotID = -1;
    public Pot currentPot;

    [Header("패널 구성 요소")]
    public GameObject potionPanel;  // 포션 패널
    public GameObject materialListPanel;    // 일반 재료 패널
    public GameObject magicPanel;   // 마법 재료 패널
    public Image magicCircleImage;  // 마법진 이미지
    public Image potionImage;       // 포션 이미지
    public Text craftingTime;       // 제작하는데 걸리는 시간 or 남은 시간 or 현재 상태
    [Header("재료 버튼")]
    public Button magic;            // 마법 재료
    public Button material1;        // 일반 재료 첫번째 (상)
    public Button material2;        // 일반 재료 두번째 (좌)
    public Button material3;        // 일반 재료 세번째 (우)
    [Header("시작 & 종료 버튼")]
    public Button startButton;      // 시작 버튼
    public Button endButton;        // 종료 버튼

    [Header("참조")]
    public Storage storage;

    public int currentButton;      // 선택된 버튼
    //private float remainingTime = -1.0f;

    private void Update()
    {
        if (currentPot != null)
        {
            if (currentPot.currentState == PotState.Crafting)   // 제작중이라면
            {
                craftingTime.text = "남은 시간 : " + Mathf.FloorToInt(currentPot.remainingTime).ToString("N0") + "초";
            }
        }
    }

    // currentPotID를 참고하여 구성요소들을 초기화하는 메서드
    public void InitializePotionUI()
    {
        if (currentPotID != -1)
        {
            currentPot = PotionManager.instance.potList[currentPotID].GetComponent<Pot>();  // 선택한 솥의 Pot 컴포넌트

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


    //====================버튼에 붙일 함수들=====================


    // 일반 재료 패널을 엶 위가 1 왼쪽이 2 오른쪽이 3
    public void OpenBasicMaterialPanel(int ID)       // 마법진 위에 있는 동그라미 버튼에 붙이는 함수
    {
        if (currentPot == null)
        {
            return;
        }

        currentButton = ID;
        materialListPanel.SetActive(true);
        magicPanel.SetActive(false);

        // 패널을 열때 이미 선택되었다면 false로 아니라면 창고에 존재하는 것만 true로
        if (currentPot.basicMaterial[currentButton - 1] != 0)
        {
            ButtonInteractBasic(false);
        }
        else
        {
            ButtonInteractionBasic();
        }
    }

    // 마법 재료 패널을 엶
    public void OpenMagicMaterialPanel()       // 마법진 위에 있는 동그라미 버튼에 붙이는 함수
    {
        if (currentPot == null)
        {
            return;
        }

        currentButton = 0;
        magicPanel.SetActive(true);
        materialListPanel.SetActive(false);

        // 패널을 열때 이미 선택되었다면 false로 아니라면 창고에 존재하는 것만 true로
        if (currentPot.magicID != -1)
        {
            ButtonInteractMagic(false);
        }
        else
        {
            ButtonInteractionMagic();
        }
    }

    // 작물 ID
    public void SelectMaterial(int ID)    // 재료 패널에 있는 각 재료 버튼에 붙이는 함수
    {
        if (ID <= 47)   // 일반 작물이라면
        {
            // pot 배열에 옮겨주기, 버튼 리소스 변경
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

            // 작물 선택했으니 false로
            ButtonInteractBasic(false);
        }
        else
        {
            // pot에 옮겨주기
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

                // 작물 선택했으니 false로
                ButtonInteractMagic(false);
            }
        }

        if (NotSelectedMaterial())  // 다 선택되었다면
        {
            currentPot.ChangeState(PotState.ReadyToStart);
            magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_2");
            magicCircleImage.color = Color.yellow;       // 제작 준비 완료되면 노란색\
            startButton.interactable = true;
        }
        else
        {
            currentPot.ChangeState(PotState.Selecting);
            magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_1");
            startButton.interactable = false;
        }
    }

    // 선택 초기화 버튼
    public void ClearSelectMaterial()
    {
        // UI 초기화
        magic.image.color = new Color(1f, 1f, 1f, 0f);
        material1.image.color = new Color(1f, 1f, 1f, 0f);
        material2.image.color = new Color(1f, 1f, 1f, 0f);
        material3.image.color = new Color(1f, 1f, 1f, 0f);

        // 솥 초기화
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

        // 제작 시간 UI 초기화
        craftingTime.text = null;
        potionImage.sprite = Resources.Load<Sprite>("Achievements/Rewards/Reward_Q");

        magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_0");
        magicCircleImage.color = Color.white;

        startButton.interactable = false;
        endButton.interactable = false;
    }

    // 시작 버튼에 붙이는 함수
    public void StartCrafting()
    {
        currentPot.remainingTime = currentPot.totalCraftingTime;

        startButton.interactable = false;
        magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_2");
        magicCircleImage.color = Color.green;       // 제작 중에는 초록색

        magic.interactable = false;
        material1.interactable = false;
        material2.interactable = false;
        material3.interactable = false;

        SubtractCrop(currentPot.magicID);
        SubtractCrop(currentPot.basicMaterial[0]);
        SubtractCrop(currentPot.basicMaterial[1]);
        SubtractCrop(currentPot.basicMaterial[2]);

        currentPot.ChangeState(PotState.Crafting);
    }

    // 종료=수확 버튼에 붙이는 함수
    public void HarvestingPotion()
    {
        GameManager.AddCoins(PotionDatabase.GetPotionPrice(currentPot.magicID));
        AchievementsDatabase.AddProgressToAchievement(currentPot.magicID + 14, 1);
        AchievementsDatabase.UnlockAchievement(currentPot.magicID + 15);
        
        ClearSelectMaterial();
        Debug.Log("포션 제작 수확완료");
    }

    //====================State 별 초기화 메서드==========

    public void HandleEmpty()
    {
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

        startButton.interactable = false;
        endButton.interactable = false;
        currentButton = -1;
    }

    public void HandleSelecting()
    {
        materialListPanel.SetActive(false);
        magicPanel.SetActive(false);
        magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_1");
        magicCircleImage.color = Color.white;

        magic.interactable = true;
        material1.interactable = true;
        material2.interactable = true;
        material3.interactable = true;

        SelectedMagic();

        material1.image.color = new Color(1f, 1f, 1f, 0f);
        material2.image.color = new Color(1f, 1f, 1f, 0f);
        material3.image.color = new Color(1f, 1f, 1f, 0f);

        if (currentPot.basicMaterial[0] != 0)   // 첫번째 일반 작물 선택됨
        {
            material1.image.sprite = LoadIcon(currentPot.basicMaterial[0]);
            material1.image.color = Color.white;
        }
        if (currentPot.basicMaterial[1] != 0)   // 두번째 일반 작물 선택됨
        {
            material2.image.sprite = LoadIcon(currentPot.basicMaterial[1]);
            material2.image.color = Color.white;
        }
        if (currentPot.basicMaterial[2] != 0)   // 세번째 일반 작물 선택됨
        {
            material3.image.sprite = LoadIcon(currentPot.basicMaterial[2]);
            material3.image.color = Color.white;
        }

        startButton.interactable = false;
        endButton.interactable = false;
        currentButton = -1;
    }

    public void HandleReadyToStart()
    {
        materialListPanel.SetActive(false);
        magicPanel.SetActive(false);
        magicCircleImage.sprite = Resources.Load<Sprite>($"Potions/MagicCircle_2");
        magicCircleImage.color = Color.yellow;

        magic.interactable = true;
        material1.interactable = true;
        material2.interactable = true;
        material3.interactable = true;
        SelectedMagic();

        material1.image.color = Color.white;
        material2.image.color = Color.white;
        material3.image.color = Color.white;

        material1.image.sprite = LoadIcon(currentPot.basicMaterial[0]);
        material2.image.sprite = LoadIcon(currentPot.basicMaterial[1]);
        material3.image.sprite = LoadIcon(currentPot.basicMaterial[2]);
        startButton.interactable = true;
        endButton.interactable = false;
        currentButton = -1;
    }

    public void HandleCrafting()
    {
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

        craftingTime.text = "남은 시간 : " + Mathf.FloorToInt(currentPot.remainingTime).ToString("N0") + "초";

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
        startButton.interactable = false;
        endButton.interactable = false;
        currentButton = -1;
    }

    public void HandleCompleted()
    {
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

        craftingTime.text = "제작 완료";

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
        startButton.interactable = false;
        endButton.interactable = true;
        currentButton = -1;
    }

    //====================기타 메서드=====================

    // 일반 작물 선택 버튼들의 상호작용 유무를 초기화
    public void ButtonInteractionBasic()
    {
        // 선택된 작물도 false로
        // 부모 오브젝트의 모든 자식들을 가져오기
        Button[] buttons = materialListPanel.GetComponentsInChildren<Button>();

        // 각 버튼에 대해 상호작용을 false로 설정
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
        // 부모 오브젝트의 모든 자식들을 가져오기
        Button[] buttons = materialListPanel.GetComponentsInChildren<Button>();

        // 각 버튼에 대해 상호작용을 인수로 설정
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = a;

            if (i == buttons.Length - 1)
            {
                buttons[i].interactable = true;
            }
        }
    }

    // 마법 작물 선택 버튼들의 상호작용 유무를 초기화
    public void ButtonInteractionMagic()
    {
        // 부모 오브젝트의 모든 자식들을 가져오기
        Button[] buttons = magicPanel.GetComponentsInChildren<Button>();

        // 각 버튼에 대해 상호작용을 인수로 설정
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
        // 부모 오브젝트의 모든 자식들을 가져오기
        Button[] buttons = magicPanel.GetComponentsInChildren<Button>();

        // 각 버튼에 대해 상호작용을 false로 설정
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = a;

            if (i == buttons.Length - 1)
            {
                buttons[i].interactable = true;
            }
        }
    }

    // 마법 작물 선택 유무에 따라 초기화
    public void SelectedMagic()
    {
        if (currentPot.magicID != -1)   // 마법작물이 선택되었다면
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
            craftingTime.text = "제작 시간 : " + PotionDatabase.GetCraftingTime(currentPot.magicID).ToString("N0") + "초";
        }
        else    // 마법작물 선택 안 됨
        {
            craftingTime.text = null;
            potionImage.sprite = Resources.Load<Sprite>("Achievements/Rewards/Reward_Q");
            magic.image.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    // 하나라도 선택 안 된 항목이 있다면 false 다 선택되었다면 true
    public bool NotSelectedMaterial()
    {
        // basicMaterial 리스트에서 0이 하나라도 존재하면 false 반환
        foreach (int material in currentPot.basicMaterial)
        {
            if (material == 0)
            {
                return false;
            }
        }

        // 마법 작물이 선택 안 됐다면 false
        if (currentPot.magicID == -1)
        {
            return false;
        }

        return true; // 0이 없으면 true 반환
    }

    // 특정 cropID가 창고에 1개 이상 존재하면 true 그외에는 false
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

    // 작물 ID 로 리소스 로드
    private Sprite LoadIcon(int id)
    {
        // 업적에 썼던 함수 그냥 그대로 갖다 씀
        return Resources.Load<Sprite>($"Achievements/Icons/Achievement_{id}");
    }
}
