using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    // 필드 ID와 해금 구역을 관리하는 Dictionary, 구역의 범위는 (시작 x, 끝 x) 형태로 저장
    private Dictionary<int, (Vector3Int, Vector3Int)> fieldAreas = new Dictionary<int, (Vector3Int, Vector3Int)>();
    // 필드 ID와 해금 비용을 관리하는 Dictionary
    private Dictionary<int, int> fieldUnlockCosts = new Dictionary<int, int>();
    // 필드 ID와 해금 상태를 관리하는 Dictionary
    private Dictionary<int, bool> unlockedFields = new Dictionary<int, bool>();

    private void Start()
    {
        fieldUnlockCosts[1] = 200;
        fieldUnlockCosts[-1] = 200;
        fieldUnlockCosts[2] = 300;
        fieldUnlockCosts[-2] = 300;
        fieldUnlockCosts[3] = 500;
        fieldUnlockCosts[-3] = 500;

        // 처음에 중앙 필드는 해금된 상태로 설정
        unlockedFields[0] = true;
    }

    // 해금 범위를 설정하는 메서드
    public void InitializeFieldAreas()
    {
        // 중앙 구역 해금 (시작할 때 게임화면에 보이는 범위)
        int centralFieldID = 0;
        fieldAreas[centralFieldID] = (new Vector3Int(-18, -10, 0), new Vector3Int(17, -7, 0));

        // 잠겨 있는 땅들을 설정 (왼쪽)
        for (int i = 1; i <= 3; i++)
        {
            fieldAreas[-i] = (new Vector3Int(-18 - (i * 10), -10, 0), new Vector3Int(-9 - (i * 10), -7, 0));
        }

        // 잠겨 있는 땅들을 설정 (오른쪽)
        for (int i = 1; i <= 3; i++)
        {
            fieldAreas[i] = (new Vector3Int(9 + (i * 10), -10, 0), new Vector3Int(18 + (i * 10), -7, 0));
        }
    }

    // 특정 필드를 해금하는 메서드
    public void StartUnlockField(int fieldID)
    {
        if (fieldAreas.ContainsKey(fieldID))
        {
            UnlockField(fieldID);
        }
        else
        {
            Debug.LogWarning("필드 ID가 존재하지 않습니다.");
        }
    }

    // 필드를 해금하는 메서드
    public void UnlockField(int fieldID)
    {
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
        if (GameManager.coin >= (ulong)unlockCost)
        {
            // 코인을 차감하고 필드를 해금
            GameManager.SubtractCoins(unlockCost);
            unlockedFields[fieldID] = true;
            Debug.Log($"땅 {fieldID} 해금 완료. 남은 코인: {GameManager.coin}");
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

    // 필드 해금 비용을 설정하는 메서드 (필요시 외부에서 설정 가능)
    public void SetUnlockCost(int fieldID, int cost)
    {
        fieldUnlockCosts[fieldID] = cost;
    }
}
