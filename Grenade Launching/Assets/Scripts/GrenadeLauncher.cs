using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : MonoBehaviour
{
    private ParabolaRootManager m_RootManager;

    private void Start()
    {
        m_RootManager = GetComponentInChildren<ParabolaRootManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            ShootLauncher();
    }

    public void ShootLauncher()
    {
        m_RootManager.Shoot();
    }
}
