using System;
using System.Collections.Generic;
//csharpstudy.com --------참고
//병렬 프로그래밍(Parallel)
//병렬처리: 일거리를 나누고 나눈 작업을 병렬로 실행, 결과를 집계하는 단꼐로 진행됨
//일거리를 나누는 방식은 Data Parallelism, Taks Parallelism으로 나뉨
//Data Parallelism: 대량의 데이터를 CPU마다 일감을 나누어 동시에 병렬로 처리(처리하는 일거리는 같음)
//Task Parallelism: 큰 작업 Task를 분할, 각 스레드들이 나누어 다른 작업을 각각 처리하는 것(분업과 같이)
//Parallel Framework(PFX)는 PLINQ(Parallel LinQ), Parallel클래스(For/Foreach)를 중심으로 하는 Data Parallelism,
//Task, TaskFactory등 Task Parallelism을 지원하는 클래스로 나뉨
//Parallel.Invoke() : Task Parallelism을 지원함
using System.Threading;
using System.Threading.Tasks;
namespace Parallelism
{

    class Program
    {
        static void Main(string[] args)
        {
            //순차 실행
            for(int i = 0; i < 1000; i++)
            {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}, {i}");
            }
            Console.Read();
            //병렬 실행
            Parallel.For(0, 1000, (i) =>
            {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}, {i}");
            });
            Console.WriteLine("Hello World!");
        }
        #region DataParallelism 예시
        const int Max = 1000000;
        const int Shift = 3;
        static void SequentialEncrypt()
        {
            //테스트 데이터 설정
            string text = "I am";
            List<string> textList = new List<string>(Max);
            for(int i = 0; i <Max; i++)
            {
                textList.Add(text);
            }
            //순차 처리
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //처리에 걸린 시간을 알아보기 위한 시간 설정
            watch.Start();
            for(int i = 0; i < Max; i++)
            {
                char[] cArr = textList[i].ToCharArray();

                //문자 암호화
                for(int x =0; x < cArr.Length; x++)
                {
                    //시저 암호 방식(로마시대 암호 알파벳을 3칸 다음의 알파벳으로 치환하는 방식 ..)
                    if (cArr[x] >= 'a' && cArr[x] <= 'z')
                    {
                        cArr[x] = (char)('a' + ((cArr[x] - 'a' + Shift) % 26));
                    }
                    else if (cArr[x] >= 'A' && cArr[x] <= 'Z')
                    {
                        cArr[x] = (char)('A' + ((cArr[x] - 'A' + Shift) % 26));
                    }
                }
                textList[i] = new string(cArr);

            }
            watch.Stop();
            Console.WriteLine(watch.Elapsed.ToString());
        }
        //병렬처리
        static void ParallelEncrypt()
        {
            string text = "I am";
            List<string> textList = new List<string>(Max);
            for(int i = 0; i < Max; i++)
            {
                textList.Add(text);
            }
            //병렬처리...
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            Parallel.For(0, Max, i =>
            {
                char[] cArr = textList[i].ToCharArray();

                for(int x = 0; x < cArr.Length; x++)
                {
                    //시저 암호 방식(로마시대 암호 알파벳을 3칸 다음의 알파벳으로 치환하는 방식 ..)
                    if (cArr[x] >= 'a' && cArr[x] <= 'z')
                    {
                        cArr[x] = (char)('a' + ((cArr[x] - 'a' + Shift) % 26));
                    }
                    else if (cArr[x] >= 'A' && cArr[x] <= 'Z')
                    {
                        cArr[x] = (char)('A' + ((cArr[x] - 'A' + Shift) % 26));
                    }
                }
                textList[i] = new string(cArr);
            });
            watch.Stop();
            Console.WriteLine(watch.Elapsed.ToString());
        }
        //Data Parallelism의 예시를 보면 처리하는 작업은 완전히 동일함...
        //데이터 처리 갯수만 나누어 처리하는 것
        #endregion
    }
}
