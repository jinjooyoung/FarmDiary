using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject cellIndicator;               // 그리드 커서
    private GameObject previewObject;               // 미리보기 오브젝트

    //[SerializeField]
    //private SpriteRenderer previewPrefab;

    private SpriteRenderer cellIndicatorRenderer;
    //private SpriteRenderer previewInstance;

    private void Start()
    {
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<SpriteRenderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)        // 프리뷰 시작
    {
        SpriteRenderer renderer = prefab.GetComponentInChildren<SpriteRenderer>();      // 스프라이트 렌더러를 받아옴
        previewObject = Instantiate(prefab);        // 프리뷰로 사용할 오브젝트를 생성
        PrepareCursor(size);                        // 커서의 사이즈를 선택한 오브젝트 크기에 맞게 조절
        SetAlpha(renderer, 0.2f);                   // 프리뷰 상태에서 반투명하게 보이도록 알파값 변경
        cellIndicator.SetActive(true);              // 셀인디케이터가 보이도록 true

       /* FarmFieldClickHandler farmFieldClickHandler = previewObject.GetComponentInChildren<FarmFieldClickHandler>();
        if (farmFieldClickHandler != null)
        {
            farmFieldClickHandler.enabled = false;
        }*/

        SpriteRenderer previewRenderer = previewObject.GetComponentInChildren<SpriteRenderer>();
        if (previewRenderer != null)
        {
            previewRenderer.sortingLayerName = "MiddleGround";
        }
    }

    private void PrepareCursor(Vector2Int size)     // 커서 크기 자체는 오브젝트 크기에 맞춰 커지는데 커서의 위치가 어긋남 해결필요
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, size.y, 1);
        }
    }

    public void SetAlpha(SpriteRenderer renderer, float alpha)      // 스프라이트(오브젝트) 투명도 조절
    {
        Color color = renderer.color;
        color.a = alpha; // 알파값 조정
        renderer.color = color; // 적용
    }

    public void StopShowingPreview()        // 프리뷰 종료
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
    }

    public void UpdatePreviewOBJPos(Vector3 position, bool validity)        // 프리뷰 오브젝트의 위치를 업데이트
    {
        if (previewObject != null)
        {
            MovePreview(position);      // 프리뷰 오브젝트 이동
        }
        MoveCursor(position);       // 프리뷰 그리드 커서 이동
        ApplyFeedbackToCursor(validity);
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y, 1);
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = new Vector3(position.x, position.y, 1);
    }

    // 설치 가능 유무(bool 값)에 따라 커서 색깔을 변경하는 함수
    public void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        cellIndicatorRenderer.color = c;
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }
}
