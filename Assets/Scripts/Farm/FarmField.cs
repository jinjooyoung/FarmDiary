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

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.Addfield(this);
        }
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