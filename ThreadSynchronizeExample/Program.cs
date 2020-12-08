// csharpstudy.com 참고
//쓰레드 동기화(Thread Synchronization): 한 메서드가 여러 메서드에서 동시에 실행되고 해당 메서드가 클래스 객체의 필드를 읽거나 쓸 때 여러 스레드가 동시에 필드값에 접근할 수 있게 된다
//쓰레드는 순차적으로 실행되지 않으므로 메서드의 실행 결과는 예측하기 어려운 상태가 된다. 스레드들이 공유된 자원에 동시에 접근하는 것을 막고 순차적으로 접근할 수 있도록 하는 것 , 위와 같이 구현된 코드는 Thread-Safe 한 코드라고 일컫는다
//대부분의 .NET 클래스들은 Thread-Safe하지 않는데 Thread-Safe를 구현하려면 locking오버헤드와 해당 접근을 제어하는 코드가 추가로 필요하다
//대부분의 실무에서는 위와같은 경우를 많이 접하지 않기 때문..
//스레드 동기화를 위한 것: Monitor, Mutex, Semaphore, SpinLock, ReaderWriterLock, AutoResetEvent, ManualResetEvent
//키워드: lock, await
//locking으로 공유 리소스에 접근을 제한하는 것: Monitor, Mutex, Semaphore, SpinLock, ReaderWriterLock
//다른 스레드에 신호(Signal)을 보내 스레드 흐름을 제어하는 것: AutoResetEvent, ManualResetEvent, CountdownEvent등




