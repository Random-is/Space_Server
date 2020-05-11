using System;
using System.Collections.Generic;
using System.Linq;

namespace Space_Server.utility {
    public class ConcurrentList<T> : List<T> {
        private readonly object _locker = new object();
        
        public bool TryGet(int index, out T item) {
            lock (_locker) {
                try {
                    item = this[index];

                } catch (Exception) {
                    item = default;
                    return false;
                }
                return true;
            }
        }

        public bool TryAdd(T item) {
            lock (_locker) {
                try {
                    Add(item);
                } catch (Exception) {
                    return false;
                }
                return true;
            }
        }

        public bool TryAddRange(IEnumerable<T> list) {
            lock (_locker) {
                return list.All(TryAdd);
            }
        }

        public bool TryRemove(T item) {
            lock (_locker) {
                try {
                    Remove(item);
                } catch (Exception) {
                    return false;
                }
                return true;
            }
        }

        public bool TryRemoveAt(int index) {
            lock (_locker) {
                try {
                    RemoveAt(index);
                } catch (Exception) {
                    return false;
                }
                return true;
            }
        }
        
        public bool TryPop(out T item) {
            lock (_locker) {
                if (TryGet(0, out item))
                    if (TryRemove(item))
                        return true;
                item = default;
                return false;
            }
        }

        public bool TryInsert(int index, T item) {
            lock (_locker) {
                try {
                    Insert(index, item);
                } catch (Exception) {
                    return false;
                }
                return true;
            }
        }
    }
}