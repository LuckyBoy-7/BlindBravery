namespace Lucky.Framework.Inputs_
{
    public abstract class VirtualInput
    {
        public VirtualInput()
        {
            Inputs.Register(this);
        }

        public void Deregister()
        {
            Inputs.Register(this);
        }

        public abstract void Update();

        public enum OverlapBehaviors  // 同轴的两个方向一起按下是什么行为, 例如左右都按着
        {
            CancelOut,  // 不动
            TakeOlder,  // 用先前的
            TakeNewer  // 用后来的 
        }
    }
}