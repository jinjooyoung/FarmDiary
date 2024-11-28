using System.Collections.Generic;
using System;
using UnityEngine;
using static Crop;

[System.Serializable]
public class GridData
{
    // �׸��� ���� ��ġ(Vector3Int)�� Ű�� ����ϰ�, �� ��ġ�� ��ġ�� ������Ʈ�� ����
    public Dictionary<Vector3Int, PlacementData> placedFields = new();
    public Dictionary<Vector3Int, PlacementData> placedDecos = new();
    public Dictionary<Vector3Int, PlacementData> placedFacilities = new();
    public Dictionary<Vector3Int, PlacementData> placedCrops = new();

    // ��ġ�� ������Ʈ�� ������ �����ϰ� �� �� �� ��ġ �������� üũ�ϴ� �޼���
    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        // ��ġ�� ��ġ�� �׸��� �������� �޾ƿͼ� ����Ʈ�� ����
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        // ��ġ�� ������Ʈ�� ������ ����
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

        // foreach �ݺ��� ���鼭 �̹� ������ ĭ�� �ִ��� Ȯ��
        // ��¥�� �� �Լ��� ȣ���ϴ� �������� ��ġ ���� ������ üũ������ �� �� �� üũ�����μ� Ȥ�� �� ���׸� ����
        if (ID < 5) // �� ID�� �� ��ųʸ��� �߰�
        {
            foreach (var pos in positionToOccupy)
            {
                if (placedFields.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                else if (placedDecos.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                else if (placedFacilities.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                placedFields[pos] = data;
                // ������Ʈ�� �����ϴ� ĭ�� pos���� ������Ʈ�� ������ ��ųʸ��� ����
            }
        }
        else if (ID >= 200 && ID < 300)     // ���ڿ�����Ʈ ID�� ���� ��ųʸ��� �߰�
        {
            foreach (var pos in positionToOccupy)
            {
                if (placedFields.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                else if (placedDecos.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                else if (placedFacilities.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                placedDecos[pos] = data;
                // ������Ʈ�� �����ϴ� ĭ�� pos���� ������Ʈ�� ������ ��ųʸ��� ����
            }
        }
        else  // �� ���� ID�� ��� ������Ʈ�̹Ƿ� ��� ��ųʸ��� ���� (�۹� ID�� ��¥�� �ؿ��� ȣ���ϱ⶧����)
        {
            foreach (var pos in positionToOccupy)
            {
                if (placedFields.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                else if (placedDecos.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                else if (placedFacilities.ContainsKey(pos))
                {
                    throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
                }
                placedFacilities[pos] = data;
                // ������Ʈ�� �����ϴ� ĭ�� pos���� ������Ʈ�� ������ ��ųʸ��� ����
            }
        }
    }

    // ��ġ�� �۹��� ������ �����ϰ� �� �� �� ��ġ �������� üũ�ϴ� �޼���
    public void AddCropAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex, 
                            CropState cropState, 
                            SeedPlantedState seedPlantedState)
    {
        // ��ġ�� ��ġ�� �׸��� �������� �޾ƿͼ� ����Ʈ�� ����
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        // ��ġ�� ������Ʈ�� ������ ����
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

        // foreach �ݺ��� ���鼭 �̹� ������ ĭ�� �ִ��� Ȯ��
        // ��¥�� �� �Լ��� ȣ���ϴ� �������� ��ġ ���� ������ üũ������ �� �� �� üũ�����μ� Ȥ�� �� ���׸� ����
        foreach (var pos in positionToOccupy)
        {
            if (placedCrops.ContainsKey(pos))
            {
                throw new Exception($"Dictionary �� �̹� �� ��ġ�� �����մϴ�. {pos}");
            }
            placedCrops[pos] = data;
            // ������Ʈ�� �����ϴ� ĭ�� pos���� ������Ʈ�� ������ ��ųʸ��� ����
        }
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

    // ������Ʈ�� ����� �޾ƿͼ� �� ��ġ�� ��ġ �������� bool�� ����
    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        // ������Ʈ�� ������(���� ��ġ X ����) �׸��� ĭ�� ��ġ�� ����Ʈ�� ����
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);

        // ����� ĭ���� ���鼭 �� ��ġ�� �̹� �ٸ� ������Ʈ�� �����Ǿ� �ִ��� ���
        foreach (var pos in positionToOccupy)
        {
            if (placedFields.ContainsKey(pos) || pos.y > -7 || placedDecos.ContainsKey(pos) || placedFacilities.ContainsKey(pos))     // �� ĭ�̶� �̹� ������ ��ġ�� ������ false�� ��ȯ�Ͽ� ��ġ �Ұ���
            {
                return false;
            }
            //Debug.Log(pos);
        }
        return true;        // ��ġ �����̸� true ������
    }

    // ������ �׸��� ��ġ�� ���� �װ��� ��ġ�� ������Ʈ�� �ε����� ��ȯ�ϴ� �޼���
    // ���⿡�� ���ϴ� ������Ʈ �ε����� ��ũ���ͺ������Ʈ�� �ε����� �ƴ� GridData�� ��ġ�� �� ���� �߰��Ǵ� ����Ʈ�� �ε���
    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedFields.ContainsKey(gridPosition) == false)       // �ƹ��͵� ��ġ�Ǿ� ���� ���� ĭ�̶�� -1 ����
        {
            return -1;
        }
        return placedFields[gridPosition].PlacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedFields[gridPosition].occupiedPositions)
        {
            placedFields.Remove(pos);
            placedCrops.Remove(pos);
        }
    }

    internal void RemoveCropAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedCrops[gridPosition].occupiedPositions)
        {
            placedCrops.Remove(pos);
        }
    }

