using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TerrainUtils;

public class AgentManager : MonoBehaviour
{
    List<Agent> allies = new();
    List<Agent> enemies = new();

    [SerializeField]
    List<Agent> allyTowers;

    [SerializeField]
    List<Agent> enemyTowers;

    [System.Serializable]
    public struct Card
    {
        public GameObject gameObject;
        public int elixirCost;
    }

    [SerializeField]
    List<Card> availableTroops;

    public bool gameIsOver = false;

    void Start()
    {
        enemies.AddRange(enemyTowers);
        allies.AddRange(allyTowers);
    }

    void Update()
    {
        if (gameIsOver)
        {
            DestroyTroops(allies);
            DestroyTroops(enemies);
            enemies.Clear();
            allies.Clear();

            return;
        }

        checkAttacks(allies, enemies);
        checkAttacks(enemies, allies);

        RemoveDeadTroops(allies);
        RemoveDeadTroops(enemies);

        gameIsOver = CheckIfGameIsOver(allies) || CheckIfGameIsOver(enemies);
    }

    private bool CheckIfGameIsOver(List<Agent> agents)
    {
        foreach (Agent agent in agents)
        {
            if (!agent.IsDead() && agent.IsBuilding)
            {
                return false;
            }
        }

        return true;
    }

    private void DestroyTroops(List<Agent> agents)
    {
        foreach (Agent agent in agents)
        {
            GameObject obj = agent.gameObject;

            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }

    public int AddEnemy(int idx, Vector3 position, float currentElixir)
    {
        int elixirCost = availableTroops[idx].elixirCost;

        if (elixirCost > currentElixir)
        {
            return 0;
        }

        Agent agent = InstantiateTroop(idx, position, Color.red);

        if (!agent.IsGroupOfAgents)
        {
            enemies.Add(agent);
            return elixirCost;
        }

        foreach (Agent groupAgent in agent.Agents)
        {
            enemies.Add(groupAgent);
        }

        return elixirCost;
    }

    /// <summary>
    /// Intancia a carta e retorna quanto de elixir deve ser decrementado.
    /// Caso n�o tenha elixir suficiente, retorna zero (pois n�o instancia a carta).
    /// </summary>
    public int AddAlly(int idx, Vector3 position, float currentElixir)
    {
        int elixirCost = availableTroops[idx].elixirCost;

        if (elixirCost > currentElixir)
        {
            return 0;
        }

        Agent agent = InstantiateTroop(idx, position, Color.blue);

        if (!agent.IsGroupOfAgents)
        {
            allies.Add(agent);
            return elixirCost;
        }

        foreach (Agent groupAgent in agent.Agents)
        {
            allies.Add(groupAgent);
        }

        return elixirCost;
    }

    private Agent InstantiateTroop(int idx, Vector3 position, Color color)
    {
        position.y = 0;

        GameObject gameObject = Instantiate(availableTroops[idx].gameObject, position, Quaternion.identity);
        Renderer renderer = gameObject.GetComponent<Renderer>();

        if(renderer == null)
        {
            foreach (Transform child in gameObject.transform)
            {
                Renderer childRenderer = child.GetComponent<Renderer>();

                if (childRenderer != null)
                {
                    childRenderer.material.color = color;
                }
            }

            return gameObject.GetComponent<Agent>();
        }

        renderer.material.color = color;
        return gameObject.GetComponent<Agent>();
    }

    private void checkAttacks(List<Agent> attackers, List<Agent> victims)
    {
        foreach (Agent attacker in attackers)
        {
            /// O atacante pode ter morrido no ataque anterior
            if (attacker.IsDead())
            {
                continue;
            }

            /// O atacante pode estar com cooldown do ataque
            if (!attacker.CanAttack())
            {
                continue;
            }

            bool attackerHitSomething = false;

            foreach (Agent victim in victims)
            {
                float distance = Vector3.Distance(attacker.transform.position, victim.transform.position);

                if (distance < attacker.attackDistance + victim.transform.localScale.x)
                {
                    attackerHitSomething = true;
                    attacker.Attack(victim);

                    if (!attacker.MakesAreaDamageAttack)
                    {
                        break;
                    }
                }
            }

            /// Se n�o existem alvos no range do atacante e o agente pode andar
            if (!attackerHitSomething && !attacker.IsBuilding)
            {
                attacker.MoveTo(GetNextTarget(attacker, victims));
                attacker.Move();
            }
        }
    }

    private Agent GetNextTarget(Agent agent, List<Agent> agentEnemies)
    {
        Agent nextTarget = null;
        float minDist = 1000f;

        foreach (Agent enemy in agentEnemies)
        {
            if (enemy.IsDead())
            {
                continue;
            }

            if (agent.TargetsBuilding && !enemy.IsBuilding)
            {
                continue;
            }

            float distance = Vector3.Distance(agent.transform.position, enemy.transform.position);

            if (distance > minDist)
            {
                continue;
            }

            if (distance < agent.VisionReach || enemy.IsBuilding)
            {
                minDist = distance;
                nextTarget = enemy;
            }
        }

        return nextTarget;
    }

    private void RemoveDeadTroops(List<Agent> troops)
    {
        if (!troops.Any())
        {
            return;
        }

        for (int i = troops.Count - 1; i >= 0; i--)
        {
            if (troops[i].IsDead())
            {
                troops[i].Destroy();
                troops.RemoveAt(i);
            }
        }
    }
}
