using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropGrowthManager : MonoBehaviour
{
    private List<Crop> crops = new List<Crop>();    // 심어진 작물들을 저장할 리스트

    // 리스트에 작물을 추가하는 메서드
    public void RegisterCrop(Crop crop)
    {
        crops.Add(crop);
    }

    // 1초마다 모든 작물의 성장 정도를 체크하는 루틴
    IEnumerator StartGrowthCheck()
    {
        while (true)    // 항시 체크해야 하므로
        {
            yield return new WaitForSeconds(1f); // 1초 간격으로 체크

            float currentTime = Time.time;
            foreach (var crop in crops)
            {
                crop.CheckGrowth(currentTime); // 성장 여부 체크
            }
        }
    }
}

// 작물의 정보가 들어있는 클래스
public class Crop : MonoBehaviour
{
    private int growthStage = 0;
    private float[] growthTimes; // 각 단계마다 성장에 걸리는 시간 (초 단위)
    private float nextGrowthTime = 0f;

    public void Initialize(float[] growthDurations)
    {
        this.growthTimes = growthDurations;
        StartGrowing(); // 작물 심기 시작
    }

    // 작물이 심어진 후 첫 단계에 진입
    public void StartGrowing()
    {
        nextGrowthTime = Time.time + growthTimes[growthStage]; // 첫 번째 단계로 진입
    }

    // 성장 상태를 확인하고 단계 변경
    public void CheckGrowth(float currentTime)
    {
        if (currentTime >= nextGrowthTime && growthStage < growthTimes.Length - 1) // 아직 최대 단계에 도달하지 않은 경우
        {
            growthStage++;
            UpdateVisual(); // 리소스 업데이트 (SetActive 등)

            // 다음 성장 단계로의 시간 설정
            nextGrowthTime = currentTime + growthTimes[growthStage];
        }
    }

    // 각 성장 단계에 맞게 리소스 변화 처리
    private void UpdateVisual()
    {
        Debug.Log($"작물이 성장하여 현재 {growthStage} 단계입니다.");
        // SetActive(true/false) 코드 작성 해야함
    }
}
