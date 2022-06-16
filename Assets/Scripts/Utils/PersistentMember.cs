using UnityEngine;
using System.Collections;

namespace Kayfo
{
    abstract public class PersistentMember<T>
    {
        protected T m_defaultValue;
        protected T m_value;
        protected string m_key;
        protected bool loadedOnce = false;

        public PersistentMember(string _key, T _defaultValue)
        {
            m_key = _key;
            m_defaultValue = _defaultValue;
        }

        public T Get()
        {
            if (loadedOnce)
                return m_value;

            if (PlayerPrefs.HasKey(m_key))
            {
                m_value = GetInternal();
            }
            else
            {
                m_value = m_defaultValue;
            }

            loadedOnce = true;
            return m_value;
        }

        public void Set(T _value)
        {
            m_value = _value;
            SetInternal(_value);
        }

        abstract protected T GetInternal();
        abstract protected void SetInternal(T _value);
    }

    public class PersistentBool : PersistentMember<bool>
    {
        public PersistentBool(string _key, bool _default) : base(_key, _default)
        {
        }

        protected override bool GetInternal()
        {
            return PlayerPrefs.GetInt(m_key) == 1;
        }

        protected override void SetInternal(bool _value)
        {
            PlayerPrefs.SetInt(m_key, _value ? 1 : 0);
        }
    }

    public class PersistentInt : PersistentMember<int>
    {
        public PersistentInt(string _key, int _default) : base(_key, _default)
        {
        }

        protected override int GetInternal()
        {
            return PlayerPrefs.GetInt(m_key);
        }

        protected override void SetInternal(int _value)
        {
            PlayerPrefs.SetInt(m_key, (int)_value);
        }
    }

    public class PersistentFloat : PersistentMember<float>
    {
        public PersistentFloat(string _key, float _default) : base(_key, _default)
        {
        }

        protected override float GetInternal()
        {
            return PlayerPrefs.GetFloat(m_key);
        }

        protected override void SetInternal(float _value)
        {
            PlayerPrefs.SetFloat(m_key, _value);
        }
    }

    public class PersistentString : PersistentMember<string>
    {
        public PersistentString(string _key, string _default) : base(_key, _default)
        {
        }

        protected override string GetInternal()
        {
            return PlayerPrefs.GetString(m_key);
        }

        protected override void SetInternal(string _value)
        {
            PlayerPrefs.SetString(m_key, _value);
        }
    }
}