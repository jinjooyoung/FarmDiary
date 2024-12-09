using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewSaveData : MonoBehaviour
{
    public Grid grid;
    public PlacementSystem placementSystem;
    public OBJPlacer objPlacer;
    public GridData gridData;
    public ObjectsDatabaseSO objectsdatabase;

    // ������ ������ ����Ʈ
    private List<OBJData> objDataList = new List<OBJData>();

    private void Awake()
    {
        // ���� GridData �ν��Ͻ��� ����
        gridData = placementSystem.placedOBJData;
    }

    // ������ ��� ������Ʈ�� ID�� ��ġ(gridPos), ��ġ ���� (index)�� ����
    [System.Serializable]
    public class OBJData
    {
        public int ID;                  // ���� ID
        public Vector3Int objPosition; // ��ġ �׸��� ��ġ
        public int Index;               // ��ġ ����
    }

    // OBJData ����Ʈ�� ���� ���� Ŭ����
    [System.Serializable]
    public class OBJDataListWrapper
    {
        public List<OBJData> objDataList;
    }

    // ID�� �޾Ƽ� ������ ������Ʈ�� ���Ϲ޴� �Լ�    �ε忡�� ���
    public GameObject GetPrefabFromResourcesByID(int id)
    {
        return Resources.Load<GameObject>($"Prefabs/{id}");
    }

    //===================================

    public void SaveOBJs()
    {
        objDataList.Clear();  // ����Ʈ �ʱ�ȭ

        // placedGameObjects�� �ִ� ��� ���� ������Ʈ�� ���� ó��
        for (int i = 0; i < objPlacer.placedGameObjects.Count; i++)
        {
            GameObject placedObj = objPlacer.placedGameObjects[i];

            if (placedObj != null)
            {
                // ���� ������Ʈ �̸����� ID�� ���� (��: "123(Clone)"���� 123�� ����)
                int objectID = int.Parse(placedObj.name.Split('(')[0]);

                // ���� ������Ʈ�� transform.position�� Grid�� Cell�� ��ȯ
                Vector3Int objPosition = grid.WorldToCell(placedObj.transform.position);

                // OBJData ����
                OBJData objData = new OBJData
                {
                    ID = objectID,
                    objPosition = objPosition,
                    Index = i
                };

                // ����Ʈ�� �߰�
                objDataList.Add(objData);
            }
        }

        // OBJData ����Ʈ�� JSON���� ��ȯ�Ͽ� ����
        string json = JsonUtility.ToJson(new OBJDataListWrapper { objDataList = objDataList });

        // SaveSystem�� Save �޼��带 ����Ͽ� ���Ͽ� ����
        SaveSystem.Save(json, "PlacedObjects");
        Debug.Log("������Ʈ ���� �Ϸ�");
    }

    //-------------------------------------------------------------------

    public void LoadOBJs()
    {
        // SaveSystem�� Load �޼��带 ����Ͽ� JSON �����͸� ���Ͽ��� �о����
        string json = SaveSystem.Load("PlacedObjects");

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("����� ������ �����ϴ�!");
            return;
        }

        // JSON �����͸� OBJData ����Ʈ�� ��ø��������
        OBJDataListWrapper dataWrapper = JsonUtility.FromJson<OBJDataListWrapper>(json);

        if (dataWrapper == null || dataWrapper.objDataList == null || dataWrapper.objDataList.Count == 0)
        {
            Debug.LogWarning("�ε��� �����Ͱ� �����ϴ�!");
            return;
        }

        // ���� ���� �̹� �����ϴ� ������Ʈ�� �ִٸ� ����
        foreach (GameObject obj in objPlacer.placedGameObjects)
        {
            Destroy(obj); // ������ ��ġ�� ������Ʈ ����
        }
        objPlacer.placedGameObjects.Clear(); // ����Ʈ �ʱ�ȭ

        // OBJData ����Ʈ�� Index �� = ��ġ�� ���� �������� ����
        var sortedObjDataList = dataWrapper.objDataList.OrderBy(objData => objData.Index).ToList();

        int currentForeachIndex = 0;

        // �ε��� OBJData ����Ʈ�� ��ġ�ߴ� ������� ��ȸ�ϸ� ������Ʈ�� ����
        foreach (OBJData objData in sortedObjDataList)
        {
            int selectedCropIndex = objectsdatabase.objectsData.FindIndex(data => data.ID == objData.ID);

            GameObject prefab = GetPrefabFromResourcesByID(objData.ID);     // ID�� ������ �޾ƿ�

            if (prefab == null)     // �������� �������� ������
            {
                Debug.LogWarning($"ID {objData.ID}�� �ش��ϴ� �������� Resources ������ �����ϴ�!");
                continue;
            }
            GameObject newObject = Instantiate(prefab);                             // ������Ʈ ����
            newObject.transform.position = grid.CellToWorld(objData.objPosition);   // ������ OBJ�� �׸��� �������� ���� ���������� �ٲ㼭 �̵�

            Pot potComponent = newObject.GetComponent<Pot>();

            if (potComponent != null)
            {
                PotionManager.instance.AddPot(newObject);
            }

            objPlacer.placedGameObjects.Add(newObject);               // ��ġ�� ������Ʈ ����Ʈ�� ����

            // ��ġ ���� ���� 1.0���� ����
            SpriteRenderer newObjectRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
            Color color = newObjectRenderer.color;
            color.a = 1.0f;
            newObjectRenderer.color = color;

            // ������Ʈ�� �����ϴ� ��� �׸��� ������ ����Ʈ
            List<Vector3Int> positionToOccupy = CalculatePositions(objData.objPosition, objectsdatabase.objectsData[selectedCropIndex].Size);
            PlacementData data = new PlacementData(positionToOccupy, objData.ID, currentForeachIndex);
            
            if (objData.ID < 4)     // �� ������Ʈ���
            {
                foreach (var pos in positionToOccupy)
                {
                    // ���ʿ� �����ϴ� ���� �����Ѱű� ������ �� ��ġ�� �ٸ� ������ ����Ǿ��ִ��� Ȯ�� ���ʿ�
                    gridData.placedFields[pos] = data;  // ��ųʸ��� ���� ����
                }
            }
            else if (objData.ID > 3 && objData.ID < 9)      // �ü� ������Ʈ���
            {
                foreach (var pos in positionToOccupy)
                {
                    gridData.placedFacilities[pos] = data;  // ��ųʸ��� ���� ����
                }
            }
            else if (objData.ID > 8 && objData.ID < 100)    // �۹� ������Ʈ���
            {
                // �� ��ġ�� �����ϴ� ���� �޾ƿ�. ���� �̹� �����ϰ� �� ��ġ�� ���� OBJ �ε����� ���� �۹� OBJ �ε������� �۾ƾ���
                // �ٵ� �׻� ���� ���� ��ġ�ǰ� �ű⿡ �۹��� ��ġ�ϴϱ� ���� �޾ƿ� �� ��������
                GameObject fieldObject = GetFieldObjectAt(objData.objPosition);
                newObject.transform.SetParent(fieldObject.transform);   // �۹� ������Ʈ�� �� ������Ʈ�� ������ ����
                Crop cropScript = newObject.GetComponent<Crop>();              // Crop ��ũ��Ʈ ��������

                if (cropScript != null)
                {
                    cropScript.Initialize(objectsdatabase.objectsData[selectedCropIndex].GrowthTimes);

                    // �ʱ�ȭ ���� ������ �ɾ� ���¸� ����
                    cropScript.PlantSeed();

                    // CropGrowthManager�� ���
                    CropGrowthManager.Instance.RegisterCrop(cropScript, objData.objPosition);
                }

                foreach (var pos in positionToOccupy)
                {
                    gridData.placedCrops[pos] = data;  // ��ųʸ��� ���� ����
                }
            }
            else     // ������ �ٹ� ������Ʈ���
            {
                foreach (var pos in positionToOccupy)
                {
                    gridData.placedDecos[pos] = data;  // ��ųʸ��� ���� ����
                }
            }

            currentForeachIndex++;
        }

        Debug.Log("������Ʈ �ε� �Ϸ�");
    }

    //========================================================

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

    private GameObject GetFieldObjectAt(Vector3Int gridPosition)
    {
        if (gridData.placedFields.TryGetValue(gridPosition, out PlacementData placement))
        {
            int objIndex = placement.PlacedObjectIndex;

            // �ε����� ��ȿ�� ��� OBJList���� ������Ʈ�� ��ȯ
            if (objIndex > -1 && objIndex < objPlacer.placedGameObjects.Count)
            {
                return objPlacer.placedGameObjects[objIndex];
            }
        }

        return null; // ��ġ�� �� ������Ʈ�� ���ų� ��ȿ���� ������ null ��ȯ
    }
}