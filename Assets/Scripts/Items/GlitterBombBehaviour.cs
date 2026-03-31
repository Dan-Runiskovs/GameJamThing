using System.Collections.Generic;
using UnityEngine;

public class GlitterBombBehaviour : MonoBehaviour
{
    private float _internalTimer = 0;

    [SerializeField]
    private float _explodeTime = 3;

    private readonly List<ItemScript> _itemsInRange = new();        //All ItemScripts of all items in Pick Up range

    // Update is called once per frame
    void Update()
    {
        _internalTimer += Time.deltaTime;

        if (_internalTimer >= _explodeTime) Explode();
    }

    private void Explode()
    {
        Debug.Log("Boom");

        foreach(var item in _itemsInRange)
        {
            Debug.Log("There was: " + item.name);
            item.Invalidate();
        }

        Destroy(gameObject);
    }

    //Checks which items enter the player's Pick Up range
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemScript itemScript = other.GetComponent<ItemScript>();
            if (itemScript == null) return;

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

        if (!_itemsInRange.Contains(item))
            _itemsInRange.Add(item);
    }


    //Removes items from the pick up range list
    private void RemoveItemFromPickUpRange(ItemScript item)
    {
        if (item == null) return;

        _itemsInRange.Remove(item);
    }
}
