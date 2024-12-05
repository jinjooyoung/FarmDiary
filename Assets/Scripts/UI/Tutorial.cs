using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class TutorialUI : MonoBehaviour
{
    private GameObject tutorialCanvas;
    private int index = -1;
    private int keyboardCount = 0;
    private GameObject effectOBJ;
    private GameObject effectsobj;
    private GameObject effectOBJs;
    private GameObject panelOBJ;

    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private OBJPlacer OBJPlacer;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject DecoPanel;
    [SerializeField] private GameObject GridVisualization;
    [SerializeField] private GameObject SeedPanel;

    HashSet<int> values = new HashSet<int> { 0, 1, 2, 6, 8, 11, 13, 14, 16, 17, 18, 19, 20 };

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
        PlayerPrefs.DeleteKey("TutorialDone");      // 테스트용. 항상 튜토리얼을 처음보는 상태로 만듦. 다 개발되면 삭제해야함
        PlayerPrefs.DeleteKey("TutorialKeyboard");
        OBJPlacer.potCount = 0;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0)     // 튜토리얼을 봤는지 안 봤는지. 
        {
            Debug.Log("튜토리얼 시작 호출됨");
            // 튜토리얼을 보지 않았다면 아래 코드 실행
            TutorialStart();
            effectOBJ = tutorialCanvas.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
            effectsobj = tutorialCanvas.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
        }
    }

    private void Update()
    {
        if ((PlayerPrefs.GetInt("TutorialDone", 0) == 0))
        {
            if (values.Contains(index)) // 엔터를 눌러야하는 화면일때
            {
                if (index == 16)
                {
                    effectOBJs.transform.position = new Vector3(2.4f, -3.5f, 90);
                }
                else
                {
                    effectsobj.transform.position = new Vector3(2.4f, -3.3f, 90);
                }
                
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))  // 엔터 키를 누르면
                {
                    if (index == 6 || index == 11)
                    {
                        placementSystem.StopPlacement();    // 설치 상태를 종료시켜서 그리드 프리뷰를 끔
                    }
                    else if (index == 14)
                    {
                        AchievementsDatabase.UnlockAchievement(6);
                        AchievementsDatabase.UnlockAchievement(7);
                        AchievementsDatabase.UnlockAchievement(8);
                        keyboardCount = AchievementsDatabase.GetAchievementByID(6).Progress;
                        PlayerPrefs.SetInt("TutorialKeyboard", 1);
                    }
                    // 다음 화면으로 넘어가는 로직
                    NextPanel();
                }
            }
            else if (index == 4 || index == 9)
            {
                // 그리드 뷰 on
                if (GridVisualization.activeSelf)
                {
                    NextPanel();
                }
            }
            else if (index == 3)
            {
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
                    NextPanel();
                }
            }
            else if (index == 5)
            {
                // objPlacer 로직
                if (OBJPlacer.placedGameObjects.Count == 1)
                {
                    NextPanel();
                }
            }
            else if (index == 10)
            {
                if (OBJPlacer.placedGameObjects.Count == 2)
                {
                    NextPanel();
                }
            }
            else if (index == 12)
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
                if (AchievementsDatabase.GetAchievementByID(6).Progress > keyboardCount + 3)
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
        Debug.LogWarning("튜토리얼 스타트 호출됨");
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
        if (index < 20)
        {
            tutorialCanvas.transform.GetChild(index).gameObject.SetActive(false);   // 현재 패널을 끄고
            index++;
            tutorialCanvas.transform.GetChild(index).gameObject.SetActive(true);    // 다음 패널을 킴

            // 아래는 FloatingTextEffect를 위한 로직
            // 다음 패널의 하위 두번째 오브젝트
            effectOBJ = tutorialCanvas.transform.GetChild(index).gameObject.transform.GetChild(1).gameObject;
            effectsobj = tutorialCanvas.transform.GetChild(index).gameObject.transform.GetChild(2).gameObject;

            if (index == 12 || index == 16)
            {
                effectOBJs = tutorialCanvas.transform.GetChild(index).gameObject.transform.GetChild(3).gameObject;
            }
            else if (index == 17)
            {
                GameManager.AddCoins(2500);
            }

            StartCoroutine(FloatingTextEffect(effectOBJ, 0));
        }
        else if (index == 20)
        {
            index = -1;
            PlayerPrefs.SetInt("TutorialDone", 1);
            AchievementsDatabase.TutorialAchievement();
            tutorialCanvas.SetActive(false);    // 마지막 패널이면 튜토리얼 캔버스를 끔
        }

        if (index == 3 || index == 4 || index == 7 || index == 9)
        {
            // 오른쪽부터 좌우
            StartCoroutine(FloatingTextEffect(effectsobj, 2));
        }
        else if (index == 5 || index == 10 || index == 14)
        {
            // 왼쪽부터 좌우
            StartCoroutine(FloatingTextEffect(effectsobj, 1));
        }
        else if (index == 16)
        {
            StartCoroutine(FloatingTextEffect(effectsobj, 1));
            StartCoroutine(FloatingTextEffect(effectOBJs, 3));
        }
        else if (index == 12)   // 카메라 이동 패널
        {
            StartCoroutine(FloatingTextEffect(effectsobj, 1));
            StartCoroutine(FloatingTextEffect(effectOBJs, 2));
        }
        else if (index != 20)
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
