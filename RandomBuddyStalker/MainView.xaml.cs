using Avalonia.ReactiveUI;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Reactive.Linq;

namespace ReactiveAvalonia.RandomBuddyStalker {
    public class MainView : ReactiveWindow<MainViewModel> {
        private TextBlock tblBuddyInfo => this.FindControl<TextBlock>("tblBuddyInfo");
        private Button btnStalkBuddy => this.FindControl<Button>("btnStalkBuddy");

        public MainView() {
            ViewModel = new MainViewModel();

            this
                .WhenActivated(
                    disposables => {
                        Console.WriteLine(
                            $"[v  {Thread.CurrentThread.ManagedThreadId}]: " +
                            "View activated");

                        Disposable
                            .Create(
                                () => 
                                    Console.WriteLine(
                                        $"[v  {Thread.CurrentThread.ManagedThreadId}]: " +
                                        "View deactivated"))
                            .DisposeWith(disposables);

                        Observable
                            .Interval(TimeSpan.FromSeconds(1))
                            .Take(1)
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Subscribe(
                                _ => {
                                    Console.WriteLine(
                                        $"--[v  {Thread.CurrentThread.ManagedThreadId}]: " +
                                        $"{tblBuddyInfo.Text}");
                                    Console.WriteLine(
                                        $"--[v  {Thread.CurrentThread.ManagedThreadId}]: " +
                                        $"{btnStalkBuddy.Name}");
                                },
                                err => Console.WriteLine($"error: {err}"),
                                () => { Console.WriteLine("Done with the introductions..."); })
                            .DisposeWith(disposables);
                    });

            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
