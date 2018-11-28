using System;
using System.Windows.Forms;

namespace ThreePM
{
    public static class Registry
    {
        public static bool GetValue(string keyName, bool defaultValue)
        {
            return Convert.ToBoolean(GetValue(keyName, defaultValue.ToString()));
        }

        public static int GetValue(string keyName, int defaultValue)
        {
            return Convert.ToInt32(GetValue(keyName, defaultValue.ToString()));
        }

        public static string GetValue(string keyName, string defaultValue)
        {
            object o = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + Application.CompanyName + @"\" + Application.ProductName, keyName, defaultValue);
            if (o == null)
            {
                SetValue(keyName, defaultValue);
                return defaultValue;
            }
            else
            {
                return o.ToString();
            }
        }

        public static void SetValue(string keyName, object value)
        {
            Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + Application.CompanyName + @"\" + Application.ProductName, keyName, value);
        }
    }
}
