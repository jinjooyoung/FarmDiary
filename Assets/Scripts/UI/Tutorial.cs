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
        tutorialCanvas = gameObject;        // tutorialCanvas 변수에 현재 오브젝트를 할당
        tutorialCanvas.SetActive(true);

        // 튜토리얼캔버스의 하위 자식들을 false로 만듦
        for (int i = 0; i < 21; i++)
        {
            tutorialCanvas.transform.GetChild(i).gameObject.SetActive(false);
        }
        PlayerPrefs.DeleteKey("TutorialDone");      // 테스트용. 항상 튜토리얼을 처음보는 상태로 만듦. 다 개발되면 삭제해야함
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0)     // 튜토리얼을 봤는지 안 봤는지. 
        {
            Debug.Log("튜토리얼 시작 호출됨");
            // 튜토리얼을 보지 않았다면 아래 코드 실행
            TutorialStart();
            PlayerPrefs.SetInt("TutorialDone", 1);          // 튜토리얼을 봤음
        }

        if (values.Contains(index)) // 엔터를 눌러야하는 화면일때
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))  // 엔터 키를 누르면
            {
                // 다음 화면으로 넘어가는 로직
            }
        }
    }

    public void TutorialStart()     // 튜토리얼을 보여주기 시작하는 함수
    {
        index = 0;
        GameObject txtOBJ = tutorialCanvas.transform.GetChild(index).gameObject;
        txtOBJ.SetActive(true);
        StartCoroutine(FloatingTextEffect(txtOBJ));
    }

    // 오브젝트가 위아래로 움직이는 효과
    private IEnumerator FloatingTextEffect(GameObject textOBJ, float amplitude = 30f, float frequency = 1f)
    {
        Vector3 startPos = textOBJ.transform.position; // 초기 위치 저장
        float timer = 0f;

        while (textOBJ.activeSelf) // 오브젝트 SetActive(true)인 경우 무한 반복
        {
            timer += Time.deltaTime;
            float yOffset = Mathf.Sin(timer * frequency * Mathf.PI * 2) * amplitude; // 위아래로 움직이는 y 오프셋 계산

            // 위치 업데이트
            textOBJ.transform.position = startPos + new Vector3(0, yOffset, 0);

            yield return null; // 한 프레임 대기
        }
    }

    // 아래 함수들 수정해야함
    // 버튼에 붙이는 함수
    public void TutorialNextOnClick(int i)
    {
        if (0 <= i && i <= 11)
        {
            tutorialCanvas.transform.GetChild(i).gameObject.SetActive(false);       // 현재 이미지를 끄고
            tutorialCanvas.transform.GetChild(i + 1).gameObject.SetActive(true);      // 다음 이미지를 켠다
        }
        else
        {
            //Debug.Log("존재하지 않음");
        }

        if (i == 12)     // 마지막 이미지 버튼일 때 작동
        {
            tutorialCanvas.transform.GetChild(12).gameObject.SetActive(false);        // 마지막 이미지 끄기
            Time.timeScale = 1.0f;
        }
    }

    public void TutorialBeforeOnClick(int i)
    {
        if (0 <= i && i <= 12)
        {
            tutorialCanvas.transform.GetChild(i).gameObject.SetActive(false);       // 현재 이미지를 끄고
            tutorialCanvas.transform.GetChild(i - 1).gameObject.SetActive(true);      // 다음 이미지를 켠다
        }
    }

    public void TutorialGoToRecap()
    {
        tutorialCanvas.transform.GetChild(0).gameObject.SetActive(false);        // 첫 번째 이미지 끄기
        tutorialCanvas.transform.GetChild(11).gameObject.SetActive(true);       // 요약으로 바로 가기
    }
}
