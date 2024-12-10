using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Crop;
using static Pot;

public class NewSaveData : MonoBehaviour
{
    [Header("��ġ ������Ʈ �����͸� ���� ����")]
    public Grid grid;
    public PlacementSystem placementSystem;
    public OBJPlacer objPlacer;
    public GridData gridData;

    [Header("������ ���̽� SO�� ���� ����")]
    public ObjectsDatabaseSO objectsdatabaseSO;
    public AchievementsDatabaseSO achievementsDatabaseSO;

    [Header("AI ������ �� ���� �ð� üũ�� ���� ����")]
    public Transform playerPos;
    public AIStateManager aIStateManager;
    public CropGrowthManager cropGrowthManager;

    [Header("â�� �����͸� ���� ����")]
    public Storage storage;

    // ������ ������ ����Ʈ
    private List<OBJData> objDataList = new List<OBJData>();


    private void Awake()
    {
        // ���� GridData �ν��Ͻ��� ����
        gridData = placementSystem.placedOBJData;
    }

    //========================================================================

    // AI ������ ����
    [System.Serializable]
    public class MainData
    {
        //public int currentCropIndexByCropGrowthManagerList;
        public Vector3 playerPosition;
        public List<int> harvestedCropByID;
        public int waterAmount;
        public float savedTime;
        //public string stateName;
    }

    // ������ ��� ������Ʈ�� ID�� ��ġ(gridPos), ��ġ ���� (index)�� ����
    [System.Serializable]
    public class OBJData
    {
        public int ID;                  // ���� ID
        public Vector3Int objPosition;  // ��ġ �׸��� ��ġ
        public int Index;               // ��ġ ����
        public CropData cropData;
        public PotData potData;
    }

    // �۹� ������
    [System.Serializable]
    public class CropData
    {
        public CropState cropStateData;     // ���� �۹� ����
        public int currentStageData;        // �۹� ���� �ܰ�
        public float growthStartTime;       // �۹� ���� ���� �ð�
    }

    // �� ������
    [System.Serializable]
    public class PotData
    {
        public PotState potStateData;       // ���� ���� ����
        public int magicIDData;             // ������ ���� �۹�    ���� �� �ߴٸ� �⺻ �� -1
        public List<int> selectedCropData;  // ������ �Ϲ� �۹� ����Ʈ
        public float remainingTimeData;     // ���� ���� �ð�
    }

    // ���� SO ������
    [System.Serializable]
    public class AchievementsSOData
    {
        public int progressData;        // ���� ���൵
        public bool IsUnlockedData;     // ���� �ر� ����
        public bool IsClearData;        // ���� Ŭ���� ����
    }

    // ������Ʈ ���� ���� SO ������
    [System.Serializable]
    public class OBJSOData
    {
        public int BuyPriceData;        // ������Ʈ ���� ����
    }

    //========================================================================

    // OBJData ����Ʈ�� ���� ���� Ŭ����
    [System.Serializable]
    public class DataListWrapper<T>
    {
        public List<T> DataList;
    }

    // ID�� �޾Ƽ� ������ ������Ʈ�� ���Ϲ޴� �Լ�    �ε忡�� ���
    public GameObject GetPrefabFromResourcesByID(int id)
    {
        return Resources.Load<GameObject>($"Prefabs/{id}");
    }

    //===================================

    // ��ġ�� ��� ������Ʈ ID, �׸��� ��ġ, ��ġ ���� 3���� ������ �⺻, ���� Crop ��ũ��Ʈ�� �ִ� �۹� ������Ʈ���
    // �߰������� �۹��� ����, �ܰ�, �ð��� ������
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

                // �۹��̶��
                Crop crop = placedObj.GetComponent<Crop>();
                if (crop != null)
                {
                    CropData cropData = new CropData
                    {
                        cropStateData = crop.cropState,         // �۹� ���� Enum
                        currentStageData = crop.currentStage,   // �۹� ���� �ܰ�
                        growthStartTime = crop.growthStartTime  // �۹� ���� ���� �ð�
                    };

                    // CropData�� objData�� �߰�
                    objData.cropData = cropData;
                }

