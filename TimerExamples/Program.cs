using System;
//----csharpstudy.com----출처
//시간을 알려주는 타이머: 멀티스레딩을 지원하는 클래스: System.Threading.Timer, System.Timers.Timer
//싱글 스레드 지원 클래스: System.Windows.Forms.Timer, System.Windows.Threading.DispatcherTimer
//멀티스레딩을 지원하는 Timer: 특정 간격으로 실행되는 이벤트 핸들러를 스레드풀에서 할당된 작업스레드를 이용하여 실행, 
//이벤트 핸들러: Interval보다 더 오래 실행되면 다른 작업 스레드가 핸들러를 실행하려 함> Thread safe하게 작성해야 함(lock이라던가)
using System.IO;
using System.Net;
using System.Timers;

namespace TimerExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            #region MultiThreadingTimer
            //객체 생성
            Timer timer = new System.Timers.Timer();
            timer.Interval = 60 * 60 * 1000;//1시간 간격으로
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            #endregion
            Console.WriteLine("Hello World!");
            #region SingleThreadingTimer
            //윈폼에 있는 클래스를 이용하기 때문에 git에서 windowsform을 사용하게 되면 에러가 발생함....
            // 
            void ButtonClick(object sender, EventArgs e)
            {
                //Timer timer = new System.Windows.Forms.Timer();타이머 객체 설정만 다름
                //보다 간편히 쓰기 위한 것,,... 
                //WPF같은 경우에는 System.Windows.Threading.DispatcherTimer클래스가 있음
                //별도의 작업 스레드 생성 없이 UI 스레드에서 실행됨. UI컨트롤이나 UI text등에 직접 접근하여 활용이 가능함
                //UI스레드를 같이 쓰기 때문에 이벤트 핸들러의 사용이 길어지면 UI가 느리게 반응하는(UI Hang) 현상이 발생할 수 있음
                Timer timer = new System.Timers.Timer();
                timer.Interval = 1000;
                //timer.Tick += Timer_Tick; 윈폼에 있는 것....
                timer.Start();
            }
            #endregion
        }

        static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //웹 홈페이지의 html문서를 받아오기
            WebClient web = new WebClient();
            string webpage = web.DownloadString("");//특정 주소의 html받아 문자열로 받아오기

            string time = DateTime.Now.ToString("yyyyMMdd_hhmmss");//날짜 형식을 이러이러하게..
            string outputFile = $"page_{time}.html";
            //파일을 저장하기
            File.WriteAllText(outputFile, webpage);
            
        }
        //
        void Timer_Tick(object sender, EventArgs e)
        {
            //UI스레드에서 실행되고 추가로 UI컨트롤에 직접 접근이 가능함..

        }
    }
}
