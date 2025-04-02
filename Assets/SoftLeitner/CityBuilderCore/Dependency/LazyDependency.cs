using System.Diagnostics;

namespace CityBuilderCore
{
    /// <summary>
    /// provides access to a dependency by resolving it at the moment it is needed and buffering it for further use
    /// </summary>
    /// <typeparam name="T">the type of the dependency</typeparam>
    public class LazyDependency<T>
    {
        private T _value;
        public T Value
        {
            [DebuggerStepThrough]
            get
            {
                if (_value == null)
                    _value = Dependencies.Get<T>();
                return _value;
            }
        }
    }
}
