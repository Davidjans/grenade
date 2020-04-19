using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSimulationUpdate : MonoBehaviour
{
    public Transform Target;
    public float firingAngle = 45.0f;
    public float gravity = 9.8f;

    public Transform Projectile;
    private Transform myTransform;
    private Vector3 m_OldPosition;
    private float elapse_time;
    private bool traveling = true;
    private float target_Distance;
    private float projectile_Velocity;
    private float Vx;
    private float Vy;
    private float flightDuration;
    void Awake()
    {
        myTransform = transform;
    }

    void Start()
    {
        Projectile.position = myTransform.position + new Vector3(0, 0.0f, 0);
        Vector3 target = Target.position;
        //target.y -= 2f;
        target_Distance = Vector3.Distance(Projectile.position, target);
        projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);
        Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
        flightDuration = target_Distance / Vx;
        Projectile.rotation = Quaternion.LookRotation(Target.position - Projectile.position);
    }

    void Update()
    {
        SimulateProjectile();
    }

    void SimulateProjectile()
    {

        if (elapse_time < flightDuration && traveling == true)
        {
            m_OldPosition = Projectile.position;
            Projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;
        }

        //if (Vector3.Distance(transform.position, Target.position) < 1)
        //{
        //    //Projectile.LookAt(new Vector3(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime));
        //    Rigidbody rigidbody = GetComponent<Rigidbody>();
        //    rigidbody.isKinematic = false;

        //    rigidbody.AddForce((transform.position - m_OldPosition) * 100);
        //    traveling = false;
        //}
    }
}
