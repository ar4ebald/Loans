using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Loans.Options
{
    public class JwtSettings
    {
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string IssuerSigningKey { get; set; }

        public SymmetricSecurityKey SigningKey
        {
            get { return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(IssuerSigningKey)); }
        }
    }
}
