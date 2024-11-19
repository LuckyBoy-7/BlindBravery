using System.Collections.Generic;
using Lucky.Kits.Extensions;
using UnityEngine;

namespace Lucky.Kits.Collections
{
    public class QuadTree<T>
    {
        public QuadTree<T>[] Children = new QuadTree<T>[4]; // 0123表示1234象限
        public List<T> values = new();
        private float x; // 长方形的中心
        private float y;
        private float width;
        private float height;
        private int depth;

        public QuadTree(float x, float y, float width, float height, int depth)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public void Add(T value, Vector2 position)
        {
            values.Add(value);
            if (depth == 0)
                return;
            QuadTree<T> child = GetChildByPosition(position);
            child.Add(value, position);
        }

        public void Remove(T value, Vector2 position)
        {
            values.Remove(value);
            if (depth == 0)
                return;
            QuadTree<T> child = GetChildByPosition(position);
            child.Remove(value, position);
        }

        /// <summary>
        /// 拿到最大深度的有值得4叉树节点
        /// </summary>
        /// <returns></returns>
        public List<T> GetDeepestValueList(Vector2 position)
        {
            if (depth == 0)
                return new List<T>();
            var child = GetChildByPosition(position);
            var lst = child.GetDeepestValueList(position);
            if (lst.Count > 0) // 如果子类有更小个数的值列表, 则选择递归答案
                return lst;
            return values; // 否则返回当前列表, 当然也可能为空
        }

        private QuadTree<T> GetChildByPosition(Vector2 position)
        {
            int idx = position.x switch
            {
                > 0 when position.y >= 0 => 0,
                <= 0 when position.y > 0 => 1,
                < 0 when position.y <= 0 => 2,
                >= 0 when position.y < 0 => 3,
                _ => 0
            };

            if (Children[idx] == null)
            {
                if (idx == 0)
                    Children[idx] = new QuadTree<T>(x + width / 4, y + height / 4, width / 2, height / 2, depth - 1);
                else if (idx == 1)
                    Children[idx] = new QuadTree<T>(x - width / 4, y + height / 4, width / 2, height / 2, depth - 1);
                else if (idx == 2)
                    Children[idx] = new QuadTree<T>(x - width / 4, y - height / 4, width / 2, height / 2, depth - 1);
                else if (idx == 3)
                    Children[idx] = new QuadTree<T>(x + width / 4, y - height / 4, width / 2, height / 2, depth - 1);
            }

            return Children[idx];
        }
    }
}