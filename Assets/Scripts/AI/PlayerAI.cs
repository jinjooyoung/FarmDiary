using System.Collections;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    public enum State
    {
        Idle,
        CheckSeed,
        Watering,
        Harvesting,
        GoToWater,
        GoingHome
    }
    
    public State currentState;

    public FarmField farmField;  // 2x2 �� �ʵ�

    [SerializeField] private Transform waterPosition;  // �� ������ ��ġ
    [SerializeField] private Transform homePosition;   // ���� ��ġ �߰�
    [SerializeField] private float movementSpeed = 2f;  // AI �̵� �ӵ�
    [SerializeField] private int maxWaterAmount = 5;    // �� �ִ� ������ �߰�
    [SerializeField] private int currentWaterAmount;    // ���� �� ������

    private Vector2 currentTarget;  // ���� �̵� ��ǥ ����

    private void Start()
    {
        currentWaterAmount = maxWaterAmount;  // ���� �� �ִ� ���������� �ʱ�ȭ
        currentState = State.Idle;  // �ʱ� ���¸� Idle�� ����
    }

    private void Update()
    {
        // ���¿� ���� �ൿ�� ó��
        HandleState();
    }

    private void HandleState()
    {
        // ���� ���� ��� �ڵ����� ���� ä�췯 ������ �߰�
        if (currentWaterAmount <= 0 && currentState != State.GoToWater)
        {
            currentState = State.GoToWater;
            currentTarget = waterPosition.position; // ���� ������ �̵�
        }

        switch (currentState)
        {
            case State.Idle:
                if (farmField.IsSeedPlanted(new Vector2(0, 0)))
                {
                    Debug.Log("�۹��� �ɾ������ϴ�.");
                    currentState = State.CheckSeed;
                }
                else
                {
                    Debug.Log("�۹��� �ɾ������� �ʽ��ϴ�.");
                }
                break;

            case State.CheckSeed:
                if (farmField.NeedsWater(new Vector2(0, 0)))
                {
                    if (currentWaterAmount > 0)
                    {
                        currentState = State.Watering;
                        currentTarget = farmField.fieldPosition; // ������ �̵�
                    }
                    else
                    {
                        currentState = State.GoToWater;
                        currentTarget = waterPosition.position; // ���� ������ �̵�
                    }
                }
                else if (farmField.IsReadyToHarvest(new Vector2(0, 0)))
                {
                    currentState = State.Harvesting;
                    currentTarget = farmField.fieldPosition; // ��Ȯ�� ������ �̵�
                }
                break;

            case State.Watering:
                // ���� �ְ� ������ ���ư�
                if (MoveToPosition(currentTarget))  // ���� Ÿ��(��)�� �����ߴ��� Ȯ��
                {
                    WaterCrop();  // ���� �ִ� ����
                    currentState = State.GoingHome;  // ������ ���� ���·� ����
                }
                break;

            case State.Harvesting:
                // ��Ȯ�� ������ �̵�
                if (MoveToPosition(currentTarget))
                {
                    HarvestCrop();
                    currentState = State.GoingHome; // ��Ȯ �� ������ ����
                }
                break;

            case State.GoToWater:
                // �� �����̷� �̵�
                if (MoveToPosition(waterPosition.position))
                {
                    // ���� ä��� ����
                    currentWaterAmount = maxWaterAmount; // �� �ִ�ġ�� ä��
                    Debug.Log("���� ä�����ϴ�.");

                    // �� ���� Ȯ��
                    if (farmField.IsSeedPlanted(new Vector2(0, 0)) && farmField.cropState != FarmField.CropState.Empty)
                    {
                        // ������ �ɾ��� �ְ� ���� �ʿ��ϸ� ������ �̵�
                        currentState = State.Watering;
                        currentTarget = farmField.fieldPosition;
                    }
                    else
                    {
                        // ������ ���ų� ���� ������� ��� ���·� ��ȯ
                        Debug.Log("�翡 �ɾ��� �ִ� ������ ���ų� ���� ������ϴ�.");
                        currentState = State.GoingHome; // ��� ���·� ��ȯ
                    }
                }
                break;

            case State.GoingHome:
                // ������ �̵�
                if (MoveToPosition(homePosition.position))
                {
                    Debug.Log("���� �����߽��ϴ�. ��� ��...");
                    currentState = State.Idle; // ��� ���·� ���ư���
                }
                break;
        }
    }

    public bool MoveToPosition(Vector2 target)
    {
        // ��ǥ ��ġ�� �̵�
        transform.position = Vector2.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
    
        // ��ǥ ��ġ�� ���� �����ߴ��� Ȯ��
        return Vector2.Distance(transform.position, target) < 0.1f;
    }

    public void WaterCrop()
    {
        // ������ �ɾ��� �ְ� ���� �ʿ��� ������ ���� ���� ��
        if (farmField.IsSeedPlanted(new Vector2(0, 0)) && farmField.NeedsWater(new Vector2(0, 0)))
        {
            if (currentWaterAmount > 0)
            {
                farmField.WaterCrop(new Vector2(0, 0));  // ���� Ư�� ��ġ�� ���� ��
                currentWaterAmount--;  // �� ���
                Debug.Log($"���� ����ϴ�. ���� ��: {currentWaterAmount}");
            }
            else if (farmField.IsSeedPlanted(new Vector2(0, 0)) && farmField.cropState == FarmField.CropState.Watered)
            {
                currentState = State.Idle;
            }
            else
            {
                Debug.Log("���� �ֱ⿣ ���� �����մϴ�.");
            }
        }
        else
        {
            Debug.Log("���� �� �� �����ϴ�: ������ �ɾ��� ���� �ʰų� ���� �ʿ����� �ʽ��ϴ�.");
        }
    }

    public void HarvestCrop()
    {
        // �۹��� ��Ȯ�ϴ� �ڵ�
        farmField.Harvest(new Vector2(0, 0)); // ��Ȯ �޼��� ȣ��
        Debug.Log("�۹��� ��Ȯ�߽��ϴ�");
    }

    public void RefuelWater()
    {
        currentWaterAmount = maxWaterAmount;  // �� ����
        Debug.Log("���� �ٽ� ä�����ϴ�.");
    }
}
