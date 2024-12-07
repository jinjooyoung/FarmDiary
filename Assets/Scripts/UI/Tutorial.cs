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
        tutorialCanvas = gameObject;        // tutorialCanvas ������ ���� ������Ʈ�� �Ҵ�
        tutorialCanvas.SetActive(true);

        // Ʃ�丮��ĵ������ ���� �ڽĵ��� false�� ����
        for (int i = 0; i < 21; i++)
        {
            tutorialCanvas.transform.GetChild(i).gameObject.SetActive(false);
        }

        // �� �Ʒ��� �׽�Ʈ������ ���߿� �����ؾ���
        //PlayerPrefs.DeleteKey("TutorialDone");      // �׽�Ʈ��. �׻� Ʃ�丮���� ó������ ���·� ����. �� ���ߵǸ� �����ؾ���
        //PlayerPrefs.DeleteKey("TutorialKeyboard");
        OBJPlacer.potCount = 0;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0)     // Ʃ�丮���� �ô��� �� �ô°�? = ù �����ΰ�?
        {
            // Ʃ�丮���� ���� �ʾҴٸ� �Ʒ� �ڵ� ����
            index = 0;
            GameManager.instance.InitializePlayerPrefs();   // PlayerPrefs�� �����ϴ� ��� �͵� �ʱ�ȭ
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
            if (values.Contains(index)) // ���͸� �������ϴ� ȭ���϶�
            {
                // ���� ������ ��ġ ����
                effectsobj.transform.position = new Vector3(2.4f, -3.3f, 90);

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))  // ���� Ű�� ������
                {
                    if (index == 25)
                    {
                        // Ű���� ���� ���� ����
                        AchievementsDatabase.UnlockAchievement(6);
                        AchievementsDatabase.UnlockAchievement(7);
                        AchievementsDatabase.UnlockAchievement(8);

                        // ���� Ű���� �Է��� Ƚ�� �޾ƿ� (0���ٵ� Ȥ�� �𸣴ϱ� �ϴ� �޾ƿ�)
                        keyboardCount = AchievementsDatabase.GetAchievementByID(6).Progress;
                        // Ű���� Ʃ�丮�� �����ߴٰ� �˸� = Ű���� �Է��ϸ� +1�� �Ǳ� ����
                        PlayerPrefs.SetInt("TutorialKeyboard", 1);
                    }
                    else if (index == 23)
                    {
                        potionUIManager.startButton.interactable = true;
                    }
                    else if (index == 21)
                    {
                        // ���� �а� ���� ������ ��ȣ�ۿ� �����ϰ�
                        potionUIManager.magic.interactable = true;
                        potionUIManager.material1.interactable = true;
                        potionUIManager.material2.interactable = true;
                        potionUIManager.material3.interactable = true;
                    }
                    else if (index == 30)
                    {
                        GameManager.AddCoins(102000);
                    }
                    // ���� ȭ������ �Ѿ�� ����
                    NextPanel();
                }
            }
            else if (index == 4 || index == 9 || index == 18)      // ����
            {
                // �׸��� �� on
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
                // ��ġ �� on
                if (DecoPanel.activeSelf)
                {
                    NextPanel();
                }
            }
            else if (index == 7)
            {
                // ���� �� on
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
                // �ֱٿ� ��ġ�� ������Ʈ�� �̸��� ���Ŷ��
                if (OBJPlacer.placedGameObjects.Count > 0 && OBJPlacer.placedGameObjects[OBJPlacer.placedObjectIndex].name == "2x2_Field(Clone)")     // �� ��ġ
                {
                    NextPanel();
                    FieldButton2.interactable = false;
                    placementSystem.StopPlacement();
                }
            }
            else if (index == 10)
            {
                if (OBJPlacer.placedGameObjects.Count > 1 && OBJPlacer.placedGameObjects[OBJPlacer.placedObjectIndex].name == "Amaranth_9(Clone)")     // �Ƹ����� ��ġ
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
                    Debug.LogWarning("�������� 1");
                    UIManager.instance.SeedButtons[0].interactable = false;
                    UIManager.instance.SeedButtons[1].interactable = false;
                    UIManager.instance.SeedButtons[2].interactable = true;
                    UIManager.instance.SeedButtons[39].interactable = false;
                }
                else if (created == 2)
                {
                    Debug.LogWarning("�������� 2");
                    UIManager.instance.SeedButtons[0].interactable = false;
                    UIManager.instance.SeedButtons[1].interactable = false;
                    UIManager.instance.SeedButtons[2].interactable = false;
                    UIManager.instance.SeedButtons[39].interactable = true;
                }
                else if (created == 3)
                {
                    Debug.LogWarning("�������� 3");
                    UIManager.instance.SeedButtons[0].interactable = false;
                    UIManager.instance.SeedButtons[1].interactable = false;
                    UIManager.instance.SeedButtons[2].interactable = false;
                    UIManager.instance.SeedButtons[39].interactable = false;
                }

                // ������ �ϳ��� if���� Exists�� && �����ڷ� �� ���� �˻��ߴµ� �ڶ�� �ӵ��� ���� ������ ��ġ���� ������ �÷��̾ ��Ȯ�ؼ� �����Ǿ� �Ѿ �� ���Ե�
                // �׷��� ���������� ������
                // �ƽ��Ķ�Ž��� ��ġ�Ǹ�
                if (OBJPlacer.placedGameObjects[OBJPlacer.placedObjectIndex].name == "Asparagus_10(Clone)")
                {
                    Debug.LogWarning("�ƽ��Ķ�Ž� ��ġ��");
                    OBJPlacer.placedObjectIndex = 0;
                    created = 1;
                }

                if (OBJPlacer.placedGameObjects[OBJPlacer.placedObjectIndex].name == "BalloonFlower_11(Clone)")
                {
                    Debug.LogWarning("������ ��ġ��");
                    OBJPlacer.placedObjectIndex = 0;
                    created = 2;
                }

                if (OBJPlacer.placedGameObjects[OBJPlacer.placedObjectIndex].name == "Mandrake_48(Clone)")
                {
                    Debug.LogWarning("�ǵ巹��ũ ��ġ��");
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
                if (OBJPlacer.placedGameObjects[OBJPlacer.placedObjectIndex].name == "Pot(Clone)")     // �� ��ġ
                {
                    NextPanel();
                    placementSystem.StopPlacement();
                }
            }
            else if (index == 20)
            {
                if (PotionPanel.activeSelf)
                {
                    // ���� �г��� ���� ���� �������� ���ͷ��� �Ұ��� ����
                    potionUIManager.magic.interactable = false;
                    potionUIManager.material1.interactable = false;
                    potionUIManager.material2.interactable = false;
                    potionUIManager.material3.interactable = false;
                    NextPanel();
                }
            }
            else if (index == 22)
            {
                // �۹��� ��� �־ �غ� ���°� �Ǹ� �������� �ѱ�
                if (potionUIManager.currentPot.currentState == PotState.ReadyToStart)
                {
                    potionUIManager.startButton.interactable = false;   // �ٷ� ���� ��ư �� ������
                    NextPanel();
                }
            }
            else if (index == 24)
            {
                // ���� ��ư�� ������ ������ ���°� �Ǹ�
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
                // ���� ���� �Ϸ� ���°� �Ǹ�
                if (potionUIManager.currentPot.currentState == PotState.Completed)
                {
                    NextPanel();
                }
            }
            else if (index == 29)
            {
                // ���� �Ǹ� ������ �� ���°� Emtpy�� �Ǹ�
                if (potionUIManager.currentPot.currentState == PotState.Empty)
                {
                    NextPanel();
                }
            }
            else
            {
                Debug.LogWarning("�������� �ʴ� Ʃ�丮�� �ε����Դϴ�.");
            }
        }
    }

    public void TutorialStart()     // Ʃ�丮���� �����ֱ� �����ϴ� �Լ�
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
        GameObject txtOBJ = tutorialCanvas.transform.GetChild(index).gameObject.transform.GetChild(1).gameObject;   // �г��� ����
        GameObject OBJ = tutorialCanvas.transform.GetChild(index).gameObject.transform.GetChild(2).gameObject;   // �г��� ����
        StartCoroutine(FloatingTextEffect(txtOBJ,0));
        StartCoroutine(FloatingTextEffect(OBJ, 3));
    }

    // �г��� �ѱ�� �޼���
    public void NextPanel()
    {
        if (index < 35)
        {
            tutorialCanvas.transform.GetChild(index).gameObject.SetActive(false);   // ���� �г��� ����
            index++;
            tutorialCanvas.transform.GetChild(index).gameObject.SetActive(true);    // ���� �г��� Ŵ

            // �Ʒ��� FloatingTextEffect�� ���� ����
            // ���� �г��� ���� �ι�° ������Ʈ
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

            tutorialCanvas.SetActive(false);    // ������ �г��̸� Ʃ�丮�� ĵ������ ��
        }

        if (index == 3 || index == 4 || index == 9 || index == 17 || index == 18 || index == 22 || index == 24 || index == 29)
        {
            // �����ʺ��� �¿�
            StartCoroutine(FloatingTextEffect(effectsobj, 2));
        }
        else if (index == 5 || index == 7 || index == 10 || index == 15 || index == 19)
        {
            // ���ʺ��� �¿�
            StartCoroutine(FloatingTextEffect(effectsobj, 1));
        }
        else if (index == 23 || index == 27)
        {
            StartCoroutine(FloatingTextEffect(effectsobj, 2));
            StartCoroutine(FloatingTextEffect(effectOBJs, 3));
        }
        else if (index == 13)   // ī�޶� �̵� �г�
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

    // ������Ʈ�� �����̴� ȿ�� (type 0 : ���Ʒ�, 1 : ���ʺ��� �¿�, 2 : �����ʺ��� �¿�, 3 : ������Ʈ�� ������)
    private IEnumerator FloatingTextEffect(GameObject textOBJ, int type, float amplitude = 0.05f, float frequency = 0.7f)
    {
        Vector3 textBoxPos = new Vector3(1.69f, -2.48f, 90.00f);
        Vector3 startPos = textOBJ.transform.position; // �ʱ� ��ġ ����
        Vector3 offsetFromCamera = textOBJ.transform.position - mainCamera.transform.position; // ī�޶� ���� ��ġ ������ ����
        float timer = 0f;

        // UI �̹��� ������Ʈ ��������
        UnityEngine.UI.Image imageComponent = textOBJ.GetComponent<UnityEngine.UI.Image>();

        while (textOBJ.activeSelf) // ������Ʈ SetActive(true)�� ��� ���� �ݺ�
        {
            timer += Time.deltaTime;
            Vector3 cameraBasedPosition = mainCamera.transform.position + offsetFromCamera;

            if (type == 0) // ���Ʒ��� �����̴� ȿ��
            {
                float yOffset = Mathf.Sin(timer * frequency * Mathf.PI * 2) * amplitude;
                textOBJ.transform.position = textBoxPos + new Vector3(0, yOffset, 0);
            }
            else if (type == 1) // ���ʺ��� ���������� �����̴� ȿ��
            {
                float xOffset = Mathf.Sin(timer * frequency * Mathf.PI * 2) * amplitude;
                textOBJ.transform.position = cameraBasedPosition + new Vector3(xOffset, 0, 0);
            }
            else if (type == 2) // �����ʺ��� �������� �����̴� ȿ��
            {
                float xOffset = Mathf.Sin((timer + 0.5f) * frequency * Mathf.PI * 2) * amplitude; // 0.5f�� ���� ����
                textOBJ.transform.position = cameraBasedPosition + new Vector3(xOffset, 0, 0);
            }
            else if (type == 3) // ���ڰŸ��� ȿ��
            {
                if (imageComponent != null) // �̹��� ������Ʈ�� �ִ� ���
                {
                    // ���İ��� 1�� ����
                    imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 1f);

                    // ��� �� ���İ��� 0.2���� ����
                    yield return new WaitForSeconds(0.7f);
                    imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 0.2f);

                    // ��� �� ���İ��� 1�� ����
                    yield return new WaitForSeconds(0.7f);
                }
            }

            yield return null; // �� ������ ���
        }

        // �ڷ�ƾ ���� �� ���� ���� ���� (type == 3�� ���)
        if (type == 3 && imageComponent != null)
        {
            // ���İ��� 1�� ����
            imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 1f);
        }
    }
}
