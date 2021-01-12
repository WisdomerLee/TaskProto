using System;
//참고 사이트: https://nowonbun.tistory.com/246
//비동기 소켓 방식 중 이벤트 기반 비동기식 소켓 접속 방식
//동기 소켓 방식이 적합한 경우: 클라이언트의 행동과 무관하게 보여지는 정보가 우선: 동기식
//비동기 소켓 방식이 적합한 경우: 클라이언트의 행동에 따라 서버에 반응이 필요한 경우
//비동기 종류: TAP(Task-based Asynchronous Pattern), EAP(Event-based Asynchronous Pattern), APM(Asynchronous Programming Model) 3가지로 존재함....(3가지나 되는 것)
//TAP: async, await방식..
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkEx
{
    class Client : SocketAsyncEventArgs
    {
        //메시지는 개행으로 구분
        static char CR = (char)0x0D;
        static char LF = (char)0x0A;
        
        //기본 소켓
        Socket socket;

        //메시지용 버퍼
        StringBuilder sb = new StringBuilder();
        //접속하는 위치!
        IPEndPoint remoteAddr;

    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

}
