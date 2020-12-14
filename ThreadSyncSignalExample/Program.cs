//csharpstudy.com 에서 참고

//스레드 동기화 중 접근 제한하는 방식이 아니라 대기중인 스레드에 신호를 보내어 스레드의 흐름을 통제하는 방식도 있음
//AutoResetEvent, ManualResetEvent, CountdownEvent, Wait/Pulse등이 있음 : Event는 OS에서 스레드 동기화에 사용하는 리소스임..

using System;
using System.Threading;
using System.Collections.Generic;

namespace ThreadSyncSignalExample
{
    class Program
    {
        //AutoResetEvent에서 활용될 필드..
        static AutoResetEvent autoEvent = new AutoResetEvent(false);
        //ManualResetEvent에서 활용될 필드
        static ManualResetEvent manualEvent = new ManualResetEvent(false);
        //CountdownEvent 객체 필드: 5개 설정
        static CountdownEvent countEvent = new CountdownEvent(5);

        static void Main(string[] args)
        {
            #region AutoResetEvent
            //이벤트를 기다리는 스레드에 신호를 보내 하나의 스레드만 통과, 다른 것들은 대기를 시키는 방식
            //스레드 A가 AutoResetEvent 객체의 WaitOne()메서드를 통해 대기
            //스레드 B가 AutoResetEvent객체의 Set()메서드를 호출> 스레드 A는 대기를 해제하고 다음으로 넘어감
            
            //스레드 생성
            Thread A = new Thread(Run);
            A.Name = "Thread A";
            A.Start();

            //메인 스레드
            Thread.Sleep(1000);
            autoEvent.Set();//대기중인 스레드에 전송!

            //신호 처리 클래스를 활용한 AutoResetEvent 예제
            Traffic traffic = new Traffic();

            //상하, 좌우 처리용 스레드 생성
            Thread v = new Thread(()=>  traffic.ProcessVertical());
            Thread h = new Thread(()=> traffic.ProcessHorizontal());
            v.Start();
            h.Start();

            //메인스레드에서 데이터 전송
            for(int i = 0; i < 30; i++)
            {
                traffic.AddVertical(new int[] { i, i + 1, i + 2 });
                traffic.AddHorizontal(new int[] { i, i + 1, i + 2 });
                Thread.Sleep(10);
            }
            Thread.Sleep(1000);
            traffic.Running = false;
            #endregion
            #region ManualResetEvent
            //AutoResetEvent는 한 번 열리면 대기중이던 스레드 중에 하나를 골라서 실행
            //ManualResetEvent는 한 번 열리면 대기중인 모든 스레드를 실행하게 됨
            //코드에서 수동으로 Reset()을 호출하여 닫고 이후 도착한 스레드들을 다시 대기하도록 함


            //스레드 생성 
            //10개 스레드 모두 manualEvent.WaitOne()으로 실행 중지 후 대기 상태로 만들기
            for (int i = 0; i < 10; i++)
            {
                new Thread(RunManual).Start(i);
            }

            //메인스레드 대기
            Thread.Sleep(1000);

            //ManualResetEvent 객체 Set()함수 호출
            //10개 스레드 모두 실행 계속
            manualEvent.Set();
            #endregion
            #region CountdownEvent
            //ManualResetEvent: 한 스레드에서 신호를 보내 여러 스레드를 통제
            //CountdownEvent: 한 스레드에서 여러 스레드로부터 신호를 받아 기다리는데 사용
            
            //10개 스레드 시작
            //10개 중에 5개만 투표가 끝나면 중지
            for(int i = 0; i < 10; i++)
            {
                new Thread(Vote).Start(i);
            }

            //신호 대기
            countEvent.Wait();

            Console.WriteLine("투표가 종료되었습니다");

            #endregion
        }
        #region AutoResetEvent에서 쓰일 함수
        static void Run()
        {
            string name = Thread.CurrentThread.Name;
            Console.WriteLine($"{name}: Run start");//시작 여부를 확인하기 위한 것...

            //AutoResetEvent 신호를 대기함
            autoEvent.WaitOne();
            Console.WriteLine($"{name}: 대기했던 실행 ");

            Console.WriteLine($"{name}: 실행 완료");
        }

