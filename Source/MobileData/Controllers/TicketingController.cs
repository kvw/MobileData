using System;
using System.Web.Mvc;
using Kinepolis.Mobile.Ticketing.Library;
using Kinepolis.Mobile.Ticketing.Library.Stub;
using Kinepolis.MobileTicketing.Infrastructure;

namespace Kinepolis.MobileTicketing.Controllers
{
    public abstract class TicketingController : Controller
    {
        protected IMobileTicketing MobileTicketing { get; private set; }

        protected TicketingController()
        {
            this.MobileTicketing = MobileTicketingFactory.CreateForControllers();
        }
    }
}