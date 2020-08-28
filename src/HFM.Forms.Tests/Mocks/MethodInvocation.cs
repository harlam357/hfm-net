using System;
using System.Diagnostics;
using System.Linq;

namespace HFM.Forms.Mocks
{
    [DebuggerDisplay("{Name} {ArgumentsString}")]
    public class MethodInvocation
    {
        public string Name { get; }
        public object[] Arguments { get; }
        public string ArgumentsString => Arguments is null ? null : String.Join(", ", Arguments.Where(x => x != null));

        public MethodInvocation(string name, params object[] arguments)
        {
            Name = name;
            Arguments = arguments;
        }
    }

    [DebuggerDisplay("{ReturnValue} {Name} {ArgumentsString}")]
    public class MethodInvocationWithReturnValue : MethodInvocation
    {
        public object ReturnValue { get; }

        public MethodInvocationWithReturnValue(object returnValue, string name, params object[] arguments) : base(name, arguments)
        {
            ReturnValue = returnValue;
        }
    }
}
