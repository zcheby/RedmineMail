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
using System.Reflection;

namespace RedmineMail.ViewModels
{
    public class NewIssueWindowViewModel : ViewModel
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

        private MainWindowViewModel parentViewModel;

        public NewIssueWindowViewModel(MainWindowViewModel parentViewModel)
        {
            this.parentViewModel = parentViewModel;

            this.Tracker = System.Configuration.ConfigurationManager.AppSettings["DefaultTracker"];
            // 本日より1週間後を初期期限とする。
            this.DueDate = System.DateTime.Today.AddDays(7);

            var defaultPriority = System.Configuration.ConfigurationManager.AppSettings["DefaultPriority"];
            this.SelectedPriority = this.parentViewModel.PriorityList.Where(x => x.Name == defaultPriority).First();
        }

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
            }
        }
        #endregion

        public List<Priority> PriorityList
        {
            get
            { return this.parentViewModel.PriorityList; }
        }

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
            }
        }
        #endregion

        public List<User> UserList
        {
            get
            { return this.parentViewModel.UserList; }
        }

        #region DueDate変更通知プロパティ
        private DateTime _DueDate;

        public DateTime DueDate
        {
            get
            { return _DueDate; }
            set
            { 
                if (_DueDate == value)
                    return;
                _DueDate = value;
                RaisePropertyChanged("DueDate");
            }
        }
        #endregion

        #region NewIssueCommand
        private ListenerCommand<Issue> _NewIssueCommand;

        public ListenerCommand<Issue> NewIssueCommand
        {
            get
            {
                if (_NewIssueCommand == null)
                {
                    _NewIssueCommand = new ListenerCommand<Issue>(NewIssue);
                }
                return _NewIssueCommand;
            }
        }

        public void NewIssue(Issue parameter)
        {
            if (this.SelectedUser == null)
            {
                System.Windows.MessageBox.Show("担当者を選択して下さい。");
                return;
            }

            // ステータス
            var status = System.Configuration.ConfigurationManager.AppSettings["DefaultStatus"];
            //宛先
            var to = System.Configuration.ConfigurationManager.AppSettings["ToMailAddress"];
            //題名
            string subject = " ";
            // オーダー番号
            var projectId = System.Configuration.ConfigurationManager.AppSettings["ProjectId"];
            //本文
            var body = string.Format(@"
project: {0}%0D%0A
tracker: {1}%0D%0A
status: {2}%0D%0A
priority: {3}%0D%0A
assigned to: {4}%0D%0A
due date: {5}%0D%0A
                ", projectId, this.Tracker, status, this.SelectedPriority.Name, this.SelectedUser.MailAdress, this.DueDate.ToString("yyyy-MM-dd"));

            //標準のメールクライアントを開く
            System.Diagnostics.Process.Start(
                string.Format("mailto:{0}?subject={1}&body={2}", to, subject, body));

            // ウィンドウを閉じる
            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }
        #endregion
    }
}
