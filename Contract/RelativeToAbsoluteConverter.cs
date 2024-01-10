using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Contract
{
    public class RelativeToAbsoluteConverter
    {
        public static string Convert(string value)
        {
            string relative = (string)value;
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            string absolute = $"{folder}{relative}";
            return absolute;
        }

        public static string ConvertBack(string value)
        {
            throw new NotImplementedException();
        }
    }
}

