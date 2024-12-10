using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance; // 싱글톤 패턴 사용
    public GameObject loadingIcon;

    [Header("직렬화 클래스, 세이브 로직")]
    [SerializeField] private NewSaveData newSaveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 모든 데이터를 한번에 저장하는 메서드
    public async Task SaveGameData()
    {
        loadingIcon.SetActive(true);

        // 동기 작업
        // AI 데이터 저장
        newSaveData.SaveAIData();
        // 창고 저장
        newSaveData.SaveStorage();
        // 업적 데이터 저장
        newSaveData.SaveAchievements();
        // 구매 가격 저장
        newSaveData.SaveBuyPrice();


        // 비동기 작업
        // 설치되는 모든 오브젝트 저장
        await newSaveData.SaveOBJs();

        UpdateUIAfterSaveLoad();
    }

    // 모든 데이터를 한번에 로드하는 메서드
    public async Task LoadGameData()
    {
        loadingIcon.SetActive(true);
        // 동기 작업
        // AI 데이터 로드
        newSaveData.LoadAIData();
        // 창고 데이터 로드
        newSaveData.LoadStorage();
        // 업적 데이터 로드
        newSaveData.LoadAchievements();
        // 구매 가격 로드
        newSaveData.LoadBuyPrice();

        // 비동기 작업
        // 오브젝트 다시 재생성해서 로드함, 설치되고 나서 진행된 오브젝트 각각의 상황은 로드 안 됨, 오브젝트 먼저 로드해서 생성해두고 따로 저장한 정보 덮어쓰기 해야함
        await newSaveData.LoadOBJs();

        UpdateUIAfterSaveLoad();
    }

    // UI 업데이트 메서드 (메인 스레드에서 실행)
    private void UpdateUIAfterSaveLoad()
    {
        loadingIcon.SetActive(false);
        Debug.LogWarning("비동기 처리 완료 되어 UI 업데이트 호출됨!");
    }
}
