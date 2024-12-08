using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CropGrowthManager : MonoBehaviour
{
    public static CropGrowthManager Instance;

    public List<Crop> crops = new List<Crop>();    // �ɾ��� �۹����� ������ ����Ʈ
    //public List<Vector3Int> cropsPos = new List<Vector3Int>();
    public HashSet<Vector3Int> cropsPos = new HashSet<Vector3Int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(StartGrowthCheck());
    }

    // ����Ʈ�� �۹��� �߰��ϴ� �޼���
    public void RegisterCrop(Crop crop, Vector3Int gridPos)
    {
        crops.Add(crop);
        cropsPos.Add(gridPos);
    }

    // 1�ʸ��� ��� �۹��� ���� ������ üũ�ϴ� ��ƾ
    IEnumerator StartGrowthCheck()
    {
        while (true)    // �׽� üũ�ؾ� �ϹǷ�
        {
            yield return new WaitForSeconds(1f); // 1�� �������� üũ

            float currentTime = Time.time;
            foreach (var crop in crops)
            {
                if (crop == null)
                {
                    continue;
                }
                else
                {
                    crop.CheckGrowth(currentTime); // ���� ���� üũ
                }
            }

            // �Ʒ��� �������� ���� GridData�� placedCrops ������ �ٽ� �� �� üũ�ؼ� �����ϴ� ����
            // placedCrops�� Ű�� �����ؼ� List�� ��ȯ
            HashSet<Vector3Int> keysToCheck = new HashSet<Vector3Int>(PlacementSystem.Instance.placedOBJData.placedCrops.Keys);

            // HashSet���� ��ȸ�ϸ� ����
            foreach (var key in keysToCheck)
            {
                if (!cropsPos.Contains(key))  // ���� ���� ���� Ȯ��
                {
                    PlacementSystem.Instance.placedOBJData.placedCrops.Remove(key);
                    Debug.LogError($"��ó �������� ���� �۹� ���� {key} ����");
                }
            }
        }
    }

    private IEnumerator TrackCrops()
    {
        while (true)
        {
            // �� �� ��� Crop ������Ʈ�� ã��
            Crop[] allCrops = FindObjectsOfType<Crop>();

            foreach (var crop in allCrops)
            {
                // crops ����Ʈ�� ������ �߰�
                if (!crops.Contains(crop))
                {
                    crops.Add(crop);
                }
            }

            // 5�� ���
            yield return new WaitForSeconds(5f);
        }
    }
}
