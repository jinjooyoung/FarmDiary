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

    public bool isMove = false;
    public bool isWatering = false;
    public bool isHarvesting = false;
    public bool isFinishHarvesting = false;

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
        _animator.SetFloat("Move", isMove ? 1 : 0); // Move 파라미터 업데이트
    }

    public bool MoveToPosition(Transform target)
    {
        Vector2 targetPosition = new Vector2(target.position.x + 0.6f, target.position.y + 0.3f);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        // 스프라이트 방향 조정
        _spriteRenderer.flipX = targetPosition.x > transform.position.x;

        // 이동 거리 계산
        float distance = Vector2.Distance(transform.position, targetPosition);

        // 이동 상태 업데이트
        if (distance < 0.2f)
        {
            isMove = false; // 이동 중지
            _animator.SetFloat("Move", 0.9f);
            return true;
        }
        else
        {
            isMove = true; // 이동 중
            _animator.SetFloat("Move", 1.1f);
            return false;
        }
    }

    public void CheckSeed()
    {
        foreach (Crop crops in crop)
        {
            if (crops.IsSeedPlanted() && !crops.isPreview)
            {
                currentCrop = crops;
                Debug.Log($"씨앗이 심어진 밭: {crops.name}");
                MoveToPosition(crops.transform);
                return;
            }
        }
        Debug.Log("심어진 씨앗이 없거나 모든 씨앗이 프리뷰 상태입니다.");
    }

    public void WaterCrop()
    {
        if (currentCrop != null && currentWaterAmount > 0 && !isWatering && currentCrop.NeedsWater())
        {
            StartCoroutine(WaterRoutine());
        }
        else if (currentWaterAmount <= 0)
        {
            Debug.Log("물이 부족합니다. 물을 채워야 합니다.");
            RefuelWater();
        }
        else if (isWatering)
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
        isWatering = true;
        _animator.SetBool("IsWatering", true);
        yield return new WaitForSeconds(1.5f);

        currentCrop.WaterCrop();
        currentWaterAmount--;
        Debug.Log($"물을 줬습니다. 남은 물: {currentWaterAmount}");

        _animator.SetBool("IsWatering", false);
        isWatering = false;

        // 다음 작물로 이동 준비
        currentCrop = GetNextCrop();
        if (currentCrop != null)
        {
            MoveToPosition(currentCrop.transform);
        }
        else
        {
            MoveToPosition(homePosition);
        }
    }

    public void HarvestCrop()
    {
        if (currentCrop != null && currentCrop.IsReadyToHarvest())
        {
            if (!isHarvesting)
            {
                Vector3Int pos = ConvertToVector3Int(currentCrop.seedPosition);
                if (this.data.placedCrops.ContainsKey(pos))
                {
                    this.data.placedCrops.Remove(pos);
                    Debug.Log("작물 정보 삭제가 성공하였습니다.");
                }
                else
                {
                    Debug.Log("작물 정보 삭제가 실패하였습니다.");
                }
                StartCoroutine(HarvestRoutine());
            }
        }
        else
        {
            Debug.Log("작물을 수확할 수 없습니다: 수확할 준비가 되어 있지 않습니다.");
        }
    }

    private Vector3Int ConvertToVector3Int(Vector2 position)
    {
        return new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);
    }

    public IEnumerator HarvestRoutine()
    {
        isHarvesting = true;
        _animator.SetBool("IsHarvesting", true); // 수확 애니메이션 시작
        yield return new WaitForSeconds(2f); // 애니메이션 대기 시간 설정

        if (currentCrop != null)
        {
            currentCrop.Harvest();
            crop.Remove(currentCrop);
        }

        RemoveMissingCrops();

        isFinishHarvesting = true; // 수확 완료 플래그 설정
        currentCrop = GetNextCrop();

        _animator.SetBool("IsHarvesting", false); // 수확 애니메이션 종료
        isHarvesting = false;

        if (currentCrop == null)
        {
            MoveToPosition(homePosition); // 집으로 이동
        }
        else
        {
            MoveToPosition(currentCrop.transform); // 다음 작물로 이동
            isFinishHarvesting = false; // 다음 작물로 이동 시 false로 설정
        }
    }

    public void RefuelWater()
    {
        currentWaterAmount = maxWaterAmount;
        Debug.Log("물을 다시 채웠습니다.");
    }

    public void AddSeed(Crop newCrop)
    {
        crop.Add(newCrop);
        Debug.Log($"새로운 씨앗이 추가되었습니다: {newCrop.name}");
    }

    public void RemoveMissingCrops()
    {
        crop.RemoveAll(c => c == null);
    }

    public Crop GetNextCrop()
    {
        return crop.Count > 0 ? crop[0] : null;
    }

    public void AddToInventory(Crop harvestedCrop)
    {
        harvestedCrops.Add(harvestedCrop);
        Debug.Log($"수확된 작물이 인벤토리에 추가되었습니다: {harvestedCrop.name}");
    }
}
