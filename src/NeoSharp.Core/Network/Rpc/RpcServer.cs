using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using NeoSharp.Core.Blockchain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network.Security;

namespace NeoSharp.Core.Network.Rpc
{
    public class RpcServer : IRpcServer
    {
        #region Variables

        private const int MaxPostValue = 1024 * 1024 * 2;

        private IWebHost _host;
        private readonly NetworkAcl _acl;
        private readonly RpcConfig _config;
        private readonly IBlockchain _blockchain;
        private readonly ILogger<RpcServer> _logger;
        private readonly IList<IRpcProcessRequest> _callbacks = new List<IRpcProcessRequest>();

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="blockchain">Blockchain</param>
        /// <param name="logger">Logger</param>
        /// <param name="aclLoader">ACL Loader</param>
        public RpcServer(
            RpcConfig config,
            IBlockchain blockchain,
            ILogger<RpcServer> logger,
            INetworkAclLoader aclLoader)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (aclLoader == null) throw new ArgumentNullException(nameof(aclLoader));
            _acl = aclLoader.Load(config.AclConfig) ?? NetworkAcl.Default;
        }

        private static JObject CreateErrorResponse(string id, int code, string message, object error = null)
        {
            var response = CreateResponse(id);
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
            var response = new JObject
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
                foreach (var req in _callbacks)
                {
                    var ret = req.Process(request);
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

            var response = CreateResponse(request.Id);
            response["result"] = result;
            return response;
        }

        private async Task ProcessAsync(HttpContext context)
        {
            if(_acl.IsAllowed(context.Connection.RemoteIpAddress) == false)
            {
                _logger?.LogWarning("Unauthorized request " + context.Connection.RemoteIpAddress);

                context.Response.StatusCode = 401;
                var unathorizedResponse = CreateErrorResponse(null, 401, "Forbidden");
                context.Response.ContentType = "application/json-rpc";
                await context.Response.WriteAsync(unathorizedResponse.ToString(), Encoding.UTF8);

                return;
            }

            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST";
            context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
            context.Response.Headers["Access-Control-Max-Age"] = "31536000";

            var request = new RpcRequest();

            try
            {
                // Allow only GET and POST

                if (HttpMethods.Get.Equals(context.Request.Method, StringComparison.OrdinalIgnoreCase))
                {
                    request.JsonRpc = context.Request.Query["jsonrpc"];
                    request.Id = context.Request.Query["id"];
                    request.Method = context.Request.Query["method"];
                    request.SetParams(context.Request.Query["params"]);
                }
                else if (HttpMethods.Post.Equals(context.Request.Method, StringComparison.OrdinalIgnoreCase))
                {
                    string post;

                    using (var reader = new StreamReader(context.Request.Body))
                    {
                        if (!context.Request.ContentLength.HasValue ||
                            context.Request.ContentLength > MaxPostValue)
                            throw (new ArgumentOutOfRangeException("body"));

                        post = reader.ReadToEnd();
                    }

                    var ret = JObject.Parse(post);

                    request.JsonRpc = ret["jsonrpc"].Value<string>();
                    request.Id = ret["id"].Value<string>();
                    request.Method = ret["method"].Value<string>();
                    request.SetParams(ret["params"].ToString());
                }
                else return;
            }
            catch { }

            JToken response;
            response = !request.IsValid ? CreateErrorResponse(null, -32700, "Parse error") : ProcessRequest(request);

            if (response == null || (response as JArray)?.Count == 0) return;

            context.Response.ContentType = "application/json-rpc";
            await context.Response.WriteAsync(response.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Starts the server
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

                if (_config.Ssl != null && _config.Ssl.IsValid)
                    listenOptions.UseHttps(_config.Ssl.Path, _config.Ssl.Password);
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
        /// Stops the server
        /// </summary>
        public  void Stop()
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

        /// <summary>
        /// Register an operation that can be called by the server.
        /// Usage:
        /// server.BindOperation("controllerName", "operationName", new Func<int, bool>(MyMethod));
        /// </summary>
        /// <param name="controllerName">controller name is used to organize many operations in a group</param>
        /// <param name="operationName">operation name</param>
        /// <param name="anyMethod">the method to be called when the operation is called</param>
        public void BindOperation(string controllerName, string operationName, Delegate anyMethod)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Register many operations organized in a controller class,
        /// The operations should be methods annotated with [RpcOperation] or [RpcOperation("operationName")]
        /// </summary>
        /// <param name="controller">the controller class</param>
        public void BindController(Type controller)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Calls the server operation
        /// </summary>
        /// <param name="controllerName">the controller name</param>
        /// <param name="operationName">the operation name</param>
        /// <param name="parameters">all parameters expected by the operation</param>
        /// <typeparam name="T">the return type</typeparam>
        /// <returns>the return of the operation</returns>
        public T CallOperation<T>(string controllerName, string operationName, params object[] parameters)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Calls the server operation
        /// </summary>
        /// <param name="controllerName">the controller name</param>
        /// <param name="operationName">the operation name</param>
        /// <param name="parameters">all parameters expected by the operation</param>
        /// <returns>the return of the operation, not casted</returns>
        public object CallOperation(string controllerName, string operationName, params object[] parameters)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// removes a previous registered operation from the server
        /// </summary>
        /// <param name="controllerName">the controller name</param>
        /// <param name="operationName">the operation name</param>
        public void UnbindOperation(string controllerName, string operationName)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// removes all operations of a previous registered controller and registered operation with the informed
        /// controller name
        /// </summary>
        /// <param name="controllerName">the controller name</param>
        public void UnbindController(string controllerName)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// removes all registered operations and controllers
        /// </summary>
        public void UnbindAllOperations()
        {
            throw new NotImplementedException();
        }
    }
}