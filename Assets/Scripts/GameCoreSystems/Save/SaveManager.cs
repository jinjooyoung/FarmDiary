using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance; // 싱글톤 패턴 사용


    // 저장이 필요한 변수들이 존재하는 스크립트들 참조 또는 선언
    [Header("플레이어 위치")]
    [SerializeField] private Transform playerPos;
    [Header("OBJ Placer")]
    [SerializeField] private OBJPlacer objPlacer;
    [Header("직렬화 클래스, 세이브 로직")]
    [SerializeField] private NewSaveData newSaveData;
    /*[SerializeField] private SaveDatas saveDatas;*/


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
    }

    // 모든 데이터를 한번에 로드하는 메서드
    public void LoadGameData()
    {
        // 각각의 데이터를 로드하는 메서드를 호출
        //saveDatas.LoadCrops();

        // 오브젝트 다시 재생성해서 로드함, 설치되고 나서 진행된 오브젝트 각각의 상황은 로드 안 됨, 오브젝트 먼저 로드해서 생성해두고 따로 저장한 정보 덮어쓰기 해야함
        newSaveData.LoadOBJs();
    }
}
