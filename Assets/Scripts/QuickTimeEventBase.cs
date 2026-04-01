using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class QuickTimeEventBase : MonoBehaviour
{
    public enum EventButtons
    {
        UpArrow,DownArrow, LeftArrow, RightArrow
    }


    [Header("Events")]
    [SerializeField] private List<EventButtons> _EventSequence = new List<EventButtons>();
    [SerializeField] private float _waitTime = 5f;
    [SerializeField] private int _EventCount = 0;
    [SerializeField] private float _countdown = 0f;
    [SerializeField] bool _countdownActive = false;


    public List<Action<InputAction.CallbackContext>> handlerList = new();

    private InputActionMap _playerActionMap;
    private GameObject _player;
    private BaseStand _stand;
    private PlayerUIManager _UIManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Update()
    {
        if (!_countdownActive) return;
        _countdown += Time.deltaTime;
        if (_countdown > _waitTime) 
        {
            FailEvent();
        }
    }

    public void StartEvent(GameObject player, BaseStand stand) 
    {
        if (_player != null) return;
        _EventCount = 0;
        _countdown = 0f;
        _player = player;
        _stand = stand;
        _UIManager = player.GetComponentInChildren<PlayerUIManager>();
        _playerActionMap = player.GetComponent<PlayerInput>().actions.FindActionMap("QTE");
        _countdownActive = true;



        Action<InputAction.CallbackContext> handler = context => PressEventButton(EventButtons.UpArrow);
        handlerList.Add(handler);
        _playerActionMap.FindAction("UpArrow").started += handler;
        handler = context => PressEventButton(EventButtons.DownArrow);
        handlerList.Add(handler);
        _playerActionMap.FindAction("DownArrow").started += handler;
        handler = context => PressEventButton(EventButtons.LeftArrow);
        handlerList.Add(handler);
        _playerActionMap.FindAction("LeftArrow").started += handler;
        handler = context => PressEventButton(EventButtons.RightArrow);
        handlerList.Add(handler);
        _playerActionMap.FindAction("RightArrow").started += handler;

        _UIManager?.EnableQTEUI(_EventSequence[0]);


    }

    public void QuitEvent()
    {
        foreach (var handler in handlerList) 
        {
            _playerActionMap.FindAction("UpArrow").started -= handler;
            _playerActionMap.FindAction("DownArrow").started -= handler;
            _playerActionMap.FindAction("LeftArrow").started -= handler;
            _playerActionMap.FindAction("RightArrow").started -= handler;
        }
        _player.GetComponent<PlayerController>().EnableMovement(true);
        _player = null;
        _stand = null;
        _UIManager?.DisableQTEUI();
        _UIManager = null;
    }

    public void FailEvent() 
    {
        _countdownActive = false;
        QuitEvent();
        Debug.Log("Failed QTE");
        
    }



    private void NextEvent() 
    {
        _EventCount++;
        _countdown = 0f;

        if (_EventCount > _EventSequence.Count - 1)
        {
            Debug.Log("Success QTE");
            _countdownActive = false;
            _stand.QTESuccess();
            QuitEvent();
        }
        else _UIManager?.EnableQTEUI(_EventSequence[_EventCount]);

    }




    private void PressEventButton(EventButtons button)
    {
        if (button == _EventSequence[_EventCount]) 
        {
            NextEvent();
        }
        else { FailEvent(); }
    }



}
