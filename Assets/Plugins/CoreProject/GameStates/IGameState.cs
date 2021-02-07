namespace CoreProject.States
{
    public interface IGameState
    {
        void Begin();
        void End();
        void Update();
        void FixedUpdate();
        void LateUpdate();
    }
}