using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateManager : MonoBehaviour
{
    public AIStateMachine aiStateMachine; // AIStateMachine에 대한 참조

    public List<FarmField> farmFields = new List<FarmField>(); // 모든 밭을 저장할 리스트
    public FarmField currentFarmField; // 현재 작업 중인 밭

    public Transform waterPosition;  // 물 웅덩이 위치
    public Transform homePosition;   // 집의 위치 추가

    private Animator _animator;

    public float movementSpeed = 2f;  // AI 이동 속도

    public int maxWaterAmount = 5;    // 물 최대 보유량 추가
    public int currentWaterAmount;    // 현재 물 보유량

    private void Awake()
    {
        aiStateMachine = GetComponent<AIStateMachine>(); // AIStateMachine을 참조
        if (aiStateMachine == null)
        {
            Debug.LogError("AIStateMachine 컴포넌트를 찾을 수 없습니다.");
        }

        currentWaterAmount = maxWaterAmount;  // 시작 시 최대 보유량으로 초기화

        // 모든 FarmField 오브젝트를 찾아 리스트에 추가
        FarmField[] fields = FindObjectsOfType<FarmField>();
        farmFields.AddRange(fields);
    }

    private void Start()
    {
        // 자식 오브젝트에서 Animator 컴포넌트를 찾기
        _animator = GetComponentInChildren<Animator>();
    }

    public bool MoveToPosition(Transform target)
    {
        Vector2 targetPosition = target.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);


        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            _animator.SetBool("IsMove", false); // 도착했을 때 애니메이션 종료
            return true;
        }
        else
        {
            _animator.SetBool("IsMove", true); // 이동 중일 때 애니메이션 작동
            return false;
        }
    }


    public void CheckSeed()
    {
        foreach (FarmField field in farmFields)
        {
            if (field.IsSeedPlanted()) // 씨앗이 심어졌는지 확인
            {
                Debug.Log($"씨앗이 심어진 밭: {field.name}");
                MoveToPosition(field.transform); // 해당 밭으로 이동
                return;
            }
        }
        Debug.Log("심어진 씨앗이 없습니다.");
    }

    public void WaterCrop()
    {
        if (currentFarmField != null && currentWaterAmount > 0)
        {
            currentFarmField.WaterCrop();  // 현재 밭에 물 주기
            currentWaterAmount--;
            Debug.Log($"물을 줬습니다. 남은 물: {currentWaterAmount}");
        }
        else if (currentWaterAmount <= 0)
        {
            Debug.Log("물이 부족합니다. 물을 채워야 합니다.");
            RefuelWater();
        }
        else
        {
            Debug.Log("작물이 없습니다.");
        }
    }

    public void HarvestCrop()
    {
        if (currentFarmField != null)
        {
            currentFarmField.Harvest(); // 현재 밭에서 수확
            Debug.Log($"작물을 수확했습니다: {currentFarmField.name}");
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

    public void AddField(FarmField newField)
    {
        farmFields.Add(newField);  // 새 밭 추가
        Debug.Log($"새로운 밭이 추가되었습니다: {newField.name}");
    }
}