    public string SaveFieldsAndCrops()
    {
        GridDataSave saveData = new GridDataSave();

        // �۹� ����
        foreach (var crop in placedCrops)
        {
            Renderer renderer = crop.Value != null ? GetRendererFromPositions(crop.Key) : null;
            float alpha = renderer != null ? renderer.material.color.a : 1.0f;

            saveData.crops.Add(new GridCropSave
            {
                position = crop.Key,
                placementData = new PlacementData(
                    crop.Value.occupiedPositions,
                    crop.Value.ID,
                    crop.Value.PlacedObjectIndex,
                    crop.Value.cropState,
                    crop.Value.seedPlantedState,
                    alpha // ���� �� ����
                ),
                id = crop.Value.ID
            });
        }

        // �� ����
        foreach (var field in placedFields)
        {
            Renderer renderer = field.Value != null ? GetRendererFromPositions(field.Key) : null;
            float alpha = renderer != null ? renderer.material.color.a : 1.0f;

            saveData.fields.Add(new GridFieldSave
            {
                position = field.Key,
                placementData = new PlacementData(
                    field.Value.occupiedPositions,
                    field.Value.ID,
                    field.Value.PlacedObjectIndex,
                    field.Value.cropState,
                    field.Value.seedPlantedState,
                    alpha // ���� �� ����
                ),
                id = field.Value.ID
            });
        }

        return JsonUtility.ToJson(saveData);
    }

    // Helper �Լ�: ��ġ���� Renderer ��������
    private Renderer GetRendererFromPositions(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up, Vector3.down, out hit, 10f))
        {
            return hit.collider.GetComponent<Renderer>();
        }
        return null;
    }


    public void LoadFieldsAndCrops(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        GridDataSave saveData = JsonUtility.FromJson<GridDataSave>(json);

        placedCrops.Clear();
        placedFields.Clear();

        // �۹� ����
        foreach (var cropSave in saveData.crops)
        {
            Vector3Int intPosition = Vector3Int.FloorToInt(cropSave.position);
            placedCrops[intPosition] = cropSave.placementData;

            // ���� �� ����
            Renderer renderer = GetRendererFromPositions(cropSave.position);
            if (renderer != null)
            {
                Material material = renderer.material;
                Color color = material.color;
                color.a = cropSave.placementData.alpha;
                material.color = color;
            }

            Debug.Log($"Loading crop data at position: {intPosition}, ID: {cropSave.id}");
        }

        // �� ����
        foreach (var fieldSave in saveData.fields)
        {
            Vector3Int intPosition = Vector3Int.FloorToInt(fieldSave.position);
            placedFields[intPosition] = fieldSave.placementData;

            // ���� �� ����
            Renderer renderer = GetRendererFromPositions(fieldSave.position);
            if (renderer != null)
            {
                Material material = renderer.material;
                Color color = material.color;
                color.a = fieldSave.placementData.alpha;
                material.color = color;
            }

            Debug.Log($"Loading field data at position: {intPosition}, ID: {fieldSave.id}");
        }

        Debug.Log($"Loaded Field Positions: {string.Join(", ", placedFields.Keys)}");
        Debug.Log($"Loaded Crop Positions: {string.Join(", ", placedCrops.Keys)}");
    }

}

[System.Serializable]
public class PlacementData      // ������Ʈ�� ���� Ŭ����
{
    // ������Ʈ�� �����ϴ� �׸��� ������ ��ǥ (������Ʈ ũ�Ⱑ Ŀ�� ���� ĭ�� ������ �� �־ ����Ʈ��)
    public List<Vector3Int> occupiedPositions;
    public int ID { get; set; }                 // ObjectsDataBase Scriptable Object ���� ���Ǵ� ������Ʈ�� ���� ID
    public int PlacedObjectIndex { get; private set; }  // ObjectsDataBase Scriptable Object ���� ������Ʈ�� �ε���

    // �۹� ���� �߰�
    public CropState cropState;
    public SeedPlantedState seedPlantedState;

    public float alpha;

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex,
                        CropState cropState = CropState.NeedsWater, SeedPlantedState seedPlantedState = SeedPlantedState.No,
                        float alpha = 1.0f)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
        this.cropState = cropState;
        this.seedPlantedState = seedPlantedState;
        this.alpha = alpha;
    }

    // JSON ������ ���� �⺻ ������ �߰�
    public PlacementData() { }
}

[System.Serializable]
public class GridDataSave
{
    public List<GridCropSave> crops = new();
    public List<GridFieldSave> fields = new(); // �� ������ �߰�
}

[System.Serializable]
public class GridFieldSave
{
    public Vector3 position;       // �� ��ġ
    public PlacementData placementData; // PlacementData ����
    public int id;
}

[System.Serializable]
public class GridCropSave
{
    public Vector3 position;       // �۹� ��ġ
    public PlacementData placementData; // PlacementData ����
    public int id;
    public int currentStage; // ���� �ܰ� �߰�
    public Crop.CropState cropState; // �۹� ���� �߰�
}