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
            #region 쓰레드에 파라미터 전달하기
            //ThreadStart는 변수 전달 안 됨
            Thread t01 = new Thread(new ThreadStart(Run));
            t01.Start();
            //ParameterizedThreadStart라는 델리게이트를 활용
            //오브젝트 형식으로 받기 때문에 어느 형태라도 전달 가능....
            //클래스, 구조체를 만들어 객체를 만들어 전달할 수 있음(클래스의 여러 변수 들이라던가)

            Thread t02 = new Thread(new ParameterizedThreadStart(Calc));
            t02.Start();

            //ThreadStart에서 함수에 직접 파라미터 전달
            Thread t03 = new Thread(() => Sum(10, 20, 30));
            t03.Start();

            #endregion
            #region Foreground_Background
            //백그라운드 스레드로 설정: 메인스레드가 종료되면 바로 종료됨
            //기본값은 백그라운드가 아닌 foreground스레드로 실행됨, 메인스레드가 종료되더라도 foreground스레드가 살이있다면 프로세스는 계속 실행됨
            Thread t001 = new Thread(()=> { Run(); });
            t001.Start();

            Thread t002 = new Thread(() => { Run(); });
            t002.IsBackground = true;//기본값은 false
            t002.Start();
            #endregion

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
        static void Calc(object radius)
        {
            double r = (double)radius;
            double area = r * r * 3.14;
            Console.WriteLine($"r={r}, area = {area}");
        }
        static void Sum(int d1, int d2, int d3)
        {
            int sum = d1 + d2 + d3;
            Console.WriteLine(sum);
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
