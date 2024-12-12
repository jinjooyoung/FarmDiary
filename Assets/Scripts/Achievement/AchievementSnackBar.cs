using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSnackBar : MonoBehaviour
{
    public static AchievementSnackBar Instance;

    [SerializeField] private GameObject snackbarUI;  // 스낵바 UI 오브젝트
    [SerializeField] private Text DesText; // 설명 텍스트
    [SerializeField] private Image achievementIcon; // 아이콘 이미지

    // 큐는 선입선출 리스트 같은 것으로 데이터를 큐에 추가하고 꺼낼 수 있음
    // 꺼낼때는 무조건 처음 넣은 데이터부터 꺼냄, foreach 중에는 배열의 원소가 변경되면 오류가 생기지만 큐는 생기지 않음
    private Queue<int> achievementQueue = new Queue<int>(); // 업적 ID를 저장할 큐
    private bool isDisplaying = false; // 현재 스낵바를 표시 중인지 여부

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
        // 큐에 업적 ID 추가
        achievementQueue.Enqueue(id);

        // 스낵바를 표시 중이 아니라면 처리 시작
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

            // 초기화
            SnackBarInitialize(id);

            // 스낵바 표시 애니메이션 (위로 올라오기)
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

            // 3초 대기
            yield return new WaitForSeconds(3f);

            // 스낵바 숨기기 애니메이션 (아래로 내려가기)
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

        isDisplaying = false; // 모든 업적 처리 완료
    }

    private void SnackBarInitialize(int id)
    {
        string name = AchievementsDatabase.GetName(id); // 업적 이름 가져오기
        achievementIcon.sprite = Resources.Load<Sprite>($"Achievements/Icons/Achievement_{id}");
        DesText.text = $"업적 [{name}] 달성!";
    }
}
