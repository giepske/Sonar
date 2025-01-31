﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Sonar.Modules.Implementations
{
    public class HttpsValidationModule : WebServerModule
    {
        private readonly string _host;
        private X509Certificate2 _certificate;
        private readonly List<string> _certificateResults = new List<string>();

        public override string Name { get; set; } = "HTTPS Validation";

        public HttpsValidationModule(string host) : base(host)
        {
            _host = host;
        }

        public override Task<ModuleResult> Execute(Data data)
        {
            if (!CheckValidHost())
                return Task.FromResult(ModuleResult.Create(this, ResultType.Error, "This is not a valid host."));

            var successful = GetCertificate();

            if (_certificate == null) return Task.FromResult(ModuleResult.Create(this, ResultType.Error, "The host does not have a valid SSL Certificate."));
            if (!successful) return Task.FromResult(ModuleResult.Create(this, ResultType.Error, "The host does not have a valid SSL Certificate."));

            _certificateResults.Add("Valid from: " + _certificate.GetEffectiveDateString());
            _certificateResults.Add("Valid till: " + _certificate.GetExpirationDateString());
            _certificateResults.Add("Signature algorithm: " + _certificate.SignatureAlgorithm.FriendlyName);

            var result = _certificateResults.Aggregate(_host + " has a valid SSL Certificate.", (current, cResult) => current + Environment.NewLine + cResult);

            return Task.FromResult(ModuleResult.Create(this, ResultType.Success, result));
        }

        private bool CheckValidHost()
        {
            return _host.StartsWith("https://") || _host.StartsWith("http://");
        }

        private bool GetCertificate()
        {
            var request = WebRequest.CreateHttp(_host);
            request.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
            try
            {
                request.GetResponse();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            _certificate = new X509Certificate2(certificate);
            if (sslpolicyerrors == SslPolicyErrors.RemoteCertificateChainErrors) return false;
            return sslpolicyerrors == SslPolicyErrors.None;
        }
    }
}
