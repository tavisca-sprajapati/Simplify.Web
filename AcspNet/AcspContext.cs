﻿using System;
using System.Collections.Specialized;
using System.Web;
using AcspNet.Web;

namespace AcspNet
{
	/// <summary>
	/// Contains HTTP and ACSP context information
	/// </summary>
	public class AcspContext : IAcspContext
	{
		/// <summary>
		/// The is new session field name
		/// </summary>
		public const string IsNewSessionFieldName = "AcspIsNewSession";

		/// <summary>
		/// Initializes a new instance of the <see cref="AcspContext" /> class.
		/// </summary>
		/// <param name="httpContext">The HTTP context.</param>
		internal AcspContext(HttpContextBase httpContext)
		{
			HttpContext = httpContext;
			Request = HttpContext.Request;
			Response = HttpContext.Response;
			Session = httpContext.Session;
			QueryString = Request.QueryString;
			Form = Request.Form;

			CalculateSitePhysicalPath();
			CalculateSiteVirualPath();
			CalculateSiteUrl();
			CalculateRequestParameters();

			if (Session == null || Session[IsNewSessionFieldName] != null) return;

			Session.Add(IsNewSessionFieldName, "false");
			IsNewSession = true;
		}

		/// <summary>
		/// Indicating whether session was created with the current request
		/// </summary>
		public bool IsNewSession { get; private set; }

		/// <summary>
		///  Gets the <see cref="T:System.Web.HttpContextBase"/> object for the current HTTP request.
		/// </summary>
		public HttpContextBase HttpContext { get; private set; }

		/// <summary>
		/// Gets the System.Web.HttpRequest object for the current HTTP request
		/// </summary>
		public HttpRequestBase Request { get; private set; }

		/// <summary>
		/// Gets the System.Web.HttpResponse object for the current HTTP response
		/// </summary>
		public HttpResponseBase Response { get; private set; }

		/// <summary>
		/// Gets the System.Web.HttpSessionState object for the current HTTP request
		/// </summary>
		public HttpSessionStateBase Session { get; private set; }

		/// <summary>
		/// Gets the connection of  HTTP query string variables
		/// </summary>
		public NameValueCollection QueryString { get; private set; }

		/// <summary>
		/// Gets the connection of HTTP post request form variables
		/// </summary>
		public NameValueCollection Form { get; private set; }

		/// <summary>
		/// Gets the current web-site request action parameter (/someAction or ?act=someAction).
		/// </summary>
		/// <value>
		/// The current action (?act=someAction or yourSite/someAction).
		/// </value>
		public string CurrentAction { get; private set; }

		/// <summary>
		/// Gets the current web-site mode request parameter (/someAction/someMode/SomeID or ?act=someAction&amp;mode=somMode).
		/// </summary>
		/// <value>
		/// The current mode (?act=someAction&amp;mode=somMode).
		/// </value>
		public string CurrentMode { get; private set; }

		/// <summary>
		/// Gets the current web-site ID request parameter (/someAction/someID or ?act=someAction&amp;id=someID).
		/// </summary>
		/// <value>
		/// The current mode (?act=someAction&amp;mode=somMode).
		/// </value>
		public string CurrentID { get; private set; }

		/// <summary>
		/// Gets the web-site physical path, for example: C:/inetpub/wwwroot/YourSite
		/// </summary>
		/// <value>
		/// The site physical path.
		/// </value>
		public string SitePhysicalPath { get; private set; }

		/// <summary>
		/// Gets the web-site virtual relative path, for example: /site1 if your web-site url is http://yoursite.com/site1/
		/// </summary>
		public string SiteVirtualPath { get; private set; }

		/// <summary>
		/// Gets the web-site URL, for example: http://yoursite.com/site1/
		/// </summary>
		/// <value>
		/// The site URL.
		/// </value>
		public string SiteUrl { get; private set; }

		/// <summary>
		/// Gets current action/mode URL in formal like ?act={0}&amp;mode={1}&amp;id={2}.
		/// </summary>
		/// <returns></returns>
		public string GetActionModeUrl()
		{
			if (String.IsNullOrEmpty(CurrentAction)) return "";

			var url = "?act=" + CurrentAction;

			if (!String.IsNullOrEmpty(CurrentMode))
				url += "&amp;mode=" + CurrentMode;

			if (!String.IsNullOrEmpty(CurrentID))
				url += "&amp;id=" + CurrentID;

			return url;
		}

		private void CalculateRequestParameters()
		{
			string action = null;
			string mode = null;
			string id = null;

			if (Request.Url != null)
			{
				AcspRouteParser.ParseRoute(Request.Url.AbsolutePath, SiteVirtualPath, out action, out mode, out id);
			}

			if(string.IsNullOrEmpty(action))
				action = Request.QueryString["act"];

			if (string.IsNullOrEmpty(mode))
				mode = Request.QueryString["mode"];

			if (string.IsNullOrEmpty(id))
				id = Request.QueryString["id"];

			CurrentAction = action;
			CurrentMode = mode;
			CurrentID = id;
		}

		private void CalculateSitePhysicalPath()
		{
			if (Request.PhysicalApplicationPath != null)
			{
				SitePhysicalPath = Request.PhysicalApplicationPath.Replace("\\", "/");

				if (SitePhysicalPath.EndsWith("/"))
					SitePhysicalPath = SitePhysicalPath.Substring(0, SitePhysicalPath.Length - 1);
			}
		}

		private void CalculateSiteVirualPath()
		{
			SiteVirtualPath = Request.ApplicationPath == "/" ? "" : Request.ApplicationPath;
		}

		private void CalculateSiteUrl()
		{
			if (Request != null && Request.Url != null)
			{
				SiteUrl = String.Format("{0}://{1}{2}",
					Request.Url.Scheme,
					Request.Url.Authority,
					Request.ApplicationPath);

				if (!SiteUrl.EndsWith("/"))
					SiteUrl += "/";
			}
		}
	}
}