using System;
using System.Reflection;
using System.Web.Mvc;
using Kinepolis.Mobile.Ticketing.Library;

namespace Kinepolis.MobileTicketing.Controllers
{
    public class DeliveryController : TicketingController
    {
        //
        // GET: /Delivery/

        public ActionResult Index(string ticketFor, string secKey)
        {
            ITicketData ticketData;
            try {
                var paymentId = int.Parse(ticketFor);
                ticketData = this.MobileTicketing.GetTicketData(paymentId, secKey);
            }
            catch (Exception) {
                ticketData = null;
            }

            return View(new DeliveryModel(ticketData) {
                HostAddress = HttpContext.Request.ServerVariables["LOCAL_ADDR"],
                AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3)
            });
        }

        public class DeliveryModel
        {
            public ITicketData TicketData { get; private set; }
            public string HostAddress { get; set; }
            public string AppVersion { get; set; }

            public DeliveryModel(ITicketData ticketData)
            {
                this.TicketData = ticketData;
            }

            public DeliveryModel() : this(null)
            {
            }
        }

        public ActionResult Cancelled()
        {
            return View(new DeliveryModel {
                HostAddress = HttpContext.Request.ServerVariables["LOCAL_ADDR"],
                AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3)
            });
        }

        public ActionResult Failed()
        {
            return View(new DeliveryModel {
                HostAddress = HttpContext.Request.ServerVariables["LOCAL_ADDR"],
                AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3)
            });
        }
    }
}
