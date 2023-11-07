
namespace KudaGo.Application.Common.Messages
{
    public record class ItemSelection<T>
    {
        public T Value { get; init; }
        public bool Selected { get; init; }
        public string SelectedString => Selected ? Emoji.Check : Emoji.Plus;
    }
}
