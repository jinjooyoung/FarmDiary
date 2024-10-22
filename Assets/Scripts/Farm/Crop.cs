using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 작물의 정보가 들어있는 클래스
public class Crop : MonoBehaviour
{
    public GameObject[] growthStages; // 성장 단계별로 할당된 리소스 오브젝트들 (총 5개)
    private float[] growthTimes;  // 각 성장 단계에 필요한 시간
    private int currentStage = 0;  // 현재 작물의 성장 단계
    private float growthStartTime; // 성장이 시작된 시간

    // 성장 시간을 초기화하고, 첫 성장 단계를 설정
    public void Initialize(float[] cropGrowthTimes)
    {
        growthTimes = cropGrowthTimes;
        growthStartTime = Time.time;  // 성장이 시작된 시간을 저장
        currentStage = 0;             // 초기 성장 단계 설정

        UpdateCropVisual();            // 초기 상태 업데이트
        UpdateSortingLayer();          // 초기 소팅 레이어 업데이트
    }

    // 각 단계별로 성장을 체크하고 성장 상태를 업데이트
    // CropGrowthManager 스크립트에서 호출됨
    public void CheckGrowth(float currentTime)
    {
        if (currentStage < 4 && currentTime - growthStartTime >= growthTimes[currentStage])
        {
            currentStage++;
            UpdateCropVisual();
            UpdateSortingLayer();
        }
    }

    // 작물의 상태를 업데이트
    private void UpdateCropVisual()
    {
        // 모든 성장 단계를 일단 비활성화
        for (int i = 0; i < growthStages.Length; i++)
        {
            growthStages[i].SetActive(false);
        }

        // 현재 단계에 해당하는 오브젝트만 활성화
        if (currentStage >= 0 && currentStage < 5)
        {
            growthStages[currentStage].SetActive(true);
        }
    }

    // 소팅 레이어 업데이트
    private void UpdateSortingLayer()
    {
        foreach (var stage in growthStages)
        {
            if (stage.activeSelf) // 현재 활성화된 성장 단계만 소팅 레이어 변경
            {
                SpriteRenderer renderer = stage.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sortingLayerName = "MiddleGround"; // 원하는 소팅 레이어 이름으로 변경
                    renderer.sortingOrder = CalculateSortingOrder(); // 계산된 소팅 오더로 설정
                }
            }
        }
    }

    // 위치에 따라 소팅 오더를 계산하는 메서드 (예시)
    private int CalculateSortingOrder()
    {
        return (int)(transform.position.y * -10); // Y값을 음수로 변환하여 정렬
    }
}
