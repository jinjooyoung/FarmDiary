using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSnackBar : MonoBehaviour
{
    public static AchievementSnackBar Instance;

    [Header("���� ������ �θ� ������Ʈ")]
    public GameObject snackbarUI;       // ���� ������ ���ӿ�����Ʈ

    [Header("���� ������ ���� ���")]
    public Image achievementIcon;       // Ŭ����� ���� ������
    public Text DesText;                // ���� �ؽ�Ʈ

    public float moveSpeed = 2f;        // ������ �̵� �ӵ�
    public float popUpHeight = 100f;    // ������ ������ ��ġ�� ����
    public float waitTime = 3f;         // �����ٰ� �������� �����ִ� �ð�

    // �����ٰ� ǥ�õ� ���� ID�� ������ ����Ʈ
    public List<int> achievementIDs = new List<int>();
    // ������ ���� ���� ���� (�ߺ� ǥ�� ����)
    public Dictionary<int, bool> snackDoneDict = new Dictionary<int, bool>();

    private int ID;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SnackBarInitialize(int id)
    {
        ID = id;

        string name = AchievementsDatabase.GetName(ID);                 // ���� �̸�
        achievementIcon.sprite = Resources.Load<Sprite>($"Achievements/Icons/Achievement_{ID}");

        DesText.text = "���� [" + name + "] �޼�!";
    }

    // ������ �ִϸ��̼� �ڷ�ƾ
    public IEnumerator SnackbarAnimation()
    {
        // achievementIDs ����Ʈ���� ���������� �ϳ��� ������ ó��
        while (achievementIDs.Count > 0)
        {
            int currentID = achievementIDs[0];  // ù ��° ���� ID�� ������

            if (snackDoneDict.ContainsKey(currentID) && !snackDoneDict[currentID])
            {
                // ���� ������ �ʱ�ȭ
                SnackBarInitialize(currentID);

                // �ʱ� ��ġ ����
                Vector3 initialPosition = snackbarUI.transform.position;

                // �����ٸ� ���� �̵� (3�� ��� ��)
                float elapsedTime = 0f;
                while (elapsedTime < 1f)
                {
                    snackbarUI.transform.position = Vector3.Lerp(initialPosition, initialPosition + Vector3.up * popUpHeight, elapsedTime);
                    elapsedTime += Time.deltaTime * moveSpeed;
                    yield return null;
                }
                snackbarUI.transform.position = initialPosition + Vector3.up * popUpHeight;

                // 3�� ���
                yield return new WaitForSeconds(waitTime);

                // �����ٸ� �Ʒ��� õõ�� �̵�
                elapsedTime = 0f;
                Vector3 targetPosition = initialPosition;
                while (elapsedTime < 1f)
                {
                    snackbarUI.transform.position = Vector3.Lerp(snackbarUI.transform.position, targetPosition, elapsedTime);
                    elapsedTime += Time.deltaTime * moveSpeed;
                    yield return null;
                }
                snackbarUI.transform.position = targetPosition;

                // �ش� ������ ���� ǥ�� �Ϸ� ó��
                snackDoneDict[currentID] = true;

                // ���� ID�� ����Ʈ���� ����
                achievementIDs.RemoveAt(0);
            }
            else
            {
                // �̹� ǥ�õ� ������ �ǳʶٰ� ����Ʈ���� ����
                achievementIDs.RemoveAt(0);
            }

            yield return null;
        }
    }
}
