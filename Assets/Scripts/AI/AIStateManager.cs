using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateManager : MonoBehaviour
{
    public AIStateMachine aiStateMachine; // AIStateMachine에 대한 참조

    /*public List<FarmField> farmFields = new List<FarmField>(); // 모든 밭을 저장할 리스트
    public FarmField currentFarmField; // 현재 작업 중인 밭*/

    public List<Crop> crop = new List<Crop>();
    public Crop currentCrop;

    private int currentSeedIndex = 0; // 현재 물을 주고 있는 씨앗의 인덱스

    public Transform waterPosition;  // 물 웅덩이 위치
    public Transform homePosition;   // 집의 위치 추가

    private Animator _animator;
    public SpriteRenderer _spriteRenderer;

    public float movementSpeed = 2f;  // AI 이동 속도

    public int maxWaterAmount = 5;    // 물 최대 보유량 추가
    public int currentWaterAmount;    // 현재 물 보유량

    private bool isWatering = false;

    private void Awake()
    {
        aiStateMachine = GetComponent<AIStateMachine>(); // AIStateMachine을 참조
        if (aiStateMachine == null)
        {
            Debug.LogError("AIStateMachine 컴포넌트를 찾을 수 없습니다.");
        }

        currentWaterAmount = maxWaterAmount;  // 시작 시 최대 보유량으로 초기화

        // 모든 FarmField 오브젝트를 찾아 리스트에 추가
        /*FarmField[] fields = FindObjectsOfType<FarmField>();
        farmFields.AddRange(fields);*/

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
        Vector2 targetPosition = target.position;
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

    public void WaterCropsInOrder()
    {
        if (currentSeedIndex < crop.Count)
        {
            currentCrop = crop[currentSeedIndex]; // 현재 인덱스의 씨앗 가져오기
            bool hasReached = MoveToPosition(currentCrop.transform); // 씨앗 위치로 이동 및 도착 여부 확인

            if (hasReached && currentCrop.currentStage == 0) // 이동이 완료된 경우 및 성장 단계가 0일 때
            {
                WaterCrop(); // 물 주기
                currentSeedIndex++; // 다음 씨앗으로 인덱스 증가
            }
        }
        else
        {
            Debug.Log("모든 씨앗에 물을 주었습니다.");
            currentSeedIndex = 0; // 리셋하여 다시 물 주기 시작
        }
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
        if (currentCrop != null)
        {
            currentCrop.Harvest(); // 현재 밭에서 수확
            Debug.Log($"작물을 수확했습니다: {currentCrop.name}");
        }
        else
        {
            Debug.Log("작물이 없습니다.");
        }
    }

    public void RefuelWater()
    {
        currentWaterAmount = maxWaterAmount;  // 물 보충
        Debug.Log("물을 다시 채웠습니다.");
    }

    public void AddSeed(Crop newCrop)
    {
        if (newCrop.isPreview)
        {
            newCrop.isPreview = false;  // 프리뷰 태그 해제
            Debug.Log($"프리뷰 해제된 씨앗이 추가되었습니다: {newCrop.name}");
        }

        crop.Add(newCrop);  // 씨앗 리스트에 추가
        Debug.Log($"새로운 씨앗이 추가되었습니다: {newCrop.name}");
    }
}