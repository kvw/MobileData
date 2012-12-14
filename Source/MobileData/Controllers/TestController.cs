using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using HtmlAgilityPack;
using Kinepolis.Mobile.Ticketing.Library;
using Kinepolis.Mobile.Ticketing.Library.Stub;
using iPASoftware.iRAD.Basics.Extensions;

namespace Kinepolis.MobileTicketing.Controllers
{
    public class TestController : TicketingController
    {
        private CombiSelection combiInfo;

        public TestController()
        {
            combiInfo = new CombiSelection() {
                CountryId = "BE",
                LanguageId = "NL",
                Type = 220065
            };
        }

        public ActionResult Index()
        {
            var ticketing = this.MobileTicketing as Mobile.Ticketing.Library.MobileTicketing;
            var complexes = ticketing != null ? ticketing.GetComplexes() : this.MobileTicketing.GetComplexes(combiInfo);
            return View(new ComplexesModel(complexes));
        }

        public ActionResult Complex(string id)
        {
            var ticketing = this.MobileTicketing as Mobile.Ticketing.Library.MobileTicketing;
            IEnumerable<ICombi> programmation;
            if (ticketing != null) {
                var combi = ticketing.GetTestProgrammation(id);
                programmation = combi != null ? combi.AsArray() : new ICombi[] { };
            }
            else
                programmation = this.MobileTicketing.GetProgrammation(combiInfo, id);
            return View(new ProgrammationModel(id, programmation));
        }

        public ActionResult Event(string countryId, string complexId, int contentId, string hallId, string performanceDate, string performanceTime)
        {
            //var eventSelection = new EventSelection() {
            //    CountryId = "BE",
            //    LanguageId = "ENG",
            //    ComplexId = complexId,
            //    HallId = hallId,
            //    ContentId = contentId,
            //    PerformanceDate = performanceDate,
            //    PerformanceTime = performanceTime
            //};

            //var combi = Ticketing.GetEvent(eventSelection);
            //if (combi == null)
            //    return HttpNotFound();

            return
                View((object)VirtualPathUtility.ToAbsolute(string.Format(
                        "~/?CNTR={6}&LNG={7}&COMPFEATID={0}&COUGRPFEATID={1}&COMPID={2}&PERFHALL={3}&PERFTIME={4}&PERFDATE={5}",
                        contentId,
                        contentId,
                        complexId,
                        hallId,
                        performanceTime,
                        performanceDate,
                        countryId,
                        string.Compare(countryId, "BE", StringComparison.OrdinalIgnoreCase) == 0 ? "NL" : countryId)));
        }

        public ActionResult TestEvent()
        {
            return Redirect("~/");
        }

        [HttpGet]
        public ActionResult Payment(string url)
        {
            return View((object)url);
        }

        [HttpPost, ActionName("Payment")]
        public ActionResult ContinuePayment(string url)
        {
            HtmlDocument doc = null;
            try {
                var web = new HtmlWeb();
                doc = web.Load(url);

                var form = doc.DocumentNode.Descendants("FORM").Single();
                var action = form.Attributes["ACTION"].Value;

                var formData = new NameValueCollection();
                foreach (var input in doc.DocumentNode.SelectNodes("//input")) {
                    formData.Add(input.Attributes["NAME"].Value, input.Attributes["VALUE"].Value);
                }

                var post = new PaymentWebClient();
                var bytes = post.UploadValues(action, formData);
                using (var ms = new MemoryStream(bytes))
                    doc.Load(ms);

                form = doc.DocumentNode.Descendants("FORM").First();
                action = form.Attributes["ACTION"].Value;

                formData.Clear();
                var skipNames = new string[] { "payment", "alias", "aliasoperation", "cancel" };
                foreach (var input in doc.DocumentNode.SelectNodes("//input")) {
                    var name = input.Attributes["NAME"];
                    if (name == null) continue;
                    if (skipNames.Contains(name.Value)) continue;
                    var valueAtt = input.Attributes["VALUE"];
                    var value = string.Empty;
                    if (valueAtt != null) value = valueAtt.Value;

                    formData[name.Value] = value;
                }
                formData["Ecom_Payment_Card_Name"] = "autotest";
                formData["Ecom_Payment_Card_Number"] = "4111 1111 1111 1111";
                formData["Ecom_Payment_Card_ExpDate_Month"] = "01";
                formData["Ecom_Payment_Card_ExpDate_Year"] = "2015";
                formData["Comp_Expirydate"] = "201501";
                formData["Ecom_Payment_Card_Verification"] = "123";

                post = new PaymentWebClient();
                //action = HttpUtility.UrlDecode(action); does not do the trick?
                action = action.Replace("&#58;", ":");
                bytes = post.UploadValues(action, formData);
                using (var ms = new MemoryStream(bytes))
                    doc.Load(ms);

                var uri = post.ResponseUri;
                return Redirect(Url.Action("Index", "Delivery") + uri.Query);
            }
            catch (Exception ex) {
                return View("BadPayment", new BadPaymentModel(ex, doc));
            }
        }

        public class BadPaymentModel
        {
            public string Error { get; private set; }
            public string Html { get; private set; }

            public BadPaymentModel(Exception exception, HtmlDocument doc)
            {
                Error = exception == null ? "(unknown)" : exception.ToString();
                Html = doc == null ? "(empty)" : doc.DocumentNode.OuterHtml;
            }
        }

        class PaymentWebClient : WebClient
        {
            private Uri _responseUri;

            public Uri ResponseUri
            {
                get { return _responseUri; }
            }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                WebResponse response = base.GetWebResponse(request);
                _responseUri = response.ResponseUri;
                return response;
            }
        }

        public class ProgrammationModel
        {
            public string Complex { get; set; }
            public List<IGrouping<string, IEvent>> Programmation { get; set; }

            public ProgrammationModel(string complex, IEnumerable<ICombi> programmation)
            {
                Complex = "ITBE";
                var list = programmation.SelectMany(x => x.AllowedEvents).Where(e => e.ShowTime.Date <= DateTime.Now.Date).GroupBy(e => e.Content.Id + e.ShowTime.ToString("yyyyMMdd")).ToList();
                if (!list.Any()) {
                    list = programmation.SelectMany(x => x.AllowedEvents).Where(e => e.ShowTime.Date <= DateTime.Now.Date.AddDays(1)).GroupBy(e => e.Content.Id + e.ShowTime.ToString("yyyyMMdd")).ToList();
                }

                Programmation = new List<IGrouping<string, IEvent>>(list);
            }
        }

        public class ComplexesModel
        {
            public List<IComplex> Complexes { get; set; }

            public ComplexesModel(IEnumerable<IComplex> list)
            {
                Complexes = new List<IComplex>(list);
            }
        }

    }
}
