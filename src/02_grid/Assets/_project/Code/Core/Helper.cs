using System;
using UnityEditor;
using UnityEngine;

namespace Core
{
    public readonly record struct UndoGroup : IDisposable
    {
        private readonly int _groupId;

        public UndoGroup(string name)
        {
            Undo.SetCurrentGroupName(name);
            _groupId = Undo.GetCurrentGroup();
        }

        public void Dispose()
        {
            Undo.CollapseUndoOperations(_groupId);
        }
    }

    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject go)
            where T : Component
        {
            var x = go.GetComponent<T>();
            if (x != null)
            {
                return x;
            }
            #if UNITY_EDITOR
            x = Undo.AddComponent<T>(go);
            #else
            x = go.AddComponent<T>();
            #endif
            return x;
        }
    }
}

