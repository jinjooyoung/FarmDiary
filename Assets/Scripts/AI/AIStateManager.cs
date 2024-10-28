using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateManager : MonoBehaviour
{
    public AIStateMachine aiStateMachine; // AIStateMachine�� ���� ����

    public List<Crop> crop = new List<Crop>();
    public Crop currentCrop;

    private int currentSeedIndex = 0; // ���� ���� �ְ� �ִ� ������ �ε���

    public Transform waterPosition;  // �� ������ ��ġ
    public Transform homePosition;   // ���� ��ġ �߰�

    public Animator _animator;
    public SpriteRenderer _spriteRenderer;

    public float movementSpeed = 2f;  // AI �̵� �ӵ�

    public int maxWaterAmount = 5;    // �� �ִ� ������ �߰�
    public int currentWaterAmount;    // ���� �� ������

    private bool isWatering = false;
    private bool isMaking = false;

    private void Awake()
    {
        aiStateMachine = GetComponent<AIStateMachine>(); // AIStateMachine�� ����
        if (aiStateMachine == null)
        {
            Debug.LogError("AIStateMachine ������Ʈ�� ã�� �� �����ϴ�.");
        }

        currentWaterAmount = maxWaterAmount;  // ���� �� �ִ� ���������� �ʱ�ȭ

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
        // ��ǥ ��ġ�� ���� ��ġ���� ���������� 0.8��ŭ �̵�
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
            // ������ �ɾ��� �ְ�, ������ ���°� �ƴ� ��츸 ó��
            if (crops.IsSeedPlanted() && !crops.isPreview)
            {
                currentCrop = crops;  // ���� �������� ����
                Debug.Log($"������ �ɾ��� ��: {crops.name}");
                MoveToPosition(crops.transform); // �ش� ������ �̵�
                return;  // ù ��° ��ȿ�� ���Ѹ� ó���ϰ� ����
            }
        }
        Debug.Log("�ɾ��� ������ ���ų� ��� ������ ������ �����Դϴ�.");
    }

    public void WaterCrop()
    {
        // ���� �۹��� ���� �ܰ� 0, �� �ʿ� �����̰� ���� ����� ���� ���� �ֵ��� ����
        if (currentCrop != null && currentWaterAmount > 0 && !isWatering && currentCrop.NeedsWater())
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
            Debug.Log("���� �۹��� ���� �� �� �����ϴ�.");
        }
    }

    private IEnumerator WaterRoutine()
    {
        isWatering = true;
        _animator.SetBool("IsWatering", true);
        yield return new WaitForSeconds(1.5f);

        currentCrop.WaterCrop();
        currentWaterAmount--;
        Debug.Log($"���� ����ϴ�. ���� ��: {currentWaterAmount}");

        _animator.SetBool("IsWatering", false);
        isWatering = false;
    }

    public void HarvestCrop()
    {
        if (currentCrop != null && currentCrop.IsReadyToHarvest())
        {
            if (!isMaking)
            {
                StartCoroutine(HarvestRoutine()); // ��Ȯ ��ƾ ����
            }
        }
        else
        {
            Debug.Log("�۹��� ��Ȯ�� �� �����ϴ�: ��Ȯ�� �غ� �Ǿ� ���� �ʽ��ϴ�.");
        }
    }

    private IEnumerator HarvestRoutine()
    {
        isMaking = true;
        _animator.SetBool("IsMaking", true); // ��Ȯ �ִϸ��̼� ����
        yield return new WaitForSeconds(5f); // �ִϸ��̼� ��� �ð� ����

        if (currentCrop != null)
        {
            currentCrop.Harvest();
            Debug.Log($"�۹��� ��Ȯ�߽��ϴ�: {currentCrop.name}");

            crop.Remove(currentCrop);  // ��Ȯ �� ����Ʈ���� ����
        }

        RemoveMissingCrops(); // ����Ʈ���� Missing �׸� ����

        // ���� �۹� ����: �����ִ� ù ��° �۹��� ������Ʈ
        currentCrop = crop.Count > 0 ? crop[0] : null;

        _animator.SetBool("IsMaking", false); // �ִϸ��̼� ����
        isMaking = false;

        MoveToPosition(homePosition); // ������ �̵�
    }

    public void RefuelWater()
    {
        currentWaterAmount = maxWaterAmount;  // �� ����
        Debug.Log("���� �ٽ� ä�����ϴ�.");
    }

    public void AddSeed(Crop newCrop)
    {
        crop.Add(newCrop);
        Debug.Log($"���ο� ������ �߰��Ǿ����ϴ�: {newCrop.name}");
    }

    private void RemoveMissingCrops()
    {
        crop.RemoveAll(c => c == null); // Null �Ǵ� Missing�� �۹� ����
    }
}