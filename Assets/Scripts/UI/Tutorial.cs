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
        tutorialCanvas = gameObject;        // tutorialCanvas ������ ���� ������Ʈ�� �Ҵ�
        tutorialCanvas.SetActive(true);

        // Ʃ�丮��ĵ������ ���� �ڽĵ��� false�� ����
        for (int i = 0; i < 21; i++)
        {
            tutorialCanvas.transform.GetChild(i).gameObject.SetActive(false);
        }

        // �� �Ʒ��� �׽�Ʈ������ ���߿� �����ؾ���
        PlayerPrefs.DeleteKey("TutorialDone");      // �׽�Ʈ��. �׻� Ʃ�丮���� ó������ ���·� ����. �� ���ߵǸ� �����ؾ���
        PlayerPrefs.DeleteKey("TutorialKeyboard");
        OBJPlacer.potCount = 0;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0)     // Ʃ�丮���� �ô��� �� �ô���. 
        {
            Debug.Log("Ʃ�丮�� ���� ȣ���");
            // Ʃ�丮���� ���� �ʾҴٸ� �Ʒ� �ڵ� ����
            TutorialStart();
            effectOBJ = tutorialCanvas.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
            effectsobj = tutorialCanvas.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
        }
    }

    private void Update()
    {
        if ((PlayerPrefs.GetInt("TutorialDone", 0) == 0))
        {
            if (values.Contains(index)) // ���͸� �������ϴ� ȭ���϶�
            {
                if (index == 16)
                {
                    effectOBJs.transform.position = new Vector3(2.4f, -3.5f, 90);
                }
                else
                {
                    effectsobj.transform.position = new Vector3(2.4f, -3.3f, 90);
                }
                
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))  // ���� Ű�� ������
                {
                    if (index == 6 || index == 11)
                    {
                        placementSystem.StopPlacement();    // ��ġ ���¸� ������Ѽ� �׸��� �����並 ��
                    }
                    else if (index == 14)
                    {
                        AchievementsDatabase.UnlockAchievement(6);
                        AchievementsDatabase.UnlockAchievement(7);
                        AchievementsDatabase.UnlockAchievement(8);
                        keyboardCount = AchievementsDatabase.GetAchievementByID(6).Progress;
                        PlayerPrefs.SetInt("TutorialKeyboard", 1);
                    }
                    // ���� ȭ������ �Ѿ�� ����
                    NextPanel();
                }
            }
            else if (index == 4 || index == 9)
            {
                // �׸��� �� on
                if (GridVisualization.activeSelf)
                {
                    NextPanel();
                }
            }
            else if (index == 3)
            {
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
                    NextPanel();
                }
            }
            else if (index == 5)
            {
                // objPlacer ����
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
                Debug.LogWarning("�������� �ʴ� Ʃ�丮�� �ε����Դϴ�.");
            }
        }
    }

    public void TutorialStart()     // Ʃ�丮���� �����ֱ� �����ϴ� �Լ�
    {
        Debug.LogWarning("Ʃ�丮�� ��ŸƮ ȣ���");
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
        if (index < 20)
        {
            tutorialCanvas.transform.GetChild(index).gameObject.SetActive(false);   // ���� �г��� ����
            index++;
            tutorialCanvas.transform.GetChild(index).gameObject.SetActive(true);    // ���� �г��� Ŵ

            // �Ʒ��� FloatingTextEffect�� ���� ����
            // ���� �г��� ���� �ι�° ������Ʈ
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
            tutorialCanvas.SetActive(false);    // ������ �г��̸� Ʃ�丮�� ĵ������ ��
        }

        if (index == 3 || index == 4 || index == 7 || index == 9)
        {
            // �����ʺ��� �¿�
            StartCoroutine(FloatingTextEffect(effectsobj, 2));
        }
        else if (index == 5 || index == 10 || index == 14)
        {
            // ���ʺ��� �¿�
            StartCoroutine(FloatingTextEffect(effectsobj, 1));
        }
        else if (index == 16)
        {
            StartCoroutine(FloatingTextEffect(effectsobj, 1));
            StartCoroutine(FloatingTextEffect(effectOBJs, 3));
        }
        else if (index == 12)   // ī�޶� �̵� �г�
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
