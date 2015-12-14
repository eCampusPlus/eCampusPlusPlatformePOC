using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using eCampusPlus.Engine.Configuration.Drivers;
using Fr.eCampusPlus.Engine.Model.POCO;
using Fr.eCampusPlus.Engine.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace eCampusPlusPlatformePOC.Controllers
{
    public class HomeController : Controller
    {
        private string ServerPath
        {
            get { return Server.MapPath(Url.Content("~/bin/{0}")); }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CampusAccountCreation()
        {
            setUp();
            //Browser start
            Browser.SetWebDriver("FIREFOX");

            string plateformeId = "FR";
            string targetId = "MA";

            //REGISTRATION
            string pageId = "RGTR";
            string url = eCampusPlusConfig.Plateforme.FirstOrDefault(pt => pt.PlateformeId.Equals(plateformeId)).Targets.FirstOrDefault(t => t.TargetId.Equals(targetId)).Accesses.FirstOrDefault(a => a.AccesseId.Equals(pageId)).Url;
            var page = new Page(ServerPath, plateformeId, pageId, url);
            ProcessingAction(page, eCampusPlusUser, eCampusPlusConfig);

            Browser.WebDriver.Quit();
            return View("Index");
        }

        public ActionResult CampusAccountValidation(string lienValidation)
        {
            setUp();
            //Browser start
            Browser.SetWebDriver("FIREFOX");

            string plateformeId = "FR";
            string targetId = "MA";

            //Confirmation
            string pageId = "ACNT-CONF";
            string url = lienValidation;

            var page = new Page(ServerPath,plateformeId, pageId, url);
            ProcessingAction(page, eCampusPlusUser, eCampusPlusConfig);

            //LOGIN
            pageId = "ACNT-LGN";
            url =
                eCampusPlusConfig.Plateforme.FirstOrDefault(pt => pt.PlateformeId.Equals(plateformeId))
                    .Targets.FirstOrDefault(t => t.TargetId.Equals(targetId))
                    .Accesses.FirstOrDefault(a => a.AccesseId.Equals(pageId))
                    .Url;
            page = new Page(ServerPath,plateformeId, pageId, url);
            ProcessingAction(page, eCampusPlusUser, eCampusPlusConfig);

            //Candidate
            pageId = "ACNT-NAV";
            page = new Page(ServerPath,plateformeId, pageId, string.Empty, false);
            ProcessingAction(page, eCampusPlusUser, eCampusPlusConfig);
            
            pageId = "ACNT-CAND-NAV";
            page = new Page(ServerPath,plateformeId, pageId, string.Empty, false);
            ProcessingAction(page, eCampusPlusUser, eCampusPlusConfig);
            
            pageId = "ACNT-CAND-STDNT-INFO";
            page = new Page(ServerPath,plateformeId, pageId, string.Empty, false);
            ProcessingAction(page, eCampusPlusUser, eCampusPlusConfig);

            pageId = "ACNT-CAND-STDNT-SKILLS";
            page = new Page(ServerPath,plateformeId, pageId, string.Empty, false);
            ProcessingAction(page, eCampusPlusUser, eCampusPlusConfig);

            pageId = "ACNT-CAND-STDNT-LANGUA";
            page = new Page(ServerPath,plateformeId, pageId, string.Empty, false);
            ProcessingAction(page, eCampusPlusUser, eCampusPlusConfig);

            pageId = "ACNT-CAND-STDNT-CHOSE-SCHOOL";
            page = new Page(ServerPath,plateformeId, pageId, string.Empty, false);
            ProcessingAction(page, eCampusPlusUser, eCampusPlusConfig);

            //END
            Browser.WebDriver.Quit();

            return View("Index");
        }

        private static void ProcessingAction(Page page, eCampusPlusUser eCampusPlusUser, eCampusPlusConfiguration eCampusPlusConfig)
        {

            var eCampusUserProperties = eCampusPlusUser.GetType().GetProperties();
            page.PageElements.ForEach(e =>
            {
                var property = eCampusUserProperties.FirstOrDefault(p => p.Name.Equals(e.Name));
                if (property != null)
                {
                    string value = property.GetValue(eCampusPlusUser).ToString();
                    PagesHelper.PerformAction((PagesHelper.ActionElementType)Enum.Parse(typeof(PagesHelper.ActionElementType), e.ElementType, true), e.Accessor, value);
                    if (e.RequireReload)
                    {
                        Browser.WebDriver.Navigate().Refresh();
                        PagesHelper.PerformAction((PagesHelper.ActionElementType)Enum.Parse(typeof(PagesHelper.ActionElementType), e.PreActionField.ElementType, true), e.PreActionField.Accessor);
                    }
                }
            });

        }

        private void setUp()
        {

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            //TEST DATA
            //USER DATA
            eCampusPlusUser = new eCampusPlusUser();
            using (StreamReader sr = new StreamReader(Server.MapPath(Url.Content("~/bin/App_Data/eCampusPlusTestData/eCampusPlusTestData.json"))))
            {
                eCampusPlusUser = serializer.Deserialize(sr, eCampusPlusUser.GetType()) as eCampusPlusUser;
            }
            //CAMPUS DATA
            eCampusPlusConfig = new eCampusPlusConfiguration();
            using (StreamReader sr = new StreamReader( Server.MapPath(Url.Content("~/bin/eCampusPlusEngineData/eCampusPlusEngineData.json"))))
            {
                eCampusPlusConfig = serializer.Deserialize(sr, eCampusPlusConfig.GetType()) as eCampusPlusConfiguration;
            }
        }

        private eCampusPlusUser eCampusPlusUser { get; set; }

        private eCampusPlusConfiguration eCampusPlusConfig { get; set; }
    }
}