using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInfo : MonoBehaviour
{
    public List<Crop> storedCrops = new List<Crop>(); // 집안에 저장할 작물 리스트

    public AIStateManager aiStateManager; // AIStateManager 참조

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("들어옴");

            // 집에 들어가면 수확한 작물을 집으로 가져간다.
            foreach (var harvestedCrop in aiStateManager.harvestedCrops)
            {
                AddCropToHouse(harvestedCrop); // 작물을 집에 추가
            }

            // 수확한 작물 목록을 비운다.
            aiStateManager.harvestedCrops.Clear();
            Debug.Log("모든 수확한 작물이 집으로 이동되었습니다.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("나감");
    }

    private void AddCropToHouse(Crop crop)
    {
        storedCrops.Add(crop); // 작물을 집 리스트에 추가
        Debug.Log(crop.ID);
    }
}
