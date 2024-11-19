using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lucky.Framework.Inputs_
{
    /// <summary>
    /// 对Key的封装, 一对多
    /// </summary>
    public class Binding
    {

        public List<KeyCode> Keys = new List<KeyCode>();

        /// <summary>
        /// 其他和自己不能共存的binding, 简单来说就是不能绑一样的键, 常用于一些跟UI有关的操作, 比如移动彩蛋, 确认取消什么的
        /// </summary>
        public List<Binding> ExclusiveFrom = new List<Binding>();

        public bool HasInput => Keys.Count > 0;

        public Binding(params KeyCode[] keys)
        {
            Keys = keys.ToList();
        }

        public bool Add(params KeyCode[] keys)
        {
            bool anySuccess = false;
            foreach (KeyCode key in keys)
            {
                if (!Keys.Contains(key))
                {
                    bool success = true;
                    foreach (var other in ExclusiveFrom)
                    {
                        if (other.Needs(key)) // 但凡其他的binding需要这个键位, 我们就不跟他抢
                        {
                            success = false;
                            break;
                        }
                    }

                    if (success)
                    {
                        Keys.Add(key);
                        anySuccess = true; 
                    }
                }
            }

            return anySuccess;
        }

        /// <summary>
        /// 查询这个key对自己来说是否重要, 或者说能不能删
        /// </summary>
        public bool Needs(KeyCode key) => Keys.Contains(key);

        /// <summary>
        /// 清空键盘binding, 尽量留一个没被其他binding绑过的key
        /// </summary>
        public bool ClearKeyboard()
        {
            if (ExclusiveFrom.Count > 0)
            {
                // 只剩一个key就不需要清空了
                if (Keys.Count <= 1)
                    return false;

                KeyCode keys = Keys[0];
                Keys.Clear();
                Keys.Add(keys);
            }
            else
            {
                Keys.Clear();
            }

            return true;
        }

        public bool Check()
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                if (UnityEngine.Input.GetKey(Keys[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Pressed()
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                if (UnityEngine.Input.GetKeyDown(Keys[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Released()
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                if (UnityEngine.Input.GetKeyUp(Keys[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static void SetExclusive(params Binding[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                list[i].ExclusiveFrom.Clear();
            }

            for (var i = 0; i < list.Length; i++)
            {
                for (var j = i + 1; j < list.Length; j++)
                {
                    list[i].ExclusiveFrom.Add(list[j]);
                    list[j].ExclusiveFrom.Add(list[i]);
                }
            }
        }
    }
}