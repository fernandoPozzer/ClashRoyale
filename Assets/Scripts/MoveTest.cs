using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    [SerializeField]
    private Transform targetPos;

    private float bridgeLenght;

    private List<Vector3> ittinerary;

    public float attackDistance = 5f;
    public float speed = 0.001f;

    private HealthBar healthBar;
    private float initialDistance;
    private float health;

    void Start()
    {
        ittinerary = Navigation.GetIttinerary(transform.position, targetPos.position);

        /*bridgeLenght = leftBridge.transform.localScale.z;
        MoveTo(targetPos.position);*/

        healthBar = gameObject.GetComponentInChildren<HealthBar>();
        initialDistance = GetDistanceXZ(targetPos.position, transform.position);
        health = initialDistance;
    }

    void Update()
    {
        Move();
        Hit(0.01f);
    }

    private void Hit(float damage)
    {
        health = Mathf.Max(health - damage, 0);
        healthBar.UpdateHeath(health, initialDistance);

        if (health == 0)
        {
            Destroy(gameObject);
        }
    }

    private Vector3 GetDirectionVector(Vector3 origin, Vector3 destination)
    {
        Vector3 direction = destination - origin;
        direction.y = 0;
        direction.Normalize();

        return direction;
    }

    private void Move()
    {
        if (!ittinerary.Any())
        {
            return;
        }

        bool IsFinalIttineraryPoint = ittinerary.Count() == 1;

        if (HasReachedDestination(ittinerary[0], IsFinalIttineraryPoint))
        {
            ittinerary.RemoveAt(0);

            if (IsFinalIttineraryPoint)
            {
                /// TODO: Comece ATAQUE
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
}
