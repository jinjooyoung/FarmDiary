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
        Vector2 mousePosition = inputManager.GetSelectedMapPosition();      // ���� ���콺 ��ġ
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);          // ���� �׸��� ��ġ        

        if (Input.GetKeyDown(KeyCode.E))        // ��ġ�� ������Ʈ�� ������ �޾ƿͼ� Debug�� ���
        {
            if (placedData.placedFields.ContainsKey(gridPosition))
            {
                PlacementData placementData = placedData.placedFields[gridPosition];

                // placementData�� null���� Ȯ��
                if (placementData == null)
                {
                    Debug.LogError("PlacementData is null!");
                    return;
                }

                // ���� ���������� ����ٸ�
                Debug.Log($"Found object with ID: {placementData.ID}");
                Debug.Log($"Placed Object Index: {placementData.PlacedObjectIndex}");

                // ������Ʈ�� �����ϴ� ��ġ�� ���
                Debug.Log("Occupied Positions:");
                int count = 0;
                foreach (var position in placementData.occupiedPositions)
                {
                    count++;
                    Debug.Log($"- {position}");
                }
                Debug.Log($"�� {count} ĭ");
            }
            else
            {
                // ���� �������� ���� ���
                Debug.Log($"No object found at grid position: {gridPosition}");
            }

            /*// TryGetObjectAt �޼��带 ����� �� ��ġ�� ������Ʈ�� �ִ��� Ȯ��
            if (placedOBJData.TryGetObjectAt(gridPosition, out PlacementData placementData))
            {
                // ������Ʈ�� ���� ���
                Debug.Log($"������Ʈ ID: {placementData.ID}");
                Debug.Log($"������Ʈ �ε���: {placementData.PlacedObjectIndex}");
                Debug.Log("������Ʈ�� ������ ��ġ��:");

                foreach (Vector3Int cellPosition in placementData.occupiedPositions)
                {
                    Debug.Log($"- {cellPosition}");
                }
            }
            else
            {
                Debug.Log("�ش� ��ġ�� ��ġ�� ������Ʈ�� �����ϴ�.");
            }*/
        }
    }
}
