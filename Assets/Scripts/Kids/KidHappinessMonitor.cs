using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class KidHappinessMonitor : MonoBehaviour
{
    private readonly List<KidBehaviour> _kidsAlive = new();

    private float _avgHappiness = 0.0f;
    public float AvgHappiness
    {
        get { return _avgHappiness; }
    }

    public void AddKid(KidBehaviour kid)
    {
        if (kid == null) return;
        if(!_kidsAlive.Contains(kid))
            _kidsAlive.Add(kid);
    }

    // Update is called once per frame
    void Update()
    {
        if(_kidsAlive.Count <= 0) return;

        _avgHappiness = 0.0f;
        foreach(var kid in _kidsAlive)
        {
            _avgHappiness += kid.Happiness;
        }

        _avgHappiness /= _kidsAlive.Count;
    }
}
