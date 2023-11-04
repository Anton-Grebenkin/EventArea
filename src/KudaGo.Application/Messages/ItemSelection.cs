using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KudaGo.Application.Messages
{
    public record class ItemSelection<T>
    {
        public T Value { get; init; }
        public bool Selected { get; init; }
        public string SelectedString => Selected ? Emoji.Check : Emoji.Plus;
    }
}
