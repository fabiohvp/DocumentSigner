using DocumentSigner.Helpers;
using DocumentSignerApi.Models;
using DocumentSignerApi.Security;
using DocumentSignerApi.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace DocumentSigner.Controllers
{
    public class ClientController : ApiController
    {
        private ITokenService TokenService;

        public ClientController()
        {
            TokenService = new TokenService(ConfigurationManager.AppSettings[SettingsHelper.USER_KEY_STORE_NAME]);
        }

        [HttpGet]
        public string GetVersion()
        {
            return Assembly
                .GetEntryAssembly()
                .GetName()
                .Version
                .ToString();
        }

        [HttpGet]
        public HttpResponseMessage ReadCertificates(bool filtroCertificadosICPBrasil = true)
        {
            var filtro = default(string);

            if (filtroCertificadosICPBrasil)
            {
                filtro = "ICP-Brasil";
            }

            var certificates = TokenService
                .ReadCertificates(filtro);

            return Request.CreateResponse(HttpStatusCode.OK, certificates);
        }

        [HttpGet]
        public HttpResponseMessage SignHashs
        (
            [FromUri]SignRequest[] signRequests,
            string thumbprint,
            string hashAlgorithm = HashAlgorithms.SHA1
        )
        {
            var model = new SignHashsViewModel
            {
                SignRequests = signRequests,
                HashAlgorithm = hashAlgorithm,
                Thumbprint = thumbprint
            };

            return SignHashsPOST(model);
        }

        [HttpPost]
        public HttpResponseMessage SignHashsPOST(SignHashsViewModel model)
        {
            var results = new ConcurrentBag<SignResponse>();

            model
                .SignRequests
                .AsParallel()
                .ForAll(authenticatedAttributeBase64 =>
                {
                    var signResponse = SignHash(authenticatedAttributeBase64, model.HashAlgorithm, model.Thumbprint);

                    results.Add(signResponse);
                });

            return Request.CreateResponse(HttpStatusCode.OK, results.ToArray());
        }

        private SignResponse SignHash(SignRequest signRequest, string hashAlgorithm, string thumbprint)
        {
            try
            {
                var authenticatedAttribute = Convert.FromBase64String(signRequest.AuthenticatedAttributeBase64);
                var signedHash = TokenService.SignHash(thumbprint, hashAlgorithm, authenticatedAttribute);
                var signedHashBase64 = Convert.ToBase64String(signedHash);

                return new SignResponse
                {
                    FileId = signRequest.FileId,
                    SignedHashBase64 = signedHashBase64
                };
            }
            catch (Exception ex)
            {
                return new SignResponse
                {
                    FileId = signRequest.FileId,
                    Exception = ex
                };
            }
        }

        public class SignHashsViewModel
        {
            private string _HashAlgorithm;
            private string _SignatureAlgorithm;

            public string HashAlgorithm
            {
                get
                {
                    if (string.IsNullOrEmpty(_HashAlgorithm))
                    {
                        _HashAlgorithm = HashAlgorithms.SHA1;
                    }

                    return _HashAlgorithm;
                }
                set
                {
                    _HashAlgorithm = value;
                }
            }

            public string SignatureAlgorithm
            {
                get
                {
                    if (string.IsNullOrEmpty(_SignatureAlgorithm))
                    {
                        _SignatureAlgorithm = CryptographicAlgorithms.RSA;
                    }

                    return _SignatureAlgorithm;
                }
                set
                {
                    _SignatureAlgorithm = value;
                }
            }

            public string Thumbprint { get; set; }

            public IEnumerable<SignRequest> SignRequests { get; set; }
        }

        public class SignRequest
        {
            public string FileId { get; set; }
            public string AuthenticatedAttributeBase64 { get; set; }
        }

        public class SignResponse
        {
            public string FileId { get; set; }
            public string SignedHashBase64 { get; set; }
            public Exception Exception { get; set; }
        }
    }
}
