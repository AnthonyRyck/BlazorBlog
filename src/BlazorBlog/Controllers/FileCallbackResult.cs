using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BlazorBlog.Controllers
{
	public class FileCallbackResult : FileResult
    {
        private Func<Stream, ActionContext, Task> _callback;

        /// <summary>
        /// Creates a new <see cref="FileCallbackResult"/> instance.
        /// </summary>
        /// <param name="contentType">The Content-Type header of the response.</param>
        /// <param name="callback">The stream with the file.</param>
        public FileCallbackResult(string contentType, Func<Stream, ActionContext, Task> callback)
            : this(MediaTypeHeaderValue.Parse(contentType), callback)
        {
        }

        /// <summary>
        /// Creates a new <see cref="FileCallbackResult"/> instance.
        /// </summary>
        /// <param name="contentType">The Content-Type header of the response.</param>
        /// <param name="callback">The stream with the file.</param>
        public FileCallbackResult(MediaTypeHeaderValue contentType, Func<Stream, ActionContext, Task> callback)
            : base(contentType?.ToString())
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            Callback = callback;
        }

        /// <summary>
        /// Gets or sets the callback responsible for writing the file content to the output stream.
        /// </summary>
        public Func<Stream, ActionContext, Task> Callback
        {
            get
            {
                return _callback;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _callback = value;
            }
        }

        /// <inheritdoc />
        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = new FileCallbackResultExecutor(context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>());
            return executor.ExecuteAsync(context, this);
        }

        private sealed class FileCallbackResultExecutor : FileResultExecutorBase
        {
            public FileCallbackResultExecutor(ILoggerFactory loggerFactory)
                : base(CreateLogger<FileCallbackResultExecutor>(loggerFactory))
            {
            }

            public Task ExecuteAsync(ActionContext context, FileCallbackResult result)
            {
                SetHeadersAndLog(context, result, null, true);
                return result.Callback(context.HttpContext.Response.Body, context);
            }
        }
    }
}
