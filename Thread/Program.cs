using System;
//스레드를 사용하기 위해 다음의 library를 사용함
using System.Threading;

namespace ThreadExamples
{
    class Program
    {

        static void Main(string[] args)
        {
            #region 쓰레드 생성의 여러방식
            //메서드를 전달밭아 ThreadStart의 델리게이트 타입 객체를 만들고(new ThreadStart(Run)) Thread 클래스 생성자로 전달
            Thread t1 = new Thread(new ThreadStart(Run));
            t1.Start();
            //ThreadStart Delegate객체를 컴파일러가 Run메서드 함수 프로토 아비으로 추론하여 생성
            Thread t2 = new Thread(Run);
            t2.Start();

            //익명메서드로 생성
            Thread t3 = new Thread(delegate ()
            {
                Run();
            });
            t3.Start();
            //람다식으로 스레드 생성
            Thread t4 = new Thread(() => Run());
            t4.Start();
            //모두 합쳐서 요약식으로 하면..? >스레드를 람다식으로 만들고 바로 시작하게
            new Thread(() => Run()).Start();
            #endregion

            //만약 다른 클래스에서 생성된 함수를 스레드에서 실행해야 한다면..?
            //클래스 객체를 만들어 Thread 델리게이트에 전달하면 됨
            Helper obj = new Helper();
            Thread t = new Thread(obj.Run);
            t.Start();
        }
        void DoTest()
        {
            //새 스레드를 만들고 Run이라는 함수를 ThreadStart를 통해 delegate로 전달
            Thread t1 = new Thread(new ThreadStart(Run));
            //시작
            t1.Start();
            //메인 스레드에서 함수 실행
            Run();
        }

        static void Run()
        {
            Console.WriteLine($"Thread{Thread.CurrentThread.ManagedThreadId} 시작");
            //뭔가 복잡한 로직 -파일열기/저장/이미지 전달 등...
            //3초 스레드 재우기
            Thread.Sleep(3000);
            Console.WriteLine($"Thread{Thread.CurrentThread.ManagedThreadId} 끝");

        }
    }
    class Helper
    {
        public void Run()
        {
            Console.WriteLine("다른 클래스에서 생성된 함수");
        }
    }
}
