namespace VainEngine
{
    public class Component
    {
        public GameObject obj;
        public bool _INIT { get; private set; } = true;
        public virtual void Update() { }
        public virtual void End() { }
        public virtual void Start() { _INIT = false; }
    }
}