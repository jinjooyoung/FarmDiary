using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateManager : MonoBehaviour
{
    public AIStateMachine aiStateMachine; // AIStateMachine�� ���� ����

    public List<FarmField> farmFields = new List<FarmField>(); // ��� ���� ������ ����Ʈ
    public FarmField currentFarmField; // ���� �۾� ���� ��

    public List<Crop> crop = new List<Crop>();
    public Crop currentCrop;

    private int currentSeedIndex = 0; // ���� ���� �ְ� �ִ� ������ �ε���

    public Transform waterPosition;  // �� ������ ��ġ
    public Transform homePosition;   // ���� ��ġ �߰�

    private Animator _animator;
    public SpriteRenderer _spriteRenderer;

    public float movementSpeed = 2f;  // AI �̵� �ӵ�

    public int maxWaterAmount = 5;    // �� �ִ� ������ �߰�
    public int currentWaterAmount;    // ���� �� ������

    private bool isWatering = false;

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
        foreach (FarmField field in farmFields)
        {
            if (field.IsSeedPlanted()) // ������ �ɾ������� Ȯ��
            {
                currentFarmField = field;
                Debug.Log($"������ �ɾ��� ��: {field.name}");
                MoveToPosition(field.transform); // �ش� ������ �̵�
                return;
            }
        }
        Debug.Log("�ɾ��� ������ �����ϴ�.");
    }

    public void WaterCropsInOrder()
    {
        if (currentSeedIndex < crop.Count)
        {
            currentCrop = crop[currentSeedIndex]; // ���� �ε����� ���� ��������
            MoveToPosition(currentCrop.transform); // ���� ��ġ�� �̵�

            if (Vector2.Distance(transform.position, currentCrop.transform.position) < 0.1f)
            {
                WaterCrop(); // �� �ֱ�
                currentSeedIndex++; // ���� �������� �ε��� ����
            }
        }
        else
        {
            Debug.Log("��� ���ѿ� ���� �־����ϴ�.");
            currentSeedIndex = 0; // �����Ͽ� �ٽ� �� �ֱ� ����
        }
    }

    /*public void WaterCrop()
    {
        // AI�� ���� �翡 �����ߴ��� Ȯ���ϰ�, �������� �ʾҴٸ� ���� ���� ����
        if (currentCrop != null && currentWaterAmount > 0 && !isWatering)
        {
            // �翡 �����ߴ��� Ȯ��
            if (Vector2.Distance(transform.position, currentCrop.transform.position) < 0.1f)
            {
                StartCoroutine(WaterRoutine());
            }
            else
            {
                Debug.Log("���� �翡 �������� �ʾҽ��ϴ�. �̵� ���Դϴ�.");
            }
        }
        else if (currentWaterAmount <= 0)
        {
            Debug.Log("���� �����մϴ�. ���� ä���� �մϴ�.");
            RefuelWater();
        }
        else if (isWatering)
        {
            Debug.Log("�̹� ���� �ְ� �ֽ��ϴ�.");
        }
        else
        {
            Debug.Log("�۹��� �����ϴ�.");
        }
    }*/

    public void WaterCrop()
    {
        // AI�� ���� �翡 �����ߴ��� Ȯ���ϰ�, �������� �ʾҴٸ� ���� ���� ����
        if (currentCrop != null && currentWaterAmount > 0 && !isWatering)
        {
            StartCoroutine(WaterRoutine());
        }
        else if (currentWaterAmount <= 0)
        {
            Debug.Log("���� �����մϴ�. ���� ä���� �մϴ�.");
            RefuelWater();
        }
        else if (isWatering)
        {
            Debug.Log("�̹� ���� �ְ� �ֽ��ϴ�.");
        }
        else
        {
            Debug.Log("�۹��� �����ϴ�.");
        }
    }

    private IEnumerator WaterRoutine()
    {
        isWatering = true;
        _animator.SetBool("IsWatering", true);
        yield return new WaitForSeconds(1.5f);

        currentFarmField.WaterCrop();
        currentWaterAmount--;
        Debug.Log($"���� ����ϴ�. ���� ��: {currentWaterAmount}");

        _animator.SetBool("IsWatering", false);
        isWatering = false;
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

    public void AddSeed(Crop newCrop)
    {
        crop.Add(newCrop);
        Debug.Log($"���ο� ������ �߰��Ǿ����ϴ�: {newCrop.name}");
    }
}