                // ���̶��
                Pot pot = placedObj.GetComponent<Pot>();
                if (pot != null)
                {
                    PotData potData = new PotData
                    {
                        potStateData = pot.currentState,
                        magicIDData = pot.magicID,
                        selectedCropData = new List<int>(pot.basicMaterial),
                        remainingTimeData = pot.remainingTime
                    };

                    // potData�� objData�� �߰�
                    objData.potData = potData;
                }

                // ����Ʈ�� �߰�
                objDataList.Add(objData);
            }
        }

        // OBJData ����Ʈ�� JSON���� ��ȯ�Ͽ� ����
        string json = JsonUtility.ToJson(new DataListWrapper<OBJData> { DataList = objDataList });

        // SaveSystem�� Save �޼��带 ����Ͽ� ���Ͽ� ����
        SaveSystem.Save(json, "PlacedObjects");
        Debug.Log("���� �Ϸ�: ������Ʈ");
    }

    // AI ������ ���� (AI ��ġ, ��Ȯ�ؼ� AI�� ���� �ִ� �۹� ����, AI�� �����ִ� �� ����)
    public void SaveAIData()
    {
        // AIData ��ü ���� �� ������ �Ҵ�
        MainData aiData = new MainData
        {
            // �÷��̾��� ���� ��ġ ����
            playerPosition = playerPos.position,

            // AIStateManager�� ��Ȯ�� �۹� ID ����Ʈ ����
            harvestedCropByID = new List<int>(aIStateManager.harvestedCrops),

            // AIStateManager�� ���� ���� �� ����
            waterAmount = aIStateManager.currentWaterAmount,

            // ���� �ð� ����
            savedTime = cropGrowthManager.currentTime
        };

        // JSON ����ȭ
        string json = JsonUtility.ToJson(aiData);

        // SaveSystem�� Save �޼���� ���� ����
        SaveSystem.Save(json, "AIData");
        Debug.Log("���� �Ϸ�: AI ������");
    }

    // â�� �����ϴ� �۹��� ���� ����
    public void SaveStorage()
    {
        if (storage == null || storage.storedCropsByID == null)
        {
            Debug.LogWarning("Storage �Ǵ� ������ �����Ͱ� �����ϴ�!");
            return;
        }

        // CropStorage �����͸� DataListWrapper�� ���α�
        DataListWrapper<CropStorage> cropWrapper = new DataListWrapper<CropStorage>
        {
            DataList = storage.storedCropsByID
        };

        // JSON ����ȭ
        string json = JsonUtility.ToJson(cropWrapper);

        // ���Ͽ� ����
        SaveSystem.Save(json, "StoredCrops");
        Debug.Log("���� �Ϸ�: StoredCrops");
    }

    // �������� ����Ǵ� ���� ���� ����
    public void SaveAchievements()
    {
        // ���� ������ ����Ʈ���� Progress, IsUnlocked, Clear ���� ����
        List<AchievementsSOData> achievementsToSave = new List<AchievementsSOData>();

        foreach (var achievement in achievementsDatabaseSO.achievementsData)
        {
            // �ʿ��� �����͸� ����
            AchievementsSOData saveData = new AchievementsSOData
            {
                progressData = achievement.Progress,
                IsUnlockedData = achievement.IsUnlocked,
                IsClearData = achievement.Clear
            };

            achievementsToSave.Add(saveData);
        }

        // AchievementData ����Ʈ�� DataListWrapper�� ���α�
        DataListWrapper<AchievementsSOData> achievementWrapper = new DataListWrapper<AchievementsSOData>
        {
            DataList = achievementsToSave
        };

        // JSON ����ȭ
        string json = JsonUtility.ToJson(achievementWrapper);

        // ���Ͽ� ����
        SaveSystem.Save(json, "Achievements");
        Debug.Log("���� �Ϸ�: Achievements");
    }

    // �����Ǵ� ���� ���� ���� ����
    public void SaveBuyPrice()
    {
        List<OBJSOData> buyPriceList = new List<OBJSOData>();

        // objectsData���� 0������ 7������ BuyPrice�� �����ͼ� OBJSOData ����Ʈ�� ����
        // �ٸ� ������Ʈ �����ʹ� ���� ���� ������ ����
        for (int i = 0; i < 8 && i < objectsdatabaseSO.objectsData.Count; i++)
        {
            OBJSOData data = new OBJSOData
            {
                BuyPriceData = objectsdatabaseSO.objectsData[i].BuyPrice
            };
            buyPriceList.Add(data);
        }

        // OBJSOData ����Ʈ�� JSON �������� ����ȭ
        string json = JsonUtility.ToJson(new DataListWrapper<OBJSOData> { DataList = buyPriceList });

        // SaveSystem�� ����� �����͸� ����
        SaveSystem.Save(json, "ObjectsBuyPrice");
        Debug.Log("���� �Ϸ�: ObjectsBuyPrice");
    }

    //-------------------------------------------------------------------

    // ������ ��� ������Ʈ �ٽ� ����, ������ ���� ���� �ε�
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
        DataListWrapper<OBJData> dataWrapper = JsonUtility.FromJson<DataListWrapper<OBJData>>(json);

        if (dataWrapper == null || dataWrapper.DataList == null || dataWrapper.DataList.Count == 0)
        {
            Debug.LogWarning("�ε��� ������Ʈ �����Ͱ� �����ϴ�!");
            return;
        }

        // ���� ���� �̹� �����ϴ� ������Ʈ�� �ִٸ� ����
        foreach (GameObject obj in objPlacer.placedGameObjects)
        {
            Destroy(obj); // ������ ��ġ�� ������Ʈ ����
        }
        objPlacer.placedGameObjects.Clear(); // ����Ʈ �ʱ�ȭ

        // OBJData ����Ʈ�� Index �� = ��ġ�� ���� �������� ����
        var sortedObjDataList = dataWrapper.DataList.OrderBy(objData => objData.Index).ToList();

        int currentForeachIndex = 0;

        // �ε��� OBJData ����Ʈ�� ��ġ�ߴ� ������� ��ȸ�ϸ� ������Ʈ�� ����
        foreach (OBJData objData in sortedObjDataList)
        {
            int selectedCropIndex = objectsdatabaseSO.objectsData.FindIndex(data => data.ID == objData.ID);

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
                objPlacer.potCount++;
            }

            objPlacer.placedGameObjects.Add(newObject);               // ��ġ�� ������Ʈ ����Ʈ�� ����

            // ��ġ ���� ���� 1.0���� ����
            SpriteRenderer newObjectRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
            Color color = newObjectRenderer.color;
            color.a = 1.0f;
            newObjectRenderer.color = color;

            // ������Ʈ�� �����ϴ� ��� �׸��� ������ ����Ʈ
            List<Vector3Int> positionToOccupy = CalculatePositions(objData.objPosition, objectsdatabaseSO.objectsData[selectedCropIndex].Size);
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
                // ���̶��
                Pot potScript = newObject.GetComponent<Pot>();
                if (potScript != null)
                {
                    potScript.currentState = objData.potData.potStateData;
                    potScript.magicID = objData.potData.magicIDData;
                    potScript.basicMaterial = objData.potData.selectedCropData;
                    potScript.remainingTime = objData.potData.remainingTimeData;

                    if (potScript.currentState == PotState.Crafting)
                    {
                        potScript.animator.SetBool("IsCrafting", true);
                    }
                }

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
                    // �۹��� �ʱ�ȭ �� ���� ���� �ݿ�
                    cropScript.Initialize(objectsdatabaseSO.objectsData[selectedCropIndex].GrowthTimes);

                    // �ʱ�ȭ ���� ������ �ɾ� ���¸� ����
                    cropScript.PlantSeed();

                    // CropGrowthManager�� ���
                    CropGrowthManager.Instance.RegisterCrop(cropScript, objData.objPosition);

                    // ����� cropState, currentStage, growthStartTime ����
                    cropScript.cropState = objData.cropData.cropStateData;
                    cropScript.currentStage = objData.cropData.currentStageData;
                    cropScript.growthStartTime = objData.cropData.growthStartTime;

                    cropScript.UpdateCropVisual();
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

        Debug.Log("�ε� �Ϸ�: ������Ʈ");
    }

    // AI ������ �ε��ؼ� �Ҵ�
    public void LoadAIData()
    {
        // SaveSystem�� Load �޼��带 ����Ͽ� JSON �����͸� �ε�
        string json = SaveSystem.Load("AIData");

        // ����� ������ ���� ��� ó��
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("����� AI �����Ͱ� �����ϴ�!");
            return;
        }

        // JSON �����͸� AIData ��ü�� ��ø��������
        MainData aiData = JsonUtility.FromJson<MainData>(json);

        if (aiData == null)
        {
            Debug.LogWarning("AI �����͸� �ε��ϴ� �� �����߽��ϴ�!");
            return;
        }

        // �ε�� �����͸� �� ������ �ٽ� �Ҵ�
        playerPos.position = aiData.playerPosition; // �÷��̾� ��ġ ����
        aIStateManager.harvestedCrops = new List<int>(aiData.harvestedCropByID); // ��Ȯ�� �۹� ID ����Ʈ ����
        aIStateManager.currentWaterAmount = aiData.waterAmount; // ���� �� ����
        cropGrowthManager.loadTime = aiData.savedTime;
        cropGrowthManager.currentTime = aiData.savedTime;

        Debug.Log("�ε� �Ϸ�: AI ������");
    }

    // â�� ������ �ε�
    public void LoadStorage()
    {
        // ����� JSON ������ �о����
        string json = SaveSystem.Load("StoredCrops");

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("����� ������ �����ϴ�!");
            return;
        }

        // JSON ������ȭ
        DataListWrapper<CropStorage> cropWrapper = JsonUtility.FromJson<DataListWrapper<CropStorage>>(json);

        if (cropWrapper == null || cropWrapper.DataList == null)
        {
            Debug.LogWarning("�ε��� �����Ͱ� �����ϴ�!");
            return;
        }

        // ����� �����ͷ� Storage�� storedCropsByID �����
        storage.storedCropsByID = new List<CropStorage>(cropWrapper.DataList);
        Debug.Log("�ε� �Ϸ�: StoredCrops");
    }

    // ���� ������ �ε�
    public void LoadAchievements()
    {
        // ����� JSON �����͸� �о��
        string json = SaveSystem.Load("Achievements");

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("����� Achievements �����Ͱ� �����ϴ�.");
            return;
        }

        // JSON �����͸� DataListWrapper�� ��ȯ
        DataListWrapper<AchievementsSOData> achievementWrapper = JsonUtility.FromJson<DataListWrapper<AchievementsSOData>>(json);

        // ���� ������ ����
        for (int i = 0; i < achievementWrapper.DataList.Count; i++)
        {
            // ����� ������ �ش� ������ ������Ʈ
            AchievementsSOData savedAchievementData = achievementWrapper.DataList[i];
            AchievementData achievement = achievementsDatabaseSO.achievementsData[i];

            achievement.SetProgress(savedAchievementData.progressData);

            if (savedAchievementData.IsUnlockedData)
            {
                achievement.Unlock();
            }
            
            if (savedAchievementData.IsClearData)
            {
                achievement.SetClear();
            }
        }

        Debug.Log("�ε� �Ϸ�: Achievements");
    }

    // ������Ʈ ���Ű��� �ε�
    public void LoadBuyPrice()
    {
        // ����� JSON ������ �ҷ�����
        string json = SaveSystem.Load("ObjectsBuyPrice");

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("����� ���� ���� �����Ͱ� �����ϴ�!");
            return;
        }

        // JSON�� OBJSOData ����Ʈ�� ��ø��������
        DataListWrapper<OBJSOData> buyPriceDataWrapper = JsonUtility.FromJson<DataListWrapper<OBJSOData>>(json);

        if (buyPriceDataWrapper == null || buyPriceDataWrapper.DataList == null || buyPriceDataWrapper.DataList.Count == 0)
        {
            Debug.LogWarning("�ҷ��� ���� ���� �����Ͱ� �����ϴ�!");
            return;
        }

        // objectsData�� 0������ 7�������� ID�� �´� BuyPrice�� ����
        for (int i = 0; i < buyPriceDataWrapper.DataList.Count && i < objectsdatabaseSO.objectsData.Count; i++)
        {
            objectsdatabaseSO.objectsData[i].SetBuyPrice(buyPriceDataWrapper.DataList[i].BuyPriceData);
        }

        Debug.Log("������Ʈ ���� ���� �ε� �Ϸ�!");
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