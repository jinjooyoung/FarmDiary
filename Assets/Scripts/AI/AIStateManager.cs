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
            Debug.LogError("AIStateMachine ������Ʈ�� ã�� �� �����ϴ�.");
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
        _animator.SetFloat("Move", isMove ? 1 : 0); // Move �Ķ���� ������Ʈ
    }

    public bool MoveToPosition(Transform target)
    {
        Vector2 targetPosition = new Vector2(target.position.x + 0.6f, target.position.y + 0.3f);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        // ��������Ʈ ���� ����
        _spriteRenderer.flipX = targetPosition.x > transform.position.x;

        // �̵� �Ÿ� ���
        float distance = Vector2.Distance(transform.position, targetPosition);

        // �̵� ���� ������Ʈ
        if (distance < 0.2f)
        {
            isMove = false; // �̵� ����
            _animator.SetFloat("Move", 0.9f);
            return true;
        }
        else
        {
            isMove = true; // �̵� ��
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
                Debug.Log($"������ �ɾ��� ��: {crops.name}");
                MoveToPosition(crops.transform);
                return;
            }
        }
        Debug.Log("�ɾ��� ������ ���ų� ��� ������ ������ �����Դϴ�.");
    }

    public void WaterCrop()
    {
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

        // ���� �۹��� �̵� �غ�
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
                    Debug.Log("�۹� ���� ������ �����Ͽ����ϴ�.");
                }
                else
                {
                    Debug.Log("�۹� ���� ������ �����Ͽ����ϴ�.");
                }
                StartCoroutine(HarvestRoutine());
            }
        }
        else
        {
            Debug.Log("�۹��� ��Ȯ�� �� �����ϴ�: ��Ȯ�� �غ� �Ǿ� ���� �ʽ��ϴ�.");
        }
    }

    private Vector3Int ConvertToVector3Int(Vector2 position)
    {
        return new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);
    }

    public IEnumerator HarvestRoutine()
    {
        isHarvesting = true;
        _animator.SetBool("IsHarvesting", true); // ��Ȯ �ִϸ��̼� ����
        yield return new WaitForSeconds(2f); // �ִϸ��̼� ��� �ð� ����

        if (currentCrop != null)
        {
            currentCrop.Harvest();
            crop.Remove(currentCrop);
        }

        RemoveMissingCrops();

        isFinishHarvesting = true; // ��Ȯ �Ϸ� �÷��� ����
        currentCrop = GetNextCrop();

        _animator.SetBool("IsHarvesting", false); // ��Ȯ �ִϸ��̼� ����
        isHarvesting = false;

        if (currentCrop == null)
        {
            MoveToPosition(homePosition); // ������ �̵�
        }
        else
        {
            MoveToPosition(currentCrop.transform); // ���� �۹��� �̵�
            isFinishHarvesting = false; // ���� �۹��� �̵� �� false�� ����
        }
    }

    public void RefuelWater()
    {
        currentWaterAmount = maxWaterAmount;
        Debug.Log("���� �ٽ� ä�����ϴ�.");
    }

    public void AddSeed(Crop newCrop)
    {
        crop.Add(newCrop);
        Debug.Log($"���ο� ������ �߰��Ǿ����ϴ�: {newCrop.name}");
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
        Debug.Log($"��Ȯ�� �۹��� �κ��丮�� �߰��Ǿ����ϴ�: {harvestedCrop.name}");
    }
}
