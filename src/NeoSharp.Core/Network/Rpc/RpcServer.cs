using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NeoSharp.Core.Blockchain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network.Rpc
{
    public class RpcServer : IRpcServer
    {
        #region Variables

        private const int MaxPostValue = 1024 * 1024 * 2;

        private IWebHost _host;
        private readonly INetworkACL _acl;
        private readonly RpcConfig _config;
        private readonly IBlockchain _blockchain;
        private readonly ILogger<RpcServer> _logger;
        private readonly IList<IRpcProcessRequest> _callbacks;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="blockchain">Blockchain</param>
        /// <param name="logger">Logger</param>
        /// <param name="aclFactory">ACL</param>
        public RpcServer(
            RpcConfig config,
            IBlockchain blockchain, ILogger<RpcServer> logger,
            NetworkACLFactory aclFactory)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (aclFactory == null) throw new ArgumentNullException(nameof(aclFactory));

            _acl = aclFactory.CreateNew();
            _acl?.Load(config?.ACL);
            _callbacks = new List<IRpcProcessRequest>();
        }

        private static JObject CreateErrorResponse(string id, int code, string message, object error = null)
        {
            JObject response = CreateResponse(id);
            response["error"] = new JObject
            {
                ["code"] = code,
                ["message"] = message
            };

            if (error != null)
                response["error"]["data"] = new JObject(error);

            return response;
        }

        private static JObject CreateResponse(string id)
        {
            JObject response = new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id
            };
            return response;
        }

        private JObject ProcessRequest(RpcRequest request)
        {
            JObject result = null;

            try
            {
                foreach (IRpcProcessRequest req in _callbacks)
                {
                    object ret = req.Process(request);
                    if (ret != null)
                    {
                        result = JObject.FromObject(ret);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                return CreateErrorResponse(request.Id, ex.HResult, ex.Message, ex.StackTrace);
#else
                return CreateErrorResponse(request.Id, ex.HResult, ex.Message);
#endif
            }

            JObject response = CreateResponse(request.Id);
            response["result"] = result;
            return response;
        }

        private async Task ProcessAsync(HttpContext context)
        {
            if(_acl != null && !_acl.IsAllowed(context.Connection.RemoteIpAddress))
            {
                _logger?.LogWarning("Unauthorized request" + context.Connection.RemoteIpAddress.ToString());

                context.Response.StatusCode = 401;
                var unathorized_response = CreateErrorResponse(null, 401, "Forbidden");
                context.Response.ContentType = "application/json-rpc";
                await context.Response.WriteAsync(unathorized_response.ToString(), Encoding.UTF8);

                return;
            }

            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST";
            context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
            context.Response.Headers["Access-Control-Max-Age"] = "31536000";

            RpcRequest request = new RpcRequest();

            try
            {
                // Allow only GET and POST

                if (context.Request.Method == "GET")
                {
                    request.JsonRpc = context.Request.Query["jsonrpc"];
                    request.Id = context.Request.Query["id"];
                    request.Method = context.Request.Query["method"];
                    request.SetParams(context.Request.Query["params"]);
                }
                else if (context.Request.Method == "POST")
                {
                    string post;

                    using (StreamReader reader = new StreamReader(context.Request.Body))
                    {
                        if (!context.Request.ContentLength.HasValue ||
                            context.Request.ContentLength > MaxPostValue)
                            throw (new ArgumentOutOfRangeException("body"));

                        post = reader.ReadToEnd();
                    }

                    JObject ret = JObject.Parse(post);

                    request.JsonRpc = ret["jsonrpc"].Value<string>();
                    request.Id = ret["id"].Value<string>();
                    request.Method = ret["method"].Value<string>();
                    request.SetParams(ret["params"].ToString());
                }
                else return;
            }
            catch { }

            JToken response;
            if (!request.IsValid)
            {
                response = CreateErrorResponse(null, -32700, "Parse error");
            }
            else
            {
                response = ProcessRequest(request);
            }

            if (response == null || (response as JArray)?.Count == 0) return;

            context.Response.ContentType = "application/json-rpc";
            await context.Response.WriteAsync(response.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Start server
        /// </summary>
        public void Start()
        {
            // Check started

            if (_host != null)
            {
                _logger?.LogInformation("RPC server already started");
                return;
            }

            _host = new WebHostBuilder().UseKestrel(options => options.Listen(_config.ListenEndPoint, listenOptions =>
            {
                // Config SSL

                if (_config.SSL != null && _config.SSL.IsValid)
                    listenOptions.UseHttps(_config.SSL.Path, _config.SSL.Password);
            }))
            .Configure(app =>
            {
                app.UseResponseCompression();
                app.Run(ProcessAsync);
            })
            .ConfigureServices(services =>
            {
                services.AddResponseCompression(options =>
                {
                    // options.EnableForHttps = false;
                    options.Providers.Add<GzipCompressionProvider>();
                    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json-rpc" });
                });

                services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Fastest;
                });
            })
            .Build();

            _host.Start();
            _logger?.LogInformation("RPC server started");
        }

        /// <summary>
        /// Stop server
        /// </summary>
        public void Stop()
        {
            if (_host == null)
            {
                _logger?.LogInformation("RPC server already stopped");
                return;
            }

            if (_host != null)
            {
                _host.Dispose();
                _host = null;
            }

            _logger?.LogInformation("RPC server stopped");
        }

        /// <summary>
        /// Free resources
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }
}