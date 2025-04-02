using System;
using System.Collections.Generic;

namespace CityBuilderCore
{
    /// <summary>
    /// stores the types of <see cref="IUndoRedoAction"/> in the system so they can be deserialized
    /// </summary>
    public static class UndoRedoTypes
    {
        private static Dictionary<string, Type> _typeDict = new Dictionary<string, Type>();
        private static Dictionary<Type, string> _nameDict = new Dictionary<Type, string>();

        public static void Register(string name, Type type)
        {
            _typeDict.Add(name, type);
            _nameDict.Add(type, name);
        }

        public static Type GetType(string name) => _typeDict.GetValueOrDefault(name);
        public static string GetName(Type type) => _nameDict.GetValueOrDefault(type);
        public static string GetName(IUndoRedoAction action) => _nameDict.GetValueOrDefault(action.GetType());
    }
}
