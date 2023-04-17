using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PagePanel : MonoBehaviour
{
    [SerializeField] private Text _titleHeader;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Page _homePage;

    private Page _currentPage;

    void Awake() {

        Open(_homePage);

        _exitButton.onClick.AddListener(() => {
            Open(_homePage);
        });
    }

    public void Open(Page page) {
        if (_currentPage) {
            _currentPage.gameObject.SetActive(false);
            _currentPage.ParentPanel = null;
        }

        page.ParentPanel = this;
        SetTitle(_titleHeader.text = page.Title);
        page.gameObject.SetActive(true);
        _currentPage = page;
    }

    public void Open(Page page, string title) {
        Open(page);
        SetTitle(title);
    }

    public void SetTitle(string title) {
        _titleHeader.text = title;
    }

}
