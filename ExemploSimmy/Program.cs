using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Latency;
using Polly.Contrib.Simmy.Outcomes;
using Polly.Retry;
using System;
using System.Net.Sockets;

namespace ExemploSimmy
{
    class Program
    {
        static void Main(string[] args)
        {
            Policy policy = PollyWaitAndRetry();

            //Policy policy = PollyWaitAndRetryMathPow();

            //Policy policy = SimmyInjectException1();

            //Policy policy = SimmyInjectLatency();

            //Policy policy = Policy.Wrap(PollyWaitAndRetry(), SimmyInjectException1());

            //Policy policy = Policy.Wrap(PollyWaitAndRetry(), SimmyInjectException1(), SimmyInjectLatency());

            //Policy policy = Policy.Wrap(PollyWaitAndRetry(), SimmyInjectException1(), SimmyInjectException2());

            policy.Execute(() =>
            {
                //throw new SocketException(errorCode: 10013);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Ação Executada");
                Console.ForegroundColor = ConsoleColor.White;
            });

            Console.ReadKey();
        }

        private static Policy PollyWaitAndRetry()
        {
            RetryPolicy policy = Policy
                .Handle<Exception>()
                .WaitAndRetry
                (
                    sleepDurations: new TimeSpan[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(4),
                    },
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retentativa: {retryCount}");
                        Console.WriteLine($"Erro: {exception.Message}");
                        Console.WriteLine($"TimeSpan: {timeSpan}");
                        Console.WriteLine();
                    }
                );

            return policy;
        }

        private static Policy PollyWaitAndRetryMathPow()
        {
            RetryPolicy policy = Policy
                .Handle<Exception>()
                .WaitAndRetry
                (
                    retryCount: 8,
                    sleepDurationProvider: (retryCount, exception, context) =>
                    {
                        return TimeSpan.FromSeconds(Math.Pow(retryCount, 2));
                    },
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retentativa: {retryCount}");
                        Console.WriteLine($"Erro: {exception.Message}");
                        Console.WriteLine($"TimeSpan: {timeSpan}");
                        Console.WriteLine();
                    }
                );

            return policy;
        }

        private static Policy SimmyInjectException1()
        {
            var fault = new SocketException(errorCode: 10013);

            InjectOutcomePolicy chaosPolicy = MonkeyPolicy
                .InjectException(configureOptions =>
                {
                    configureOptions.Fault(fault);
                    configureOptions.InjectionRate(0.6);
                    configureOptions.Enabled(true);
                });

            return chaosPolicy;
        }

        private static Policy SimmyInjectException2()
        {
            var fault = new ApplicationException("Erro 02");

            InjectOutcomePolicy chaosPolicy = MonkeyPolicy
                .InjectException(configureOptions =>
                {
                    configureOptions.Fault(fault);
                    configureOptions.InjectionRate(0.6);
                    configureOptions.Enabled(true);
                });

            return chaosPolicy;
        }

        private static Policy SimmyInjectLatency()
        {
            InjectLatencyPolicy chaosPolicy = MonkeyPolicy
                .InjectLatency(configureOptions =>
                {
                    configureOptions.Latency(TimeSpan.FromSeconds(5));
                    configureOptions.InjectionRate(1);
                    configureOptions.Enabled(true);
                });

            return chaosPolicy;
        }
    }
}
