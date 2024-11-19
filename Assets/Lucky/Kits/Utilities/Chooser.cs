using System;
using System.Collections.Generic;
using System.Globalization;
using Lucky.Kits.Extensions;
using Lucky.Kits.Utilities;

namespace Lucky.Kits.Utilities
{
    /// <summary>
    /// 对分层随机抽样的封装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Chooser<T>
    {
        private List<Choice> choices = new List<Choice>();

        private class Choice
        {
            public T Value;  // 值
            public float Weight;  // 值占的权重

            public Choice(T value, float weight)
            {
                Value = value;
                Weight = weight;
            }
        }


        public Chooser(T firstChoice, float weight)
        {
            Add(firstChoice, weight);
        }

        public Chooser(params T[] choices)
        {
            foreach (T t in choices)
            {
                Add(t, 1f);
            }
        }

        public int Count => choices.Count;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }

                return choices[index].Value;
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }

                choices[index].Value = value;
            }
        }

        public Chooser<T> Add(T choice, float weight)
        {
            if (weight <= 0)
                throw new ArgumentException("Weight must be positive!");
            choices.Add(new Choice(choice, weight));
            TotalWeight += weight;
            return this;
        }

        // 抽一个值
        public T Choose()
        {
            if (TotalWeight <= 0f)
            {
                return default(T);
            }

            if (choices.Count == 1)
            {
                return choices[0].Value;
            }

            double rand = RandomUtils.NextFloat(TotalWeight);
            float cur = 0f;
            for (int i = 0; i < choices.Count - 1; i++)
            {
                cur += choices[i].Weight;
                if (cur > rand)
                {
                    return choices[i].Value;
                }
            }

            return choices[^1].Value;
        }

        public float TotalWeight { get; private set; }

        public static Chooser<TT> FromString<TT>(string data) where TT : IConvertible
        {
            // 生成一个chooser对象
            Chooser<TT> chooser = new Chooser<TT>();
            // 切分字符串, 理论上应该会形成 val1: weight1, val2: weight2 ... valn: weightn
            string[] array = data.Split(new[] { ',' });
            // 如果只提供了一个val且不提供weight, 则weight为1并返回
            if (array.Length == 1 && array[0].IndexOf(':') == -1)
            {
                chooser.Add((TT)Convert.ChangeType(array[0], typeof(TT)), 1f);
                return chooser;
            }

            foreach (string text in array)
            {
                if (text.IndexOf(':') == -1)
                {
                    // 不提供weight, 则weight默认为1
                    chooser.Add((TT)Convert.ChangeType(text, typeof(TT)), 1f);
                }
                else
                {
                    string[] pair = text.Split(new[] { ':' });
                    // 去空格
                    string val = pair[0].Trim();
                    string weight = pair[1].Trim();
                    chooser.Add((TT)Convert.ChangeType(val, typeof(TT)), Convert.ToSingle(weight, CultureInfo.InvariantCulture));
                }
            }

            return chooser;
        }

    }
}