using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobberBehaviour : MonoBehaviour
{
    public Transform cop;
    public GameObject treasure;
    public float dist2Steal = 10f;
    Moves moves;
    UnityEngine.AI.NavMeshAgent agent;

    private WaitForSeconds wait = new WaitForSeconds(0.05f); // == 1/20
    delegate IEnumerator State();
    private State state;

    IEnumerator Start()
    {
        moves = gameObject.GetComponent<Moves>();
        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

        yield return wait;

        state = Wander;

        while (enabled)
            yield return StartCoroutine(state());
    }

    IEnumerator Wander()
    {
        Debug.Log("Wander state");

        while (Vector3.Distance(cop.position, treasure.transform.position) < dist2Steal)
        {
            moves.Wander();
            yield return wait;
        };

        state = Approaching;
    }

    IEnumerator Approaching()
    {
        Debug.Log("Approaching state");

        agent.speed = 2f;
        moves.Seek(treasure.transform.position);

        bool stolen = false;
        while (Vector3.Distance(cop.position, treasure.transform.position) > dist2Steal)
        {
            if (Vector3.Distance(treasure.transform.position, transform.position) < 2f)
            {
                stolen = true;
                break;
            };
            yield return wait;
        };

        if (stolen)
        {
            treasure.GetComponent<Renderer>().enabled = false;
            Debug.Log("Stolen");
            state = Hiding;
        }
        else
        {
            agent.speed = 1f;
            state = Wander;
        }
    }


    IEnumerator Hiding()
    {
        Debug.Log("Hiding state");

        while (true)
        {
            moves.Hide();
            yield return wait;
        };
    }
}

public class Wandering : StateMachineBehaviour
{
    Moves moves;
    BlackBoard blackboard;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        moves = animator.GetComponent<Moves>();
        blackboard = animator.GetComponent<BlackBoard>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(blackboard.cop.position, blackboard.treasure.transform.position) > blackboard.dist2Steal)
            animator.SetTrigger("away");
        else
            moves.Wander();
    }
}

public class Approaching : StateMachineBehaviour
{
    Moves moves;
    BlackBoard blackboard;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        moves = animator.GetComponent<Moves>();
        blackboard = animator.GetComponent<BlackBoard>();
        animator.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 2f;
        moves.Seek(blackboard.treasure.transform.position);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(blackboard.treasure.transform.position, animator.transform.position) < 2f)
        {
            blackboard.treasure.GetComponent<Renderer>().enabled = false;
            Debug.Log("Stolen");
            animator.SetTrigger("stolen");
        }
        else
            if (Vector3.Distance(blackboard.cop.position, blackboard.treasure.transform.position) < blackboard.dist2Steal)
        {
            animator.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 1f;
            animator.SetTrigger("near");
        };
    }
}

public class Hiding : StateMachineBehaviour
{
    Moves moves;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        moves = animator.GetComponent<Moves>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        moves.Hide();
    }
}