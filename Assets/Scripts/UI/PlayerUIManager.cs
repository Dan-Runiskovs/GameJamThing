using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{





    [Header("UIs")]
    [SerializeField] private GameObject _UpArrowUI;
    [SerializeField] private GameObject _DownArrowUI;
    [SerializeField] private GameObject _LeftArrowUI;
    [SerializeField] private GameObject _RightArrowUI;
    [Space]
    [SerializeField] private GameObject _XUI;



    public void EnableQTEUI(QuickTimeEventBase.EventButtons button) 
    {


        DisableQTEUI();


        switch (button) 
        {
            case QuickTimeEventBase.EventButtons.UpArrow:
                _UpArrowUI.SetActive(true);
                break;

            case QuickTimeEventBase.EventButtons.DownArrow:
                _DownArrowUI.SetActive(true);
                break;

            case QuickTimeEventBase.EventButtons.LeftArrow:
                _LeftArrowUI.SetActive(true);
                break;

            case QuickTimeEventBase.EventButtons.RightArrow:
                _RightArrowUI.SetActive(true);
                break;
        }
    }

    public void EnablePickupIndicator(bool enable) 
    {
        _XUI.SetActive(enable);
    }

    public void DisableQTEUI() 
    {
        _UpArrowUI.SetActive(false);
        _DownArrowUI.SetActive(false);
        _LeftArrowUI.SetActive(false);
        _RightArrowUI.SetActive(false);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
