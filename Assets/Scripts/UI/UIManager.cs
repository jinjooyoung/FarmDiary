using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject StoragePanel;
    public GameObject seedPanel;
    public GameObject DecoPanel;
    public GameObject AchievementsPanel;
    public GameObject CollectionPanel;
    public GameObject settingPanel;

    public Button StorageButton;
    public Button seedButton;
    public Button DecoButton;
    public Button AchievementsButton;
    public Button CollectionButton;
    public Button settingButton;

    void Start()
    {
        // ��ư�� Ŭ�� �̺�Ʈ �߰�
        StorageButton.onClick.AddListener(() => TogglePanel(StoragePanel));
        seedButton.onClick.AddListener(() => TogglePanel(seedPanel));
        DecoButton.onClick.AddListener(() => TogglePanel(DecoPanel));
        AchievementsButton.onClick.AddListener(() => TogglePanel(AchievementsPanel));
        CollectionButton.onClick.AddListener(() => TogglePanel(CollectionPanel));
        settingButton.onClick.AddListener(() => TogglePanel(settingPanel));
    }

    // �г��� Ȱ��ȭ ���¸� ����ϴ� �޼���
    void TogglePanel(GameObject panel)
    {
        // ��� �г��� ���� �ݱ�
        CloseAllPanels();

        // Ŭ���� �гθ� ���� ������ �ݴ�� ����
        panel.SetActive(!panel.activeSelf);
    }

    // ��� �г��� �ݴ� �޼���
    public void CloseAllPanels()
    {
        StoragePanel.SetActive(false);
        seedPanel.SetActive(false);
        DecoPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        CollectionPanel.SetActive(false);
        settingPanel.SetActive(false);
    }
}
