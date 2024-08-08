﻿using System.ComponentModel;
using System.Drawing;

namespace LogicPOS.UI.Extensions
{
    public static class StringExtensions
    {
        public static Size ToSize(this string sizeString)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Size));
            return (Size)converter.ConvertFromInvariantString(sizeString);
        }
    }
}