        #endregion
        #region ManualResetEvent에서 쓰일 함수
        static void RunManual(object id)
        {
            Console.WriteLine($"{id} 대기중");

            //ManualResetEvent 신호 대기
            manualEvent.WaitOne();

            Console.WriteLine($"{id} 실행");
        }
        #endregion
        #region CountdownEvent에서 쓰일 함수
        static void Vote(object id)
        {
            if (countEvent.CurrentCount > 0)
            {
                //신호를 보냄.. 1씩 줄기 시작
                countEvent.Signal();

                Console.WriteLine($"{id}: 투표");
            }
            else
            {
                Console.WriteLine($"{id}:투표할 수 없습니다");
            }
        }
        #endregion
    }
    #region AutoResetEvent예시에서 활용된 클래스
    //통행처리용 이벤트 예시로 만든 클래스
    class Traffic
    {
        bool _running = true;

        //상하, 좌우 통행 신호 역할을 하게 될 AutoResetEvent들
        AutoResetEvent _eventVertical = new AutoResetEvent(true);
        AutoResetEvent _eventHorizontal = new AutoResetEvent(false);

        Queue<int> _Qvertical = new Queue<int>();
        Queue<int> _Qhorizontal = new Queue<int>();

        //상하 큐 데이터 처리
        //Vertical 방향의 처리 신호를 받으면 vertical 큐의 모든 큐 아이템을 처리
        //좌우 방향 처리 신호 시스템을 보냄
        public void ProcessVertical()
        {
            while (_running)
            {
                //상하 처리 신호 대기
                _eventVertical.WaitOne();

                //상하 처리 큐에 존재하는 모든 데이터 처리
                //큐는 다른 스레드에서도 접근 가능> lock으로 제한을
                lock (_Qvertical)
                {
                    while (_Qvertical.Count > 0)
                    {
                        int val = _Qvertical.Dequeue();
                        Console.WriteLine($"상하처리:{val}");
                    }
                }

                //좌우 처리 신호 보내기
                _eventHorizontal.Set();
            }

            Console.WriteLine("상하 처리 프로세스 완료");
        }

        //좌우 방향 큐 데이터 처리
        //Horizontal 처리 신호를 받으면 horizontal큐의 모든 큐 아이템을 처리
        //상하 처리 신호를 보내기
        public void ProcessHorizontal()
        {
            while (_running)
            {
                //좌우 처리 신호 대기
                _eventHorizontal.WaitOne();

                //좌우 처리 큐에 존재하는 모든 데이터 처리
                //역시 다른 스레드에서도 접근 가능하므로 제한을...
                lock (_Qhorizontal)
                {
                    while (_Qhorizontal.Count > 0)
                    {
                        int val = _Qhorizontal.Dequeue();
                        Console.WriteLine($"좌우 처리:{val}");
                    }
                }
                //상하 처리 신호 보내기
                _eventVertical.Set();
            }
            Console.WriteLine("좌우 처리 프로세스 완료");
        }
        //실행중이니...?
        public bool Running
        {
            get { return _running; }
            set { _running = value; }
        }

        //상하 데이터 더하기
        public void AddVertical(int[] data)
        {
            lock (_Qvertical)
            {
                foreach(var item in data)
                {
                    _Qvertical.Enqueue(item);
                }
            }
        }
        //좌우 데이터 더하기
        public void AddHorizontal(int[] data)
        {
            lock (_Qhorizontal)
            {
                foreach(var item in data)
                {
                    _Qhorizontal.Enqueue(item);
                }
            }
        }

    }
    #endregion
}
