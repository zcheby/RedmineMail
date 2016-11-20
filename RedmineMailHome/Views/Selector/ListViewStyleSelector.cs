using RedmineMail.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RedmineMail.Views.Selector
{
    public class ListViewStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var issueViewModel = (IssueViewModel)item;

            if (issueViewModel.IsUpdated == true)
            {
                var f = (FrameworkElement)container;
                return (Style)f.FindResource("IsUpdated");
            }

            return null;
        }
    }
}
