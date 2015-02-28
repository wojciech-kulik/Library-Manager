using Caliburn.Micro;
using ClientApplication.DBService;
using ClientApplication.ViewModels;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication.Utilities
{
    public class NavigationService : INavigationService
    {
        SimpleContainer _container;
        IWindowManager _windowManager;

        public NavigationService(SimpleContainer container, IWindowManager windowManager)
        {
            _container = container;
            _windowManager = windowManager;
        }

        public INavigationService<TViewModel> GetWindow<TViewModel>()
        {
            return new NavigationService<TViewModel>(_windowManager, (TViewModel)_container.GetInstance(typeof(TViewModel), null));
        }
    }




    public class NavigationService<TVM> : INavigationService<TVM>
    {
        IWindowManager _windowManager;
        TVM _viewModel;
        System.Action _action;

        public NavigationService(IWindowManager windowManager, TVM viewModel)
        {
            _windowManager = windowManager;
            _viewModel = viewModel;
        }

        public INavigationService<TVM> WithParam<TProperty>(Expression<Func<TVM, TProperty>> property, TProperty value)
        {
            var prop = (PropertyInfo)((MemberExpression)property.Body).Member;
            prop.SetValue(_viewModel, value, null);

            return this;
        }

        public INavigationService<TVM> DoBeforeShow(Action<TVM> action)
        {
            action(_viewModel);
            return this;
        }

        public INavigationService<TVM> DoIfSuccess(System.Action action)
        {
            _action = action;
            return this;
        }

        public void ShowWindow(IDictionary<string, object> settings = null)
        {
            _windowManager.ShowWindow(_viewModel, null, settings);
        }

        public bool ShowWindowModal(IDictionary<string, object> settings = null)
        {
            bool result = _windowManager.ShowDialog(_viewModel, null, settings) ?? false;
            if (result && _action != null)
                _action();

            return result;
        }
    }
}
