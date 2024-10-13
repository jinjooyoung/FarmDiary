using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateManager : MonoBehaviour
{
    public AIStateMachine aiStateMachine; // AIStateMachine�� ���� ����

    public FarmField farmField;  // 2x2 �� �ʵ�
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
        farmField.CheckSeedPlanted();  // ������ Ȯ�� �ϴ� �޼���
        Debug.Log("���� ������ Ȯ�� �մϴ�");
    }

    public void WaterCrop()
    {
        farmField.WaterCrop();  // ���� Ư�� ��ġ�� ���� ��
        currentWaterAmount--;  // �� ���
        Debug.Log($"���� ����ϴ�. ���� ��: {currentWaterAmount}");
    }


    public void HarvestCrop()
    {
        // �۹��� ��Ȯ�ϴ� �ڵ�
        farmField.Harvest(); // ��Ȯ �޼��� ȣ��
        Debug.Log("�۹��� ��Ȯ�߽��ϴ�");
    }

    public void RefuelWater()
    {
        currentWaterAmount = maxWaterAmount;  // �� ����
        Debug.Log("���� �ٽ� ä�����ϴ�.");
    }

    public void AddField()
    {
        // ���� ���� �� �� ���� �� �� ���� ���縦 �ϴ� ����
    }
}
