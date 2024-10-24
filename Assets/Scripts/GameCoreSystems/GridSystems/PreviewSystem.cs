using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject cellIndicator;               // �׸��� Ŀ��
    private GameObject previewObject;               // �̸����� ������Ʈ

    //[SerializeField]
    //private SpriteRenderer previewPrefab;

    private SpriteRenderer cellIndicatorRenderer;
    //private SpriteRenderer previewInstance;

    private void Start()
    {
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<SpriteRenderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)        // ������ ����
    {
        SpriteRenderer renderer = prefab.GetComponentInChildren<SpriteRenderer>();      // ��������Ʈ �������� �޾ƿ�
        previewObject = Instantiate(prefab);        // ������� ����� ������Ʈ�� ����
        PrepareCursor(size);                        // Ŀ���� ����� ������ ������Ʈ ũ�⿡ �°� ����
        SetAlpha(renderer, 0.2f);                   // ������ ���¿��� �������ϰ� ���̵��� ���İ� ����
        cellIndicator.SetActive(true);              // ���ε������Ͱ� ���̵��� true

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

    private void PrepareCursor(Vector2Int size)     // Ŀ�� ũ�� ��ü�� ������Ʈ ũ�⿡ ���� Ŀ���µ� Ŀ���� ��ġ�� ��߳� �ذ��ʿ�
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, size.y, 1);
        }
    }

    public void SetAlpha(SpriteRenderer renderer, float alpha)      // ��������Ʈ(������Ʈ) ���� ����
    {
        Color color = renderer.color;
        color.a = alpha; // ���İ� ����
        renderer.color = color; // ����
    }

    public void StopShowingPreview()        // ������ ����
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
    }

    public void UpdatePreviewOBJPos(Vector3 position, bool validity)        // ������ ������Ʈ�� ��ġ�� ������Ʈ
    {
        if (previewObject != null)
        {
            MovePreview(position);      // ������ ������Ʈ �̵�
        }
        MoveCursor(position);       // ������ �׸��� Ŀ�� �̵�
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

    // ��ġ ���� ����(bool ��)�� ���� Ŀ�� ������ �����ϴ� �Լ�
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
