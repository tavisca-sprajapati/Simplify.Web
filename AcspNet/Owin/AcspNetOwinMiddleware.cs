﻿using System;
using System.Threading.Tasks;
using AcspNet.Bootstrapper;
using AcspNet.Diagnostics;
using AcspNet.Meta;
using AcspNet.Routing;
using DryIoc;
using Microsoft.Owin;

namespace AcspNet.Owin
{
	/// <summary>
	/// AcspNet engine root
	/// </summary>
	public class AcspNetOwinMiddleware : OwinMiddleware
	{
		private readonly Container _container = new Container();

		/// <summary>
		/// Initializes a new instance of the <see cref="AcspNetOwinMiddleware"/> class.
		/// </summary>
		/// <param name="next"></param>
		public AcspNetOwinMiddleware(OwinMiddleware next)
			: base(next)
		{
			var bs = BootstrapperFactory.CreateBootstrapper();

			// Registering all AcspNet pipeline types

			_container.Register(typeof(IControllerFactory), bs.ControllerFactoryType, Reuse.Singleton);
			_container.Register(typeof(IControllerMetaDataFactory), bs.ControllerMetaDataFactoryType, Reuse.Singleton);
			_container.Register(typeof(IControllersMetaStore), bs.ControllersMetaStoreType, Reuse.Singleton);
			_container.Register(typeof(IRouteMatcher), bs.RouteMatcherType, Reuse.Singleton);
			_container.Register(typeof(IControllersHandler), bs.ControllersHandlerType, Reuse.Singleton);
			_container.Register(typeof(IRequestHandler), bs.RequestHandlerType, Reuse.InResolutionScope);
		}

		/// <summary>
		/// Process an individual request.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override Task Invoke(IOwinContext context)
		{
			try
			{
				var request = _container.Resolve<IRequestHandler>();
				request.ProcessRequest(context);
			}
			catch (Exception e)
			{
				return context.Response.WriteAsync(ExceptionInfoPageGenerator.Generate(e));
			}

			return Task.Delay(0);
		}
	}
}