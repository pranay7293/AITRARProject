using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNPC : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(player != null)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);

    }
    
}
