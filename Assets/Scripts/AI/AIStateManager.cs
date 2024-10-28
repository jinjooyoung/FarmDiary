using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateManager : MonoBehaviour
{
    public AIStateMachine aiStateMachine; // AIStateMachine에 대한 참조

    public List<Crop> crop = new List<Crop>();
    public Crop currentCrop;

    private int currentSeedIndex = 0; // 현재 물을 주고 있는 씨앗의 인덱스

    public Transform waterPosition;  // 물 웅덩이 위치
    public Transform homePosition;   // 집의 위치 추가

    public Animator _animator;
    public SpriteRenderer _spriteRenderer;

    public float movementSpeed = 2f;  // AI 이동 속도

    public int maxWaterAmount = 5;    // 물 최대 보유량 추가
    public int currentWaterAmount;    // 현재 물 보유량

    private bool isWatering = false;
    private bool isMaking = false;

    private void Awake()
    {
        aiStateMachine = GetComponent<AIStateMachine>(); // AIStateMachine을 참조
        if (aiStateMachine == null)
        {
            Debug.LogError("AIStateMachine 컴포넌트를 찾을 수 없습니다.");
        }

        currentWaterAmount = maxWaterAmount;  // 시작 시 최대 보유량으로 초기화

        Crop[] crops = FindObjectsOfType<Crop>();
        crop.AddRange(crops);
    }

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();

        Transform playerChild = transform.Find("Character1");
        if (playerChild != null)
        {
            _spriteRenderer = playerChild.GetComponent<SpriteRenderer>();
        }
    }

    public bool MoveToPosition(Transform target)
    {
        // 목표 위치를 기존 위치에서 오른쪽으로 0.8만큼 이동
        Vector2 targetPosition = new Vector2(target.position.x + 0.6f, target.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        _spriteRenderer.flipX = targetPosition.x > transform.position.x;

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            _animator.SetBool("IsMove", false);
            return true;
        }
        else
        {
            _animator.SetBool("IsMove", true);
            return false;
        }
    }

    public void CheckSeed()
    {
        foreach (Crop crops in crop)
        {
            // 씨앗이 심어져 있고, 프리뷰 상태가 아닌 경우만 처리
            if (crops.IsSeedPlanted() && !crops.isPreview)
            {
                currentCrop = crops;  // 현재 씨앗으로 설정
                Debug.Log($"씨앗이 심어진 밭: {crops.name}");
                MoveToPosition(crops.transform); // 해당 밭으로 이동
                return;  // 첫 번째 유효한 씨앗만 처리하고 종료
            }
        }
        Debug.Log("심어진 씨앗이 없거나 모든 씨앗이 프리뷰 상태입니다.");
    }

    public void WaterCrop()
    {
        // 현재 작물이 성장 단계 0, 물 필요 상태이고 물이 충분할 때만 물을 주도록 수정
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
    }

    public void HarvestCrop()
    {
        if (currentCrop != null && currentCrop.IsReadyToHarvest())
        {
            if (!isMaking)
            {
                StartCoroutine(HarvestRoutine()); // 수확 루틴 시작
            }
        }
        else
        {
            Debug.Log("작물을 수확할 수 없습니다: 수확할 준비가 되어 있지 않습니다.");
        }
    }

    private IEnumerator HarvestRoutine()
    {
        isMaking = true;
        _animator.SetBool("IsMaking", true); // 수확 애니메이션 시작
        yield return new WaitForSeconds(5f); // 애니메이션 대기 시간 설정

        if (currentCrop != null)
        {
            currentCrop.Harvest();
            Debug.Log($"작물을 수확했습니다: {currentCrop.name}");

            crop.Remove(currentCrop);  // 수확 후 리스트에서 제거
        }

        RemoveMissingCrops(); // 리스트에서 Missing 항목 제거

        // 현재 작물 갱신: 남아있는 첫 번째 작물로 업데이트
        currentCrop = crop.Count > 0 ? crop[0] : null;

        _animator.SetBool("IsMaking", false); // 애니메이션 종료
        isMaking = false;

        MoveToPosition(homePosition); // 집으로 이동
    }

    public void RefuelWater()
    {
        currentWaterAmount = maxWaterAmount;  // 물 보충
        Debug.Log("물을 다시 채웠습니다.");
    }

    public void AddSeed(Crop newCrop)
    {
        crop.Add(newCrop);
        Debug.Log($"새로운 씨앗이 추가되었습니다: {newCrop.name}");
    }

    private void RemoveMissingCrops()
    {
        crop.RemoveAll(c => c == null); // Null 또는 Missing된 작물 제거
    }
}