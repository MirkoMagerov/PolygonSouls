using UnityEngine;

public class DogClawAttackState : DogEnemyState
{
    private float attackDuration = 1.0f;
    private float timer = 0f;
    private float damageDealtTime = 0.5f; // Momento en que se aplica el daño durante la animación
    private bool hasDealDamage = false;

    public DogClawAttackState(DogStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        stateMachine.Agent.isStopped = true;
        stateMachine.Animator.SetTrigger("Claw");
        timer = 0f;
        hasDealDamage = false;

        // Mirar hacia el jugador
        FacePlayer();
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;

        // Seguir mirando al jugador durante parte del ataque
        if (timer < 0.4f)
        {
            FacePlayer();
        }

        // Aplicar daño en el momento correcto de la animación
        if (timer >= damageDealtTime && !hasDealDamage)
        {
            TryDealDamage();
            hasDealDamage = true;
        }

        if (timer >= attackDuration)
        {
            // Transicionar al estado de circling agresivo después del ataque
            stateMachine.TransitionToCirclingState();
        }
    }

    private void TryDealDamage()
    {
        if (stateMachine.Player == null) return;

        // Verificar si el jugador está dentro del rango de ataque
        float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, stateMachine.Player.position);
        if (distanceToPlayer <= stateMachine.AttackRange)
        {
            // Verificar si estamos mirando hacia el jugador (±40 grados para el zarpazo)
            Vector3 directionToPlayer = stateMachine.Player.position - stateMachine.transform.position;
            directionToPlayer.y = 0;
            float angleToPlayer = Vector3.Angle(stateMachine.transform.forward, directionToPlayer.normalized);

            if (angleToPlayer <= 40f)
            {
                // Aplicar daño
                stateMachine.DealDamage();
            }
        }
    }

    private void FacePlayer()
    {
        if (stateMachine.Player != null)
        {
            Vector3 direction = stateMachine.Player.position - stateMachine.transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            stateMachine.transform.rotation = Quaternion.Lerp(
                stateMachine.transform.rotation,
                targetRotation,
                Time.deltaTime * 15f); // Velocidad de rotación más alta para movimientos agresivos
        }
    }

    public override void ExitState()
    {
        stateMachine.Agent.isStopped = false;
    }
}