using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class FieldManager : MonoBehaviour
{
    // 필드 ID와 해금 구역을 관리하는 Dictionary, 구역의 범위는 (시작 x, 끝 x) 형태로 저장
    private Dictionary<int, (Vector3Int, Vector3Int)> fieldAreas = new Dictionary<int, (Vector3Int, Vector3Int)>();
    // 필드 ID와 해금 비용을 관리하는 Dictionary
    private Dictionary<int, int> fieldUnlockCosts = new Dictionary<int, int>();
    // 필드 ID와 해금 상태를 관리하는 Dictionary
    private Dictionary<int, bool> unlockedFields = new Dictionary<int, bool>();

    private List<Vector3Int> unlockedAreas; // 해금된 지역 목록

    [SerializeField] private GameObject[] fieldObjects;     // 잠금된 필드를 보여주는 오브젝트
    [SerializeField] private GameObject[] buttons;

    private void Start()
    {
        // 모든 필드 비용 적용
        SetUnlockCosts();

        // 처음에 중앙 필드는 해금된 상태로 설정
        unlockedFields[0] = true;
        InitializeFieldAreas();         // 해금 범위 설정
        InitializeUnlockedFields();     // 해금 초기화 (테스트용 코드, 나중에 삭제해야함)
        LoadUnlockedFields();           // 해금 정보 불러오기
    }

    // 해금 범위를 설정하는 메서드
    public void InitializeFieldAreas()
    {
        // 중앙 구역 해금 (시작할 때 게임화면에 보이는 범위)
        int centralFieldID = 0;
        fieldAreas[centralFieldID] = (new Vector3Int(-18, -10, 0), new Vector3Int(17, -7, 0));

        // 잠겨 있는 땅들을 설정 (왼쪽)
        fieldAreas[-1] = (new Vector3Int(-27, -10, 0), new Vector3Int(-19, -7, 0)); // ID -1: -27부터 -19까지 (9칸)
        for (int i = 2; i <= 11; i++)
        {
            fieldAreas[-i] = (new Vector3Int(-18 - (i * 10), -10, 0), new Vector3Int(-9 - (i * 10), -7, 0));
        }

        // 잠겨 있는 땅들을 설정 (오른쪽)
        fieldAreas[1] = (new Vector3Int(18, -10, 0), new Vector3Int(26, -7, 0)); // ID +1: 18부터 26까지 (9칸)
        for (int i = 2; i <= 11; i++)
        {
            fieldAreas[i] = (new Vector3Int(7 + (i * 10), -10, 0), new Vector3Int(16 + (i * 10), -7, 0));
        }
    }

    /*// ID에 할당된 구역의 모든 그리드 셀 좌표를 담은 딕셔너리를 반환하는 메서드
    public Dictionary<int, List<Vector3Int>> AreasList(int ID)
    {
        // 반환할 딕셔너리
        Dictionary<int, List<Vector3Int>> result = new Dictionary<int, List<Vector3Int>>();

        // 키가 있는지 확인하고, 존재한다면 그 밸류값을 area에 저장
        if (fieldAreas.TryGetValue(ID, out (Vector3Int start, Vector3Int end) area))
        {
            // 반환할 딕셔너리의 밸류값으로 사용할 리스트 선언
            List<Vector3Int> areaCoordinates = new List<Vector3Int>();

            // 범위 내의 모든 좌표를 계산
            for (int x = area.start.x; x <= area.end.x; x++)
            {
                for (int y = area.start.y; y <= area.end.y; y++)
                {
                    areaCoordinates.Add(new Vector3Int(x, y, 0));
                }
            }

            // 결과 딕셔너리에 추가
            result.Add(ID, areaCoordinates);
        }
        else
        {
            Debug.Log($"ID : {ID} 가 존재하지 않습니다.");
        }

        return result;
    }*/

    private void DisableFieldObject(int fieldID)
    {
        // 필드 ID에 맞는 인덱스를 계산
        int index = fieldID < 0 ? fieldID + 11 : fieldID + 10; // 음수일 때는 11을 더하고, 양수일 때는 10을 더합니다.

        if (fieldObjects[index] != null && buttons[index] != null)
        {
            fieldObjects[index].SetActive(false); // 해당 오브젝트 비활성화
            buttons[index].SetActive(false);
            Debug.Log($"땅 {fieldID} 해금 완료. 오브젝트 비활성화됨.");
        }
    }

    // 필드를 해금하는 메서드
    public void UnlockField(int fieldID)
    {
        if (!fieldAreas.ContainsKey(fieldID))
        {
            Debug.LogWarning("필드 ID가 존재하지 않습니다.");
            return;
        }

        // 필드가 이미 해금되었는지 확인
        if (unlockedFields.ContainsKey(fieldID) && unlockedFields[fieldID])
        {
            Debug.Log("이미 해금된 땅입니다.");
            return;
        }

        // 해금 비용이 설정되어 있는지 확인
        if (!fieldUnlockCosts.ContainsKey(fieldID))
        {
            Debug.Log("해당 필드의 해금 비용이 설정되지 않았습니다.");
            return;
        }

        int unlockCost = fieldUnlockCosts[fieldID];  // 필드 ID에 해당하는 해금 비용 가져오기

        // 코인이 충분한지 확인
        if (GameManager.currentCoin >= unlockCost)
        {
            // 코인을 차감하고 필드를 해금
            GameManager.SubtractCoins(unlockCost);
            unlockedFields[fieldID] = true;

            // 해금 상태를 PlayerPrefs에 저장
            SaveUnlockedField(fieldID, true);

            DisableFieldObject(fieldID);

            Debug.Log($"땅 {fieldID} 해금 완료. 남은 코인: {GameManager.currentCoin}");
            // 해당 구역의 상호작용 활성화
        }
        else
        {
            Debug.Log("코인이 부족합니다.");
        }
    }

    // 필드가 해금되었는지 여부를 반환하는 메서드
    public bool IsFieldUnlocked(int fieldID)
    {
        return unlockedFields.ContainsKey(fieldID) && unlockedFields[fieldID];
    }

    // 필드 해금 비용을 설정하는 메서드
    public void SetUnlockCost(int fieldID, int cost)
    {
        fieldUnlockCosts[fieldID] = cost;
    }

    // 모든 필드에 비용을 적용시키는 메서드
    private void SetUnlockCosts()
    {
        int[] costs = new int[] { 200, 300, 400, 600, 800, 1100, 1400, 1800, 2300, 2800, 3500 }; // 비용 배열

        for (int i = 1; i <= 11; i++) // 1부터 3까지 반복
        {
            SetUnlockCost(i, costs[i - 1]);     // 양수 필드 ID에 대한 비용 설정
            SetUnlockCost(-i, costs[i - 1]);    // 음수 필드 ID에 대한 비용 설정
        }
    }

    // 해금정도를 불러와서 적용시키는 메서드
    private void LoadUnlockedFields()
    {
        for (int i = -11; i <= -1; i++)
        {
            // PlayerPrefs에서 해금 상태를 가져옴 (기본값: false)
            bool isUnlocked = PlayerPrefs.GetInt($"UnlockedFieldID_{i}", 0) == 1;
            unlockedFields[i] = isUnlocked;
        }

        for (int i = 1; i <= 11; i++)
        {
            // PlayerPrefs에서 해금 상태를 가져옴 (기본값: false)
            bool isUnlocked = PlayerPrefs.GetInt($"UnlockedFieldID_{i}", 0) == 1;
            unlockedFields[i] = isUnlocked;
        }
    }

    // 해금을 저장하는 메서드
    private void SaveUnlockedField(int fieldID, bool isUnlocked)
    {
        // Dictionary에 해금 상태 저장
        unlockedFields[fieldID] = isUnlocked;

        // PlayerPrefs에 해금 상태 저장 (1: 해금됨, 0: 해금되지 않음)
        PlayerPrefs.SetInt($"UnlockedFieldID_{fieldID}", isUnlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    // 해금을 초기화 시키는 메서드
    private void InitializeUnlockedFields()
    {
        for (int i = -11; i <= -1; i++)
        {
            PlayerPrefs.SetInt($"UnlockedFieldID_{i}", 0);
        }

        for (int i = 1; i <= 11; i++)
        {
            PlayerPrefs.SetInt($"UnlockedFieldID_{i}", 0);
        }
    }
}
