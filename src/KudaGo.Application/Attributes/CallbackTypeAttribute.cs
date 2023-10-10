using KudaGo.Application.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KudaGo.Application.Attributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CallbackTypeAttribute : Attribute
    {
        public CallbackType CallbackType { get; }
        public CallbackTypeAttribute(CallbackType callbackType)
        {
            CallbackType = callbackType;
        }
    }
}
