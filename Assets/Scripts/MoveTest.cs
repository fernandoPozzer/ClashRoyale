using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    [SerializeField]
    private Transform targetPos;

    [SerializeField]
    private Transform leftBridge;

    [SerializeField]
    private Transform rightBridge;

    private float bridgeLenght;

    private List<Vector3> ittinerary;

    public float attackDistance = 5f;
    public float speed = 0.001f;

    private HealthBar healthBar;
    private float initialDistance;

    void Start()
    {
        ittinerary = new List<Vector3>();

        bridgeLenght = leftBridge.transform.localScale.z;
        MoveTo(targetPos.position);

        healthBar = gameObject.GetComponentInChildren<HealthBar>();
        initialDistance = GetDistanceXZ(targetPos.position, transform.position);
    }

    void Update()
    {
        Move();

        healthBar.UpdateHeath(GetDistanceXZ(targetPos.position, transform.position), initialDistance);
    }

    private void MoveTo(Vector3 target)
    {
        if (IsTargetInTheSameField(target))
        {
            ittinerary.Add(target);
            return;
        }

        Vector3 bridgeToUse = GetBridgeToUse();
        (Vector3 entrance, Vector3 exit) = GetBridgeEntranceAndExit(bridgeToUse);

        ittinerary.Add(entrance);
        ittinerary.Add(exit);
        ittinerary.Add(target);
    }

    /// <summary>
    /// Verifica se não é preciso atravessar a ponte para chegar ao alvo.
    /// </summary>
    private bool IsTargetInTheSameField(Vector3 target)
    {
        Vector3 d1 = GetDirectionVector(target, leftBridge.position);
        Vector3 d2 = GetDirectionVector(transform.position, leftBridge.position);

        return Vector3.Dot(d1, d2) > 0;
    }

    private Vector3 GetDirectionVector(Vector3 origin, Vector3 destination)
    {
        Vector3 direction = destination - origin;
        direction.y = 0;
        direction.Normalize();

        return direction;
    }

    /// <summary>
    /// Retorna a posição da ponte mais próxima do agente.
    /// </summary>
    private Vector3 GetBridgeToUse()
    {
        if(Vector3.Distance(transform.position, leftBridge.position) < Vector3.Distance(transform.position, rightBridge.position))
        {
            return leftBridge.position;
        }

        return rightBridge.position;
    }

    /// <summary>
    /// Retorna qual o ponto de entrada e de saída da ponte.
    /// </summary>
    private (Vector3 entrance, Vector3 exit) GetBridgeEntranceAndExit(Vector3 bridgePosition)
    {
        (Vector3 entrance, Vector3 exit) points;

        Vector3 offset = new Vector3(0, 0, bridgeLenght);

        Vector3 p1 = bridgePosition + offset;
        Vector3 p2 = bridgePosition - offset;
        
        if (Vector3.Distance(transform.position, p1) < Vector3.Distance(transform.position, p2))
        {
            points.entrance = p1;
            points.exit = p2;

            return points;
        }

        points.entrance = p2;
        points.exit = p1;

        return points;
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

        //Vector3 movingDirection = target - transform.position;
        //movingDirection.y = 0;
        //movingDirection.Normalize();

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
