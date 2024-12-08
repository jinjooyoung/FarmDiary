using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropStorage
{
    public int cropID; // �۹� ID
    public int cropCount; // �۹� ����
}

public class Storage : MonoBehaviour
{
    public List<CropStorage> storedCropsByID = new List<CropStorage>(); // �ν����Ϳ��� ���̴� ID�� �����

    public AIStateManager aiStateManager; // AIStateManager ����
    public UIManager UIManager;

    private void Awake()
    {
        // ����� �����Ͱ� �ִٸ� �ʱ�ȭ (GameManager���� �ε�� �� �ݿ�)
        if (storedCropsByID == null)
        {
            storedCropsByID = new List<CropStorage>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (var harvestedCrop in aiStateManager.harvestedCrops)
            {
                AddCropToHouse(harvestedCrop); // �۹��� ���� �߰�
            }

            aiStateManager.harvestedCrops.Clear();
            Debug.Log("��� ��Ȯ�� �۹��� ������ �̵��Ǿ����ϴ�.");
        }
    }

    private void AddCropToHouse(int crop)
    {
        // �ش� ID�� CropStorage ã��
        CropStorage storage = storedCropsByID.Find(s => s.cropID == crop);

        // ���ٸ� ���� �����Ͽ� �߰�
        if (storage == null)
        {
            storage = new CropStorage { cropID = crop, cropCount = 0 };
            storedCropsByID.Add(storage);
        }

        // �۹� ���� �߰�
        storage.cropCount++;
        Debug.Log($"ID {crop}�� �۹��� �߰��Ǿ����ϴ�. ���� ����: {storage.cropCount}");

        // ���� �� UI ���� ȣ��
        AchievementsDatabase.AddProgressToAchievement(crop, 1);
        UIManager.CheckAndUnlockCrops();
    }
}
