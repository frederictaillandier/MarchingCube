using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;


/*
   * Cube indexes
      6 -- 7       y  z
     4 -- 5|       | /
     |    |3       0--x
     0 -- 1
*/

class OctoNode<T>
{
    Vector3 _position;
    T _value;
    uint _size;
    List<OctoNode<T>> _children = null;

    public OctoNode(T value, uint size, Vector3 position)
    {
        _value = value;
        _size = size;
        _position = position;
        _children = null;
    }

    public uint Size
    {
        get
        {
            return _size;
        }
    }

    public Vector3 Position
    {
        get
        {
            return _position;
        }

    }

    public List<OctoNode<T>> Children
    {
        get
        {
            return _children;
        }
    }


    public T GetValue(Vector3 v)
    {
        if (_children == null)
            return _value;
        else
        {
            return _children[GetIndex(v)].GetValue(v);
        }
    }

    int GetIndex(Vector3 v)
    {
        int index = 0;

        index |= (v.x >= _position.x + _size / 2) ? 1 : 0;
        index |= (v.y >= _position.y + _size / 2) ? 4 : 0;
        index |= (v.z >= _position.z + _size / 2) ? 2 : 0;
        return index;
    }

    public void SetValue(Vector3 v, T value)
    {
        if (_size == 1 && !_value.Equals(value))
        {
            _value = value;
        }
        else if (_children == null && _value.Equals(value))
            return;
        else
        {
            if (_children == null)
            {
                _children = new List<OctoNode<T>>();
                for (int i = 0; i < 8; ++i)
                {
                    _children.Add(new OctoNode<T>(_value, _size / 2, _position + _size / 2 * new Vector3(i % 2, Mathf.Floor(i / 4), Mathf.Floor(i / 2) % 2)));
                }
            }
            var index = GetIndex(v);
            _children[index].SetValue(v, value);
            Clean(value);
        }
    }


    //Clean children if they are all having the same value
    public void Clean(T value)
    {
        if (_children != null)
        {
            for (int i = 0; i < 8; ++i)
            {
                if (_children[i]._children != null || !_children[i]._value.Equals(value))
                    return;
            }
            _value = value;
            _children = null;
        }
    }
}

class OctoTree<T>
{
    OctoNode<T> _root = null;

    public OctoTree(T value, uint size)
    {
        _root = new OctoNode<T>(value, size, Vector3.zero);
    }

    public T GetValue(Vector3 v)
    {
        if (v.x < 0)
            v.x = 0;
        else if (v.x >= _root.Size)
            v.x = _root.Size;

        if (v.y < 0)
            v.y = 0;
        else if (v.y >= _root.Size)
            v.y = _root.Size;

        if (v.z < 0)
            v.z = 0;
        else if (v.z >= _root.Size)
            v.z = _root.Size;
        return _root.GetValue(v);
    }

    public void SetValue(Vector3 v, T value)
    {
        _root.SetValue(v, value);
    }

    public OctoNode<T> Root
    {
        get
        {
            return _root;
        }
    }

    /* Use to debug
    public void ShowCubes(T value)
    {
        _root.Show(value);
    }
    */

}