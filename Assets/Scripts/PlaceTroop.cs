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

    [SerializeField]
    private ElixirManager elixirManager;

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
                int decrementedElixir = agentManager.AddAlly(currentTroop, hit.point, elixirManager.GetCurrentElixir());
                elixirManager.DecrementElixir(decrementedElixir);
            }
        }
    }

    public void setCurrentTroopMiniPekka()
    {
        currentTroop = 0;
    }

    public void setCurrentTroopSkeletonArmy()
    {
        currentTroop = 1;
    }

    public void setCurrentTroopGiant()
    {
        currentTroop = 2;
    }
}
