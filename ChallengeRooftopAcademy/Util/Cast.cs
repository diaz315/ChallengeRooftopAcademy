using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChallengeRooftopAcademy.Util
{
    public static class Cast
    {
        public static T ToCast<T>(this object strJSON)
        {
            strJSON = strJSON ?? "";
            return JsonConvert.DeserializeObject<T>(strJSON?.ToString());
        }
    }
}
