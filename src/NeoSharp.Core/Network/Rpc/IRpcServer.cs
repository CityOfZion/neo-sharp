using System;
using System.Reflection;

namespace NeoSharp.Core.Network.Rpc
{
    public interface IRpcServer
    {   
        /// <summary>
        /// Starts the server
        /// </summary>
        void Start();
        
        /// <summary>
        /// Stops the server
        /// </summary>
        void Stop();
        
        /// <summary>
        /// Register an operation that can be called by the server,
        /// eg.:
        /// server.BindOperation("controllerName", "operationName", new Func<int, bool>(MyMethod));
        /// </summary>
        /// <param name="controllerName">controller name is used to organize many operations in a group</param>
        /// <param name="operationName">operation name</param>
        /// <param name="anyMethod">the method to be called when the operation is called</param>
        void BindOperation(string controllerName, string operationName, Delegate anyMethod);
        
        /// <summary>
        /// Register many operations organized in a controller class,
        /// The operations should be methods annotated with [RpcOperation] or [RpcOperation("operationName")]
        /// </summary>
        /// <param name="controller">the controller class</param>
        void BindController(Type controller);
        
        /// <summary>
        /// Calls the server operation
        /// </summary>
        /// <param name="controllerName">the controller name</param>
        /// <param name="operationName">the operation name</param>
        /// <param name="parameters">all parameters expected by the operation</param>
        /// <typeparam name="T">the return type</typeparam>
        /// <returns>the return of the operation</returns>
        T CallOperation<T>(string controllerName, string operationName, params object[] parameters);
        
        /// <summary>
        /// Calls the server operation
        /// </summary>
        /// <param name="controllerName">the controller name</param>
        /// <param name="operationName">the operation name</param>
        /// <param name="parameters">all parameters expected by the operation</param>
        /// <returns>the return of the operation, not casted</returns>
        object CallOperation(string controllerName, string operationName, params object[] parameters);
        
        /// <summary>
        /// removes a previous registered operation from the server
        /// </summary>
        /// <param name="controllerName">the controller name</param>
        /// <param name="operationName">the operation name</param>
        void UnbindOperation(string controllerName, string operationName);
        
        /// <summary>
        /// removes all operations of a previous registered controller and registered operation with the informed
        /// controller name
        /// </summary>
        /// <param name="controllerName">the controller name</param>
        void UnbindController(string controllerName);
        
        /// <summary>
        /// removes all registered operations and controllers
        /// </summary>
        void UnbindAllOperations();
    }
}