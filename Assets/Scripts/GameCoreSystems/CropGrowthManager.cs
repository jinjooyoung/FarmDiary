using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CropGrowthManager : MonoBehaviour
{
    public static CropGrowthManager Instance;

    public List<Crop> crops = new List<Crop>();    // 심어진 작물들을 저장할 리스트
    //public List<Vector3Int> cropsPos = new List<Vector3Int>();
    public HashSet<Vector3Int> cropsPos = new HashSet<Vector3Int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(StartGrowthCheck());
    }

    // 리스트에 작물을 추가하는 메서드
    public void RegisterCrop(Crop crop, Vector3Int gridPos)
    {
        crops.Add(crop);
        cropsPos.Add(gridPos);
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
                if (crop == null)
                {
                    continue;
                }
                else
                {
                    crop.CheckGrowth(currentTime); // 성장 여부 체크
                }
            }

            // 아래는 삭제되지 않은 GridData의 placedCrops 정보를 다시 한 번 체크해서 삭제하는 로직
            // placedCrops의 키를 복사해서 List로 변환
            HashSet<Vector3Int> keysToCheck = new HashSet<Vector3Int>(PlacementSystem.Instance.placedOBJData.placedCrops.Keys);

            // HashSet으로 순회하며 삭제
            foreach (var key in keysToCheck)
            {
                if (!cropsPos.Contains(key))  // 빠른 존재 여부 확인
                {
                    PlacementSystem.Instance.placedOBJData.placedCrops.Remove(key);
                    Debug.LogError($"미처 삭제되지 않은 작물 정보 {key} 삭제");
                }
            }
        }
    }

    private IEnumerator TrackCrops()
    {
        while (true)
        {
            // 씬 내 모든 Crop 오브젝트를 찾기
            Crop[] allCrops = FindObjectsOfType<Crop>();

            foreach (var crop in allCrops)
            {
                // crops 리스트에 없으면 추가
                if (!crops.Contains(crop))
                {
                    crops.Add(crop);
                }
            }

            // 5초 대기
            yield return new WaitForSeconds(5f);
        }
    }
}
