using System;
//비동기를 간단히... async, await
//async: 해당 메서드가 await를 가지고 있음을 컴파일러에 전달
//await: awaitable클래스(GetAwaiter()메서드를 갖는 클래스)면 함께 사용되어 awaitable객체가 완료될 때까지 기다리는 역할을 수행
using System.Threading;
using System.Threading.Tasks;
namespace AsyncMethodEx
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
        //웬만하면 async void 표현은 되도록 쓰지 말 것.. async Task같은 형식으로 쓰도록 하자
        //async Task: IResultAsync라는 이벤트 객체를 반환> 성공/실패가 있음..
        static async void Run()
        {
            Task<double> task = Task<double>.Factory.StartNew(() => LongCalc(10));
            //태스크의 실행이 끝나기를 기다림.. 하지만 스레드 자체는 block되지 않음
            await task;
            //갱신되면 아래의 함수들을 실행

        }

        static double LongCalc(double r)
        {
            Thread.Sleep(3000);//이것을 쓰는 이유: thread lock이 발생하거나 실행 중 무한 루프를 돌 수 있기 때문..

            return 3.14 * r * r;
        }
    }
}
