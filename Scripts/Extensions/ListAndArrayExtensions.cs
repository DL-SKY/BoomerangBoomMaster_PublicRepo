using System.Collections.Generic;
using UnityEngine;

namespace DllSky.Extensions
{
    public static class ListAndArrayExtensions
    {
        public static void Shuffle<T>(this List<T> _list, int _count = 2)
        {
            for (int c = 0; c < _count; c++)
                for (int i = 0; i < _list.Count; i++)
                {
                    var item = _list[i];
                    _list.RemoveAt(i);

                    var newIndex = Random.Range(0, _list.Count+1);  
                    _list.Insert(newIndex, item);
                }
        }

        public static T GetRandom<T>(this T[] _array)
        {
            return _array[Random.Range(0, _array.Length)];
        }

        public static T GetRandom<T>(this List<T> _list)
        {
            return _list[Random.Range(0, _list.Count)];
        }
    }
}
