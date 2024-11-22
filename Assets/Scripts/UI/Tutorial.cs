using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    private GameObject tutorialCanvas;
    private int index = -1;
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
        PlayerPrefs.DeleteKey("TutorialDone");      // �׽�Ʈ��. �׻� Ʃ�丮���� ó������ ���·� ����. �� ���ߵǸ� �����ؾ���
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0)     // Ʃ�丮���� �ô��� �� �ô���. 
        {
            Debug.Log("Ʃ�丮�� ���� ȣ���");
            // Ʃ�丮���� ���� �ʾҴٸ� �Ʒ� �ڵ� ����
            TutorialStart();
            PlayerPrefs.SetInt("TutorialDone", 1);          // Ʃ�丮���� ����
        }

        if (values.Contains(index)) // ���͸� �������ϴ� ȭ���϶�
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))  // ���� Ű�� ������
            {
                // ���� ȭ������ �Ѿ�� ����
            }
        }
    }

    public void TutorialStart()     // Ʃ�丮���� �����ֱ� �����ϴ� �Լ�
    {
        index = 0;
        GameObject txtOBJ = tutorialCanvas.transform.GetChild(index).gameObject;
        txtOBJ.SetActive(true);
        StartCoroutine(FloatingTextEffect(txtOBJ));
    }

    // ������Ʈ�� ���Ʒ��� �����̴� ȿ��
    private IEnumerator FloatingTextEffect(GameObject textOBJ, float amplitude = 30f, float frequency = 1f)
    {
        Vector3 startPos = textOBJ.transform.position; // �ʱ� ��ġ ����
        float timer = 0f;

        while (textOBJ.activeSelf) // ������Ʈ SetActive(true)�� ��� ���� �ݺ�
        {
            timer += Time.deltaTime;
            float yOffset = Mathf.Sin(timer * frequency * Mathf.PI * 2) * amplitude; // ���Ʒ��� �����̴� y ������ ���

            // ��ġ ������Ʈ
            textOBJ.transform.position = startPos + new Vector3(0, yOffset, 0);

            yield return null; // �� ������ ���
        }
    }

    // �Ʒ� �Լ��� �����ؾ���
    // ��ư�� ���̴� �Լ�
    public void TutorialNextOnClick(int i)
    {
        if (0 <= i && i <= 11)
        {
            tutorialCanvas.transform.GetChild(i).gameObject.SetActive(false);       // ���� �̹����� ����
            tutorialCanvas.transform.GetChild(i + 1).gameObject.SetActive(true);      // ���� �̹����� �Ҵ�
        }
        else
        {
            //Debug.Log("�������� ����");
        }

        if (i == 12)     // ������ �̹��� ��ư�� �� �۵�
        {
            tutorialCanvas.transform.GetChild(12).gameObject.SetActive(false);        // ������ �̹��� ����
            Time.timeScale = 1.0f;
        }
    }

    public void TutorialBeforeOnClick(int i)
    {
        if (0 <= i && i <= 12)
        {
            tutorialCanvas.transform.GetChild(i).gameObject.SetActive(false);       // ���� �̹����� ����
            tutorialCanvas.transform.GetChild(i - 1).gameObject.SetActive(true);      // ���� �̹����� �Ҵ�
        }
    }

    public void TutorialGoToRecap()
    {
        tutorialCanvas.transform.GetChild(0).gameObject.SetActive(false);        // ù ��° �̹��� ����
        tutorialCanvas.transform.GetChild(11).gameObject.SetActive(true);       // ������� �ٷ� ����
    }
}
