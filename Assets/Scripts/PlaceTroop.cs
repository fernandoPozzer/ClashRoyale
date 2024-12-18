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

    void Update()
    {
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
                agentManager.AddAlly(0, hit.point);
            }
        }
    }
}
