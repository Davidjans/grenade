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

    void Start()
    {
        m_Parabolas = new List<Transform>();
        ParabolaRootMarker[] rootmarker = GetComponentsInChildren<ParabolaRootMarker>();
        foreach (ParabolaRootMarker root in rootmarker)
        {
            m_Parabolas.Add(root.transform);
        }
    }

    //private void Update()
    //{
        //SetTarget();
    //}


    public void SetTarget()
    {
        Ray ray;
        RaycastHit hit;
        m_AvalibleParabolas.Clear();
        for (int i = 0; i < m_Parabolas.Count; i++)
        {
            Transform[] children = m_Parabolas[i].GetComponentsInChildren<Transform>();
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
                if (i == 0)
                {
                    ypos = children[1].position.y + m_HeightIncrease / 2;
                }
                else
                {
                    ypos = m_Parabolas[i-1].GetComponentsInChildren<Transform>()[2].position.y + m_HeightIncrease;
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
                                m_AvalibleParabolas.Add(m_Parabolas[i]);
                            }
                        }
                    }
                }

               
                if (m_AvalibleParabolas.Count > 0)
                    m_TargetParabola = m_AvalibleParabolas[Random.Range(0, m_AvalibleParabolas.Count)];


                //lineRenderer.SetPosition(0, children[1].position);
                //lineRenderer.SetPosition(1, betweenPoints[0]);
                //lineRenderer.SetPosition(2, children[2].position);
                //lineRenderer.SetPosition(3, betweenPoints[1]);
                //lineRenderer.SetPosition(4, children[3].position);
            }
        }
    }
}
