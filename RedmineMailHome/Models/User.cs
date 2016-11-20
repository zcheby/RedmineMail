using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using CsvHelper.Configuration;

namespace RedmineMail.Models
{
    public class User : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        public User()
        {
            this._Name = "";
            this._MailAdress = "";
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

        #region MailAdress変更通知プロパティ
        private string _MailAdress;

        public string MailAdress
        {
            get
            { return _MailAdress; }
            set
            { 
                if (_MailAdress == value)
                    return;
                _MailAdress = value;
                RaisePropertyChanged("MailAdress");
            }
        }
        #endregion
    }

    public sealed class UserClassMap : CsvClassMap<User>
    {
        public UserClassMap()
        {
            Map(m => m.Name).Index(0);
            Map(m => m.MailAdress).Index(1);
        }
    }
}