using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace ThreadSynchronizeExample
{
    class Program
    {
        #region ThreadUnSafe
        int count = 10;
        //아래처럼 짜서는 안 됨... 예측 불가+ 환장의 조합
        public void Run()
        {
            for(int i = 0; i < 10; i++)
            {
                new Thread(UnsafeCalc).Start();
            }
        }
        
        void UnsafeCalc()
        {
            //객체필드를 스레드들이 자유롭게 접근함

            count++;
            //그리고 그 뒤에 뭔가 복잡한 로직
            Console.WriteLine(count);//확인해보기
        }
        #endregion
        #region lock
        //lock은 특정 블록의 코드(critical section)을 한 번에 하나의 스레드만 접근 가능하게 만든다
        //lock()의 파라미터에는 임의의 객체를 사용할 수 있는데 대개는 object를 활용함
        //private object obj = new object();와 같은 형태로 private 필드를 하나 선언한 뒤
        //lock(obj)처럼 활용함
        //단 lock()내부에 this 키워드는 활용하지 않을 것을 강력히 추천함
        //의도치 않은 데드락, lock granularity를 떨어뜨릴 수도 있음, 객체 전체 범위에 lock을 걸어 다수의 메서드가 필드에 접근하는 것 자체가 막혀 Lock Granualarity를 떨어뜨리고 외부에서 클래스 자체를 lock을 걸어 그 내부의 메서드를 호출하고 다시 메서드 내부에서 lock(this)를 하게 되면 외부에 잡힌 lock을 기다리다 무한히 대기하고 있는 상태가 발생할 수 있음...
        //critical section은 되도록 적게 만드는 것이 좋음
        //lock으로 잠글 객체
        private object lockObject = new object();
        public void Run2()
        {
            //여러 스레드가 동시에 실행됨
            for (int i = 0; i < 10; i++)
            {
                new Thread(SafeCalc).Start();
            }
        }
        void SafeCalc()
        {
            //한 번에 한 스레드만 lock 블록을 실행
            lock (lockObject)
            {
                count++;
            }
            //그리고 뭔가 복잡한 로직
        }
        //safe하게 처리하면 unsafe와달리 결과가 순차적으로 증가하는 형태로 출력됨....
        #endregion
        #region Monitor
        //Monitor클래스: lock처럼 특정 코드 블럭을 배타적으로 접근하게 하는 것
        //Monitor.Enter(): 특정 코드 블럭을 시작하여 한 스레드만 블럭에 접근하게 .... Monitor.Exit()은 잠금을 해제하여 다음 스레드가 접근할 수 있게 함
        //lock은 Monitor.Enter()와 Monitor.Exit()을 간단히 사용한 것이라 볼 수 있음 using()구문이 try{}finally{}의 간략한 표현이듯..
        void SafeCalc02()
        {
            //아래의 로직은 스레드 하나당 한 번에 하나씩 접근하게
            Monitor.Enter(lockObject);
            try
            {
                count++;
                //그 외 복잡한 로직

            }
            finally
            {
                //그리고 제한을 풀어낸다
                Monitor.Exit(lockObject);
            }
        }

        //또다른 것으로 Wait()과 Pulse()/PulseAll()이 있음
        //Wait()는 현재 스레드를 잠시 중지하고 lock을 풂, 다른 스레드에서 Pulse 신호가 올 때까지 대기함... 
        //다른 스레드가 lock을 획득하고 작업을 실행, Pulse()메서드를 호출하면 대기중인 스레드가 다시 lock을 얻어 다음 작업을 실행하게 됨
        //Pulse()메서드가 호출될 때 대기중인 스레드가 있으면 그 스레드가 작업을 계속하지만 대기중인 것이 없으면 Pulse신호가 사라짐
        //AutoResetEvent도 이와 비슷함. 차이가 있다면 AutoResetEvent는 Set()메서드로 펄스 신호를 보내는데 대기 중인 스레드가 없을 때에도 펄스 신호가 있었다는 것을 계속 가지고 있음...
        //Pulse()는 대기중인 하나의 스레드만 실행하지만 PulseAll()메서드는 현재 대기중인 모든 스레드를 실행하게 함

        //Monitor클래스: Wait(), Pulse()메서드를 호출하려면 해당 메서드들이 lock으로 잠긴 블럭 내에서 호출되어야 함!! 중요

        static Queue Q = new Queue();
        static object lockObj = new object();
        static bool running = true;
        
        static void Main(string[] args)
        {
            //Reader 스레드 시작
            Thread reader = new Thread(ReadQueue);
            reader.Start();

            List<Thread> threads = new List<Thread>();
            //Writer스레드 시작
            for(int i = 0; i<10; i++)
            {
                var t = new Thread(new ParameterizedThreadStart(WriteQueue));
                t.Start(i);
                threads.Add(t);
            }
            //모든 Writer스레드가 종료될 때까지 대기
            threads.ForEach(p => p.Join());

            running = false;
            //reader종료

            #endregion
            #region Mutex
            //Mutex클래스: Monitor클래스처럼 특정 코드 블럭을 배타적으로 접근시킴, Monitor는 하나의 프로세스 내에서만 사용되는 반면 Mutex는 특정 기기의 프로세스 간에 배타적 접근을 허용할 때 사용됨
            //그래서 Monitor클래스보다 50배나 느리다...
            //2개 스레드 실행
            Thread t1 = new Thread(() => MyClass.AddList(10));
            Thread t2 = new Thread(() => MyClass.AddList(20));
            t1.Start();
            t2.Start();
            //대기
            t1.Join();
            t2.Join();

            using (Mutex m = new Mutex(false, "MutexName1"))
            {
                //뮤텍스 취득을 위해 대기
                if(m.WaitOne(10))
                {
                    MyClass.MyList.Add(30);
                }
                else
                {
                    Console.WriteLine("뮤텍스 배타적 접근 권한을 얻을 수 없습니다");
                }

            }
            MyClass.ShowList();


            //또다른 것..###이게 주 용도임 위의 것은 굳이 뮤텍스를 쓸 필요 없음lock이나 Monitor를 사용하는 것이 나음..
            //하나의 기기에서 하나의 프로세스만 실행되도록..
            //GUID를 사용
            string mtxName = "@@@4-8805-0090--";
            bool createdNew;
            Mutex mtx = new Mutex(true, mtxName, out createdNew);
            //이미 해당 뮤텍스가 이미 실행중이라면..? 실행 종료
            if(!createdNew)
            {
                return;
            }
            //그것이 아니라면 실행..!
            MyApp.Launch();
            #endregion
            Console.WriteLine("Hello World!");
        }
        #region Monitor에서 사용된 메서드
        static void WriteQueue(object val)
        {
            lock(lockObj)
            {
                Q.Enqueue(val);
                Console.WriteLine($"{val}");
                Monitor.Pulse(lockObj);//lock으로 묶인 뒤에 나옴...
            }
        }
        static void ReadQueue()
        {
            while(running)
            {
                lock(lockObj)
                {
                    while(Q.Count == 0)
                    {
                        Monitor.Wait(lockObj);//lock블럭 내에서 호출!!
                    }
                    int qCount = Q.Count;
                    for(int i = 0; i<qCount;i++)
                    {
                        int val = (int)Q.Dequeue();
                        Console.WriteLine($"{val}");
                    }
                }
            }
        }
        #endregion
        #region Mutex_Class용
        public class MyClass
        {
            //뮤텍스 선언... 
            static Mutex mtx = new Mutex(false, "MutexName1");

            public static List<int> MyList = new List<int>();

            public static void AddList(int val)
            {
                //먼저 뮤텍스를 취득
                mtx.WaitOne();
                //접근권한 획득 후 실행
                MyList.Add(val);
                //접근권한 해제
                mtx.ReleaseMutex();
            }

            public static void ShowList()
            {
                MyList.ForEach(p => Console.WriteLine(0));
            }
        }
        #endregion
    }
}
