using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropStorage
{
    public int cropID; // �۹� ID
    public List<Crop> crops = new List<Crop>(); // �ش� ID�� �۹� ����Ʈ
}

public class Storage : MonoBehaviour
{
    public List<CropStorage> storedCropsByID = new List<CropStorage>(); // �ν����Ϳ��� ���̴� ID�� �����

    public AIStateManager aiStateManager; // AIStateManager ����
    public UIManager UIManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("����");

            foreach (var harvestedCrop in aiStateManager.harvestedCrops)
            {
                AddCropToHouse(harvestedCrop); // �۹��� ���� �߰�
            }

            aiStateManager.harvestedCrops.Clear();
            Debug.Log("��� ��Ȯ�� �۹��� ������ �̵��Ǿ����ϴ�.");
        }
    }
    private void AddCropToHouse(Crop crop)
    {
        // �ش� ID�� CropStorage ã��
        CropStorage storage = storedCropsByID.Find(s => s.cropID == crop.ID);

        // ���ٸ� ���� �����Ͽ� �߰�
        if (storage == null)
        {
            storage = new CropStorage { cropID = crop.ID };
            storedCropsByID.Add(storage);
        }

        storage.crops.Add(crop); // �۹� �߰�
        Debug.Log($"ID {crop.ID}�� �۹��� �߰��Ǿ����ϴ�.");

        UIManager.CheckAndUnlockCrops();
    }
}
