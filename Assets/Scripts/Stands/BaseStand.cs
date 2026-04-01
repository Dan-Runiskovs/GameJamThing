using JetBrains.Annotations;
using UnityEngine;

public class BaseStand : MonoBehaviour
{
    [Header("Stand")]
    [SerializeField] ItemScript.ItemTypes _standItem;
    [SerializeField] QuickTimeEventBase _QTE;
    [SerializeField] private bool _isTrashCan;

    public bool IsTrashCan { get { return _isTrashCan; } }
    public ItemScript.ItemTypes StandItemType {  get { return _standItem; } }


    [SerializeField] bool _itemActive = false;
    [SerializeField] KidBehaviour _kidBehaviour;

    [Header("Particle Indicators")]
    [SerializeField] GameObject _IndicatorKid;

    

    public bool HasKid { get { return _kidBehaviour != null; } }


    public void ShowIndicator(bool showIndicator) 
    {
        _IndicatorKid?.SetActive(showIndicator);
    }
    public void PlaceItem() 
    {
        _itemActive = false;


    }

    public void StartQTE(GameObject playerObject) 
    {
        playerObject?.GetComponent<PlayerController>().EnableMovement(false);
        Debug.Log("QTE");
        _QTE?.StartEvent(playerObject, this);



    }

    public void QTESuccess() 
    {
        _kidBehaviour?.Satisfy();
        _kidBehaviour = null;
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


        PlayerPickupComponent pickupComp = other.gameObject.GetComponent<PlayerPickupComponent>();
        if (pickupComp != null)
        {
            pickupComp.RemoveClosestStand(this);
        }


    }

    public void OnChildTriggerEnter(Collider other) 
    {

        PlayerPickupComponent pickupComp = other.gameObject.GetComponent<PlayerPickupComponent>();
        if (pickupComp != null)
        {
            pickupComp.SetClosestKidStand(this);
        }


    }

    public void OnChildTriggerExit(Collider other)
    {
        PlayerPickupComponent pickupComp = other.gameObject.GetComponent<PlayerPickupComponent>();
        if (pickupComp != null)
        {
            pickupComp.RemoveClosestKidStand(this);
        }
    }

    public void PlaceChild(KidBehaviour playerObject) 
    {
        Debug.Log("Kid Added");
        _kidBehaviour = playerObject;
    }
}
