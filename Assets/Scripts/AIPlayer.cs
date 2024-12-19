using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    private float currentElixir = 0f;
    private float lastIncreaseTime = 0f;
    private const float timePerDecimal = 0.1f;

    private int nextTroop = -1;

    [SerializeField]
    private AgentManager agentManager;

    [SerializeField]
    private List<Transform> troopPoints;

    void Start()
    {
        lastIncreaseTime = Time.time;
    }

    void Update()
    {
        if (agentManager.gameIsOver)
        {
            return;
        }

        if (Time.time >= lastIncreaseTime + timePerDecimal)
        {
            currentElixir = Mathf.Min(currentElixir + 0.1f, 10f);
            lastIncreaseTime = Time.time;
        }

        PlaceTroopRandomly();
    }

    private void PlaceTroopRandomly()
    {
        int troop = Random.Range(0, 3);

        if (nextTroop == -1)
        {
            nextTroop = troop;
        }

        int point = Random.Range(0, troopPoints.Count);
        Vector3 position = troopPoints[point].transform.position;

        int decrementElixir = agentManager.AddEnemy(nextTroop, position, currentElixir);
        currentElixir -= decrementElixir;

        if (decrementElixir > 0)
        {
            nextTroop = -1;
        }
    }
}
