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
using Livet.EventListeners.WeakEvents;
using System.Reflection;

namespace RedmineMail.ViewModels
{
    public class IssueViewModel : ViewModel
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

        private Issue _Issue;

        private MainWindowViewModel parentViewModel;

        public IssueViewModel(Issue issue, MainWindowViewModel parentViewModel)
        {
            _Issue = issue;
            this.parentViewModel = parentViewModel;
            this.IsUpdated = false;
        }

        #region IsUpdated変更通知プロパティ
        private bool _IsUpdated;

        public bool IsUpdated
        {
            get
            { return _IsUpdated; }
            set
            { 
                if (_IsUpdated == value)
                    return;
                _IsUpdated = value;
                RaisePropertyChanged("IsUpdated");
            }
        }
        #endregion

        public int Number
        {
            get { return this._Issue.Number; }
            set { this._Issue.Number = value; }
        }

        public string Status
        {
            get { return this._Issue.Status; }
            set { this._Issue.Status = value; }
        }

        public string Project
        {
            get { return this._Issue.Project; }
            set { this._Issue.Project = value; }
        }

        public string Tracker
        {
            get { return this._Issue.Tracker; }
            set { this._Issue.Tracker = value; }
        }

        public string Priority
        {
            get { return this._Issue.Priority; }
            set { this._Issue.Priority = value; }
        }

        public string Title
        {
            get { return this._Issue.Title; }
            set { this._Issue.Title = value; }
        }

        public string AssignedTo
        {
            get { return this._Issue.AssignedTo; }
            set { this._Issue.AssignedTo = value; }
        }

        public string Author
        {
            get { return this._Issue.Author; }
            set { this._Issue.Author = value; }
        }

        public DateTime UpdateTime
        {
            get { return this._Issue.UpdateTime; }
            set { this._Issue.UpdateTime = value; }
        }

        public DateTime DueDate
        {
            get { return this._Issue.DueDate; }
            set { this._Issue.DueDate = value; }
        }

        #region UpdateIssueCommand
        private ViewModelCommand _UpdateIssueCommand;

        public ViewModelCommand UpdateIssueCommand
        {
            get
            {
                if (_UpdateIssueCommand == null)
                {
                    _UpdateIssueCommand = new ViewModelCommand(UpdateIssue);
                }
                return _UpdateIssueCommand;
            }
        }

        public void UpdateIssue()
        {
            this.IsUpdated = true;
            this.parentViewModel.UpdateIssue(this);
        }
        #endregion
    }
}
