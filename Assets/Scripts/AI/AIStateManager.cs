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

    public bool IsMove = false;
    public bool IsWatering = false;
    public bool IsHarvesting = false;
    public bool IsFinishHarvesting = false;
    public bool IsWaterChargeing = false;

    public int totalHarvestedCount = 0;

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
        _animator.SetFloat("Move", IsMove ? 1 : 0); // Move �Ķ���� ������Ʈ
    }

    public bool MoveToPosition(Transform target)
    {
        Vector2 targetPosition = new Vector2(target.position.x + 0.6f, target.position.y + 0.3f);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        // ��������Ʈ ���� ����
        _spriteRenderer.flipX = targetPosition.x > transform.position.x;

        // �̵� �Ÿ� ���
        float distance = Vector2.Distance(transform.position, targetPosition);

        if (distance < 0.1f)
        {
            IsMove = false;
            _animator.SetFloat("Move", 0.9f);
            return true;
        }
        else
        {
            IsMove = true;
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
        if (currentCrop != null && currentWaterAmount > 0 && !IsWatering && currentCrop.NeedsWater())
        {
            StartCoroutine(WaterRoutine());
        }
        else if (currentWaterAmount <= 0)
        {
            Debug.Log("���� �����մϴ�. ���� ä���� �մϴ�.");
            RefuelWater();
        }
        else if (IsWatering)
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
        IsWatering = true;
        _animator.SetBool("IsWatering", true);
        yield return new WaitForSeconds(1.5f);

        currentCrop.WaterCrop();
        currentWaterAmount--;
        Debug.Log($"���� ����ϴ�. ���� ��: {currentWaterAmount}");

        _animator.SetBool("IsWatering", false);
        IsWatering = false;

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
            if (!IsHarvesting)
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
                totalHarvestedCount++;
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
        IsHarvesting = true;
        _animator.SetBool("IsHarvesting", true); // ��Ȯ �ִϸ��̼� ����
        yield return new WaitForSeconds(2f); // �ִϸ��̼� ��� �ð� ����

        if (currentCrop != null)
        {
            // �۹��� ��Ȯ�� �Ŀ��� ����Ʈ���� ����
            currentCrop.Harvest();
            crop.Remove(currentCrop);
        }

        RemoveMissingCrops(); // ������ �۹� ����

        // ��Ȯ �Ϸ� �� ���� �۹� Ȯ�� �� �̵�
        currentCrop = GetNextCrop();

        IsFinishHarvesting = currentCrop == null; // ��� �۹� ��Ȯ �Ϸ� ����

        // �ִϸ��̼� ���� �� ���� �ʱ�ȭ
        _animator.SetBool("IsHarvesting", false);
        IsHarvesting = false;

        // ���� �۹��� �̵�
        if (currentCrop != null)
        {
            MoveToPosition(currentCrop.transform);
        }
        else
        {
            Debug.Log("��� �۹��� ��Ȯ�߽��ϴ�.");
            MoveToPosition(homePosition);
        }
    }

    public void RefuelWater()
    {
        if (!IsWaterChargeing)
        {
            StartCoroutine(RefuelWaterRoutine());
        }
    }

    private IEnumerator RefuelWaterRoutine()
    {
        if (currentWaterAmount == 0)
        {
            MoveToPosition(waterPosition);
            IsWaterChargeing = true;
            _animator.SetBool("IsWaterChargeing", true);
        }

        while (!MoveToPosition(waterPosition))
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        currentWaterAmount = maxWaterAmount;
        Debug.Log("���� �ٽ� ä�����ϴ�.");

        _animator.SetBool("IsWaterChargeing", false);
        IsWaterChargeing = false;

        MoveToPosition(homePosition);
    }

    public void AddSeed(Crop newCrop)
    {
        crop.Add(newCrop);
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
        if (harvestedCrop.cropState == Crop.CropState.Harvested)
        {
            harvestedCrops.Add(harvestedCrop);
            Debug.Log($"��Ȯ�� �۹��� �κ��丮�� �߰��Ǿ����ϴ�: {harvestedCrop.name}");
        }
        else
        {
            Debug.Log("�߰��� �� �����ϴ�: �۹��� ��Ȯ ���°� �ƴմϴ�.");
        }
    }
}
