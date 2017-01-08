using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NGMP.WPF
{
    public static class ObservableCollectionExtension
    {

        public static void Sort<T>(this ObservableCollection<T> observable) where T : IComparable<T>, IEquatable<T>
        {
            List<T> sorted = observable.OrderBy(x => x).ToList();
            for (int i = 0; i < sorted.Count(); i++)
            {
                observable.Move(observable.IndexOf(sorted[i]), i);
            }
        }

        public static void SortDesc<T>(this ObservableCollection<T> observable) where T : IComparable<T>, IEquatable<T>
        {
            List<T> sorted = observable.OrderByDescending(x => x).ToList();
            for (int i = 0; i < sorted.Count(); i++)
            {
                observable.Move(observable.IndexOf(sorted[i]), i);
            }
        }

        public static int Remove<T>(this ObservableCollection<T> observable, Func<T, bool> condition)
        {
            var itemsToRemove = observable.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                observable.Remove(itemToRemove);
            }

            return itemsToRemove.Count;
        }
    }
}
