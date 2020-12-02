using System;
//스레드를 사용하기 위해 다음의 library를 사용함
//참고 csharpstudy.com에서 참고한 내용..
//csharpstudy.com________________

using System.Threading;
//Task는 스레드보다 더 나중에 나온 것.. library만 보아도....

using System.Threading.Tasks;

namespace ThreadExamples
{
    class Program
    {
        #region Task<T>취소를 위한 필드
        CancellationTokenSource cancelTokenSource;
        #endregion
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
            #region 비동기: 쓰레드풀의 쓰레드를 사용하는 방식 중에 하나
            //.Net의 비동기 델리게이트: 스레드 풀의 스레드를 사용하는 방식, 메서드 델리게이트(Delegate, )의 BeginInvoke()를 이용하여 스레드 작업 시작을 요청할 수 있고 EndInvoke()를 이용하여 해당 스레드의 작업이 끝날 때까지 기다려서 리턴값을 넘겨받을 수도 있음
            //BeginInvoke(): 스레드를 구동시킨 뒤 IAsyncResult 객체를 반환,
            //IAsyncResult객체: EndInvoke()등의 메서드를 실행할 때 함수 파라미터 값으로 전달됨

            //GetArea(): 메서드 활용
            //.Net 기본 제공 Func델리게이트 활용
            //Func앞의 2개의 int는 입력, 뒤의 int는 출력용
            Func<int, int, int> work = GetArea;

            IAsyncResult asyncRes = work.BeginInvoke(10,20, null, null);
            Console.WriteLine("뭔가를 메인스레드에서 작동시키고 있는 중");
            //델리게이트 객체Func의 객체로부터 EndInvoke()실행
            //스레드가 완료되면 리턴값을 돌려받음
            int result = work.EndInvoke(asyncRes);
            Console.WriteLine($"결과 :{result}");


            #endregion
            #region Task부분
            //Task, Task<T>클래스: .Net 4.0부터 도입된 것
            //Task, Parallel클래스를 합쳐 Task Parallel Library라고 하는데 이것은 기본적으로 다중 CPU병렬처리를 기반에 두고 만든 것
            //Task: ThreadPool.QueueUserWorkItem()과 같은 기능을 제공하나 보다 더 빠르고 유연한 기능을 갖춤

            //생성과 함께 시작하게 하기..
            Task.Factory.StartNew(new Action<object>(Run), null);
            Task.Factory.StartNew(new Action<object>(Run), "1st");
            Task.Factory.StartNew(Run, "2dn");


            //생성은 미리 해두지만 실행은 나중으로 하도록 미뤄두고 싶다면..?
            //Task 생성자에 Run을 지정 Task객체 생성
            Task task1 = new Task(new Action(Run));
            //람다식을 이용하여 Task객체 생성
            Task task2 = new Task(()=>{Console.WriteLine("뭔가 긴 로직");});
            //태스크시작
            task1.Start();
            task2.Start();

            //대기
            task1.Wait();
            task2.Wait();
            //Run()이라는 함수도 있는데 이 부분은 Task에 있는 여러 옵션들을 디폴트 설정으로 두고 실행하게 하는 것....
            #endregion
            #region Task<T>부분
            //Task<T>를 이용하여 스레드 생성/시작
            Task<int> task_int = Task.Factory.StartNew(()=> CalcSize("Hello world"));
            //메인스레드에서 다른 작업을 시작한다고 가정
            Thread.Sleep(1000);

            //스레드 결과 돌려주기, 스레드가 계속 실행중이라면...? 대기
            int result = task_int.Result;
            //Task 작업 취소하는 경우...
            //비동기 작업 취소: Cancellation Token을 사용, 
            //CancellationTokenSource: 클래스, CancellationToken: 구조체
            //CancellationTokenSource: CancellationToken을 생성, Cancel 요청을 CancellationToken에 보냄
            //CancellationToken: 현재 Cancel상태를 Listener들이 모니터링하는데 사용함
            //CancellationTokenSource 필드 선언> CancellationToken 생성>비동기 작업 메서드 안에 작업 취소 확인 코드 >취소 되면 CancellationTokenSource의 Cancel메서드를 호출, 작업 취소
            //밑의 부분 참고
            var task000 = Task.Factory.StartNew<object>((state) => CalcWithStateAsync(state)
            {
                CancellationToken token = (CancellationToken)state;
                //
                if(token.IsCancellationRequested)
                {
                    return null;
                }
            }, token, //상태 param, state로 전달
                token); //CancellationToken param

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
        //넓이 계산 함수
        static int GetArea(int height, int width)
        {
            return height*width;
        }

        static int CalcSize(string data)
        {
            string s = data == null ? "": data.ToString();
            //뭔가 로직
            return s.Length;
        }
        //취소 가능한 비동기 메서드
        async void CancelableRun()
        {
            //CancellationTokenSource 객체 생성
            cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            //선택영역.. StartNew메서드는 StartNew(Func, CancellationToken)을 호출
            //Token이 Cancel상태 false이면 메서드 자체가 실행되지 않음
            var task01 = Task.Factory.StartNew<object>(LongCalsAsync, token);

            dynamic res = await task01;
            if(res != null)
            {
                Console.WriteLine("Sum:" + res.Sum);
            }
            else
            {
                Console.WriteLine("Cancelled");
            }
        }
        //윈 폼에서 스타트 버튼을 눌렀을 때 생기는 이벤트....성(콘솔 프로젝트가 아닌 윈도우폼 혹은 UWP프로젝트로 작성해야 가능한 것
        void StartClick(object sender, EventArgs e)
        {
            CancelableRun();
        }
        //윈폼에서 캔슬버튼 눌렸을 때....
        void CancelClick(object sender, EventArgs e)
        {
            cancelTokenSource.Cancel();
        }
        object LongCalsAsync()
        {
            int sum = 0;
            for(int i = 0; i<100;i++)
            {
                //작업 취소 상태 확인
                if(cancelTokenSource.Token.IsCancellationRequested)
                {
                    return null;
                }
                sum +=i;
                Thread.Sleep(1000);
            }
            return new { Sum = sum };
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
