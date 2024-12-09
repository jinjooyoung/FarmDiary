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

        // 일단 솥부터
        //newSaveData.SaveOBJs();
    }

    // 모든 데이터를 한번에 로드하는 메서드
    public void LoadGameData()
    {
        // 각각의 데이터를 로드하는 메서드를 호출
        //saveDatas.LoadCrops();
    }
}
