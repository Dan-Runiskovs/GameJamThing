using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeField] private float _timeToPlay = 300.0f;

    [SerializeField] private KidHappinessMonitor _happinessMonitor;
    [SerializeField] private GameObject _sadTimerObject;
    private TextMeshProUGUI _sadTimerText;
    private float _sadTime = 0.0f;
    private float _sadTimeLimit = 10.0f;
    private bool _isSadTimeTicking = false;

    public enum GameStateEnum
    {
        Win,
        Loss,
        Ongoing
    }

    private GameStateEnum _gameState;
    public GameStateEnum gameState
    {
        get { return _gameState; }
    }

    private float _gameTimer = 0.0f;
    public float GameTimer
    {
        get { return _gameTimer; }
    }

    void Start()
    {
        _gameState = GameStateEnum.Ongoing;
        _sadTimerText = _sadTimerObject.GetComponent<TextMeshProUGUI>();
        if (_sadTimerText == null) Debug.Log("No text mesh found");
    }

    // Update is called once per frame
    void Update()
    {
        var dt = Time.deltaTime;

        _gameTimer += dt;

        if (_happinessMonitor.AvgHappiness <= 10.0f) _isSadTimeTicking = true;
        else
        {
            _isSadTimeTicking = false;
            _sadTime = 0.0f;
            _sadTimerText.SetText("");
        }

        if (_isSadTimeTicking)
        {
            _sadTime += dt;
            var remainingTime = _sadTimeLimit - _sadTime;
            var text = remainingTime.ToSafeString().Truncate(4, "");
            _sadTimerText.SetText(text);
            if(_sadTime >=  _sadTimeLimit)
            {
                _sadTimerText.SetText("0!");
                _gameState = GameStateEnum.Loss;
               // OptionsManager(
            }
        }

        if (_gameTimer >= _timeToPlay)
        {
            _gameState = GameStateEnum.Win;
        }
    }
}
