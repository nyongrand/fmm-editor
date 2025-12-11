using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FMMEditor.Collections
{
    /// <summary>
    /// An ObservableCollection that can suppress change notifications during bulk operations for better performance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class BulkObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification = false;

        /// <summary>
        /// Overrides the event invocation to allow suppression during bulk operations.
        /// </summary>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
            {
                base.OnCollectionChanged(e);
            }
        }

        /// <summary>
        /// Adds multiple items to the collection efficiently by suppressing notifications until complete.
        /// </summary>
        /// <param name="items">The items to add to the collection.</param>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                return;

            _suppressNotification = true;

            try
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }
            finally
            {
                _suppressNotification = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Clears and adds multiple items to the collection efficiently.
        /// </summary>
        /// <param name="items">The items to set in the collection.</param>
        public void Reset(IEnumerable<T> items)
        {
            _suppressNotification = true;

            try
            {
                Clear();
                
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Add(item);
                    }
                }
            }
            finally
            {
                _suppressNotification = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
    }
}
