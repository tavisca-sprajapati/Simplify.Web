﻿using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace AcspNet.Core
{
	/// <summary>
	/// Provides static files request handler
	/// </summary>
	public class StaticFilesRequestHandler : IStaticFilesRequestHandler
	{
		private readonly IList<string> _staticFilesPaths;
		private readonly string _sitePhysicalPath;
		private static IFileSystem _fileSystemInstance;

		/// <summary>
		/// Initializes a new instance of the <see cref="StaticFilesRequestHandler"/> class.
		/// </summary>
		/// <param name="staticFilesPaths">The static files paths.</param>
		/// <param name="sitePhysicalPath">The environment.</param>
		public StaticFilesRequestHandler(IList<string> staticFilesPaths, string sitePhysicalPath)
		{
			_staticFilesPaths = staticFilesPaths;
			_sitePhysicalPath = sitePhysicalPath;
		}

		/// <summary>
		/// Gets or sets the file system.
		/// </summary>
		/// <value>
		/// The file system.
		/// </value>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static IFileSystem FileSystem
		{
			get { return _fileSystemInstance ?? (_fileSystemInstance = new FileSystem()); }

			set
			{
				if (value == null)
					throw new ArgumentNullException();

				_fileSystemInstance = value;
			}
		}

		/// <summary>
		/// Determines whether current route path is for static file.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public bool IsStaticFileRoutePath(IOwinContext context)
		{
			var currentPath = context.Request.Path.ToString().Substring(1);

			return _staticFilesPaths.Where(currentPath.StartsWith).Any(path => FileSystem.File.Exists(_sitePhysicalPath + currentPath));
		}

		/// <summary>
		/// Processes the HTTP request for static files.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public Task ProcessRequest(IOwinContext context)
		{
			return Task.Delay(0);
		}
	}
}