using UnityEngine;

public class PatrolState : EnemyBaseState
{
    private int currentPatrolIndex = 0;

    public PatrolState(NormalSkeletonStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.Agent.speed = stateMachine.PatrolSpeed;
        stateMachine.Agent.isStopped = false;
        stateMachine.Animator.SetBool("IsRunning", false);

        GoToNextPatrolPoint();
    }

    public override void UpdateState()
    {
        if (!stateMachine.Agent.pathPending && stateMachine.Agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }

        CheckPlayerDetection();
    }

    public override void ExitState() { }

    private void GoToNextPatrolPoint()
    {
        if (stateMachine.PatrolPoints.Length == 0) return;

        currentPatrolIndex = (currentPatrolIndex + 1) % stateMachine.PatrolPoints.Length;
        stateMachine.Agent.SetDestination(stateMachine.PatrolPoints[currentPatrolIndex].position);
    }

    private void CheckPlayerDetection()
    {
        if (stateMachine.Player == null) return;

        Vector3 directionToPlayer = stateMachine.Player.position - stateMachine.transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= stateMachine.DetectionRadius)
        {
            float angle = Vector3.Angle(stateMachine.transform.forward, directionToPlayer);
            if (angle <= stateMachine.DetectionAngle / 2f)
            {
                if (Physics.Raycast(stateMachine.transform.position + Vector3.up, directionToPlayer.normalized, out RaycastHit hit, distanceToPlayer))
                {
                    if (hit.transform == stateMachine.Player)
                    {
                        stateMachine.DetectPlayer();
                    }
                }
            }
        }
    }
}