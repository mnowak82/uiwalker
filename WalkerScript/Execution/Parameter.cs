using System.Reflection;

namespace WalkerScript.Execution
{
    public class Parameter
    {
        private readonly ParameterInfo _param;

        public Parameter(ParameterInfo param)
        {
            _param = param;
        }

        public string Name { get { return _param.Name; } }
        public bool IsRequired { get { return !_param.HasDefaultValue; } }
    }
}