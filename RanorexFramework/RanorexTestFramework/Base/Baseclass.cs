﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NUnit.Framework;
using Ranorex;
using Ranorex.Core;
using RanorexTestFramework;

namespace RanorexTestFramework.Base
{
	/// <summary>
	/// Base calss, which initializes webdoument
	/// </summary>
	public class Baseclass
	{
		/// <summary>
		/// Stores Webdocument object
		/// </summary>
		public WebDocument webdoc;
		/// <summary>
		/// Stores root directory information for reports
		/// </summary>
		public String rootdirectory;
		/// <summary>
		/// Holds the Ranorex path of opened web document
		/// </summary>
		public String webdocpath;
		/// <summary>
		/// Base Directory for automation
		/// </summary>
		public String automationreportdirectory;
		
		/// <summary>
		/// int() returns the driver instance
		/// </summary>
		/// <returns>Returns WebDoc object</returns>
		public WebDocument init()
		{
			String _domainrul=ConfigurationManager.AppSettings["URL"].ToString();
			String _browser=ConfigurationManager.AppSettings["Browser"].ToString();
			//Close all the existing application browser windows and launch application in specified browser
			try{
				
				IList<WebDocument> lis=Ranorex.Host.Local.Find<WebDocument>("/dom[@domain='" + _domainrul + "']");
				foreach(WebDocument web in lis)
				{
					web.Close();
				}
				Ranorex.Host.Local.OpenBrowser(_domainrul, _browser, true, true);
				Ranorex.Host.Local.FindSingle("/dom[@domain='" + _domainrul + "']",TimeSpan.FromSeconds(60));
				
				if(_browser.ToLower()=="chrome")
				{
					webdocpath="/dom[@domain='" + _domainrul + "' and @browsername='Chrome']";
					webdoc = new WebDocument(webdocpath);
				}
				else if(_browser.ToLower()=="firefox")
				{
					webdocpath="/dom[@domain='" + _domainrul + "' and @browsername='Mozilla']";
					webdoc = new WebDocument(webdocpath);
				}
				else
				{
					
					webdocpath="/dom[@domain='" + _domainrul + "' and @browsername='IE']";
					webdoc = new WebDocument(webdocpath);
				}
			}catch(Ranorex.ElementNotFoundException exec)
			{
				Report.Failure("Ranorex unable to interact with browser");
				Assert.Fail("Ranorex unable to interact with browser");
			}
			
			initiateReport();
			try{
				webdoc.WaitForDocumentLoaded();
			}catch(ElementNotFoundException e)
			{
				Report.Failure("Unable to identify browser"+webdoc.BrowserName);
				Assert.Fail("Unable to identify browser "+webdoc.BrowserName);
			}
			
			
			return webdoc;
		}
		
		/// <summary>
		/// Initates report in the specified directory
		/// </summary>
		public void initiateReport()
		{
			automationreportdirectory=Directory.GetParent(Directory.GetCurrentDirectory()).ToString()+"//AutomationReports";
			rootdirectory=automationreportdirectory+"//recent";
			Directory.CreateDirectory(rootdirectory+"//"+NUnit.Framework.TestContext.CurrentContext.Test.Name.ToString());
			Report.Setup(ReportLevel.Info,rootdirectory+"//"+NUnit.Framework.TestContext.CurrentContext.Test.Name.ToString()+"//"+NUnit.Framework.TestContext.CurrentContext.Test.Name.ToString()+".xml",false,true);
			Report.SystemSummary();			
			Report.LogHtml(ReportLevel.Info,"<div><b><u>"+NUnit.Framework.TestContext.CurrentContext.Test.Name.ToString()+"</u></b> - "+webdoc.BrowserName+"</div>");
		
		}
	}
}
