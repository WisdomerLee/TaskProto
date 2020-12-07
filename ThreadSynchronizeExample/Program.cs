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
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
