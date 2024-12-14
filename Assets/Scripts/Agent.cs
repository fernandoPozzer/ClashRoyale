using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float attackDistance = 5f;
    public float attackSpeed = 1f;
    public int attackDamage = 20;

    public float speed = 0.001f;

    public bool canMove = true;

    public const int maxHealth = 2000;
    public int health = 2000;

    private HealthBar healthBar;

    private List<Vector3> ittinerary;
    private bool isMoving = false;

    void Start()
    {
        healthBar = gameObject.GetComponentInChildren<HealthBar>();
    }

    void Update()
    {
    
    }

    public void MoveTo(Vector3 target)
    {
        ittinerary = Navigation.GetIttinerary(transform.position, target);
        isMoving = true;
    }

    public void ReceiveAttack(int damage)
    {
        health = Mathf.Max(health - damage, 0);
        healthBar.UpdateHeath(health, maxHealth);
    }

    public void MakeAttack()
    {

    }

    public bool IsDead()
    {
        return health == 0;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void Move()
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
                /// COMECAR ATAQUE
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
