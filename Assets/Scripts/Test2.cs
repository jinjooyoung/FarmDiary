using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class Test2 : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private Grid grid;

    /*[SerializeField]
    private ObjectsDatabaseSO database;*/

    [SerializeField]
    private GridData placedData;

    [SerializeField]
    private PlacementSystem placementSystem;

    /*[SerializeField]
    private int selectedObjectIndex = -1;
    private int ID = -1;*/

    void Start()
    {
        placedData = placementSystem.placedOBJData;

        if (placedData == null)
        {
            Debug.LogError("GridData is null!");
            return;
        }

        if (placedData.placedFields == null)
        {
            Debug.LogError("placedObjects is null!");
            return;
        }

        
    }

    void Update()
    {
        Vector2 mousePosition = inputManager.GetSelectedMapPosition();      // 현재 마우스 위치
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);          // 현재 그리드 위치        

        if (Input.GetKeyDown(KeyCode.E))        // 설치된 오브젝트의 정보를 받아와서 Debug로 출력
        {
            if (placedData.placedFields.ContainsKey(gridPosition))
            {
                PlacementData placementData = placedData.placedFields[gridPosition];

                // placementData가 null인지 확인
                if (placementData == null)
                {
                    Debug.LogError("PlacementData is null!");
                    return;
                }

                // 값을 성공적으로 얻었다면
                Debug.Log($"Found object with ID: {placementData.ID}");
                Debug.Log($"Placed Object Index: {placementData.PlacedObjectIndex}");

                // 오브젝트가 차지하는 위치들 출력
                Debug.Log("Occupied Positions:");
                int count = 0;
                foreach (var position in placementData.occupiedPositions)
                {
                    count++;
                    Debug.Log($"- {position}");
                }
                Debug.Log($"총 {count} 칸");
            }
            else
            {
                // 값이 존재하지 않을 경우
                Debug.Log($"No object found at grid position: {gridPosition}");
            }

            /*// TryGetObjectAt 메서드를 사용해 그 위치에 오브젝트가 있는지 확인
            if (placedOBJData.TryGetObjectAt(gridPosition, out PlacementData placementData))
            {
                // 오브젝트의 정보 출력
                Debug.Log($"오브젝트 ID: {placementData.ID}");
                Debug.Log($"오브젝트 인덱스: {placementData.PlacedObjectIndex}");
                Debug.Log("오브젝트가 차지한 위치들:");

                foreach (Vector3Int cellPosition in placementData.occupiedPositions)
                {
                    Debug.Log($"- {cellPosition}");
                }
            }
            else
            {
                Debug.Log("해당 위치에 설치된 오브젝트가 없습니다.");
            }*/
        }
    }
}
