using System;

namespace Lucky.Kits.Managers.ObjectPool_
{
    public interface IRecycle
    {
        public void OnGet();
        public void OnRelease();

    }
}