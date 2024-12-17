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

    [SerializeField]
    List<GameObject> availableTroops;

    void Start()
    {
        AddEnemy(0, new Vector3(-1, 1, 28.5f));

        AddAlly(0, new Vector3(-10, 1, 12f));

        enemies.AddRange(enemyTowers);
        allies.AddRange(allyTowers);
    }

    void Update()
    {
        checkAttacks(allies, enemies);
        checkAttacks(enemies, allies);

        RemoveDeadTroops(allies);
        RemoveDeadTroops(enemies);
    }

    public void AddEnemy(int idx, Vector3 position)
    {
        enemies.Add(InstantiateTroop(idx, position, Color.red));
    }

    public void AddAlly(int idx, Vector3 position)
    {
        allies.Add(InstantiateTroop(idx, position, Color.blue));
    }

    private Agent InstantiateTroop(int idx, Vector3 position, Color color)
    {
        position.y = 0;

        GameObject gameObject = Instantiate(availableTroops[idx], position, Quaternion.identity);
        gameObject.GetComponent<Renderer>().material.color = color;
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

                if (distance < attacker.attackDistance)
                {
                    Debug.Log($"{attacker.name} atacou {victim.name}");

                    attackerHitSomething = true;
                    attacker.Attack(victim);

                    if (!attacker.MakesAreaDamageAttack)
                    {
                        break;
                    }
                }
            }

            /// Se não existem alvos no range do atacante e o agente pode andar
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

            if (distance < minDist)
            {
                distance = minDist;
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
