using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateManager : MonoBehaviour
{
    public AIStateMachine aiStateMachine; // AIStateMachine�� ���� ����

    public List<FarmField> farmFields = new List<FarmField>(); // ��� ���� ������ ����Ʈ
    public FarmField currentFarmField; // ���� �۾� ���� ��

    public Transform waterPosition;  // �� ������ ��ġ
    public Transform homePosition;   // ���� ��ġ �߰�

    public float movementSpeed = 2f;  // AI �̵� �ӵ�

    public int maxWaterAmount = 5;    // �� �ִ� ������ �߰�
    public int currentWaterAmount;    // ���� �� ������

    private void Awake()
    {
        aiStateMachine = GetComponent<AIStateMachine>(); // AIStateMachine�� ����
        if (aiStateMachine == null)
        {
            Debug.LogError("AIStateMachine ������Ʈ�� ã�� �� �����ϴ�.");
        }

        currentWaterAmount = maxWaterAmount;  // ���� �� �ִ� ���������� �ʱ�ȭ

        // ��� FarmField ������Ʈ�� ã�� ����Ʈ�� �߰�
        FarmField[] fields = FindObjectsOfType<FarmField>();
        farmFields.AddRange(fields);
    }

    public bool MoveToPosition(Transform target)
    {
        // ��ǥ ��ġ�� �̵� (�ʵ��� ��Ȯ�� Transform ��ġ ���)
        Vector2 targetPosition = target.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        // ��ǥ ��ġ�� ���� �����ߴ��� Ȯ��
        return Vector2.Distance(transform.position, targetPosition) < 0.1f;
    }

    public void CheckSeed()
    {
        foreach (FarmField field in farmFields)
        {
            if (field.IsSeedPlanted()) // ������ �ɾ������� Ȯ��
            {
                Debug.Log($"������ �ɾ��� ��: {field.name}");
                MoveToPosition(field.transform); // �ش� ������ �̵�
                return;
            }
        }
        Debug.Log("�ɾ��� ������ �����ϴ�.");
    }

    public void WaterCrop()
    {
        if (currentFarmField != null && currentWaterAmount > 0)
        {
            currentFarmField.WaterCrop();  // ���� �翡 �� �ֱ�
            currentWaterAmount--;
            Debug.Log($"���� ����ϴ�. ���� ��: {currentWaterAmount}");
        }
        else if (currentWaterAmount <= 0)
        {
            Debug.Log("���� �����մϴ�. ���� ä���� �մϴ�.");
            RefuelWater();
        }
        else
        {
            Debug.Log("�۹��� �����ϴ�.");
        }
    }

    public void HarvestCrop()
    {
        if (currentFarmField != null)
        {
            currentFarmField.Harvest(); // ���� �翡�� ��Ȯ
            Debug.Log($"�۹��� ��Ȯ�߽��ϴ�: {currentFarmField.name}");
        }
        else
        {
            Debug.Log("�۹��� �����ϴ�.");
        }
    }

    public void RefuelWater()
    {
        currentWaterAmount = maxWaterAmount;  // �� ����
        Debug.Log("���� �ٽ� ä�����ϴ�.");
    }

    public void AddField(FarmField newField)
    {
        farmFields.Add(newField);  // �� �� �߰�
        Debug.Log($"���ο� ���� �߰��Ǿ����ϴ�: {newField.name}");
    }
}