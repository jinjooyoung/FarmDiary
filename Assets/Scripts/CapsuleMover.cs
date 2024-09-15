using UnityEngine;

public class CapsuleMover : MonoBehaviour
{
    public float speed = 5f;  // 이동 속도
    public float leftLimit = -9f;  // 왼쪽 한계
    public float rightLimit = 9f;  // 오른쪽 한계

    private Vector3 targetPosition;

    void Start()
    {
        // 초기 목표 위치는 오른쪽 한계점으로 설정
        targetPosition = new Vector3(rightLimit, transform.position.y, transform.position.z);
    }

    void Update()
    {
        // 목표 위치로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 목표 위치에 도달했을 때 반대쪽으로 목표를 변경
        if (transform.position.x == rightLimit)
        {
            targetPosition = new Vector3(leftLimit, transform.position.y, transform.position.z);
        }
        else if (transform.position.x == leftLimit)
        {
            targetPosition = new Vector3(rightLimit, transform.position.y, transform.position.z);
        }
    }
}