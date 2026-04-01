using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class VehicleRoofTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("VehicleTrigger")]
    [SerializeField] private GameObject _roof = null;
    [SerializeField] private List<GameObject> _playersInTrigger = new(); 
    
    private void OnTriggerEnter(Collider other)
    {
        //if (!other.CompareTag("Player")) return;


        PlayerController playerCtr= other.gameObject.GetComponent<PlayerController>();
        if (playerCtr != null && !_playersInTrigger.Contains(playerCtr.gameObject))
        {
            _playersInTrigger.Add(playerCtr.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {


        PlayerController playerCtr = other.gameObject.GetComponent<PlayerController>();
        if (playerCtr != null && _playersInTrigger.Contains(playerCtr.gameObject))
        {
            _playersInTrigger.Remove(playerCtr.gameObject);
        }


    }

    private void Update()
    {
        if (_playersInTrigger.Count > 0) 
        {
            _roof.SetActive(false);
        } else _roof.SetActive(true);
    }
}
