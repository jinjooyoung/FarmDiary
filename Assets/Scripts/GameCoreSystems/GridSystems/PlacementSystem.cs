using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;
    private int selectedObjectIndex = -1;       // �ƹ��͵� ���õ��� �ʾ��� �� �⺻ �ε���

    [SerializeField]
    private GameObject gridVisualization;

    /*[SerializeField]
    private AudioSource source;     // ��ġ ���� �ҽ�*/

    private GridData furnitureData;

    private List<GameObject> placedGameObjects = new();

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    private void Start()
    {
        StopPlacement();
        furnitureData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        // FindIndex �޼��� : ����Ʈ���� ������ �����ϴ� ù ��° ����� �ε����� ã�� �޼���
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        // �־��� ID�� ������ ID�� ���� ������Ʈ �������� �ε����� ã�Ƽ� ������ ����

        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"Can't find index {ID}");
            return;
        }
        gridVisualization.SetActive(true);              // ��ü �׸��� UI
        preview.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab,
            database.objectsData[selectedObjectIndex].Size);
        inputManager.OnClicked += PlaceStructure;
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

        bool placementValidity = GetPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
        {
            return;
        }

        //source.Play();        // ��ġ�� �� ���� ���
        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);

        SpriteRenderer newObjectRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
        preview.SetAlpha(newObjectRenderer, 1.0f);

        placedGameObjects.Add(newObject);
        GridData selectedData = furnitureData;
        selectedData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID, placedGameObjects.Count - 1);

        preview.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), false);
    }

    private bool GetPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        return furnitureData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        preview.StopShowingPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
        {
            return;
        }
        Vector2 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            bool placementValidity = GetPlacementValidity(gridPosition, selectedObjectIndex);

            mouseIndicator.transform.position = mousePosition;
            preview.UpdatePreviewOBJPos(grid.CellToWorld(gridPosition), placementValidity);
            lastDetectedPosition = gridPosition;
        }
    }
}
