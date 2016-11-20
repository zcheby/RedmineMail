using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using RedmineMail.Models;
using CsvHelper;
using System.IO;
using System.Reflection;
using System.Windows;

namespace RedmineMail.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        /* コマンド、プロパティの定義にはそれぞれ 
         * 
         *  lvcom   : ViewModelCommand
         *  lvcomn  : ViewModelCommand(CanExecute無)
         *  llcom   : ListenerCommand(パラメータ有のコマンド)
         *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop   : 変更通知プロパティ(.NET4.5ではlpropn)
         *  
         * を使用してください。
         * 
         * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
         * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
         * LivetCallMethodActionなどから直接メソッドを呼び出してください。
         * 
         * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
         * 同様に直接ViewModelのメソッドを呼び出し可能です。
         */

        /* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
         */

        /* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
         * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
         * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
         * 
         * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
         * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
         * 
         * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
         * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
         * 
         * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
         */

        /* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         * 
         * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
         * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
         */

        private List<IssueViewModel> masterIssueList;

        public void Initialize()
        {
            ReadIssueCsv();
            ReadStatusCsv();
            ReadTrackerCsv();
            ReadPriorityCsv();
            ReadUserCsv();
            InitialComboBox();
            NarrowIssueList();
        }

        private void InitialComboBox()
        {
            this.SelectedStatus = this.StatusList.First();
            this.SelectedTracker = this.TrackerList.First();
            this.SelectedPriority = this.PriorityList.First();
            this.SelectedUser = this.UserList.First();
        }

        /// <summary>
        /// チケット一覧の絞り込みを行います。
        /// </summary>
        private void NarrowIssueList()
        {
            var narrowIssueList = new List<IssueViewModel>(this.masterIssueList);

            if (this.SelectedStatus != null && this.SelectedStatus.Name != "")
            {
                narrowIssueList = narrowIssueList.Where<IssueViewModel>(issue => issue.Status == this.SelectedStatus.Name).ToList<IssueViewModel>();
            }

            if (this.SelectedTracker != null && this.SelectedTracker.Name != "")
            {
                narrowIssueList = narrowIssueList.Where<IssueViewModel>(issue => issue.Tracker == this.SelectedTracker.Name).ToList<IssueViewModel>();
            }

            if (this.SelectedPriority != null && this.SelectedPriority.Name != "")
            {
                narrowIssueList = narrowIssueList.Where<IssueViewModel>(issue => issue.Priority == this.SelectedPriority.Name).ToList<IssueViewModel>();
            }

            if (this.SelectedUser != null && this.SelectedUser.Name != "")
            {
                narrowIssueList = narrowIssueList.Where<IssueViewModel>(issue => issue.AssignedTo == this.SelectedUser.Name).ToList<IssueViewModel>();
            }

            this.IssueList = new ObservableSynchronizedCollection<IssueViewModel>(narrowIssueList);
        }

        private void ReadIssueCsv()
        {
            // チケットの読み込み
            var filePath = System.Configuration.ConfigurationManager.AppSettings["IssueCSVFilePath"];
            var csv = new CsvReader(new StreamReader(filePath, Encoding.GetEncoding("SHIFT_JIS")));
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.RegisterClassMap<IssueClassMap>();
            this.masterIssueList = csv.GetRecords<Issue>().Select(x => new IssueViewModel(x, this)).ToList();
        }

        private void ReadStatusCsv()
        {
            // ステータスの読み込み
            var filePath = System.Configuration.ConfigurationManager.AppSettings["StatusCSVFilePath"];
            var csv = new CsvReader(new StreamReader(filePath, Encoding.GetEncoding("SHIFT_JIS")));
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.RegisterClassMap<StatusClassMap>();
            this.StatusList = csv.GetRecords<Status>().ToList<Status>();
            // 絞り込み用のダミーを追加
            this.StatusList.Insert(0, new Status());
        }

        private void ReadTrackerCsv()
        {
            // トラッカーの読み込み
            var filePath = System.Configuration.ConfigurationManager.AppSettings["TrackerCSVFilePath"];
            var csv = new CsvReader(new StreamReader(filePath, Encoding.GetEncoding("SHIFT_JIS")));
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.RegisterClassMap<TrackerClassMap>();
            this.TrackerList = csv.GetRecords<Tracker>().ToList<Tracker>();
            // 絞り込み用のダミーを追加
            this.TrackerList.Insert(0, new Tracker());
        }

        private void ReadPriorityCsv()
        {
            // 優先度の読み込み
            var filePath = System.Configuration.ConfigurationManager.AppSettings["PriorityCSVFilePath"];
            var csv = new CsvReader(new StreamReader(filePath, Encoding.GetEncoding("SHIFT_JIS")));
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.RegisterClassMap<PriorityClassMap>();
            this.PriorityList = csv.GetRecords<Priority>().ToList<Priority>();
            // 絞り込み用のダミーを追加
            this.PriorityList.Insert(0, new Priority());
        }

        private void ReadUserCsv()
        {
            // ユーザの読み込み
            var filePath = System.Configuration.ConfigurationManager.AppSettings["UserCSVFilePath"];
            var csv = new CsvReader(new StreamReader(filePath, Encoding.GetEncoding("SHIFT_JIS")));
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.RegisterClassMap<UserClassMap>();
            this.UserList = csv.GetRecords<User>().ToList<User>();
            // ダミーを追加
            this.UserList.Insert(0, new User());
        }

        #region SelectedStatus変更通知プロパティ
        private Status _SelectedStatus;

        public Status SelectedStatus
        {
            get
            { return _SelectedStatus; }
            set
            {
                if (_SelectedStatus == value)
                    return;
                _SelectedStatus = value;
                RaisePropertyChanged("SelectedStatus");
                NarrowIssueList();
            }
        }
        #endregion

        #region StatusList変更通知プロパティ
        private List<Status> _StatusList;

        public List<Status> StatusList
        {
            get
            { return _StatusList; }
            set
            {
                if (_StatusList == value)
                    return;
                _StatusList = value;
                RaisePropertyChanged("StatusList");
            }
        }
        #endregion

        #region SelectedTracker変更通知プロパティ
        private Tracker _SelectedTracker;

        public Tracker SelectedTracker
        {
            get
            { return _SelectedTracker; }
            set
            { 
                if (_SelectedTracker == value)
                    return;
                _SelectedTracker = value;
                RaisePropertyChanged("SelectedTracker");
                NarrowIssueList();
            }
        }
        #endregion

        #region TrackerList変更通知プロパティ
        private List<Tracker> _TrackerList;

        public List<Tracker> TrackerList
        {
            get
            { return _TrackerList; }
            set
            { 
                if (_TrackerList == value)
                    return;
                _TrackerList = value;
                RaisePropertyChanged("TrackerList");
            }
        }
        #endregion

        #region SelectedPriority変更通知プロパティ
        private Priority _SelectedPriority;

        public Priority SelectedPriority
        {
            get
            { return _SelectedPriority; }
            set
            {
                if (_SelectedPriority == value)
                    return;
                _SelectedPriority = value;
                RaisePropertyChanged("SelectedPriority");
                NarrowIssueList();
            }
        }
        #endregion

        #region PriorityList変更通知プロパティ
        private List<Priority> _PriorityList;

        public List<Priority> PriorityList
        {
            get
            { return _PriorityList; }
            set
            {
                if (_PriorityList == value)
                    return;
                _PriorityList = value;
                RaisePropertyChanged("PriorityList");
            }
        }
        #endregion

        #region SelectedUser変更通知プロパティ
        private User _SelectedUser;

        public User SelectedUser
        {
            get
            { return _SelectedUser; }
            set
            {
                if (_SelectedUser == value)
                    return;
                _SelectedUser = value;
                RaisePropertyChanged("SelectedUser");
                NarrowIssueList();
            }
        }
        #endregion

        #region UserList変更通知プロパティ
        private List<User> _UserList;

        public List<User> UserList
        {
            get
            { return this._UserList; }
            set
            {
                if (_UserList == value)
                    return;
                _UserList = value;
                RaisePropertyChanged("UserList");
            }
        }
        #endregion

        #region Issues変更通知プロパティ
        private ObservableSynchronizedCollection<IssueViewModel> _IssueList;

        public ObservableSynchronizedCollection<IssueViewModel> IssueList
        {
            get
            { return _IssueList; }
            set
            { 
                if (_IssueList == value)
                    return;
                _IssueList = value;
                RaisePropertyChanged("IssueList");
            }
        }
        #endregion

        #region NewIssueCommand
        private ViewModelCommand _NewIssueCommand;

        public ViewModelCommand NewIssueCommand
        {
            get
            {
                if (_NewIssueCommand == null)
                {
                    _NewIssueCommand = new ViewModelCommand(NewIssue);
                }
                return _NewIssueCommand;
            }
        }

        public void NewIssue()
        {
            Messenger.Raise(new TransitionMessage(new NewIssueWindowViewModel(this), "NewIssueTransition"));
        }
        #endregion


        #region ReloadIssueCommand
        private ViewModelCommand _ReloadIssueCommand;

        public ViewModelCommand ReloadIssueCommand
        {
            get
            {
                if (_ReloadIssueCommand == null)
                {
                    _ReloadIssueCommand = new ViewModelCommand(ReloadIssue);
                }
                return _ReloadIssueCommand;
            }
        }

        public void ReloadIssue()
        {
            ReadIssueCsv();
            NarrowIssueList();
        }
        #endregion



        public void UpdateIssue(IssueViewModel issueViewModel)
        {
            Messenger.Raise(new TransitionMessage(new UpdateIssueWindowViewModel(issueViewModel, this), "UpdateIssueTransition"));
        }

        public void UpdateIssueList()
        {
            NarrowIssueList();
        }
    }
}
