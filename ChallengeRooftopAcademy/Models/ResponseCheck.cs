using System;
using System.Collections.Generic;
using System.Text;

namespace ChallengeRooftopAcademy.Models
{
    public class ResponseCheck
    {
        public bool message { get; set; } = false;
        public List<string> blocks { get; set; } = new List<string>();
        public string encoded { get; set; } = string.Empty;
        public string merged { get; set; } = string.Empty;
    }
}
