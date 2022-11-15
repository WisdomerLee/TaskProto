using System;
//비동기를 간단히... async, await
//async: 해당 메서드가 await를 가지고 있음을 컴파일러에 전달
//await: awaitable클래스(GetAwaiter()메서드를 갖는 클래스)면 함께 사용되어 awaitable객체가 완료될 때까지 기다리는 역할을 수행


using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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
        //긴 처리 함수
        static double LongCalc(double r)
        {
            Thread.Sleep(3000);//이것을 쓰는 이유: thread lock이 발생하거나 실행 중 무한 루프를 돌 수 있기 때문..

            return 3.14 * r * r;
        }

        #region 추가 async await내용
        //https://velog.io/@jinuku/C-async-await%EB%A5%BC-%EC%9D%B4%EC%9A%A9%ED%95%98%EC%97%AC-%EB%B9%84%EB%8F%99%EA%B8%B0%EB%A5%BC-%EB%8F%99%EA%B8%B0%EC%B2%98%EB%9F%BC-%EA%B5%AC%ED%98%84%ED%95%98%EA%B8%B0-1

        //장점: 스레드 안정성 보장, 동시성 구현
// async await 키워드를 활용하는 장점: 동기적 방식으로 코드를 구사하고 비동기 형태로 바꾸기 쉬움, 코드 형식도 간단...
// 코드상의 내용
// var 결과 = await 표현식; 
// 그외 함수 문장;
// >컴파일러가 바꾸는 코드
// var awaiter = 표현식.GetAwaiter();
// awaiter.OnCompleted(()=>{
//     var 결과 = awaiter.GetResult();
//   });

        //소수를 세는 비동기 메서드
        Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(()=>
                ParallelEnumerable.Range(start, count).Count( n =>
                    Enumerable.Range(2, (int) Math.Sqrt(n)-1).All( i => n % i >0)));
        }
        //소수를 세는 비동기 메서드를 호출하는 것 : async가 함수 선언에 포함되어야 하고(void나 return 되는 형식 앞에) 함수 내부에서 await가 선언되어야 함 async로 쓰인 함수 본문에 await가 없으면... 동기함수처럼 처리됨
        public async void DisplayPrimesCount()
        {
            //await 표현식이 하나의 int값으로 평가됨
            int result = await GetPrimesCountAsync(2, 1000000);
            //비동기로 대기시켜야 할 함수의 리턴값이 void면....?
            await Task.Delay(5000);//값이 없이 await 함수 이런식으로 쓰면 됨....

            Console.WriteLine(result);
        }
        //void 리턴인 함수를 Task리턴 함수로 바꾸고 async를 적용시키면 await적용 가능한 비동기 함수가 됨
        async Task PrintAnswer()
        {
            //5초 대기
            await Task.Delay(5000);
            int answer = 21 * 2;
            Console.WriteLine(answer);
        }
        //비동기 함수를 처리하기 위해서....?
        //아래의 함수도 비동기 await로 비동기 처리를 할 수 있음 > 연쇄 비동기 함수 호출 가능....
        async Task Go()
        {
            await PrintAnswer();
            Console.WriteLine("완료");
        }
        //반환형식이 void가 아닌 함수를 비동기화 하려면.. TResult형식이면  Task<TResult> 반환으로 바꾸어주면 됨
        async Task<int> GetAnswer()
        {
            await Task.Delay(5000);
            int answer = 21 *2;
            return answer;
        }
        //비동기로 바꾸는 형태가 의외로 간단...>

        //비동기 람다식

        Func<Task> unnamed = async () =>
        {
            await Task.Delay(5000);
            Console.WriteLine("작동");
        };
        //호출하는 방법도 동일함
        async Task CallUnname()
        {
            //
            await unnamed();
            int answer = await unnamed1();
        }
        //리턴값이 있는 비동기 람다식
        Func<Task<int>> unnamed1 = async () =>
        {
            await Task.Delay(2000);
            return 123
        };

        #endregion
    }
}


