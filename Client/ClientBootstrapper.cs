using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using ClientApplication.ViewModels;
using Caliburn.Micro;
using System.Reflection;
using Common;
using ClientApplication.Utilities;
using AutoMapper;
using System.Windows;

namespace ClientApplication
{
    public class ClientBootstrapper: BootstrapperBase
    {
        private SimpleContainer container;

        public ClientBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {      
            container = new SimpleContainer();
            container.Instance<SimpleContainer>(container);

            //ViewModels
            container.Singleton<MainWindowViewModel>();
            container.PerRequest<ClientDetailsViewModel>();
            container.PerRequest<BooksReturnViewModel>();
            container.PerRequest<LendingDetailsViewModel>();
            container.PerRequest<BooksListViewModel>();
            container.PerRequest<BookDetailsViewModel>();
            container.PerRequest<LoginViewModel>();
            container.PerRequest<UserDetailsViewModel>();
            container.PerRequest<UsersListViewModel>();
        
            //Services            
            container.PerRequest<IWindowManager, MyWindowManager>();
            container.PerRequest<IDBServiceManager<IDatabaseService>, DBServiceManager>();
            container.PerRequest<ISettingsService, SettingsService>();
            container.Singleton<INavigationService, NavigationService>();

            //AutoMapper
            Mapper.CreateMap<ClientDTO, ClientDTO>();
            Mapper.CreateMap<BookCategoryDTO, BookCategoryDTO>();
            Mapper.CreateMap<BookDTO, BookDTO>();
            Mapper.CreateMap<EmployeeDTO, EmployeeDTO>();
            Mapper.CreateMap<LendingDTO, LendingDTO>();
            Mapper.CreateMap<AddressDTO, AddressDTO>();
            Mapper.CreateMap<LentBookDTO, LentBookDTO>();
            Mapper.CreateMap<PersonDTO, PersonDTO>();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return container.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.GetAllInstances(serviceType);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            INavigationService nav = (INavigationService)container.GetInstance(typeof(INavigationService), null);
            nav.GetWindow<MainWindowViewModel>().ShowWindow();

            bool result = nav.GetWindow<LoginViewModel>()
                             .ShowWindowModal(new Dictionary<string, object>() { { "WindowStyle", WindowStyle.ToolWindow }, { "ResizeMode", ResizeMode.NoResize } });

            if (!result)
                Application.Current.Shutdown();
            else
                ((MainWindowViewModel)container.GetInstance(typeof(MainWindowViewModel), null)).RefreshClients();
        }
    }
}