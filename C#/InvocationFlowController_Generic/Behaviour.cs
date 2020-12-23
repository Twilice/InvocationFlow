namespace TLM.InvocationFlow.Example
{
    public class Behaviour : BehaviourBase
    {
        public Vector2 position;
    }

    public struct Vector2
    {
        public float x;
        public float y;
    }

    public class BehaviourBase
    {
        public bool Valid { get; private set; } = true;
        public void Destroy() => Valid = false;
    }
}
