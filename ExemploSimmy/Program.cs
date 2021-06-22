using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;
using Polly;
using System;
using System.Net.Sockets;
using Polly.Retry;

namespace ExemploSimmy
{
    class Program
    {
        static void Main(string[] args)
        {
            Policy policy = PollyWaitAndRetry();

            //Policy policy = PollyWaitAndRetryMathPow();

            //Policy policy = SimmyInjectException();

            //Policy policy = Policy.Wrap(PollyWaitAndRetry(), SimmyInjectException1());

            //Policy policy = Policy.Wrap(PollyWaitAndRetry(), SimmyInjectException1(), SimmyInjectException2());

            policy.Execute(() => 
            {
                throw new SocketException(errorCode: 10013);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Ação Executada");
                Console.ForegroundColor = ConsoleColor.White;
            });

            Console.ReadKey();
        }

        private static Policy ComoMontarUmaPolicy()
        {
            var policy = Policy
                .Handle<ApplicationException>() // Handler
                .Or<NotSupportedException>() // Outro Handler
                .WaitAndRetry // Pattner (Retry, WaitAndRetry, CircuitBreaker, Fallback, RetryForever)
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
            InjectOutcomePolicy chaosPolicy = MonkeyPolicy.InjectException(with =>
                with.Fault(fault)
                    .InjectionRate(0.4)
                    .Enabled(true)
                );

            return chaosPolicy;
        }

        private static Policy SimmyInjectException2()
        {
            var fault = new ApplicationException("Erro 02");
            InjectOutcomePolicy chaosPolicy = MonkeyPolicy.InjectException(with =>
                with.Fault(fault)
                    .InjectionRate(0.5)
                    .Enabled(true)
                );

            return chaosPolicy;
        }
    }
}
