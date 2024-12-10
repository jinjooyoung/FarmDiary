using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance; // 싱글톤 패턴 사용

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
    public void SaveGameData()
    {
        // 각각의 저장해야 할 데이터들을 SaveDatas에서 처리
        //saveDatas.SaveCrops(objPlacer.placedGameObjects);

        // 설치되는 모든 오브젝트 저장
        newSaveData.SaveOBJs();
        // AI 데이터 저장
        newSaveData.SaveAIData();
        // 창고 저장
        newSaveData.SaveStorage();
        // 업적 데이터 저장
        newSaveData.SaveAchievements();
        // 구매 가격 저장
        newSaveData.SaveBuyPrice();
    }

    // 모든 데이터를 한번에 로드하는 메서드
    public void LoadGameData()
    {
        // 각각의 데이터를 로드하는 메서드를 호출
        //saveDatas.LoadCrops();

        // 오브젝트 다시 재생성해서 로드함, 설치되고 나서 진행된 오브젝트 각각의 상황은 로드 안 됨, 오브젝트 먼저 로드해서 생성해두고 따로 저장한 정보 덮어쓰기 해야함
        newSaveData.LoadOBJs();
        // AI 데이터 로드
        newSaveData.LoadAIData();
        // 창고 데이터 로드
        newSaveData.LoadStorage();
        // 업적 데이터 로드
        newSaveData.LoadAchievements();
        // 구매 가격 로드
        newSaveData.LoadBuyPrice();
    }
}
