﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab16
{

    class Customer
    {
        public async Task TakeFromStore(BlockingCollection<string> bc)
        {
            Random rand = new Random();
            if (bc.Count == 0)
            {
                Console.WriteLine("Склад пуст, соре");
                Thread.Sleep(rand.Next(1000));
            }
            else
            {
                bc.Take();
                Console.WriteLine("Товар взят со склада.");
                Thread.Sleep(rand.Next(1000));
            }
        }
    }
    class Provider
    {
        public string thing;
        public int timing;
        public Provider(string s, int i)
        {
            thing = s;
            timing = i;
        }
        public async Task AddToStore(BlockingCollection<string> bc)
        {
            await Task.Run(() =>
            {
                bc.Add(this.thing);
                Console.WriteLine("Добавлен товар: " + thing);
                Thread.Sleep(timing);
            });

        }
    }

    class Program
    {
        static void Algorithm(int a,int b)
        {
            while (b != 0)
            { 
                b = a % (a = b);
                Console.WriteLine(b);
                Thread.Sleep(1000);
            }
            Console.WriteLine("НОД:"+a);
        }

        static void Algorithm2(int a, int b,CancellationToken token)
        {
            while (b != 0)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Операция прервана токеном");
                    return;
                }
                b = a % (a = b);
                Console.WriteLine(b);
                Thread.Sleep(1000);
            }
            Console.WriteLine("НОД:" + a);
        }


        static void ForAsyncMethod()
        {
            for(int i = 1; i < 11; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(300);
            }
           
        }

        static async void AsyncMethod()
        {
            await Task.Run(() => ForAsyncMethod());
        }

        static void Display(Task t)
        {
            Console.WriteLine("Id задачи: {0}", Task.CurrentId);
            Console.WriteLine("Id предыдущей задачи: {0}", t.Id);

        }

        static public void CreateBigArr(int x)
        {

            Random rand = new Random();
            int[] arr = new int[1000000];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = rand.Next(10);
            }
            Console.WriteLine("Выполнена задача номер " + x);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Первое задание:");
            Stopwatch stopwatch = new Stopwatch();
            Task algorithmTask = new Task(()=>Algorithm(2500,24354));
            stopwatch.Start();
            algorithmTask.Start();
            
            Thread.Sleep(500);
            Console.WriteLine("Статус задачи: " + algorithmTask.Status);
            Console.WriteLine("Завершена ли задача: "+algorithmTask.IsCompleted);
            algorithmTask.Wait();
            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed;
            Console.WriteLine("Время выполнения: "+timeSpan);
            Console.ReadKey();

            Console.WriteLine("\nВторое задание:");
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            Task algorithmTask2 = new Task(() => Algorithm2(2500, 24354, cancellationToken));
            algorithmTask2.Start();
            Thread.Sleep(2500);
            cancellationTokenSource.Cancel();
            Console.ReadKey();

            int z = 0;
            Func<int> func = () =>
            {

                return ++z;
            };

            Console.WriteLine("\nТретье задание:");
            Task<int> returnOne = new Task<int>(func);
            Task<int> returnTwo = new Task<int>(func);
            Task<int> returnThree = new Task<int>(func);

            returnOne.Start();
            returnTwo.Start();
            returnThree.Start();

            Func<int> Factorial = () =>
            {
                return returnOne.Result * returnTwo.Result * returnThree.Result;
            };

            Task<int> resultTask = new Task<int>(Factorial);

            resultTask.Start();

            Console.WriteLine("Result = " + resultTask.Result);
            Console.ReadKey();

            Console.WriteLine("\nЧетвертое задание(1): ");
            Task taskContOne = new Task(() => {
                Console.WriteLine("Id задачи: {0}", Task.CurrentId);
            });
            Task taskContTwo = taskContOne.ContinueWith(Display);
            taskContOne.Start();
            Console.ReadKey();

            Console.WriteLine("\nЧетвертое задание(2): ");
            Random rand = new Random();
            Task<int> what = Task.Run(() => rand.Next(10) * rand.Next(10));

            var awaiter = what.GetAwaiter();

            awaiter.OnCompleted(() =>
            {
                Console.WriteLine("Result: " + awaiter.GetResult());
            });
            Console.ReadKey();

            Console.WriteLine("\nПятое задание:");

            stopwatch.Restart();
            Parallel.For(0, 5, CreateBigArr);
            stopwatch.Stop();
            Console.WriteLine("Время при Parallel.For:  " + stopwatch.Elapsed + '\n');

            stopwatch.Restart();
            for (int j = 0; j < 5; j++)
            {
                int[] arr = new int[1000000];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = rand.Next(10);
                }
                Console.WriteLine("Выполнена задача номер " + j);
            }
            stopwatch.Stop();
            Console.WriteLine("Время при For: " + stopwatch.Elapsed + '\n');
            Console.WriteLine();

            List<int> list = new List<int>() { 1, 2, 3, 4, 5 };

            stopwatch.Restart();
            ParallelLoopResult result = Parallel.ForEach<int>(list, CreateBigArr);
            stopwatch.Stop();
            Console.WriteLine("Время при Parallel.Foreach: " + stopwatch.Elapsed + '\n');

            stopwatch.Restart();
            foreach (int x in list)
            {
                int[] arr = new int[1000000];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = rand.Next(10);
                }
                Console.WriteLine("Выполнена задача номер " + x);
            }
            stopwatch.Stop();
            Console.WriteLine("Время при Foreach: " + stopwatch.Elapsed + '\n');
            Console.ReadKey();

            Console.WriteLine("\nШестое задание:");
            Parallel.Invoke(() => Algorithm(20,255), () => Display(algorithmTask));
            Console.ReadKey();

            Console.WriteLine("\nСедимое задание:");
            BlockingCollection<string> bc = new BlockingCollection<string>(20);
            Provider p1 = new Provider("Пылесос", 10);
            Provider p2 = new Provider("Магнитофон", 300);
            Provider p3 = new Provider("Микроволновка", 500);
            Provider p4 = new Provider("Холодильник", 700);
            Provider p5 = new Provider("Телевизор", 900);
            Customer c1 = new Customer(); Customer c2 = new Customer(); Customer c3 = new Customer(); Customer c4 = new Customer(); Customer c5 = new Customer();
            Customer c6 = new Customer(); Customer c7 = new Customer(); Customer c8 = new Customer(); Customer c9 = new Customer(); Customer c10 = new Customer();
            Customer[] carr = { c1, c2, c3, c4, c5, c6, c7, c8, c9, c10 };
            Task bcTask = Task.Run(async () =>
            {
                while (bc.Count != bc.BoundedCapacity)
                {
                    Parallel.Invoke(
                        () => p1.AddToStore(bc),
                        () => p2.AddToStore(bc),
                        () => p3.AddToStore(bc),
                        () => p4.AddToStore(bc),
                        () => p5.AddToStore(bc),
                        () => c1.TakeFromStore(bc),
                        () => c2.TakeFromStore(bc),
                        () => c3.TakeFromStore(bc),
                        () => c4.TakeFromStore(bc),
                        () => c5.TakeFromStore(bc),
                        () => c6.TakeFromStore(bc),
                        () => c7.TakeFromStore(bc),
                        () => c8.TakeFromStore(bc),
                        () => c9.TakeFromStore(bc),
                        () => c10.TakeFromStore(bc)
                        );
                }
                Console.WriteLine("Склад полон");
            });
            Console.Write("\n\n");
            Console.ReadKey();

            Console.WriteLine("\nВосьмое задание:");
            AsyncMethod();
            for(int i = 11; i < 21; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(500);
            }
        }
    }
}
