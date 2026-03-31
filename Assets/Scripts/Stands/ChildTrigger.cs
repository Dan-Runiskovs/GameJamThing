using UnityEngine;

public class ChildTrigger : MonoBehaviour
{
    [SerializeField] BaseStand _Stand;

    private void Start()
    {
        if (_Stand == null) _Stand = gameObject.transform.parent.GetComponent<BaseStand>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (!other.CompareTag("Player")) return;


        _Stand.OnChildTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _Stand.OnChildTriggerExit(other);
    }
}
