using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Microsoft.Concurrency.TestTools.Alpaca.Views
{
    public class PropertyBindingList<T> : BindingList<T>
    {

        protected override bool SupportsSortingCore { get { return true; } }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            var items = this.Items as List<T>;
            if (items != null)
            {
                var comparer= new PropertyComparer<T>(prop, direction);
                items.Sort(comparer);
            }

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override bool SupportsSearchingCore { get { return true; } }

        protected override int FindCore(PropertyDescriptor prop, object key)
        {
            var comparer= new PropertyComparer<T>(prop, ListSortDirection.Ascending);

            for (int i = 0; i < Items.Count; i++)
            {
                T item = Items[i];
                if (prop.GetValue(item).Equals(key))
                    return i;
            }

            return -1;
        }

    }

}
