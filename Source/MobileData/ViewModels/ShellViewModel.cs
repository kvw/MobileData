using System;
using System.Reflection;
using Microsoft.Practices.Unity;
using iPASoftware.iRAD.Client.Composite;
using iPASoftware.iRAD.Client.ViewModels;
using iPASoftware.iRAD.Shared.Aspects.Composite;

namespace iRAD.MobileData.ViewModels
{

    [NotifyPropertiesChanged]
    class ShellViewModel : ViewModelBase
    {
        [Dependency]
        public IOperationsTracker OperationsTracker { get; set; }

        public ShellViewModel()
        {
            MainContent = new BootingViewModel();
        }

        #region Binding Properties

  
        public object MainContent { get; set; }
        public string AppVersion { get; set; }
        public string DllVersion { get; set; }
        public string HostAddress { get; set; }

        #endregion

        public override async void OnBindComplete()
        {
            base.OnBindComplete();
            
            // subscribe to events
         
            try
            {
                AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
               
                HostAddress = Application.Info.HostAddress.MapToIPv4().ToString();
            }
            catch (Exception)
            {
            }
        }


    }

    internal class BootingViewModel
    {
    }
}