using System;
using System.Collections.Generic;
using System.Text;

namespace ChallengeRooftopAcademy.Service.Interfaces
{
    public interface IServiceCache
    {
        void set(object key, object value);
        T get<T>(object key);
    }
}
