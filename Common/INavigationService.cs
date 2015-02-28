using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface INavigationService<TVM>
    {
        INavigationService<TVM> WithParam<TProperty>(Expression<Func<TVM, TProperty>> property, TProperty value);

        INavigationService<TVM> DoIfSuccess(System.Action action);

        INavigationService<TVM> DoBeforeShow(Action<TVM> action);

        void ShowWindow(IDictionary<string, object> settings = null);

        bool ShowWindowModal(IDictionary<string, object> settings = null);
    }



    public interface INavigationService
    {
        INavigationService<TViewModel> GetWindow<TViewModel>();
    }
}
