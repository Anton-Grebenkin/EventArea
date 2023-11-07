namespace KudaGo.Application.Common.Abstractions
{
    public interface IRegisterService<Tkey, Ttype>
    {
        public IDictionary<Tkey, Type> Tpes { get; }
    }
}
