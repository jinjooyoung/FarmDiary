using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSnackBar : MonoBehaviour
{
    public static AchievementSnackBar Instance;

    [Header("업적 스낵바 부모 오브젝트")]
    public GameObject snackbarUI;       // 업적 스낵바 게임오브젝트

    [Header("업적 스낵바 구성 요소")]
    public Image achievementIcon;       // 클리어된 업적 아이콘
    public Text DesText;                // 설명 텍스트

    public float moveSpeed = 2f;        // 스낵바 이동 속도
    public float popUpHeight = 100f;    // 스낵바 보여질 위치의 높이
    public float waitTime = 3f;         // 스낵바가 떠오르고 멈춰있는 시간

    // 스낵바가 표시될 업적 ID를 관리할 리스트
    public List<int> achievementIDs = new List<int>();
    // 업적의 진행 상태 추적 (중복 표시 방지)
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

        string name = AchievementsDatabase.GetName(ID);                 // 업적 이름
        achievementIcon.sprite = Resources.Load<Sprite>($"Achievements/Icons/Achievement_{ID}");

        DesText.text = "업적 [" + name + "] 달성!";
    }

    // 스낵바 애니메이션 코루틴
    public IEnumerator SnackbarAnimation()
    {
        // achievementIDs 리스트에서 순차적으로 하나씩 업적을 처리
        while (achievementIDs.Count > 0)
        {
            int currentID = achievementIDs[0];  // 첫 번째 업적 ID를 가져옴

            if (snackDoneDict.ContainsKey(currentID) && !snackDoneDict[currentID])
            {
                // 업적 스낵바 초기화
                SnackBarInitialize(currentID);

                // 초기 위치 저장
                Vector3 initialPosition = snackbarUI.transform.position;

                // 스낵바를 위로 이동 (3초 대기 전)
                float elapsedTime = 0f;
                while (elapsedTime < 1f)
                {
                    snackbarUI.transform.position = Vector3.Lerp(initialPosition, initialPosition + Vector3.up * popUpHeight, elapsedTime);
                    elapsedTime += Time.deltaTime * moveSpeed;
                    yield return null;
                }
                snackbarUI.transform.position = initialPosition + Vector3.up * popUpHeight;

                // 3초 대기
                yield return new WaitForSeconds(waitTime);

                // 스낵바를 아래로 천천히 이동
                elapsedTime = 0f;
                Vector3 targetPosition = initialPosition;
                while (elapsedTime < 1f)
                {
                    snackbarUI.transform.position = Vector3.Lerp(snackbarUI.transform.position, targetPosition, elapsedTime);
                    elapsedTime += Time.deltaTime * moveSpeed;
                    yield return null;
                }
                snackbarUI.transform.position = targetPosition;

                // 해당 업적에 대한 표시 완료 처리
                snackDoneDict[currentID] = true;

                // 업적 ID를 리스트에서 제거
                achievementIDs.RemoveAt(0);
            }
            else
            {
                // 이미 표시된 업적은 건너뛰고 리스트에서 제거
                achievementIDs.RemoveAt(0);
            }

            yield return null;
        }
    }
}
