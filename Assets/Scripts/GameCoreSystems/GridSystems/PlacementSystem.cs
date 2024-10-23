using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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

    private int ID = -1;


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
        this.ID = ID;
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

    public void StartCropPlant(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);              // ��ü �׸��� UI
        buildingState = new CropPlacementState(ID,
                                           grid,
                                           preview,
                                           database,
                                           placedOBJData,
                                           objectPlacer);
        inputManager.OnClicked += PlaceStructure;       // ��ġ ���϶� �׻� �Է��� ���� �� �ֵ��� �̺�Ʈ�� �Ҵ�
        inputManager.OnExit += StopPlacement;
    }

    public void StartCropRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new CropRemovingState(grid, preview, placedOBJData, objectPlacer);
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

        int selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);

        // ��ġ�� ������Ʈ�� �����ϴ� ĭ�� �ʵ� ID�� ��� ����Ʈ (1������ 2���ϱ�)
        List<int> IDs = GetFieldIDs(gridPosition, database.objectsData[selectedObjectIndex].Size);

        if (IDs.Count == 1)
        {
            if (!fieldManager.IsFieldUnlocked(IDs[0]))
            {
                preview.ApplyFeedbackToCursor(false);
                return;
            }
        }
        else
        {
            if (!fieldManager.IsFieldUnlocked(IDs[0]) || !fieldManager.IsFieldUnlocked(IDs[1]))
            {
                preview.ApplyFeedbackToCursor(false);
                return;
            }
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

    private List<int> GetFieldIDs(Vector3Int mouseGridPosition, Vector2Int objectSize)
    {
        // ���콺 ��ġ�κ��� ��ǥ ����Ʈ�� ���
        List<Vector3Int> positions = CalculatePositions(mouseGridPosition, objectSize);

        HashSet<int> uniqueFieldIDs = new HashSet<int>();

        foreach (var position in positions)
        {
            int fieldID = GetFieldIDFromPosition(position);
            uniqueFieldIDs.Add(fieldID); // �ߺ��� �ڵ����� ����
        }

        // HashSet�� List�� ��ȯ�Ͽ� ��ȯ
        return uniqueFieldIDs.ToList();
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, y, 0));
            }
        }
        return returnVal;
    }

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
