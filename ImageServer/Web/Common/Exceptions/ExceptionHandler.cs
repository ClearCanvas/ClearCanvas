#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Web;
using ClearCanvas.Common;
using System.Text;
using ClearCanvas.Common.Rest;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Common.Exceptions
{
    public class ExceptionHandler
    {
        private static ExceptionHandler instance = new ExceptionHandler();
        private static HttpContext context;
 
        public static ExceptionHandler Instance()
        {
            return instance;
        }
 
        //instance members
        private ExceptionHandler()
        {
            context = HttpContext.Current;
        }

        

		public static void ThrowException(Exception e)
		{
			context = HttpContext.Current;
			Platform.Log(LogLevel.Error, e);
			
			// Handle cross-site script attacks.
			// They can cause HttpRequestValidationException or HttpException with a special error message.
			// They can also generate HttpException without any message. We  should handle all of them.
			// We should also avoid using context.Server.Transfer() because it will end up throwing exception too.
			// Instead, we should call Response.RedirectPermanent
			var sb = new StringBuilder();
			if (e is HttpRequestValidationException || (e is HttpException &&
				(string.IsNullOrEmpty(e.Message) || e.Message.StartsWith("A potentially dangerous"))))
			{
				HttpContext.Current.Response.RedirectPermanent(ImageServerConstants.PageURLs.InvalidRequestErrorPage, true);
				return;
			}


			if (context.Items.Contains(ImageServerConstants.ContextKeys.ErrorMessage))
				context.Items.Remove(ImageServerConstants.ContextKeys.ErrorMessage);
			if (context.Items.Contains(ImageServerConstants.ContextKeys.StackTrace))
				context.Items.Remove(ImageServerConstants.ContextKeys.StackTrace);

			context.Items.Add(ImageServerConstants.ContextKeys.ErrorMessage, sb.ToString());
			context.Items.Add(ImageServerConstants.ContextKeys.StackTrace, e.StackTrace);
			try
			{
				context.Server.Transfer(ImageServerConstants.PageURLs.ErrorPage);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex);
			}
		}

    	public static void ThrowException(BaseWebException e)
        {
            if (e != null)
            {                
                context = HttpContext.Current;

                context.Server.ClearError();

                string logMessage = string.Format("<Error>\n\t<CustomMessage>{0}</CustomMessage>\n\t<SystemMessage>{1}</SystemMessage>\n\t<Source>{2}</Source>\n\t<StackTrace>{3}</StackTrace>\n<Error>", e.LogMessage, e.Message, e.Source, e.StackTrace);
                Platform.Log(LogLevel.Error, logMessage);
                
                context.Items.Add(ImageServerConstants.ContextKeys.StackTrace, logMessage);

                if(e.ErrorMessage != null && !e.ErrorMessage.Equals(string.Empty))
                    context.Items.Add(ImageServerConstants.ContextKeys.ErrorMessage, e.ErrorMessage);
                if (e.ErrorDescription != null && !e.ErrorDescription.Equals(string.Empty))
                    context.Items.Add(ImageServerConstants.ContextKeys.ErrorDescription, e.ErrorDescription);
                
                context.Server.Transfer(ImageServerConstants.PageURLs.ErrorPage);   
            }
        }
        public static void ThrowError(string message)
        {
            context = HttpContext.Current;

            context.Server.ClearError();

            Platform.Log(LogLevel.Error, message);
            context.Items.Add(ImageServerConstants.ContextKeys.StackTrace, message);
            context.Server.Transfer(ImageServerConstants.PageURLs.ErrorPage);
        }

        public static void ThrowException(AuthorizationException e)
        {
                context = HttpContext.Current;

                context.Server.ClearError();

                if (e.ErrorMessage != null && !e.ErrorMessage.Equals(string.Empty))
                    context.Items.Add(ImageServerConstants.ContextKeys.ErrorMessage, e.ErrorMessage);
                if (e.ErrorDescription != null && !e.ErrorDescription.Equals(string.Empty))
                    context.Items.Add(ImageServerConstants.ContextKeys.ErrorDescription, e.ErrorDescription);


                Platform.Log(LogLevel.Error, "{0} is not authorized to view {1}", SessionManager.Current.User.DisplayName, context.Request.RawUrl);
                context.Server.Transfer(ImageServerConstants.PageURLs.AuthorizationErrorPage);
        }


        public static string ThrowAJAXException(Exception e)
        {
            Exception baseException = e.GetBaseException();

            string message = baseException.Message;
            string source = baseException.Source;
            string stackTrace = baseException.StackTrace;

            if (e.Data["ExtraInfo"] != null)
            {
                message += "\nExtra Info: " + e.Data["ExtraInfo"].ToString();
            }
            else
            {
                message += "\nExtra Info: " + "An unspecified error occurred.";
            }

            string logMessage = string.Format("Message: {0}\nSource:{1}\nStack Trace:{2}", message, source, stackTrace);
            Platform.Log(LogLevel.Error, logMessage);

            return logMessage;
        }
    }
}
