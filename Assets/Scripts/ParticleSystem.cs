using UnityEngine;

public class ParticleSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject _particle;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayParticle()
    {
        Instantiate(_particle, transform.position, transform.rotation);
    }

}
