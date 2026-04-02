using System.Collections.Generic;
using UnityEngine;

public class GlitterBombBehaviour : MonoBehaviour
{
    private float _internalTimer = 0;

    [SerializeField]
    private float _explodeTime = 3;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _startTickInterval = 1f;
    [SerializeField] private float _minTickInterval = 0.1f; // fastest it can go

    private float _tickTimer = 0f;
    private float _tickInterval = 1f;
    private float _currentTickInterval;

    void Start()
    {
        _currentTickInterval = _startTickInterval;
    }

    void Update()
    {
        float delta = Time.deltaTime;

        _internalTimer += delta;
        _tickTimer += delta;

        // Play tick sound with increasing speed
        if (_tickTimer >= _currentTickInterval)
        {
            _audioSource.Play();
            _tickTimer = 0f;

            // Decrease interval each tick (faster beeping)
            _currentTickInterval *= 0.9f;

            // Clamp so it doesn't go insane
            if (_currentTickInterval < _minTickInterval)
                _currentTickInterval = _minTickInterval;
        }

        if (_internalTimer >= _explodeTime)
        {
            Explode();
        }
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
