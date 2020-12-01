using System;
//BackgroundWorker클래스: 스레드 풀에서 작업 스레드(Worker Thread)를 할당받아 작업을 실행하는 Wrapper클래스..?
//BackgroundWorker는 이벤트 기반으로 비동기 처리를 진행하는 패턴(Event-based Asynchronous Pattern)을 구현한 클래스
//BackgroundWorker가 반환하는 객체는 DoWork이벤트 핸들러로 작업할 내용을 지정, RunWorkerAsync()메서드를 호출하여 작업을 지시
//현재는 거의 쓰이지 않음....
using System.ComponentModel;

public class Etcsclasses
{
    //백그라운드로 시작될 객체 (이벤트 포함)
	BackgroundWorker worker;
    //작업에 활용될 함수
	public void Execute()
    {
        //스레드 풀에서 작업 시작
        worker = new BackgroundWorker();//객체 지정이 먼저
        //이벤트가 취소될 수 있나?
        worker.WorkerSupportsCancellation = true;
        //이벤트가 진행되는 상황을 알려줄 수 있는가?
        worker.WorkerReportsProgress = true;

        worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
        //진행상황이 변경될 때 이벤트...
        worker.ProgressChanged += new ProgressChangedEventHandler();
        //진행상황이 끝나면 발생할 이벤트
        worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler();

        worker.RunWorkerAsync();//비동기 형태로 작업을 진행

    }
    //
    void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
        //이벤트에 집어넣을 함수이므로 이벤트에서 처리할 로직이 이곳에 있어야 함
        Console.WriteLine("이벤트 처리 ");
    }
}
