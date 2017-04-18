using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class MyX509Validator : X509CertificateValidator
    {

        /// <summary>

        /// Validates a certificate.

        /// </summary>

        /// <param name="certificate">The certificate the validate.</param>

        public override void Validate(X509Certificate2 certificate)
        {

            // validate argument

            if (certificate == null)

                throw new ArgumentNullException("X509认证证书为空！");



            // check if the name of the certifcate matches

           // if (certificate.SubjectName.Name != ConfigurationManager.AppSettings["CertName"])

             ///   throw new SecurityTokenValidationException("Certificated was not issued by thrusted issuer");

        }

    }
}
