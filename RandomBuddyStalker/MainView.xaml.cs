using Avalonia.ReactiveUI;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Reactive.Linq;
using Avalonia.Media.Imaging;

namespace ReactiveAvalonia.RandomBuddyStalker {
    public class MainView : ReactiveWindow<MainViewModel> {
        private MainViewModel _vm;
        public MainView() {
            ViewModel = _vm = new MainViewModel();

            //var startAnimationCommand = ReactiveCommand.CreateFromObservable(
            //        () =>
            //            Observable
            //                .Interval(TimeSpan.FromMilliseconds(200))
            //                .TakeUntil(
            //                    _vm
            //                        .TriggeringTheTimer
            //                        .Where(trigger => trigger == MainViewModel.TimerTrigger.Stop))
            //                );

            this
                .WhenActivated(
                    disposables => {
                        Console.WriteLine(
                            $"[v  {Thread.CurrentThread.ManagedThreadId}]: " +
                            "View activated" + '\n');

                        this
                            .OneWayBind(_vm, vm => vm.BuddyName, v => v.tblBuddyName.Text)
                            .DisposeWith(disposables);

                        this
                            .OneWayBind(_vm, vm => vm.BuddyAvatar, v => v.imgAvatar.Source)
                            .DisposeWith(disposables);

                        this
                            .BindCommand(_vm, vm => vm.StalkCommand, v => v.btnStalk)
                            .DisposeWith(disposables);
                        
                        this
                            .BindCommand(_vm, vm => vm.ContinueCommand, v => v.btnContinue)
                            .DisposeWith(disposables);

                        this
                            .WhenAnyValue(v => v._vm.IsTimerRunning)
                            .Do(running => {
                                btnStalk.IsVisible = running;
                                btnContinue.IsVisible = !running;

                                pbRemainingTime.IsVisible = running;
                                
                                //brdAvatar.IsVisible = !running;
                            })
                            .Subscribe()
                            .DisposeWith(disposables);

                        this
                            .WhenAnyObservable(v => v._vm.TriggeringTheTimer)
                            .Where(trigger => trigger == MainViewModel.TimerTrigger.Start)
                            .Do(trigger => {
                                const int divisionsCount = 40;
                                int divisionSpan = MainViewModel.DecisionTimeMilliseconds / divisionsCount;
                                Observable
                                    .Timer(
                                        TimeSpan.FromMilliseconds(0),
                                        TimeSpan.FromMilliseconds(divisionSpan),
                                        RxApp.MainThreadScheduler)
                                    .TakeWhile(item => item < divisionsCount && _vm.IsTimerRunning)
                                    .Subscribe(divisionsSoFar => {
                                        int remainingTime =
                                                MainViewModel.DecisionTimeMilliseconds -
                                                divisionSpan * (int)divisionsSoFar;

                                        pbRemainingTime.Value = remainingTime;
                                    });
                            })
                            .Subscribe()
                            .DisposeWith(disposables);

                        // https://reactiveui.net/docs/handbook/events/#how-do-i-convert-my-own-c-events-into-observables
                        Observable
                            .FromEventPattern(wndMain, nameof(wndMain.Closing))
                            .Subscribe(
                                _ => {
                                    Console.WriteLine(
                                        $"[v  {Thread.CurrentThread.ManagedThreadId}]: " +
                                        "Main window closing...");
                                })
                            .DisposeWith(disposables);

                        Disposable
                            .Create(
                                () =>
                                    Console.WriteLine(
                                        $"[v  {Thread.CurrentThread.ManagedThreadId}]: " +
                                        "View deactivated"))
                            .DisposeWith(disposables);
                    });

            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            pbRemainingTime.Maximum = MainViewModel.DecisionTimeMilliseconds;
        }

        private Window wndMain => this.FindControl<Window>("wndMain");
        private TextBlock tblBuddyName => this.FindControl<TextBlock>("tblBuddyName");
        private Button btnStalk => this.FindControl<Button>("btnStalk");
        private Button btnContinue => this.FindControl<Button>("btnContinue");
        private ProgressBar pbRemainingTime => this.FindControl<ProgressBar>("pbRemainingTime");
        private Border brdAvatar => this.FindControl<Border>("brdAvatar");
        private Image imgAvatar => this.FindControl<Image>("imgAvatar");
    }
}
