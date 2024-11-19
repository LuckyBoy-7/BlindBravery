namespace Lucky.Framework.Inputs_
{
    public class VirtualIntegerAxis : VirtualInput
    {
        public Binding Positive;
        public Binding Negative;

        public OverlapBehaviors OverlapBehavior;
        public bool Inverted;
        public int Value;
        public int PreviousValue;
        private bool turned;

        public VirtualIntegerAxis(Binding negative, Binding positive, OverlapBehaviors overlapBehavior = OverlapBehaviors.TakeNewer)
        {
            Positive = positive;
            Negative = negative;
            OverlapBehavior = overlapBehavior;
        }


        public override void Update()
        {
            PreviousValue = Value;
            bool positiveCheck = Positive.Check();
            bool negativeCheck = Negative.Check();
            if (positiveCheck && negativeCheck)
            {
                switch (OverlapBehavior)
                {
                    case OverlapBehaviors.CancelOut: // 终止
                        Value = 0;
                        break;
                    case OverlapBehaviors.TakeOlder: // 用之前的
                        Value = PreviousValue;
                        break;
                    case OverlapBehaviors.TakeNewer: // 用后来的
                        if (!turned)
                        {
                            Value *= -1;
                            turned = true;
                        }

                        break;
                }
            }
            else if (positiveCheck) // 只按着一个键
            {
                turned = false;
                Value = 1;
            }
            else if (negativeCheck) // 只按着一个键
            {
                turned = false;
                Value = -1;
            }
            else // 没按键
            {
                turned = false;
                Value = 0;
            }

            if (Inverted) // 是否反转操作, 提供更多玩法)
            {
                Value = -Value;
            }
        }

        public static implicit operator float(VirtualIntegerAxis axis)
        {
            return axis.Value;
        }
    }
}