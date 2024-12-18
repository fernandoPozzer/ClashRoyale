using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceTroop : MonoBehaviour
{
    [SerializeField]
    private LayerMask towerLayer;

    [SerializeField]
    private LayerMask terrainLayer;

    [SerializeField]
    private AgentManager agentManager;

    private int currentTroop = -1;

    void Update()
    {
        if (currentTroop == -1)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Se o raycast acertou as torres
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, towerLayer))
            {
                return;
            }

            // Se o raycast acertar algo na layer do terreno
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer))
            {
                agentManager.AddAlly(currentTroop, hit.point);
            }
        }
    }

    public void setCurrentTroopMiniPekka()
    {
        currentTroop = 0;
        Debug.Log("Set Current Troop - Mini Pekka");
    }

    public void setCurrentTroopSkeletonArmy()
    {
        currentTroop = 1;
        Debug.Log("Set Current Troop - Skeleton Army");
    }

    public void setCurrentTroopGiant()
    {
        currentTroop = 2;
        Debug.Log("Set Current Troop - Giant");
    }
}
