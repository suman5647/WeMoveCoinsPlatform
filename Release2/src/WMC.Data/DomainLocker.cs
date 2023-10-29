using System;

namespace WMC.Data
{
    public delegate void UnlockHandler(long id, string lockKey);
    public interface ILockDomain
    {
        long Id { get; set; }
        string LockKey { get; set; }
        DateTime? LockUntil { get; set; }
        DomainStateLock Locker { get; set; }
    }

    public static class DomainStatic
    {
        public static bool IsLocked(this ILockDomain lockDomain)
        {
            return lockDomain.LockKey != null && lockDomain.LockUntil != null && lockDomain.LockUntil >= DateTime.UtcNow;
        }
    }
    public class DomainStateLock : ILockDomain, IDisposable
    {
        internal bool UsableChecked = false;
        private bool _Usable = false;
        private UnlockHandler unlockHandle;
        internal DomainStateLock(long id, string lockKey, bool usable, UnlockHandler unlockHandle)
        {
            this.Usable = usable;
            this.Id = id;
            this.LockKey = lockKey;
            this.unlockHandle = unlockHandle;
            UsableChecked = false;
        }

        public long Id { get; set; }
        public string LockKey { get; set; }
        public DateTime? LockUntil { get; set; }
        DomainStateLock ILockDomain.Locker { get; set; }

        public bool Usable { get { UsableChecked = true; return _Usable; } set { _Usable = value; } }

        public void Dispose()
        {
            if (Usable) // && this.IsLocked() && unlockHandle != null)
            {
                unlockHandle(Id, LockKey);
            }

            unlockHandle = null;
        }
    }

    public class StateLock<T> : DomainStateLock, ILockDomain, IDisposable where T : ILockDomain
    {
        T _Domain = default(T);
        public T Domain
        {
            get
            {
                if (!UsableChecked && Usable)
                {
                    throw new Exception("Lock missused - Check Usable");
                }
                return _Domain;
            }
        }
        internal StateLock(T lockDomain, bool usable, UnlockHandler unlockHandle)
            : base(lockDomain.Id, lockDomain.LockKey, usable, unlockHandle)
        {
            this._Domain = lockDomain;
            this._Domain.Locker = this;
        }
    }

}
