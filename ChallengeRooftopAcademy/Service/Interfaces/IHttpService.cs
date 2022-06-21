using ChallengeRooftopAcademy.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeRooftopAcademy.Service.Interfaces
{
    public interface IHttpService: IDisposable
    {
        Task getToken(string email);
        Task<Blocks> getBlocks();
        Task<ResponseCheck> getCheck(ResponseCheck blocks);
    }
}
