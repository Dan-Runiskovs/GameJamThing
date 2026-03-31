using JetBrains.Annotations;
using UnityEngine;

public class BaseStand : MonoBehaviour
{
    public enum StandType
    {
        Balloon,
        Cake,
        Pinata,
    }



    [Header("Stand")]
    [SerializeField] ItemScript.ItemTypes _standItem;
    [SerializeField] QuickTimeEventBase _QTE;
    public ItemScript.ItemTypes StandItemType {  get { return _standItem; } }

    [SerializeField] bool _itemActive;






    void Start()
    {
        
    }
    void Update()
    {
        
    }


    public void PlaceItem() 
    {
        _itemActive = false;


    }

    public void StartQTE(GameObject playerObject) 
    {
        playerObject?.GetComponent<PlayerController>().EnableMovement(false);
        Debug.Log("QTE");
        _QTE?.StartEvent(playerObject);



    }


    private void OnTriggerEnter(Collider other)
    {
        //if (!other.CompareTag("Player")) return;


        PlayerPickupComponent pickupComp = other.gameObject.GetComponent<PlayerPickupComponent>();
        if (pickupComp != null) 
        {
            pickupComp.SetClosestStand(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;


        PlayerPickupComponent pickupComp = other.gameObject.GetComponent<PlayerPickupComponent>();
        if (pickupComp != null)
        {
            pickupComp.RemoveClosestStand(this);
        }


    }

    public void OnChildTriggerEnter(Collider other) 
    {




    }

    public void OnChildTriggerExit(Collider other)
    {
    }
}
