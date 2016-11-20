using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using CsvHelper.Configuration;

namespace RedmineMail.Models
{
    public class Issue : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        public Issue()
        {

        }

        public Issue(string tracker)
        {
            this.Tracker = tracker;
        }

        #region Number変更通知プロパティ
        private int _Number;

        public int Number
        {
            get
            { return _Number; }
            set
            { 
                if (_Number == value)
                    return;
                _Number = value;
                RaisePropertyChanged("Number");
            }
        }
        #endregion

        #region Project変更通知プロパティ
        private string _Project;

        public string Project
        {
            get
            { return _Project; }
            set
            {
                if (_Project == value)
                    return;
                _Project = value;
                RaisePropertyChanged("Project");
            }
        }
        #endregion

        #region Tracker変更通知プロパティ
        private string _Tracker;

        public string Tracker
        {
            get
            { return _Tracker; }
            set
            {
                if (_Tracker == value)
                    return;
                _Tracker = value;
                RaisePropertyChanged("Tracker");
            }
        }
        #endregion

        #region Status変更通知プロパティ
        private string _Status;

        public string Status
        {
            get
            { return _Status; }
            set
            {
                if (_Status == value)
                    return;
                _Status = value;
                RaisePropertyChanged("Status");
            }
        }
        #endregion

        #region Priority変更通知プロパティ
        private string _Priority;

        public string Priority
        {
            get
            { return _Priority; }
            set
            {
                if (_Priority == value)
                    return;
                _Priority = value;
                RaisePropertyChanged("Priority");
            }
        }
        #endregion

        #region Title変更通知プロパティ
        private string _Title;

        public string Title
        {
            get
            { return _Title; }
            set
            { 
                if (_Title == value)
                    return;
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }
        #endregion

        #region Author変更通知プロパティ
        private string _Author;

        public string Author
        {
            get
            { return _Author; }
            set
            { 
                if (_Author == value)
                    return;
                _Author = value;
                RaisePropertyChanged("Author");
            }
        }
        #endregion

        #region AssignedTo変更通知プロパティ
        private string _AssignedTo;

        public string AssignedTo
        {
            get
            { return _AssignedTo; }
            set
            { 
                if (_AssignedTo == value)
                    return;
                _AssignedTo = value;
                RaisePropertyChanged("AssignedTo");
            }
        }
        #endregion


        #region UpdateTime変更通知プロパティ
        private DateTime _UpdateTime;

        public DateTime UpdateTime
        {
            get
            { return _UpdateTime; }
            set
            { 
                if (_UpdateTime == value)
                    return;
                _UpdateTime = value;
                RaisePropertyChanged("UpdateTime");
            }
        }
        #endregion


        #region DueDate変更通知プロパティ
        private DateTime _DueDate;

        public DateTime DueDate
        {
            get
            {
                var dt = new DateTime(2000, 1, 1);
                if (this._DueDate < dt)
                {
                    this._DueDate = System.DateTime.Today.AddDays(7);
                }
                return this._DueDate;
            }
            set
            { 
                if (_DueDate == value)
                    return;
                _DueDate = value;
                RaisePropertyChanged("DueDate");
            }
        }
        #endregion
    }


    public sealed class IssueClassMap : CsvClassMap<Issue>
    {
        public IssueClassMap()
        {
            Map(m => m.Number).Index(0);
            Map(m => m.Project).Index(1);
            Map(m => m.Tracker).Index(2);
            Map(m => m.Status).Index(4);
            Map(m => m.Priority).Index(5);
            Map(m => m.Title).Index(6);
            Map(m => m.Author).Index(7);
            Map(m => m.AssignedTo).Index(8);
            Map(m => m.UpdateTime).Index(9);
            Map(m => m.DueDate).Index(13);
        }
    }
}
