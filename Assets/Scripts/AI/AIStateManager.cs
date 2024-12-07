using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateManager : MonoBehaviour
{
    public AIStateMachine aiStateMachine;

    public List<Crop> crop = new List<Crop>();
    public List<Crop> harvestedCrops = new List<Crop>();
    public Crop currentCrop;

    private int currentSeedIndex = 0;
    public Transform waterPosition;
    public Transform homePosition;
    public Animator _animator;
    public SpriteRenderer _spriteRenderer;

    public float movementSpeed = 2f;
    public int maxWaterAmount = 5;
    public int currentWaterAmount;

    public bool IsMove = false;
    public bool IsWatering = false;
    public bool IsHarvesting = false;
    public bool IsWaterChargeing = false;

    public int totalHarvestedCount = 0;

    [SerializeField]
    private GridData data;

    [SerializeField]
    private PlacementSystem placementSystem;

    private void Awake()
    {
        aiStateMachine = GetComponent<AIStateMachine>();
        if (aiStateMachine == null)
        {
            Debug.LogError("AIStateMachine 컴포넌트를 찾을 수 없습니다.");
        }

        currentWaterAmount = maxWaterAmount;

        Crop[] crops = FindObjectsOfType<Crop>();
        crop.AddRange(crops);
    }

    private void Start()
    {
        data = placementSystem.placedOBJData;
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        RemoveMissingCrops();

        _animator.SetFloat("Move", IsMove ? 1 : 0); // Move 파라미터 업데이트

        // currentCrop이 삭제되었는지 확인하여 다음 작물로 이동
        if (currentCrop == null || !crop.Contains(currentCrop))
        {
            // 현재 작물이 유효하지 않다면 다음 작물을 선택하거나 행동을 멈춤
            currentCrop = GetNextCrop();

            if (currentCrop != null)
            {
                MoveToPosition(currentCrop.transform);
            }
            else
            {
                IsMove = false;  // 더 이상 이동할 작물이 없음
                return;
            }
        }
    }

    public bool MoveToPosition(Transform target)
    {
        Vector2 targetPosition = new Vector2(target.position.x + 0.6f, target.position.y + 0.3f);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        // 스프라이트 방향 조정
        _spriteRenderer.flipX = targetPosition.x > transform.position.x;

        // 이동 거리 계산
        float distance = Vector2.Distance(transform.position, targetPosition);

        if (distance < 0.1f)
        {
            IsMove = false;
            _animator.SetFloat("Move", 0.9f);
            return true;
        }
        else
        {
            IsMove = true;
            _animator.SetFloat("Move", 1.1f);
            return false;
        }
    }

    public void CheckSeed()
    {
        // 작물 목록에서 다음 심어진 씨앗을 찾습니다.
        Crop nextCrop = crop.Find(crops => crops.IsSeedPlanted() && !crops.isPreview);

        if (nextCrop != null)
        {
            currentCrop = nextCrop;
            MoveToPosition(currentCrop.transform);
        }
        else
        {
            Debug.Log("심어진 씨앗이 없거나 모든 씨앗이 프리뷰 상태입니다.");
        }
    }

    public void WaterCrop()
    {
        if (currentCrop != null && currentWaterAmount > 0 && !IsWatering && currentCrop.NeedsWater())
        {
            StartCoroutine(WaterRoutine());
        }
        else if (currentWaterAmount <= 0)
        {
            Debug.Log("물이 부족합니다. 물을 채워야 합니다.");
            RefuelWater();
        }
        else if (IsWatering)
        {
            Debug.Log("이미 물을 주고 있습니다.");
        }
        else
        {
            Debug.Log("현재 작물에 물을 줄 수 없습니다.");
        }
    }

    private IEnumerator WaterRoutine()
    {
        if (currentCrop == null)
        {
            Debug.LogWarning("현재 작물이 null입니다. 물 주기 루틴을 종료합니다.");
            IsWatering = false; // 상태 초기화
            yield break; // 코루틴 종료
        }

        IsWatering = true;
        _animator.SetBool("IsWatering", true);
        yield return new WaitForSeconds(1.5f);

        if (currentCrop != null)
        {
            currentCrop.WaterCrop();
            currentWaterAmount--;
            Debug.Log($"물을 줬습니다. 남은 물: {currentWaterAmount}");
        }

        _animator.SetBool("IsWatering", false);
        IsWatering = false;

        // 다음 작물로 이동 준비
        currentCrop = GetNextCrop();
        if (currentCrop != null)
        {
            MoveToPosition(currentCrop.transform);
        }
    }

    public void HarvestCrop()
    {
        if (currentCrop != null && currentCrop.IsReadyToHarvest())
        {
            if (!IsHarvesting)
            {
                Debug.LogError($"수확위치 : {currentCrop.seedPosition}");

                if (this.data.placedCrops.ContainsKey(currentCrop.seedPosition))
                {
                    //this.data.placedCrops.Remove(currentCrop.seedPosition);
                    this.data.RemoveCropAt(currentCrop.seedPosition);
                    Debug.LogError("작물 정보 삭제가 성공하였습니다.");
                }
                else
                {
                    Debug.LogError("작물 정보 삭제가 실패하였습니다.");
                }
                totalHarvestedCount++;
                StartCoroutine(HarvestRoutine());
            }
        }
        else
        {
            Debug.Log("작물을 수확할 수 없습니다: 수확할 준비가 되어 있지 않습니다.");
        }
    }

    /*private Vector3Int ConvertToVector3Int(Vector2 position)
    {
        return new Vector3Int(position.x, position.y, 0);
    }*/

    public IEnumerator HarvestRoutine()
    {
        IsHarvesting = true;
        _animator.SetBool("IsHarvesting", true); // 수확 애니메이션 시작
        yield return new WaitForSeconds(2f); // 애니메이션 대기 시간 설정

        if (currentCrop != null)
        {
            // 작물을 수확한 후에만 리스트에서 제거
            currentCrop.Harvest();
            crop.Remove(currentCrop);
        }

        RemoveMissingCrops(); // 누락된 작물 제거

        // 수확 완료 후 다음 작물 확인 및 이동
        currentCrop = GetNextCrop();

        // 애니메이션 종료 및 상태 초기화
        _animator.SetBool("IsHarvesting", false);
        IsHarvesting = false;

        // 다음 작물로 이동
        if (currentCrop != null)
        {
            MoveToPosition(currentCrop.transform);
        }
        else
        {
            Debug.Log("모든 작물을 수확했습니다.");
            CheckSeed();
        }
    }

    public void RefuelWater()
    {
        if (!IsWaterChargeing)
        {
            StartCoroutine(RefuelWaterRoutine());
        }
    }

    private IEnumerator RefuelWaterRoutine()
    {
        if (currentWaterAmount == 0)
        {
            MoveToPosition(waterPosition);
            IsWaterChargeing = true;
            _animator.SetBool("IsWaterChargeing", true);
        }

        while (!MoveToPosition(waterPosition))
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        currentWaterAmount = maxWaterAmount;
        Debug.Log("물을 다시 채웠습니다.");

        _animator.SetBool("IsWaterChargeing", false);
        IsWaterChargeing = false;
    }

    public void AddSeed(Crop newCrop)
    {
        crop.Add(newCrop);
    }

    public void RemoveMissingCrops()
    {
        crop.RemoveAll(c => c == null);
    }

    public Crop GetNextCrop()
    {
        // 작물 목록을 순회하여 다음 작물 찾기
        for (int i = currentSeedIndex; i < crop.Count; i++)
        {
            if (crop[i] != null && crop[i].IsSeedPlanted())
            {
                currentSeedIndex = i + 1; // 다음 인덱스 설정
                return crop[i]; // 심어진 씨앗 반환
            }
        }

        // 모든 작물을 처리한 경우 인덱스 리셋
        currentSeedIndex = 0;
        return null;
    }

    public void AddToInventory(Crop harvestedCrop)
    {
        if (harvestedCrop.cropState == Crop.CropState.Harvested)
        {
            harvestedCrops.Add(harvestedCrop);
            Debug.Log($"수확된 작물이 인벤토리에 추가되었습니다: {harvestedCrop.name}");
        }
        else
        {
            Debug.Log("추가할 수 없습니다: 작물이 수확 상태가 아닙니다.");
        }
    }
}