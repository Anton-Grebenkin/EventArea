
using KudaGo.Application.Abstractions;

namespace KudaGo.Application.Services
{
    public class RegisterService<Ttype, Tkey> : IRegisterService<Ttype, Tkey>
    {
        private readonly IDictionary<Tkey, Type> _types = new Dictionary<Tkey, Type>();

        public RegisterService(IEnumerable<Type> types, Func<Type, Tkey> getKey)
        {
            foreach (var type in types)
            {
                _types.Add(getKey(type), type);
            }
        }

        public IDictionary<Tkey, Type> Tpes { get { return _types; } }
    }
}
