using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Pot;
using static UnityEngine.Rendering.DebugUI;

public class TutorialUI : MonoBehaviour
{
    private GameObject tutorialCanvas;
    private int index = -1;
    private int created = 0;
    private int keyboardCount = 0;
    private GameObject effectOBJ;
    private GameObject effectsobj;
    private GameObject effectOBJs;
    private GameObject panelOBJ;

    [SerializeField] private UnityEngine.UI.Button FieldButton4;
    [SerializeField] private UnityEngine.UI.Button FieldButton3;
    [SerializeField] private UnityEngine.UI.Button FieldButton2;
    [SerializeField] private UnityEngine.UI.Button FieldButton1;
    [SerializeField] private UnityEngine.UI.Button Button1;
    [SerializeField] private UnityEngine.UI.Button Button2;
    [SerializeField] private UnityEngine.UI.Button Button3;
    [SerializeField] private UnityEngine.UI.Button Button4;

    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private PotionUIManager potionUIManager;
    [SerializeField] private OBJPlacer OBJPlacer;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject DecoPanel;
    [SerializeField] private GameObject GridVisualization;
    [SerializeField] private GameObject SeedPanel;
    [SerializeField] private GameObject StoragePanel;
    [SerializeField] private GameObject PotionPanel;

    HashSet<int> values = new HashSet<int> { 0, 1, 2, 6, 8, 11, 14, 16, 21, 23, 25, 27, 30, 31, 32, 33, 34, 35 };

