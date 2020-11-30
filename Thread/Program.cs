using System;
//스레드를 사용하기 위해 다음의 library를 사용함
//참고 csharpstudy.com에서 참고한 내용..
//csharpstudy.com________________

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
            #region 쓰레드 풀
            //ThreadPool을 만들어 쓰는 이유: 
            //Thread하나를 매번 만들어낼 때마다 컴퓨터 자원소모가 큰 편
            //특히 그 작업이 여러번 반복되는 경우라면 저 부하가 크게 ...
            //이미 다수의 Threadpool을 만들어 해당 ThreadPool에 이미 있는 Thread들을 활용하게 됨// 물론 Thread만드는 과정 자체가 매우 적은 편이거나 하면 오히려 ThreadPool을 쓰는 것이 최적화에 좋지 않을 수 있음
            //리턴 값이 없을 경우
            ThreadPool.QueueUserWorkItem(Calc); //전달된 값이 없으므로 radius = null이 됨
            ThreadPool.QueueUserWorkItem(Calc, 10.0);
            ThreadPool.QueueUserWorkItem(Calc, 20.0);

            //.Net의 쓰레드 풀: CPU코어당 최소 1개~N개의 작업 스레드를 생성하여 운영함
            //.Net 버전당 만들 수 있는 최대 스레드 수
            //.net2.0 : 25, .net 3.5: 250, .net 4.0 32bit:1023, .net 4.0 64bit: 32768
            //ThreadPool에서 쓰레드 생성: 최소 1개에서 계속 만들어서 쓰레드 풀에 생성 최대 스레드 풀 스레드만큼 생성 가능, 중간에 사용되는 스레드가 작업을 끝내고 스레드 풀로 돌아가면 재사용됨
            //최대 스레드 만큼 생성된 뒤에도 스레드 생성 요청이 있으면 요청 스레드는 생성되지 않고 대기줄에 들어가게 됨
            //스레드 생성시 요청하는 스레드 숫자가 해당 컴퓨터의 CPU보다 많으면..?CLR시스템: 스데르를 즉시 만들지 않고 초당 2개의 스레드를 생성하도록 늦추게 됨(Thread Throttling)
            //CPU코어가 4개인 컴퓨터: 60개의 스레드 요청이 들어오면 4개는 즉시, 남은 56개는 56/2초 동안 생성되게 됨
            //ThreadPool 클래스에서 디폴트 최대, 최소 숫자를 재설정할 수 있음
            //ThreadPool.SetMaxThreads(): 최대 숫자
            //ThreadPool.SetMinThreads(): 최소 숫자
            //밑 줄의 예시: 50개의 작업스레드에서 10개의 비동기 스레드가 항상 사용되는 것으로 예상되면 밑처럼 쓸 수 있음/ 미리 스레드를 생성하여 Thread Throttling현상을 미루는 것
            //ThreadPool.SetMinThreads(50, 10);
            #endregion

        }

        #region Thread에서 사용할 함수들
        //스레드 실행과 일반 함수실행을 알아보기 위한 함수
        void DoTest()
        {
            //새 스레드를 만들고 Run이라는 함수를 ThreadStart를 통해 delegate로 전달
            Thread t1 = new Thread(new ThreadStart(Run));
            //시작
            t1.Start();
            //메인 스레드에서 함수 실행
            Run();
        }
        //동작을 눈으로 확인하기 위해 만든 간단한 함수
        static void Run()
        {
            Console.WriteLine($"Thread{Thread.CurrentThread.ManagedThreadId} 시작");
            //뭔가 복잡한 로직 -파일열기/저장/이미지 전달 등...
            //3초 스레드 재우기
            Thread.Sleep(3000);
            Console.WriteLine($"Thread{Thread.CurrentThread.ManagedThreadId} 끝");

        }
        //원 넓이 계산
        static void Calc(object radius)
        {
            double r = (double)radius;
            double area = r * r * 3.14;
            Console.WriteLine($"r = {r}, area = {area}");
        }
        //단순 더하기
        static void Sum(int d1, int d2, int d3)
        {
            int sum = d1 + d2 + d3;
            Console.WriteLine(sum);
        }
        #endregion
    }
    //다른 클래스에 있는 함수를 스레드에서 불러올 때 사용할 예제로 만든 클래스
    class Helper
    {
        public void Run()
        {
            Console.WriteLine("다른 클래스에서 생성된 함수");
        }
    }
}
