using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gestureUtil
{
    public class FixedSizeQueue<T> : Queue<T>
    {

        public int Limit { get; set; }

        public FixedSizeQueue(int limit) : base(limit)
        {
            Limit = limit;
        }

        public new void Enqueue(T item)
        {
            while (Count >= Limit)
            {
                Dequeue();
            }
            base.Enqueue(item);
        }
    }

}
