using ChallengeRooftopAcademy.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeRooftopAcademy.Service.Interfaces
{
    public interface IServiceValidateOrderBlock
    {
        Task<List<string>> check(List<string> blocks, string token);
        Task<List<string>> executeCheck(string email);
    }
}
