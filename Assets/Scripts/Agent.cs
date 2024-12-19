using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Agent : MonoBehaviour
{
    //-------------------
    // ATTACK
    //-------------------

    public float attackDistance = 5f;
    public float attackCooldown = 1f;
    public int attackDamage = 20;
    private float lastAttackTime = 0f;

    public float VisionReach = 10f;

    /// <summary>
    /// Indica se o ataque dá dano em área.
    /// Se der, permite que mais de uma tropa inimiga tome dano.
    /// </summary>
    public bool MakesAreaDamageAttack = false;
    
    //-------------------
    // AGENT TYPE
    //-------------------

    public bool IsBuilding = false;

    /// <summary>
    /// Determina se a tropa é um grupo ou apenas um agente.
    /// </summary>
    public bool IsGroupOfAgents = false;

    [SerializeField]
    public List<Agent> Agents;

    /// <summary>
    /// Ataca apenas construções.
    /// </summary>
    public bool TargetsBuilding = false;

    //-------------------
    // HEALTH
    //-------------------

    public const int maxHealth = 2000;
    public int health = 2000;
    private HealthBar healthBar;

    //-------------------
    // MOVEMENT
    //-------------------

    private List<Vector3> ittinerary;
    private bool isMoving = false;
    public float speed = 0.001f;

    void Start()
    {
        healthBar = gameObject.GetComponentInChildren<HealthBar>();
    }

    void Update()
    {
        transform.rotation = Quaternion.identity;
    }

    public void MoveTo(Agent targetAgent)
    {
        if (targetAgent == null)
        {
            return;
        }

        Vector3 target = targetAgent.transform.position;
        ittinerary = Navigation.GetIttinerary(transform.position, target);

        isMoving = true;
    }

    public void ReceiveAttack(int damage)
    {
        health = Mathf.Max(health - damage, 0);
        healthBar.UpdateHeath(health, maxHealth);
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public void Attack(Agent victim)
    {
        lastAttackTime = Time.time;

        victim.ReceiveAttack(attackDamage);

        /// TODO: Add animação
    }

    public bool IsDead()
    {
        return health == 0;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void Move()
    {
        if (!isMoving || !ittinerary.Any())
        {
            return;
        }

        bool IsFinalIttineraryPoint = ittinerary.Count() == 1;

        if (HasReachedDestination(ittinerary[0], IsFinalIttineraryPoint))
        {
            ittinerary.RemoveAt(0);

            if (IsFinalIttineraryPoint)
            {
                isMoving = false;
                return;
            }
        }

        Vector3 target = ittinerary[0];

        Vector3 movingDirection = GetDirectionVector(transform.position, target);
        transform.position += speed * movingDirection;
    }

    /// <summary>
    /// Retorna se o agente pode passar para o próximo ponto no itinerário.
    /// O último ponto do itinerário é sempre um alvo. Logo, basta o agente estar
    /// dentro do range de ataque para parar.
    /// </summary>
    private bool HasReachedDestination(Vector3 target, bool isFinalIttineraryPoint)
    {
        float distance = GetDistanceXZ(target, transform.position);

        if (isFinalIttineraryPoint)
        {
            return distance < attackDistance;
        }

        return distance < 1f;
    }

    /// <summary>
    /// Retorna a distância ignorando o componente Y.
    /// </summary>
    private float GetDistanceXZ(Vector3 p1, Vector3 p2)
    {
        Vector3 auxiliary = p1 - p2;
        auxiliary.y = 0;

        return auxiliary.magnitude;
    }

    private Vector3 GetDirectionVector(Vector3 origin, Vector3 destination)
    {
        Vector3 direction = destination - origin;
        direction.y = 0;
        direction.Normalize();

        return direction;
    }
}
