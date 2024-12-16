using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public void AddEnemy(int idx, Vector3 position)
    {
        enemies.Add(InstantiateTroop(idx, position));
    }

    public void AddAlly(int idx, Vector3 position)
    {
        allies.Add(InstantiateTroop(idx, position));
    }

    private Agent InstantiateTroop(int idx, Vector3 position)
    {
        position.y = 0;

        GameObject gameObject = Instantiate(availableTroops[idx], position, Quaternion.identity);
        return gameObject.GetComponent<Agent>();
    }

    void Start()
    {
        AddEnemy(0, new Vector3(-12, 0, 4));
    }

    void Update()
    {
        checkAttacks(allies, enemies);
        checkAttacks(enemies, allies);

        checkAttacks(allyTowers, enemies);
        checkAttacks(enemyTowers, allies);

        checkAttacks(allies, enemyTowers);
        checkAttacks(enemies, allyTowers);

        RemoveDeadTroops(allies);
        RemoveDeadTroops(enemies);
        RemoveDeadTroops(enemyTowers);
        RemoveDeadTroops(allyTowers);
    }

    private void checkAttacks(List<Agent> attackers, List<Agent> victims)
    {
        foreach (Agent attacker in attackers)
        {
            if (attacker.IsDead())
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

                    attacker.MakeAttack();
                    attackerHitSomething = true;

                    victim.ReceiveAttack(attacker.attackDamage);
                }
            }

            /// Se não existem alvos, o agente pode andar
            if (!attackerHitSomething && attacker.canMove)
            {
                /// FINALIZAR
            }
        }
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
                troops[i].Destroy(); // Chame o método Destroy antes de remover.
                troops.RemoveAt(i);
            }
        }
    }
}
