using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float m_ExplosionForce = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Exploooooosion();
        }
    }

    private void Exploooooosion()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 4);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null && rb.CompareTag("Dynamic"))
            {
                Ray ray;
                RaycastHit rayHit;
                ray = new Ray(transform.position, rb.position - transform.position);
                Debug.DrawRay(transform.position, rb.position - transform.position ,Color.red,999);
                if (Physics.Raycast(ray, out rayHit))
                {
                    rb.AddExplosionForce(m_ExplosionForce, transform.position, 4, 0);
                }
            }
        }
    }
}
