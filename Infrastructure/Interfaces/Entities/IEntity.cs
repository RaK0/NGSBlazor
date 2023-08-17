using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.Entities
{
    public interface IEntity
    {
    }
    public abstract class AEntity<T>: IEntity
    {
        public T Id { get; set; }
    }
}
