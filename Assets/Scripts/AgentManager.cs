using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    List<Agent> allies = new();
    List<Agent> enemies = new();

    [SerializeField]
    List<Agent> allyTowers = new();

    [SerializeField]
    List<Agent> enemyTowers = new();

    public void AddEnemy(Agent agent)
    {
        enemies.Add(agent);
    }

    public void AddAlly(Agent agent)
    {
        allies.Add(agent);
    }

    void Start()
    {
        
    }

    void Update()
    {
        checkAttacks(allies, enemies);
        checkAttacks(enemies, allies);
    }

    private void checkAttacks(List<Agent> attackers, List<Agent> victims)
    {
        foreach (Agent attacker in attackers)
        {
            bool attackerHitSomething = false;

            foreach (Agent victim in victims)
            {
                float distance = Vector3.Distance(attacker.transform.position, victim.transform.position);

                if (distance < attacker.attackDistance)
                {
                    attacker.MakeAttack();
                    attackerHitSomething = true;

                    victim.ReceiveAttack(attacker.attackDamage);

                    if (victim.IsDead())
                    {
                        victims.Remove(victim);
                        victim.Destroy();
                    }
                }
            }

            /// Se não existem alvos, o agente pode andar
            if (!attackerHitSomething && attacker.canMove)
            {
                /// FINALIZAR
            }
        }
    }
}
