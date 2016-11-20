using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using CsvHelper.Configuration;

namespace RedmineMail.Models
{
    public class Tracker : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        public Tracker()
        {
            this._Name = "";
        }

        #region Name変更通知プロパティ
        private string _Name;

        public string Name
        {
            get
            { return _Name; }
            set
            { 
                if (_Name == value)
                    return;
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }
        #endregion
    }

    public sealed class TrackerClassMap : CsvClassMap<Tracker>
    {
        public TrackerClassMap()
        {
            Map(m => m.Name).Index(0);
        }
    }
}
