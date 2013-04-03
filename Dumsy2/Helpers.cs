using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Dumsy2
{
    public class Helpers
    {
        public static object GetInfoFromMemory(String key)
        {
            PhoneApplicationService memory = PhoneApplicationService.Current;
            object obj;

            if (memory.State.TryGetValue(key, out obj))
            {
                return obj;
            }
            else
            {
                obj = GetInfoFromStorage(key);
                if (obj != null)
                    memory.State[key] = obj;
                return obj;
            }
        }

        private static void SetValueToMemory(String key, object Value)
        {
            PhoneApplicationService memory = PhoneApplicationService.Current;
            memory.State[key] = Value;
        }

        public static void SetValueToStorage(String key, object Value)
        {
            IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;
            storage[key] = Value;
            
            SetValueToMemory(key, Value);
        }

        private static object GetInfoFromStorage(String key)
        {
            IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;
            object name;
            if (storage.TryGetValue(key, out name))
            {
                return name;
            }
            return null;
        }
    }

}
