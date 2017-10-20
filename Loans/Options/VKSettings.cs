using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Loans.Options
{
    // ReSharper disable once InconsistentNaming
    public class VKSettings
    {
        public string ClientId { get; set; }

        public string RedirectUri { get; set; }

        public string Display { get; set; }

        public string Scope { get; set; }

        public string Version { get; set; }

        public string SecureKey { get; set; }
    }
}
