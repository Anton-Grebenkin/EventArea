using KudaGo.Application.Common.Messages;

namespace KudaGo.Application.Common.Attributes
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
