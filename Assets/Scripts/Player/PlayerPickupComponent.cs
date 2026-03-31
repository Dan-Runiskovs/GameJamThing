using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickupComponent : MonoBehaviour
{
    private InputActionMap _playerActionMap;

    [Header("PickUp")]
    //Sockets for where you hold items
    [SerializeField] private Transform _itemSocket;

    private readonly List<ItemScript> _itemsInRange = new();        //All ItemScripts of all items in Pick Up range
    //Current held Item
    private GameObject _heldItem;
    private ItemScript _heldItemScript;

    private BaseStand _ClosestStand;

    [Header("Throw")]
    [SerializeField] private float _throwStrength = 250f;

    private void Awake()
    {
        PlayerInput input = GetComponentInParent<PlayerInput>();
        _playerActionMap = input.actions.FindActionMap("Player");

        _playerActionMap.FindAction("Interact").started += context => TryInteract();
        _playerActionMap.FindAction("Throw").started += context => TryThrow();
        _playerActionMap.FindAction("Use").started += context => TryUse();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable() => _playerActionMap.Enable();
    private void OnDisable() => _playerActionMap.Disable();

    private void TryInteract()
    {
        ItemScript closest = GetClosestItem();

        if (closest == null && _heldItem != null)
        {
            Debug.Log("Dropping");
            DropHeldItem();
            return;
        }

        if (closest == null) return;

        if (closest != null && _heldItem == null)       //Picks up closest Item
        {
            Debug.Log("Trying to pick up");
            PickUpItem(closest);
        }
        else if (closest != null && _heldItem != null)  //Swaps out held item and closest item
        {
            Debug.Log("Swapping");
            DropHeldItem(closest.transform.position);
            PickUpItem(closest);
        }
    }

    private void TryThrow()
    {
        if (_heldItem == null) return;

        DropHeldItem(default, transform.forward * _throwStrength);

    }

    private void TryUse()
    {
        ItemScript closest = GetClosestItem();

        if (closest != null && closest.GetItemType() == ItemScript.ItemTypes.Box && _heldItem == null)
        {
            closest.HandleUnboxing();
            return;
        }

        TryPlace();

    }

    private void TryPlace() 
    {
        if (_ClosestStand == null || _heldItem == null || _heldItemScript == null) return;

        if ((_ClosestStand.transform.position - transform.position).sqrMagnitude > 100)
        {
            _ClosestStand = null;
            return;
        } //Distance check incase of failure to reset

        var itemType = _ClosestStand.StandItemType;

        if (_heldItemScript.ItemType == itemType) 
        {
            Debug.Log("Place Item");
            _ClosestStand.IncrementItem();
            _heldItemScript.UnCarry(Vector3.zero);
            _heldItem.transform.SetParent(null);
            Destroy(_heldItem);
            _heldItem = null;
            _heldItemScript = null;
        }





    }

    public void SetClosestStand(BaseStand stand) 
    {

        if (stand == null) Debug.Log("Setting closest stand to null can be dangerous! Use RemoveClosestStand");
        
        if (_ClosestStand != null)
        {

            if (Vector3.Distance(transform.position, stand.transform.position) >= 
                Vector3.Distance(_ClosestStand.transform.position, stand.transform.position)) return;

        }

        _ClosestStand = stand;
    }

    public void RemoveClosestStand(BaseStand stand) 
    {
        if (_ClosestStand == stand) _ClosestStand = null;
    }


    #region Items
    //Picks up the given item
    private void PickUpItem(ItemScript item)
    {
        if (item == null || item.IsCarried()) return;

        _heldItemScript = item;
        _heldItem = item.gameObject;

        _heldItemScript.Carry();

        Transform parentSocket = _itemSocket;
        
        _heldItem.transform.position = parentSocket.position;
        _heldItem.transform.rotation = parentSocket.rotation;
        _heldItem.transform.SetParent(parentSocket);

        Debug.Log("Picked up: " + _heldItemScript.name);

        RemoveItemFromPickUpRange(item);
    }

    private void DropHeldItem(Vector3 dropPosition = default, Vector3 throwStrength = default)
    {
        if (_heldItemScript == null || _heldItem == null) return;

        _heldItemScript.UnCarry(throwStrength);
        _heldItem.transform.SetParent(null);

        if (dropPosition != default)    //Checks if you need to swap out items
        {
            _heldItem.transform.position = dropPosition;
        }

        AddItemToPickUpRange(_heldItemScript);

        _heldItem = null;
        _heldItemScript = null;
    }

    private ItemScript GetClosestItem()
    {
        ItemScript closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        for (int i = 0; i < _itemsInRange.Count; i++)
        {
            ItemScript item = _itemsInRange[i];

            if (item == null)
            {
                _itemsInRange.RemoveAt(i);
                continue;
            }

            if (item.IsCarried())
            {
                _itemsInRange.RemoveAt(i);
                continue;
            }

            float distance = (item.transform.position - playerPosition).magnitude;

            if (distance < closestDistance)
            {
                closest = item;
                closestDistance = distance;
            }
        }

        return closest;
    }

    //Checks which items enter the player's Pick Up range
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemScript itemScript = other.GetComponent<ItemScript>();
            if (itemScript == null) return;

            if (itemScript.IsCarried()) return;

            AddItemToPickUpRange(itemScript);
        }
    }

    //Checks which items leave the player's Pick Up range
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemScript itemScript = other.GetComponent<ItemScript>();
            if (itemScript == null) return;


            RemoveItemFromPickUpRange(itemScript);
        }
    }
    //Adds items to the pick up range list
    private void AddItemToPickUpRange(ItemScript item)
    {
        if (item == null) return;

        Debug.Log("Added :" + item.name);

        if (!_itemsInRange.Contains(item))
            _itemsInRange.Add(item);
    }


    //Removes items from the pick up range list
    private void RemoveItemFromPickUpRange(ItemScript item)
    {
        if (item == null) return;

        _itemsInRange.Remove(item);
    }
    #endregion Items
}
