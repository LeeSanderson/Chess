using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Microsoft.Concurrency.TestTools.Alpaca.Views
{
    public class PropertyComparer<T> : IComparer<T>
    {

        private  PropertyDescriptor prop;
        private  ListSortDirection direction;

        public PropertyComparer(PropertyDescriptor prop, ListSortDirection direction)
        {
            this.prop = prop;
            this.direction = direction;
        }

        public int Compare(T x, T y)
        {
            object valueX = prop.GetValue(x);
            object valueY = prop.GetValue(y);

            // Handle nulls
            if (valueX == null && valueY == null)
                return 0;
            else if (valueX == null)
                return -1;
            else if (valueY == null)
                return 1;

            Type propType = prop.PropertyType;
            Type propNullableTypeBase = Nullable.GetUnderlyingType(propType);

            if (prop.PropertyType.IsEnum || (propNullableTypeBase != null && propNullableTypeBase.IsEnum))
            {
                // Sort enum values by their textual representation
                valueX = valueX.ToString();
                valueY = valueY.ToString();
            }

            IComparable comparableX = valueX as IComparable;
            IComparable comparableY = valueY as IComparable;
            if (comparableX != null && comparableY != null)
                return comparableX.CompareTo(comparableY);

            throw new NotImplementedException("This class can only be used with properties whose types implement IComparable.");
        }

    }
}
