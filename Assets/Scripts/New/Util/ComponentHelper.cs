using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Zumo {
    public static class ComponentHelper {
        public static T CopyValuesFrom<T> (this Component component, T other, string[] values) where T : Component {
            var type = component.GetType();
            if (type != other.GetType()) return null; // type mis-match

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            
            foreach (var pinfo in type.GetProperties(flags)) {
                if (pinfo.CanWrite && values.Contains(pinfo.Name)) {
                    try {
                        pinfo.SetValue(component, pinfo.GetValue(other, null), null);
                    } catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            
            foreach (var finfo in type.GetFields(flags)) {
                if (values.Contains(finfo.Name)) {
                    finfo.SetValue(component, finfo.GetValue(other));
                }
            }

            return component as T;
        }

        public static T CopyComponent<T> (this GameObject gameObject, T toAdd, string[] fieldsToCopy) where T : Component {
            return gameObject.AddComponent<T>().CopyValuesFrom(toAdd, fieldsToCopy) as T;
        }
    }
}
