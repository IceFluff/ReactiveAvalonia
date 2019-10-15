﻿using System;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading;
using ReactiveUI;

namespace ReactiveAvalonia.HelloWorld {

    // https://reactiveui.net/docs/handbook/when-activated/
    // https://reactiveui.net/docs/handbook/data-binding/avalonia
    // http://avaloniaui.net/docs/reactiveui/activation#activation-example
    // Note that ISupportsActivation was renamed to IActivatableViewModel
    public class MainViewModel : ReactiveObject, IActivatableViewModel {
        public ViewModelActivator Activator { get; }

        // https://reactiveui.net/docs/handbook/view-models/#read-write-properties
        // https://reactiveui.net/docs/handbook/view-models/boilerplate-code
        private string _greeting;
        public string Greeting {
            get => _greeting;
            set => this.RaiseAndSetIfChanged(ref _greeting, value);
        }

        public MainViewModel() {
            // https://reactiveui.net/docs/handbook/when-activated/
            Activator = new ViewModelActivator();

            this.WhenActivated(
                disposables => {
                    // https://github.com/kentcb/YouIandReactiveUI/blob/master/ViewModels/Samples/Chapter%2018/Sample%2004/ChildViewModel.cs
                    Console.WriteLine($"[vm {Thread.CurrentThread.ManagedThreadId}]: ViewModel activated");

                    // https://reactiveui.net/docs/guidelines/framework/ui-thread-and-schedulers
                    Observable
                        .Interval(TimeSpan.FromSeconds(1))
                        .Take(Adjectives.Length)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Do(
                            t => {
                                var newGreeting = $"Hello, {Adjectives[t % Adjectives.Length]} world !";
                                Console.WriteLine("\n[vm {0}]: Interval Observable ->  Setting greeting to: \"{1}\"",
                                    Thread.CurrentThread.ManagedThreadId, newGreeting);
                                Greeting = newGreeting;
                            },
                            () => Console.WriteLine("\nThose are all the greetings, folks! " +
                                "Feel free to close the window now...\n"))
                        .Subscribe()
                        .DisposeWith(disposables);

                    // https://github.com/kentcb/YouIandReactiveUI/blob/master/ViewModels/Samples/Chapter%2018/Sample%2004/ChildViewModel.cs
                    // Nothing other than logging the ViewModel's deactivation
                    Disposable
                        .Create(() => Console.WriteLine($"[vm {Thread.CurrentThread.ManagedThreadId}]: ViewModel deactivated"))
                        .DisposeWith(disposables);
                });

            // https://reactiveui.net/docs/handbook/when-any/#basic-syntax
            // https://reactiveui.net/docs/guidelines/framework/dispose-your-subscriptions
            this
                .WhenAnyValue(vm => vm.Greeting)
                .Skip(1)
                .Do(name => Console.WriteLine($"[vm {Thread.CurrentThread.ManagedThreadId}]: WhenAnyValue() -> Greeting value changed to: \"{name}\""))
                .Subscribe();
        }

        private static readonly string[] Adjectives = {
            "expressive",
            "clear",
            "responsive",
            "vibrant",
            "concurrent",
            "reactive"
        };
    }
}
