public interface MissionCompleterInterface<T>
{
    public T mission { get; }
    public void Init(T newMission);
    public void FinishMission();
}
