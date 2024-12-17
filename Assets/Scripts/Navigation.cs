using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Cria uma lista de waypoints para os agentes.
/// </summary>
public class Navigation : MonoBehaviour
{
    private static Vector3 leftBridge = new Vector3(-12, 0, 22.5f);
    private static Vector3 rightBridge = new Vector3(12, 0, 22.5f);
    private static float bridgeLenght = 5f;

    public static List<Vector3> GetIttinerary(Vector3 agentPosition, Vector3 target)
    {
        List<Vector3> ittinerary = new List<Vector3>();

        if (IsTargetInTheSameField(agentPosition, target))
        {
            ittinerary.Add(target);
            return ittinerary;
        }

        Vector3 bridgeToUse = GetBridgeToUse(agentPosition);
        (Vector3 entrance, Vector3 exit) = GetBridgeEntranceAndExit(agentPosition, bridgeToUse);

        if (ShouldAddBridgeEntrancePosition(agentPosition))
        {
            ittinerary.Add(entrance);
        }

        ittinerary.Add(exit);
        ittinerary.Add(target);

        return ittinerary;
    }

    private static bool ShouldAddBridgeEntrancePosition(Vector3 agentPosition)
    {
        float minDist = 0.2f;
        float deltaX1 = Mathf.Abs(agentPosition.x - leftBridge.x);
        float deltaX2 = Mathf.Abs(agentPosition.x - rightBridge.x);

        return deltaX1 > minDist && deltaX2 > minDist;
    }

    /// <summary>
    /// Verifica se não é preciso atravessar a ponte para chegar ao alvo.
    /// </summary>
    private static bool IsTargetInTheSameField(Vector3 agentPosition, Vector3 target)
    {
        Vector3 d1 = GetDirectionVector(target, leftBridge);
        Vector3 d2 = GetDirectionVector(agentPosition, leftBridge);

        return Vector3.Dot(d1, d2) > 0;
    }

    /// <summary>
    /// Retorna a posição da ponte mais próxima do agente.
    /// </summary>
    private static Vector3 GetBridgeToUse(Vector3 agentPosition)
    {
        if (Vector3.Distance(agentPosition, leftBridge) < Vector3.Distance(agentPosition, rightBridge))
        {
            return leftBridge;
        }

        return rightBridge;
    }

    /// <summary>
    /// Retorna qual o ponto de entrada e de saída da ponte.
    /// </summary>
    private static  (Vector3 entrance, Vector3 exit) GetBridgeEntranceAndExit(Vector3 agentPosition, Vector3 bridgePosition)
    {
        (Vector3 entrance, Vector3 exit) points;

        Vector3 offset = new Vector3(0, 0, bridgeLenght);

        Vector3 p1 = bridgePosition + offset;
        Vector3 p2 = bridgePosition - offset;

        if (Vector3.Distance(agentPosition, p1) < Vector3.Distance(agentPosition, p2))
        {
            points.entrance = p1;
            points.exit = p2;

            return points;
        }

        points.entrance = p2;
        points.exit = p1;

        return points;
    }

    private static Vector3 GetDirectionVector(Vector3 origin, Vector3 destination)
    {
        Vector3 direction = destination - origin;
        direction.y = 0;
        direction.Normalize();

        return direction;
    }
}
