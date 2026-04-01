using UnityEngine;

public class ParticleCleanup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float _time;
    void Start()
    {
        Invoke(nameof(Cleanup), _time);
    }

    // Update is called once per frame
    void Cleanup() 
    {
        Destroy(gameObject);
    }
        
    
}
