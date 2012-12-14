using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using iPASoftware.iRAD.Basics.Logging;
using iPASoftware.iRAD.Client;
using iPASoftware.iRAD.Client.Composite;
using Microsoft.Practices.Unity;
using iPASoftware.iRAD.Client.Composite.WebInteraction;
using iPASoftware.iRAD.Client.ContentHandling;
using iPASoftware.iRAD.Client.Handlers;
using iPASoftware.iRAD.Client.Infrastructure;
using iRAD.MobileData.ViewModels;

namespace iRAD.MobileData
{
    class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        private IMessageBoxHandler handler = null;

        public Bootstrapper(bool threadSafe)
            : base(threadSafe)
        {
        }

        public string Referer { get; private set; }

        protected override bool OnUnhandledException(Exception exception)
        {
            try {
                EasyLog.Message("\nCRITICAL ERROR\n" + exception + "\n\n");
                Log<Bootstrapper>.CriticalError(exception, "Onherstelbare fout");
                ShowExceptionAsync(exception);
                ThreadPool.QueueUserWorkItem(_ => Application.Exit());
                Thread.Sleep(1000);
                return true;
            }
            catch (Exception) {
                return base.OnUnhandledException(exception);
            }
        }

        private async void ShowExceptionAsync(Exception exception)
        {
            await handler.ShowAsync("Een onherstelbare fout heeft zich voorgedaan bij het verwerken van uw gegevens. De applicatie wordt opnieuw opgestart.", "Onherstelbare fout.", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected override void OnContainerCreated()
        {
            base.OnContainerCreated();
            Container.Resolve<IInteractionConfiguration>().UseMapping = false;
            handler = Container.Resolve<IMessageBoxHandler>();
            // Register some factories. These are single instances per client.
                    
        }


        protected override object CreateShellViewModel()
        {
            var shellViewModel = (ShellViewModel)base.CreateShellViewModel();          
            return shellViewModel;
        }

        protected override ContentReference ContentReferenceForApplicationWrapper()
        {
            return ContentRepository.ReferenceForFile("layout/wrapper.cshtml");
        }

        protected override ContentReference ContentReferenceForMessageBoxHandlerScript()
        {
            return ContentRepository.ReferenceForFile("layout/messageboxhandler.js");
        }

        protected override void ConfigureResourcesBundles(IContentRepository contentRepository)
        {
            base.ConfigureResourcesBundles(contentRepository);

            var bundle = new ResourcesBundle();
            bundle.Include.Style(ContentRepository.ReferenceForFile("layout/default.css"));
            bundle.Include.Style(ContentRepository.ReferenceForFile("layout/modals.css"));
            bundle.Include.Script(ContentRepository.ReferenceForFile("layout/default.js"));
            //bundle.Include.Script(ContentRepository.ReferenceForFile("layout/lionbars.js"));
            contentRepository.AddResourcesBundle(bundle);
        }

        //protected override System.Collections.Generic.IEnumerable<ContentReference> ContentReferencesForPrecacheableImages()
        //{
        //    var pngs = Directory.GetFiles(MvvmHostingEnvironment.MapPath("~/Layout/img"), "*.png");
        //    var jpgs = Directory.GetFiles(MvvmHostingEnvironment.MapPath("~/Layout/img"), "*.jpg");
        //    var gifs = Directory.GetFiles(MvvmHostingEnvironment.MapPath("~/Layout/img"), "*.gif");

        //    return pngs.Union(jpgs).Union(gifs).Select(x => ContentRepository.ReferenceForFile(x)).Where(x => !ContentReference.IsNullOrNone(x));
        //}

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            ApplicationTitle = "iRAD MobileData";
        }


      
    }

}