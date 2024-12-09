using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSaveData : MonoBehaviour
{
    public Grid grid;
    public PlacementSystem placementSystem;
    public OBJPlacer objPlacer;
    public GridData gridData;

    // 저장할 데이터 리스트
    private List<OBJData> objDataList = new List<OBJData>();

    private void Awake()
    {
        // 같은 GridData 인스턴스를 공유
        gridData = placementSystem.placedOBJData;
    }

    // 생성된 모든 오브젝트의 ID와 위치(gridPos), 설치 순서 (index)를 저장
    [System.Serializable]
    public class OBJData
    {
        public int ID;                  // 고유 ID
        public Vector3Int objPosition; // 설치 그리드 위치
        public int Index;               // 설치 순서
    }

    // OBJData 리스트를 담을 래퍼 클래스
    [System.Serializable]
    public class OBJDataListWrapper
    {
        public List<OBJData> objDataList;
    }

    // ID를 받아서 프리팹 오브젝트를 리턴받는 함수    로드에서 사용
    public GameObject GetPrefabFromResourcesByID(int id)
    {
        return Resources.Load<GameObject>($"Prefabs/{id}");
    }

    //===================================

    public void SaveOBJs()
    {
        objDataList.Clear();  // 리스트 초기화

        // placedGameObjects에 있는 모든 게임 오브젝트에 대해 처리
        for (int i = 0; i < objPlacer.placedGameObjects.Count; i++)
        {
            GameObject placedObj = objPlacer.placedGameObjects[i];

            // 게임 오브젝트 이름에서 ID를 추출 (예: "123(Clone)"에서 123을 추출)
            int objectID = int.Parse(placedObj.name.Split('(')[0]);

            // 게임 오브젝트의 transform.position을 Grid의 Cell로 변환
            Vector3Int objPosition = grid.WorldToCell(placedObj.transform.position);

            // OBJData 생성
            OBJData objData = new OBJData
            {
                ID = objectID,
                objPosition = objPosition,
                Index = i
            };

            // 리스트에 추가
            objDataList.Add(objData);
        }

        // OBJData 리스트를 JSON으로 변환하여 저장
        string json = JsonUtility.ToJson(new OBJDataListWrapper { objDataList = objDataList });

        // SaveSystem의 Save 메서드를 사용하여 파일에 저장
        SaveSystem.Save(json, "PlacedObjects"); // "PlacedObjects"는 파일 이름
        Debug.Log("오브젝트 저장 완료");
    }
}