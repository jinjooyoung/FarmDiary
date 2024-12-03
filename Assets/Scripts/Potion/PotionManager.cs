using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour
{
    public static PotionManager instance;

    [SerializeField]
    private List<GameObject> potList = new List<GameObject>(new GameObject[5]);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 솥을 추가하는 메서드
    public bool AddPot(GameObject pot)
    {
        // 비어있는 ID를 찾기
        int idToAssign = GetAvailableID();
        if (idToAssign == -1)
        {
            Debug.LogError("솥의 개수는 최대 5개까지입니다.");
            return false; // 솥을 추가할 수 없음
        }

        // 고유 ID 설정
        Pot potComponent = pot.GetComponent<Pot>();  // Pot 컴포넌트 가져오기
        if (potComponent != null)
        {
            potComponent.SetID(idToAssign);  // Pot 컴포넌트에 ID 설정
        }

        potList[idToAssign] = pot;  // 해당 ID 위치에 게임 오브젝트 할당
        return true; // 추가 성공
    }

    // 솥을 삭제하는 메서드
    public void RemovePot(GameObject pot)
    {
        int index = potList.IndexOf(pot);
        if (index != -1)
        {
            // 게임 오브젝트 제거
            potList[index] = null;  // 해당 ID 위치의 게임 오브젝트 제거
            Pot potComponent = pot.GetComponent<Pot>();
            if (potComponent != null)
            {
                // 삭제된 ID는 다시 사용 가능하도록 처리
                potComponent.ClearID();
            }
        }
    }

    // 비어있는 ID를 찾아 반환하는 메서드
    private int GetAvailableID()
    {
        // potList에서 null인 인덱스를 찾아 반환
        for (int i = 0; i < 5; i++)
        {
            if (potList[i] == null)
            {
                return i;  // 비어있는 ID 반환
            }
        }

        return -1;  // 사용 가능한 ID가 없으면 -1 반환
    }
}