    private void Awake()
    {
        tutorialCanvas = gameObject;        // tutorialCanvas 변수에 현재 오브젝트를 할당
        tutorialCanvas.SetActive(true);

        // 튜토리얼캔버스의 하위 자식들을 false로 만듦
        for (int i = 0; i < 21; i++)
        {
            tutorialCanvas.transform.GetChild(i).gameObject.SetActive(false);
        }

        // 이 아래는 테스트용으로 나중에 삭제해야함
        //PlayerPrefs.DeleteKey("TutorialDone");      // 테스트용. 항상 튜토리얼을 처음보는 상태로 만듦. 다 개발되면 삭제해야함
        //PlayerPrefs.DeleteKey("TutorialKeyboard");
        OBJPlacer.potCount = 0;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0)     // 튜토리얼을 봤는지 안 봤는가? = 첫 시작인가?
        {
            // 튜토리얼을 보지 않았다면 아래 코드 실행
            index = 0;
            GameManager.instance.InitializePlayerPrefs();   // PlayerPrefs로 저장하는 모든 것들 초기화
            TutorialStart();
            effectOBJ = tutorialCanvas.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
            effectsobj = tutorialCanvas.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
        }
        else
        {
            ObjectsDatabase.InitializeTutorialGrowthTimes(9);
            ObjectsDatabase.InitializeTutorialGrowthTimes(10);
            ObjectsDatabase.InitializeTutorialGrowthTimes(11);
            ObjectsDatabase.InitializeTutorialGrowthTimes(48);
            PotionDatabase.TutorialEndCraftingTime(48);
        }
    }

    private void Update()
    {
        if ((PlayerPrefs.GetInt("TutorialDone", 0) == 0))
        {
            if (values.Contains(index)) // 엔터를 눌러야하는 화면일때
            {
                // 엔터 아이콘 위치 고정
                effectsobj.transform.position = new Vector3(2.4f, -3.3f, 90);

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))  // 엔터 키를 누르면
                {
                    if (index == 25)
                    {
                        // 키보드 업적 진행 가능
                        AchievementsDatabase.UnlockAchievement(6);
                        AchievementsDatabase.UnlockAchievement(7);
                        AchievementsDatabase.UnlockAchievement(8);

                        // 현재 키보드 입력한 횟수 받아옴 (0일텐데 혹시 모르니까 일단 받아옴)
                        keyboardCount = AchievementsDatabase.GetAchievementByID(6).Progress;
                        // 키보드 튜토리얼 시작했다고 알림 = 키보드 입력하면 +1원 되기 시작
                        PlayerPrefs.SetInt("TutorialKeyboard", 1);
                    }
                    else if (index == 23)
                    {
                        potionUIManager.startButton.interactable = true;
                    }
                    else if (index == 21)
                    {
                        // 설명 읽고 엔터 누르면 상호작용 가능하게
                        potionUIManager.magic.interactable = true;
                        potionUIManager.material1.interactable = true;
                        potionUIManager.material2.interactable = true;
                        potionUIManager.material3.interactable = true;
                    }
                    else if (index == 30)
                    {
                        GameManager.AddCoins(102000);
                    }
                    // 다음 화면으로 넘어가는 로직
                    NextPanel();
                }
            }
            else if (index == 4 || index == 9 || index == 18)      // 선택
            {
                // 그리드 뷰 on
                if (GridVisualization.activeSelf)
                {
                    if (index == 18)
                    {
                        Button4.interactable = false;
                    }
                    NextPanel();
                }
            }
            else if (index == 3 || index == 17)
            {
                if (index == 17)
                {
                    Button4.interactable = true;
                }
                // 설치 탭 on
                if (DecoPanel.activeSelf)
                {
                    NextPanel();
                }
            }
            else if (index == 7)
            {
                // 씨앗 탭 on
                if (SeedPanel.activeSelf)
                {
                    UIManager.instance.SeedButtons[1].interactable = false;
                    UIManager.instance.SeedButtons[2].interactable = false;
                    UIManager.instance.SeedButtons[39].interactable = false;
                    NextPanel();
                }
            }
            else if (index == 5)
            {
                // 최근에 설치한 오브젝트의 이름이 저거라면
                if (OBJPlacer.placedGameObjects.Count > 0 && OBJPlacer.placedGameObjects[OBJPlacer.placedObjectIndex].name == "2x2_Field(Clone)")     // 밭 설치
                {
                    NextPanel();
                    FieldButton2.interactable = false;
                    placementSystem.StopPlacement();
                }
            }
            else if (index == 10)
            {
                if (OBJPlacer.placedGameObjects.Count > 1 && OBJPlacer.placedGameObjects[OBJPlacer.placedObjectIndex].name == "Amaranth_9(Clone)")     // 아마란스 설치
                {
                    NextPanel();
                    placementSystem.StopPlacement();
                    OBJPlacer.placedObjectIndex = 0;
                    created = 0;
                }
            }
            else if (index == 12)
            {
                Debug.LogWarning(OBJPlacer.placedGameObjects.Count);
                if (created == 0)
                {
                    UIManager.instance.SeedButtons[0].interactable = false;
                    UIManager.instance.SeedButtons[1].interactable = true;
                    UIManager.instance.SeedButtons[2].interactable = false;
                    UIManager.instance.SeedButtons[39].interactable = false;
                }
                else if (created == 1)
                {
                    Debug.LogWarning("지역변수 1");
                    UIManager.instance.SeedButtons[0].interactable = false;
                    UIManager.instance.SeedButtons[1].interactable = false;
                    UIManager.instance.SeedButtons[2].interactable = true;
                    UIManager.instance.SeedButtons[39].interactable = false;
                }
                else if (created == 2)
                {
                    Debug.LogWarning("지역변수 2");
                    UIManager.instance.SeedButtons[0].interactable = false;
                    UIManager.instance.SeedButtons[1].interactable = false;
                    UIManager.instance.SeedButtons[2].interactable = false;
                    UIManager.instance.SeedButtons[39].interactable = true;
                }
                else if (created == 3)
                {
                    Debug.LogWarning("지역변수 3");
                    UIManager.instance.SeedButtons[0].interactable = false;
                    UIManager.instance.SeedButtons[1].interactable = false;
                    UIManager.instance.SeedButtons[2].interactable = false;
                    UIManager.instance.SeedButtons[39].interactable = false;
                }

                // 원래는 하나의 if문에 Exists에 && 연산자로 한 번에 검사했는데 자라는 속도가 빨라서 빠르게 설치하지 않으면 플레이어가 수확해서 삭제되어 넘어갈 수 없게됨
                // 그래서 순차적으로 점검함
                // 아스파라거스가 설치되면
                if (OBJPlacer.placedGameObjects[OBJPlacer.placedObjectIndex].name == "Asparagus_10(Clone)")
                {
                    Debug.LogWarning("아스파라거스 설치됨");
                    OBJPlacer.placedObjectIndex = 0;
                    created = 1;
                }

                if (OBJPlacer.placedGameObjects[OBJPlacer.placedObjectIndex].name == "BalloonFlower_11(Clone)")
                {
                    Debug.LogWarning("도라지 설치됨");
                    OBJPlacer.placedObjectIndex = 0;
                    created = 2;
                }

                if (OBJPlacer.placedGameObjects[OBJPlacer.placedObjectIndex].name == "Mandrake_48(Clone)")
                {
                    Debug.LogWarning("맨드레이크 설치됨");
                    OBJPlacer.placedObjectIndex = 0;
                    UIManager.instance.SeedButtons[0].interactable = false;
                    UIManager.instance.SeedButtons[1].interactable = false;
                    UIManager.instance.SeedButtons[2].interactable = false;
                    UIManager.instance.SeedButtons[39].interactable = false;
                    created = 3;

                    NextPanel();
                    placementSystem.StopPlacement();
                }
            }
            else if (index == 13)
            {
                if (mainCamera != null)
                {
                    if (Mathf.Abs(mainCamera.transform.position.x) > 5.5f)
                    {
                        NextPanel();
                        mainCamera.transform.position = new Vector3(0, 0, -10);
                    }
                }
            }
            else if (index == 15)
            {
                if (StoragePanel.activeSelf)
                {
                    NextPanel();
                }
            }
            else if (index == 19)
            {
                if (OBJPlacer.placedGameObjects[OBJPlacer.placedObjectIndex].name == "Pot(Clone)")     // 솥 설치
                {
                    NextPanel();
                    placementSystem.StopPlacement();
                }
            }
            else if (index == 20)
            {
                if (PotionPanel.activeSelf)
                {
                    // 다음 패널의 설명 보기 전까지는 인터렉션 불가능 상태
                    potionUIManager.magic.interactable = false;
                    potionUIManager.material1.interactable = false;
                    potionUIManager.material2.interactable = false;
                    potionUIManager.material3.interactable = false;
                    NextPanel();
                }
            }
            else if (index == 22)
            {
                // 작물을 모두 넣어서 준비 상태가 되면 다음으로 넘김
                if (potionUIManager.currentPot.currentState == PotState.ReadyToStart)
                {
                    potionUIManager.startButton.interactable = false;   // 바로 시작 버튼 못 누르게
                    NextPanel();
                }
            }
            else if (index == 24)
            {
                // 시작 버튼을 눌러서 제작중 상태가 되면
                if (potionUIManager.currentPot.currentState == PotState.Crafting)
                {
                    NextPanel();
                }
            }
            else if (index == 26)
            {
                if (AchievementsDatabase.GetAchievementByID(6).Progress > keyboardCount + 6)
                {
                    NextPanel();
                }
            }
            else if (index == 28)
            {
                // 포션 제작 완료 상태가 되면
                if (potionUIManager.currentPot.currentState == PotState.Completed)
                {
                    NextPanel();
                }
            }
            else if (index == 29)
            {
                // 포션 판매 눌러서 솥 상태가 Emtpy가 되면
                if (potionUIManager.currentPot.currentState == PotState.Empty)
                {
                    NextPanel();
                }
            }
            else
            {
                Debug.LogWarning("존재하지 않는 튜토리얼 인덱스입니다.");
            }
        }
    }

    public void TutorialStart()     // 튜토리얼을 보여주기 시작하는 함수
    {
        ObjectsDatabase.SetTutorialGrowthTimes(9);
        ObjectsDatabase.SetTutorialGrowthTimes(10);
        ObjectsDatabase.SetTutorialGrowthTimes(11);
        ObjectsDatabase.SetTutorialGrowthTimes(48);
        PotionDatabase.TutorialCraftingTime(48);

        FieldButton1.interactable = false;
        FieldButton2.interactable = true;
        FieldButton3.interactable = false;
        FieldButton4.interactable = false;
        Button1.interactable = false;
        Button2.interactable = false;
        Button3.interactable = false;
        Button4.interactable = false;

        index = 0;
        tutorialCanvas.transform.GetChild(index).gameObject.SetActive(true);
        GameObject txtOBJ = tutorialCanvas.transform.GetChild(index).gameObject.transform.GetChild(1).gameObject;   // 패널의 하위
        GameObject OBJ = tutorialCanvas.transform.GetChild(index).gameObject.transform.GetChild(2).gameObject;   // 패널의 하위
        StartCoroutine(FloatingTextEffect(txtOBJ,0));
        StartCoroutine(FloatingTextEffect(OBJ, 3));
    }

    // 패널을 넘기는 메서드
    public void NextPanel()
    {
        if (index < 35)
        {
            tutorialCanvas.transform.GetChild(index).gameObject.SetActive(false);   // 현재 패널을 끄고
            index++;
            tutorialCanvas.transform.GetChild(index).gameObject.SetActive(true);    // 다음 패널을 킴

            // 아래는 FloatingTextEffect를 위한 로직
            // 다음 패널의 하위 두번째 오브젝트
            effectOBJ = tutorialCanvas.transform.GetChild(index).gameObject.transform.GetChild(1).gameObject;
            effectsobj = tutorialCanvas.transform.GetChild(index).gameObject.transform.GetChild(2).gameObject;

            if (index == 13 || index == 23 || index == 27)
            {
                effectOBJs = tutorialCanvas.transform.GetChild(index).gameObject.transform.GetChild(3).gameObject;
            }

            StartCoroutine(FloatingTextEffect(effectOBJ, 0));
        }
        else if (index == 35)
        {
            index = -1;
            PlayerPrefs.SetInt("TutorialDone", 1);
            AchievementsDatabase.TutorialAchievement();

            ObjectsDatabase.InitializeTutorialGrowthTimes(9);
            ObjectsDatabase.InitializeTutorialGrowthTimes(10);
            ObjectsDatabase.InitializeTutorialGrowthTimes(11);
            ObjectsDatabase.InitializeTutorialGrowthTimes(48);
            PotionDatabase.TutorialEndCraftingTime(48);

            FieldButton1.interactable = true;
            FieldButton2.interactable = true;
            FieldButton3.interactable = true;
            FieldButton4.interactable = true;
            Button1.interactable = true;
            Button2.interactable = true;
            Button3.interactable = true;
            Button4.interactable = true;

            UIManager.instance.SeedButtons[0].interactable = true;
            UIManager.instance.SeedButtons[1].interactable = true;
            UIManager.instance.SeedButtons[2].interactable = true;
            UIManager.instance.SeedButtons[39].interactable = true;

            tutorialCanvas.SetActive(false);    // 마지막 패널이면 튜토리얼 캔버스를 끔
        }

        if (index == 3 || index == 4 || index == 9 || index == 17 || index == 18 || index == 22 || index == 24 || index == 29)
        {
            // 오른쪽부터 좌우
            StartCoroutine(FloatingTextEffect(effectsobj, 2));
        }
        else if (index == 5 || index == 7 || index == 10 || index == 15 || index == 19)
        {
            // 왼쪽부터 좌우
            StartCoroutine(FloatingTextEffect(effectsobj, 1));
        }
        else if (index == 23 || index == 27)
        {
            StartCoroutine(FloatingTextEffect(effectsobj, 2));
            StartCoroutine(FloatingTextEffect(effectOBJs, 3));
        }
        else if (index == 13)   // 카메라 이동 패널
        {
            StartCoroutine(FloatingTextEffect(effectsobj, 1));
            StartCoroutine(FloatingTextEffect(effectOBJs, 2));
        }
        else if (index != 35)
        {
            if (tutorialCanvas.activeSelf)
            {
                StartCoroutine(FloatingTextEffect(effectsobj, 3));
            }
        }
    }

    // 오브젝트가 움직이는 효과 (type 0 : 위아래, 1 : 왼쪽부터 좌우, 2 : 오른쪽부터 좌우, 3 : 오브젝트가 깜빡임)
    private IEnumerator FloatingTextEffect(GameObject textOBJ, int type, float amplitude = 0.05f, float frequency = 0.7f)
    {
        Vector3 textBoxPos = new Vector3(1.69f, -2.48f, 90.00f);
        Vector3 startPos = textOBJ.transform.position; // 초기 위치 저장
        Vector3 offsetFromCamera = textOBJ.transform.position - mainCamera.transform.position; // 카메라 기준 위치 오프셋 저장
        float timer = 0f;

        // UI 이미지 컴포넌트 가져오기
        UnityEngine.UI.Image imageComponent = textOBJ.GetComponent<UnityEngine.UI.Image>();

        while (textOBJ.activeSelf) // 오브젝트 SetActive(true)인 경우 무한 반복
        {
            timer += Time.deltaTime;
            Vector3 cameraBasedPosition = mainCamera.transform.position + offsetFromCamera;

            if (type == 0) // 위아래로 움직이는 효과
            {
                float yOffset = Mathf.Sin(timer * frequency * Mathf.PI * 2) * amplitude;
                textOBJ.transform.position = textBoxPos + new Vector3(0, yOffset, 0);
            }
            else if (type == 1) // 왼쪽부터 오른쪽으로 움직이는 효과
            {
                float xOffset = Mathf.Sin(timer * frequency * Mathf.PI * 2) * amplitude;
                textOBJ.transform.position = cameraBasedPosition + new Vector3(xOffset, 0, 0);
            }
            else if (type == 2) // 오른쪽부터 왼쪽으로 움직이는 효과
            {
                float xOffset = Mathf.Sin((timer + 0.5f) * frequency * Mathf.PI * 2) * amplitude; // 0.5f로 위상 반전
                textOBJ.transform.position = cameraBasedPosition + new Vector3(xOffset, 0, 0);
            }
            else if (type == 3) // 깜박거리는 효과
            {
                if (imageComponent != null) // 이미지 컴포넌트가 있는 경우
                {
                    // 알파값을 1로 설정
                    imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 1f);

                    // 대기 후 알파값을 0.2으로 변경
                    yield return new WaitForSeconds(0.7f);
                    imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 0.2f);

                    // 대기 후 알파값을 1로 변경
                    yield return new WaitForSeconds(0.7f);
                }
            }

            yield return null; // 한 프레임 대기
        }

        // 코루틴 종료 시 원래 상태 복원 (type == 3인 경우)
        if (type == 3 && imageComponent != null)
        {
            // 알파값을 1로 복원
            imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 1f);
        }
    }
}
