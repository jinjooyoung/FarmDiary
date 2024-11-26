using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using static Crop; // Crop Ŭ���� ������ ������ ����

public class FarmField : MonoBehaviour
{
    private GameManager gameManager;

    public bool isPreview;

    public int ID;
    public int PlacedObjectIndex;
    public CropState cropState; // �۹� ����
    public SeedPlantedState seedPlantedState; // ���� ����
    public List<Vector3Int> occupiedPositions;

    public Grid grid;

    public Vector2 fieldPos;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.Addfield(this);
        }

        grid = FindObjectOfType<Grid>();

        if (grid == null)
        {
            Debug.LogError("Grid �ý����� ã�� �� �����ϴ�!");
            return;
        }

        // ���� ������Ʈ�� ���� ��ǥ���� �׸��� ��ǥ�� ��ȯ
        SetFieldPosition(transform.position);
    }

    public void SetFieldPosition(Vector3 worldPosition)
    {
        // �׸��� ��ǥ�� ��ȯ
        Vector3Int gridPosition = grid.WorldToCell(worldPosition);
        fieldPos = new Vector2(gridPosition.x, gridPosition.y);

        Debug.Log($"�� ��ġ ����: ���� ��ġ={worldPosition}, �׸��� ��ġ={gridPosition}, seedPosition={fieldPos}");
    }

    public void SetGrid(Grid grid)
    {
        this.grid = grid;
    }

    public void LoadPlacementData(PlacementData placementData)
    {
        Debug.Log($"�ε�� ID: {placementData.ID}, ��ġ: {placementData.occupiedPositions}");
        this.ID = placementData.ID;
        this.PlacedObjectIndex = placementData.PlacedObjectIndex;
        this.cropState = placementData.cropState;
        this.seedPlantedState = placementData.seedPlantedState;
        this.occupiedPositions = placementData.occupiedPositions;
    }
}