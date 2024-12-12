using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSnackBar : MonoBehaviour
{
    public static AchievementSnackBar Instance;

    [SerializeField] private GameObject snackbarUI;  // ������ UI ������Ʈ
    [SerializeField] private Text DesText; // ���� �ؽ�Ʈ
    [SerializeField] private Image achievementIcon; // ������ �̹���

    // ť�� ���Լ��� ����Ʈ ���� ������ �����͸� ť�� �߰��ϰ� ���� �� ����
    // �������� ������ ó�� ���� �����ͺ��� ����, foreach �߿��� �迭�� ���Ұ� ����Ǹ� ������ �������� ť�� ������ ����
    private Queue<int> achievementQueue = new Queue<int>(); // ���� ID�� ������ ť
    private bool isDisplaying = false; // ���� �����ٸ� ǥ�� ������ ����

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

    public void ShowSnackbar(int id)
    {
        // ť�� ���� ID �߰�
        achievementQueue.Enqueue(id);

        // �����ٸ� ǥ�� ���� �ƴ϶�� ó�� ����
        if (!isDisplaying)
        {
            StartCoroutine(DisplaySnackbarCoroutine());
        }
    }

    private IEnumerator DisplaySnackbarCoroutine()
    {
        isDisplaying = true;

        while (achievementQueue.Count > 0)
        {
            int id = achievementQueue.Dequeue();

            // �ʱ�ȭ
            SnackBarInitialize(id);

            // ������ ǥ�� �ִϸ��̼� (���� �ö����)
            float startY = -3.43f;
            float targetY = -2.66f;
            float duration = 0.5f;
            float elapsed = 0f;

            snackbarUI.transform.localPosition = new Vector3(snackbarUI.transform.localPosition.x, startY, snackbarUI.transform.localPosition.z);

            while (elapsed < duration)
            {
                float newY = Mathf.Lerp(startY, targetY, elapsed / duration);
                snackbarUI.transform.localPosition = new Vector3(snackbarUI.transform.localPosition.x, newY, snackbarUI.transform.localPosition.z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            snackbarUI.transform.localPosition = new Vector3(snackbarUI.transform.localPosition.x, targetY, snackbarUI.transform.localPosition.z);

            // 3�� ���
            yield return new WaitForSeconds(3f);

            // ������ ����� �ִϸ��̼� (�Ʒ��� ��������)
            elapsed = 0f;
            while (elapsed < duration)
            {
                float newY = Mathf.Lerp(targetY, startY, elapsed / duration);
                snackbarUI.transform.localPosition = new Vector3(snackbarUI.transform.localPosition.x, newY, snackbarUI.transform.localPosition.z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            snackbarUI.transform.localPosition = new Vector3(snackbarUI.transform.localPosition.x, startY, snackbarUI.transform.localPosition.z);
        }

        isDisplaying = false; // ��� ���� ó�� �Ϸ�
    }

    private void SnackBarInitialize(int id)
    {
        string name = AchievementsDatabase.GetName(id); // ���� �̸� ��������
        achievementIcon.sprite = Resources.Load<Sprite>($"Achievements/Icons/Achievement_{id}");
        DesText.text = $"���� [{name}] �޼�!";
    }
}
