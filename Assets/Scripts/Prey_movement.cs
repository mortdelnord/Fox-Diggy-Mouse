using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Prey_movement : MonoBehaviour
{
    private NavMeshAgent preyAgent;
    public float range = 10.0f;

    public float timerMax;
    public float randomMax;
    public float randMin;

    private float timer = 0f;
    public bool canMove = true;


    private void Start()
    {
        preyAgent = gameObject.GetComponent<NavMeshAgent>();
        timer = timerMax;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result) // gets a randoim point on the navmesh
    {
        for (int i = 0; i < 30; i++)
        {

            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.GetAreaFromName("PreyArea")))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    void Update()
    {
        if (preyAgent.isActiveAndEnabled) // if nav agent is enebled the prey can move
        {
            canMove = true;
        }else
        {
            canMove = false;
        }
        if (canMove) // if can move, the timer counts
        {

            timer += Time.deltaTime;
            if (timer >= timerMax) // if the timer is above the max time
            {

                Vector3 point;
                if (RandomPoint(transform.position, range, out point))
                {
                    preyAgent.SetDestination(point); // get new random point to move too
                    //Debug.Log(point);
                }
                timer = 0f;
                float randomTime = Random.Range(randMin, randomMax); // reset timer and get a new random time for move timer
                timerMax = randomTime;
                //Debug.Log(timerMax);
            }
        }
    }
}
