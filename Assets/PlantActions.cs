using UnityEngine;

public class PlantActions : MonoBehaviour
{
    [SerializeField]private GameObject particle;
    private int _particleLayer;
    private int _playerLayer;
    private int _enemyLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _particleLayer = LayerMask.GetMask("Particles");
        _playerLayer = LayerMask.GetMask("Player");
        _enemyLayer = LayerMask.GetMask("Enemies");
        Instantiate(particle, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
