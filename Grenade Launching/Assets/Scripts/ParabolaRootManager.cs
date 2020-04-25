using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [ExecuteInEditMode]
public class ParabolaRootManager : MonoBehaviour
{
    public Transform m_Target;
    public Transform m_Start;
    public List<Transform> m_AvalibleParabolas;
    public Transform m_TargetParabola;
    [SerializeField] private float m_HeightIncrease = 0.15f;
    [SerializeField] private List<Transform> m_Parabolas;
    [SerializeField] private GameObject m_Grenade;
    [SerializeField] private Transform m_Origin;
    private int m_ParabolaNumber = 0;
    private bool m_GettingNewTarget;
    private bool m_CheckingIfGotATarget;
    private Vector3 m_OriginalLocalTargetPosition;
    private Vector3 m_LeftLocalTargetPosition;
    private Vector3 m_RightLocalTargetPosition;
    private List<Vector3> m_LookDirections = new List<Vector3>();
    // 0 original 1 right, 2 left.
    private int m_DirectionManager;
    private ParabolaController m_ParabolaController;
    [HideInInspector] public Vector3 m_FinalLookRotation;
    
    void Start()
    {
        m_Parabolas = new List<Transform>();
        ParabolaRootMarker[] rootmarker = GetComponentsInChildren<ParabolaRootMarker>();
        foreach (ParabolaRootMarker root in rootmarker)
        {
            m_Parabolas.Add(root.transform);
        }

        m_OriginalLocalTargetPosition = m_Target.localPosition;
    }

    private void Update()
    {
                if (m_GettingNewTarget)
        {
            if (m_CheckingIfGotATarget)
            {
                CheckToSetTarget();
            }
            else
            {
                SetTarget();
            }
        }
    }


    public void SetTarget()
    {
        Transform gameObject = GameObject.Find("GrenadeLauncherVisual").transform;
        Vector3 Direction =   gameObject.position - m_Target.position;
        gameObject.rotation = Quaternion.LookRotation(Direction);
        //gameObject.eulerAngles *= -1;
        if (m_DirectionManager == 0)
        {
            m_Target.localPosition = m_OriginalLocalTargetPosition;
        }
        else if(m_DirectionManager == 1)
        {
            m_Target.localPosition = m_OriginalLocalTargetPosition + (gameObject.right * 1.5f);
        }
        else if(m_DirectionManager == 2)
        {
            m_Target.localPosition = m_OriginalLocalTargetPosition + ((gameObject.right * -1) * 1.5f );
        }
        Ray ray;
        RaycastHit hit;
        
        Transform[] children = m_Parabolas[m_ParabolaNumber].GetComponentsInChildren<Transform>();
        //LineRenderer lineRenderer = m_Parabolas[i].GetComponent<LineRenderer>();
        
        if (children[1].name == "First")
        {
            children[1].position = m_Start.position;
        }
        if (children[3].name == "Last")
        {
            children[3].position = m_Target.position;   
        }
        if (children[2].name == "Second")
        {
            float xDistance = children[3].position.x - children[1].position.x;
            float zDistance = children[3].position.z - children[1].position.z;
            xDistance /= 2;
            zDistance /= 2;
            float xPos = children[1].position.x + xDistance;
            float zPos = children[1].position.z + zDistance;
            float ypos = 0;
            if (m_ParabolaNumber == 0)
            {
                ypos = children[1].position.y + m_HeightIncrease / 2;
            }
            else
            {
                ypos = m_Parabolas[m_ParabolaNumber - 1].GetComponentsInChildren<Transform>()[2].position.y + m_HeightIncrease;
               // Debug.LogWarning(ypos);
               // ypos = m_Parabolas[i-1] children[ - 1].position.y + m_HeightIncrease;
            }
            
            children[2].position = new Vector3(xPos, ypos, zPos);

            float yDistance =  children[2].position.y - children[1].position.y;
            yDistance /= 2;
            ypos = ypos - yDistance + (yDistance/2);
            // Vector3 newPos = new Vector3(xPos - (xDistance / 2) - ((xDistance / 2) / 2), ypos, zPos - (zDistance / 2) - ((zDistance / 2)));
            List<Vector3> betweenPoints = new List<Vector3>();
            Vector3 newPos = new Vector3(xPos - (xDistance / 2), ypos, zPos - (zDistance / 2));
            
            betweenPoints.Add(newPos);
            newPos = new Vector3(xPos + (xDistance / 2), ypos, zPos + (zDistance / 2));
            betweenPoints.Add(newPos);
            Vector3 direction = betweenPoints[0] - children[1].position;
            float distance = Vector3.Distance(betweenPoints[0], children[1].position);
            ray = new Ray(children[1].position,direction);
            Debug.DrawRay(children[1].position, direction, Color.blue);


            if (!Physics.Raycast(ray, distance))
            {
                direction = children[2].position - betweenPoints[0];
                distance = Vector3.Distance(betweenPoints[0], children[2].position);
                ray = new Ray(betweenPoints[0], direction);
                Debug.DrawRay(betweenPoints[0], direction, Color.blue);
                if (!Physics.Raycast(ray, distance))
                {
                    direction = betweenPoints[1] - children[2].position;
                    distance = Vector3.Distance(betweenPoints[1], children[2].position);
                    ray = new Ray(children[2].position, direction);
                    Debug.DrawRay(children[2].position, direction, Color.blue);
                    if (!Physics.Raycast(ray, distance))
                    {
                        direction = children[3].position - betweenPoints[1];
                        distance = Vector3.Distance(betweenPoints[1], children[3].position);
                        ray = new Ray(betweenPoints[1], direction);
                        Debug.DrawRay(betweenPoints[1], direction, Color.blue);
                        if (!Physics.Raycast(ray, distance))
                        {
                            m_AvalibleParabolas.Add(m_Parabolas[m_ParabolaNumber]);
                            m_LookDirections.Add(betweenPoints[0] - children[1].position);
                        }
                    }
                }
            }

            //lineRenderer.SetPosition(0, children[1].position);
            //lineRenderer.SetPosition(1, betweenPoints[0]);
            //lineRenderer.SetPosition(2, children[2].position);
            //lineRenderer.SetPosition(3, betweenPoints[1]);
            //lineRenderer.SetPosition(4, children[3].position);
        }
        

        m_ParabolaNumber++;
        if (m_ParabolaNumber >= m_Parabolas.Count)
        {
            m_CheckingIfGotATarget = true;
            m_ParabolaNumber = 0;
            //m_AvalibleParabolas.Clear();
        }
    }

    private void CheckToSetTarget()
    {
        if (m_AvalibleParabolas.Count > 0)
        {
            int random = Random.Range(0, m_AvalibleParabolas.Count);
            m_TargetParabola = m_AvalibleParabolas[random];
            m_FinalLookRotation = m_LookDirections[random];
            m_GettingNewTarget = false;
            m_AvalibleParabolas.Clear();
            m_LookDirections.Clear();
            GameObject grenade = Instantiate(m_Grenade, m_Origin.position, m_Origin.rotation);
            m_ParabolaController = grenade.GetComponent<ParabolaController>();
            m_ParabolaController.m_RootManager = this;
            m_ParabolaController.Launch();
        }
        else
        {
            m_DirectionManager++;
            if (m_DirectionManager > 2)
            {
                //Debug.Log("Cant get there");
                m_CheckingIfGotATarget = false;
                m_GettingNewTarget = false;
                m_DirectionManager = 0;
            }
                
        }
        m_CheckingIfGotATarget = false;
    }

    public void Shoot()
    {
        m_GettingNewTarget = true;
        m_DirectionManager = 0;
    }
}
