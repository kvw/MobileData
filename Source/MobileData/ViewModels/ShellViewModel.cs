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

        #endregion

        public override void OnInitialized()
        {
            this.MainContent = Container.Resolve<DataGridViewModel>();
        }

    }
}