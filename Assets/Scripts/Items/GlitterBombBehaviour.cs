using System.Collections.Generic;
using UnityEngine;

public class GlitterBombBehaviour : MonoBehaviour
{
    private float _internalTimer = 0;

    [SerializeField]
    private float _explodeTime = 3;

    // Update is called once per frame
    void Update()
    {
        _internalTimer += Time.deltaTime;

        if (_internalTimer >= _explodeTime) Explode();
    }

    private void Explode()
    {
        Debug.Log("Boom");

        Collider[] hits = Physics.OverlapSphere(transform.position, 3.0f);

        foreach (var hit in hits)
        {

            if (hit.gameObject == gameObject) continue;

            Vector3 velocity = hit.transform.position - transform.position;

            GetComponent<ParticleSystem>().PlayParticle();

            hit.attachedRigidbody?.AddForce(velocity*100f, ForceMode.Force);


            if (!hit.CompareTag("Item")) continue;

            ItemScript item = hit.GetComponent<ItemScript>();
            if (item == null) continue;

            if (item.ItemType == ItemScript.ItemTypes.Box ||
                item.ItemType == ItemScript.ItemTypes.GlitterBomb)
                continue;

            Debug.Log("There was: " + item.name);
            item.Invalidate();
        }

        Destroy(gameObject);
    }
}
