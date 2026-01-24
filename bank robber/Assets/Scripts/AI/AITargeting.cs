using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AITargeting : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float sightRange = 60f;
    [SerializeField] private float refreshInterval = 0.5f;
    [SerializeField] private bool allowAIVsAI = true;
    [SerializeField] private bool preferPlayer = true;

    static List<AIState> AllAI;

    public void Tick(AIState state)
    {
        if (state == null) return;

        state.targetRefreshTimer -= Time.deltaTime;
        
        if (state.targetRefreshTimer > 0f)
            return; 

        state.targetRefreshTimer = refreshInterval;
        
        Transform newTarget = PickTarget(state);

        if (newTarget != state.currentTarget)
        {
            state.currentTarget = newTarget;
            state.lastTargetTime = Time.time;
        }

    }

    private Transform PickTarget(AIState state)
    {
        Transform best = null;
        float bestSqr = float.MaxValue;
        
        Vector3 myPos = state.transform.position;

        // 1) Player 후보
        Transform playerTr = null;
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            playerTr = playerObj.transform;

        if (playerTr != null)
        {
            float dSqr = (playerTr.position - myPos).sqrMagnitude;
            if (dSqr <= sightRange * sightRange)
            {
                best = playerTr;
                bestSqr = dSqr;
            }
        }

        if (allowAIVsAI)
        {
            
            AIState[] all = FindObjectsByType<AIState>(FindObjectsSortMode.None);

            for (int i = 0; i < all.Length; i++)
            {
                AIState other = all[i];
                if (other == null) continue;
                if (other == state) continue;
                if (other.isDead) continue;

                float dSqr = (other.transform.position - myPos).sqrMagnitude;
                if (dSqr > sightRange * sightRange) continue;

                if (dSqr < bestSqr)
                {
                    bestSqr = dSqr;
                    best = other.transform;
                }
            }
        }
        return best;
    }
}
