using UnityEngine;

public class HomeScreenController : MonoBehaviour
{
    [Header("Footer")]
    [SerializeField] private FooterMenuView footerMenuView;
    [SerializeField] private FooterMenuConfig footerMenuConfig;
      
    
    private FooterMenuPresenter footerPresenter;

    void Start()
    {
        InitializeScreen();
    }

    private void InitializeScreen()
    {
        InitializeFooter();

    }

    private void InitializeFooter()
    {
        footerPresenter = new FooterMenuPresenter();
        footerPresenter.Initialize(footerMenuView, footerMenuConfig);
    }

  
}