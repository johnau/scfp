using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceplateGeneratorCore.Utility
{
    internal class EnumHelper
    {
        /// <summary>
        /// Give a property a string value
        /// </summary>
        public class StringValueAttribute : Attribute
        {
            public string Value { get; }

            public StringValueAttribute(string value)
            {
                Value = value;
            }
        }

        /// <summary>
        /// Give a property a string[] value
        /// </summary>
        public class StringArrayValueAttribute : Attribute
        {
            public string[] Value { get; }

            public StringArrayValueAttribute(string[] value)
            {
                Value = value;
            }
        }
    }
}
