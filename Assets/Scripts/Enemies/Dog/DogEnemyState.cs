public abstract class DogEnemyState
{
    protected DogStateMachine stateMachine;

    public DogEnemyState(DogStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}