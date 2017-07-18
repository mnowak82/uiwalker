using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WalkerScript.Exceptions;

namespace WalkerScript.Execution
{
    public class Operation
    {
        private readonly object _operationSource;
        private readonly MethodInfo _method;

        public Operation(MethodInfo method, object operationSource)
        {
            _operationSource = operationSource;
            _method = method;
            Parameters = new List<Parameter>();
            foreach (var param in method.GetParameters())
            {
                Parameters.Add(new Parameter(param));
            }
        }

        public void Validate(Dictionary<string, string> parameters)
        {
            var checkParameters = new HashSet<string>(parameters.Keys);

            if (_method.ReturnType == typeof(void) && parameters.ContainsKey("set"))
                throw new InvalidScriptException("Method '{0}' doesn't return value - can't set variable '{1}'", Name, parameters["set"]);

            checkParameters.Remove("set");

            var missingRequired = new List<string>();
            foreach (var par in Parameters)
            {
                var exists = checkParameters.Remove(par.Name);
                if (!exists && par.IsRequired)
                    missingRequired.Add(par.Name);
                    
            }

            if (checkParameters.Count > 0 || missingRequired.Count > 0)
            {
                var unknown = string.Join(", ", checkParameters.Select(it => string.Format("'{0}'", it)));
                var required = string.Join(", ", missingRequired.Select(it => string.Format("'{0}'", it)));

                string format = "";
                if (unknown.Length > 0 && required.Length > 0)
                    format = "Missing required parameter/s {0}; unknown parameter/s {1} for method '{2}'";
                else if (required.Length > 0)
                    format = "Missing required parameter/s {0} for method '{2}'";
                else
                    format = "Unknown parameter/s {1} for method '{2}'";

                throw new InvalidScriptException(format, required, unknown, Name);
            }
        }

        public string Name { get { return _method.Name; } }
        public IList<Parameter> Parameters { get; private set; }

        public MethodInfo Method { get { return _method; } }

        public string Execute(Dictionary<string, string> attr)
        {
            Validate(attr);

            var args = new List<object>();

            foreach (var parameter in _method.GetParameters())
            {
                string value;
                if (attr.TryGetValue(parameter.Name, out value))
                {
                    if (parameter.ParameterType == typeof(string))
                        args.Add(value);
                    else if (parameter.ParameterType == typeof(int))
                        args.Add(int.Parse(value));
                    else if (parameter.ParameterType == typeof(TimeSpan))
                        args.Add(TimeSpan.Parse(value));
                    else if (parameter.ParameterType == typeof(DateTime))
                        args.Add(DateTime.Parse(value));
                    else
                        throw new InvalidScriptException("Unknown parameter type for parameter {0}, method {1}", parameter.Name, Name);
                }
                else
                {
                    args.Add(parameter.DefaultValue);
                }
            }

            try
            {
                var ret = _method.Invoke(_operationSource, args.ToArray());
                return ret as string;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }

        }
    }
}