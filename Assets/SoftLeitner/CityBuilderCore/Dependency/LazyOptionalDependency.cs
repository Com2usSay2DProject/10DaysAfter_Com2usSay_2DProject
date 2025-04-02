using System.Diagnostics;

namespace CityBuilderCore
{
    /// <summary>
    /// provides access to an optional dependency by resolving it at the moment it is needed and buffering it for further use
    /// </summary>
    /// <typeparam name="T">the type of the dependency</typeparam>
    public class LazyOptionalDependency<T>
    {
        private T _value;
        public T Value
        {
            [DebuggerStepThrough]
            get
            {
                resolve();

                return _value;
            }
        }

        private bool _hasValue;
        public bool HasValue
        {
            [DebuggerStepThrough]
            get
            {
                resolve();

                return _hasValue;
            }
        }

        private bool _resolved;

        private void resolve()
        {
            if (_resolved)
                return;

            if (Dependencies.TryGet<T>(out var value))
            {
                _value = value;
                _hasValue = true;
            }
            else
            {
                _hasValue = false;
            }

            _resolved = true;
        }
    }
}
