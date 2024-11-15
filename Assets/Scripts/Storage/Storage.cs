using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropStorage
{
    public int cropID; // 작물 ID
    public List<Crop> crops = new List<Crop>(); // 해당 ID의 작물 리스트
}

public class Storage : MonoBehaviour
{
    public List<CropStorage> storedCropsByID = new List<CropStorage>(); // 인스펙터에서 보이는 ID별 저장소

    public AIStateManager aiStateManager; // AIStateManager 참조
    public UIManager UIManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("들어옴");

            foreach (var harvestedCrop in aiStateManager.harvestedCrops)
            {
                AddCropToHouse(harvestedCrop); // 작물을 집에 추가
            }

            aiStateManager.harvestedCrops.Clear();
            Debug.Log("모든 수확한 작물이 집으로 이동되었습니다.");
        }
    }
    private void AddCropToHouse(Crop crop)
    {
        // 해당 ID의 CropStorage 찾기
        CropStorage storage = storedCropsByID.Find(s => s.cropID == crop.ID);

        // 없다면 새로 생성하여 추가
        if (storage == null)
        {
            storage = new CropStorage { cropID = crop.ID };
            storedCropsByID.Add(storage);
        }

        storage.crops.Add(crop); // 작물 추가
        Debug.Log($"ID {crop.ID}인 작물이 추가되었습니다.");

        UIManager.CheckAndUnlockCrops();
    }
}
