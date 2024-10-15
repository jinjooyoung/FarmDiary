using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private FieldManager fieldManager;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    /*[SerializeField]
    private AudioSource source;     // ��ġ ���� �ҽ�*/

    public GridData placedOBJData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private OBJPlacer objectPlacer;


    IBuildingState buildingState;

    private void Awake()
    {
        StopPlacement();
        placedOBJData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);              // ��ü �׸��� UI
        buildingState = new PlacementState(ID,
                                           grid,
                                           preview,
                                           database,
                                           placedOBJData,
                                           objectPlacer);
        inputManager.OnClicked += PlaceStructure;       // ��ġ ���϶� �׻� �Է��� ���� �� �ֵ��� �̺�Ʈ�� �Ҵ�
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, placedOBJData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;       // ���� ���϶� �׻� �Է��� ���� �� �ֵ��� �̺�Ʈ�� �Ҵ�
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())     // UI Ŭ���� ���� ������Ʈ ��ȣ�ۿ� ���� (UI�� Ŭ���� �� Ŭ���� ��ġ�� �ڿ� ���� ��ġ�� ���� �ʵ���)
        {
            return;
        }
        Vector2 mousePosition = inputManager.GetSelectedMapPosition();      // ���� ���콺 ��ġ
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);          // ���� �׸��� ��ġ

        int fieldID = GetFieldIDFromPosition(gridPosition);

        // FieldManager�� ���� �ش� �ʵ尡 �رݵǾ����� Ȯ��
        if (!fieldManager.IsFieldUnlocked(fieldID))
        {
            preview.ApplyFeedbackToCursor(false);
            return;
        }

        buildingState.OnAction(gridPosition);       // �ش� �׸��� ��ġ�� ������Ʈ ��ġ (���� ����)
    }

    private int GetFieldIDFromPosition(Vector3Int position)
    {
        int x = position.x;

        // �߾� �ʵ� ID�� 0
        if (x >= -18 && x <= 17)
            return 0;

        // ���� �ʵ� ID ��� (ID : -1�� ���ΰ� 9ĭ, ���Ĵ� 10ĭ��)
        if (x < -18)
            return -((Mathf.Abs(x) - 19) / 10 + 1);

        // ������ �ʵ� ID ��� (ID : 1�� ���ΰ� 9ĭ, ���Ĵ� 10ĭ��)
        if (x > 17)
            return (x - 18) / 10 + 1;

        return 0;  // �⺻�� (�ʿ��� ��� ����)
    }

    // ���õ� (��ġ�� ������) ������Ʈ�� ����� �޾ƿͼ� CanPlaceObjectAt�� �Ѱ�, ��ġ �������� bool�� ����
    // PlacementState ��ũ��Ʈ�� �̵��� �޼���. ������ ���߿� �ٽ� �Űܿü��� ���� �� ���Ƽ� �ϴ� �ּ����� ��
    /*private bool GetPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        return placedOBJData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }*/

    private void StopPlacement()
    {
        if (buildingState == null)      // �̹� ��ġ�� ���� �����̸� ����
        {
            return;
        }
        gridVisualization.SetActive(false);         // �׸��� UI ��
        buildingState.EndState();                   // ������ ����
        inputManager.OnClicked -= PlaceStructure;   // ��ġ ���°� ����Ǿ����Ƿ� �̺�Ʈ �Ҵ� ����
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)    // ���õ� ������Ʈ�� ������ ����
        {
            return;
        }
        Vector2 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)       // ���ʿ��� ������Ʈ ������ ���� ���콺�� �������� ���� ������Ʈ
        {
            int fieldID = GetFieldIDFromPosition(gridPosition);

            if (!fieldManager.IsFieldUnlocked(fieldID))
            {
                preview.ApplyFeedbackToCursor(false);
            }
            buildingState.UpdateState(gridPosition);    // ������ ������Ʈ ������Ʈ ������Ʈ
            lastDetectedPosition = gridPosition;
        }
    }
}
