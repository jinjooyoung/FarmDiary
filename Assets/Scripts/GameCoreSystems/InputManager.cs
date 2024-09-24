using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;
    private Vector3 lastPosition = new Vector3(0, -3, 0);

    [SerializeField]
    private LayerMask placementLayermask;

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;                         // ���콺 ��ġ (��ũ�� ��ǥ)
        Vector3 worldPos = sceneCamera.ScreenToWorldPoint(mousePos);    // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100, placementLayermask);

        if (hit.collider != null)       // ����ĳ��Ʈ�� �浹ü�� ������
        {
            lastPosition = hit.point;   // �ش� �浹 ��ġ�� ����
        }

        return lastPosition;            // ���������� ����� ��ġ ����
    }
}
