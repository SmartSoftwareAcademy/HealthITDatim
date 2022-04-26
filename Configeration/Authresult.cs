using System.Collections.Generic;

namespace HealthITDatim.Configeration
{
    public class Authresult
    {
        public string Token { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
}
