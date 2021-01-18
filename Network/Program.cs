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
    #region EAP방식
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

        public Client(Socket socket)
        {
            this.socket = socket;
            //버퍼 초기화(기본 크기 1024로 지정)
            base.SetBuffer(new byte[1024], 0, 1024);
            base.UserToken = socket;
            //메시지가 오면 이벤트 발생
            //IOCP로 꺼내기
            base.Completed += Client_Completed;
            //메시지가 오면 이벤트 발생
            //IOCP로 넣기
            this.socket.ReceiveAsync(this);

            remoteAddr = (IPEndPoint)socket.RemoteEndPoint;
            Console.WriteLine($"Client: (From: {remoteAddr.Address.ToString()}: {remoteAddr.Port}, Connection time: {DateTime.Now}");
            this.Send("서버에 온 것을 환영해요\r\n>");

        }
        //메시지가 도착하면 발생할 이벤트
        void Client_Completed(object sender, SocketAsyncEventArgs e)
        {
            //접속한 상태에서 뭔가 메시지가 오면
            if(socket.Connected && base.BytesTransferred>0)
            {
                //수신되는 데이터는 e.Buffer에 존재함
                byte[] data = e.Buffer;

                //수신된 데이터를 문자열로...
                string msg = Encoding.ASCII.GetString(data);

                //메모리 버퍼를 초기화
                base.SetBuffer(new byte[1024], 0, 1024);
                //버퍼의 공백을 없애고 문자열을 합산
                sb.Append(msg.Trim('\0'));
                //메시지 끝이 이스케이프 \r\n 이면 서버에 표시
                if(sb.Length>=2 && sb[sb.Length-2]== CR && sb[sb.Length-1]==LF )
                {
                    //개행문자를 없애고
                    sb.Length = sb.Length - 2;

                    //string으로 변경
                    msg = sb.ToString();

                    //출력
                    Console.WriteLine(msg);
                    
                    //Client로 메아리를 보냄
                    Send($"Echo - {msg}\r\n>");
                    //메시지가 exit라면 접속을 끊을 것
                    if("exit".Equals(msg, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("접속이 끊깁니다..");
                        socket.DisconnectAsync(this);
                        return;
                    }
                    //문자열 버퍼 비우기
                    sb.Clear();

                }
                //메시지가 오면 이벤트 발생
                this.socket.ReceiveAsync(this);
            }
            else
            {
                Console.WriteLine("접속이 되지 않았습니다");
            }
        }
        //얘도 비동기 방식으로 만들 수 있지만 그럴 필요가 없음
        //SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        public void Send(String msg)
        {
            byte[] sendData = Encoding.ASCII.GetBytes(msg);
            //sendArgs.SetBuffer(sendData, 0, SendData.Length);
            //socket.SendAsync(sendArgs);
            //클라이언트로 메시지 전송
            socket.Send(sendData, sendData.Length, SocketFlags.None);
        }
    }
    //서버 이벤트로 SocketAsyncEventArgs를 상속받음
    class Server: SocketAsyncEventArgs
    {
        Socket socket;
        public Server(Socket socket)
        {
            this.socket = socket;
            base.UserToken = socket;

            //
            base.Completed += Server_Completed;

        }
        //클라이언트가 접속하면 이벤트 발생
        void Server_Completed(object sender, SocketAsyncEventArgs e)
        {
            var client = new Client(e.AcceptSocket);
            //서버 event에 client 제거
            e.AcceptSocket = null;
            //Client로부터 Accept가 되면 이벤트를 발생
            this.socket.AcceptAsync(e);
        }

    }

    //메인 프로그램은 소켓을 상속받아 서버 소켓으로 사용
    class Program : Socket
    {
        public Program(): base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            base.Bind(new IPEndPoint(IPAddress.Any, 10000));
            base.Listen(20);
            //비동기 소켓으로 server클래스를 선언
            base.AcceptAsync(new Server(this));
        }
        static void Main(string[] args)
        {
            new Program();
            Console.WriteLine("q를 누르면 종료합니다");
            while(true)
            {
                string k = Console.ReadLine();
                if("q".Equals(k, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
        }
    }

    //클라이언트용은 이렇게..
    class ProgramClient: Socket
    {
        public ProgramClient(): base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            //접속 이벤트 생성
            var client = new Client(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000));

            //접속
            base.ConnectAsync(client);
            while(true)
            {
                string k = Console.ReadLine();
                client.Send(k + "\r\n");
                if("exit".Equals(k, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
        }
        static void Main1(string[] args)
        {
            new ProgramClient();
        }
    }
    #endregion
    #region APM방식
    //소스 코드 https://nowonbun.tistory.com/685?category=507116
    //.net framework 4.0이후버전: AsyncCallback 델리게이트로 BeginAccept가 BeginSend, BeginReceive의 대기, 송신, 수신 역할을 수행함
    #region 서버쪽
    class ClientAPM
    {
        //개행문자
        static char CR = (char)0x0D;
        static char LF = (char)0x0A;

        Socket socket;

        byte[] buffer = new byte[1024];
        StringBuilder sb = new StringBuilder();
        public ClientAPM(Socket socket)
        {
            this.socket = socket;
            //buffer로 메시지를 받고 Receive함수로 메시지가 올 때까지 대기
            this.socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Receive, this);

            var remoteAddr = (IPEndPoint) socket.RemoteEndPoint;
            Console.WriteLine($"Client: (From:{remoteAddr.Address.ToString()}:{remoteAddr.Port}, Connection time : {DateTime.Now.ToString()}");
            Send("Welcome server!\r\n>");

        }
        void Receive(IAsyncResult result)
        {
            //연결되어 있으면
            if(socket.Connected)
            {
                //EndReceive를 호출하면 데이터 크기를 얻을 수 있음
                //EndReceive는 대기를 끝냄
                int size = this.socket.EndReceive(result);
                //받은 데이터를 텍스트로 전환
                //텍스트로 변환하는 이 과정을 다른 이미지 변환 혹은 데이터 자료가 담긴 클래스 형식으로 바꾸면 다른 형태의 파일을 전송할 수도 있음
                //클라이언트에서 서버로 보낼 때도 마찬가지...그림 데이터>글자>그림데이터>글자>숫자 등과 같은 것도 순서대로 바이트 배열로 바꾸고 그걸 역으로 그림데이터를 얻고 글자>그림데이터>글자>숫자 형태로 바꾸게 되면 임의의 데이터 형태도 전송이 가능함..
                sb.Append(Encoding.ASCII.GetString(buffer, 0, size));

                if(sb.Length>=2 && sb[sb.Length-2]== CR && sb[sb.Length-1]==LF)
                {
                    //개행 삭제
                    sb.Length = sb.Length - 2;
                    //문자열변환
                    string msg = sb.ToString();
                    //콘솔에 출력
                    Console.WriteLine(msg);
                    //메아리(되돌려주기)
                    Send($"메아리 - {msg}\r\n>");
                    //메시지가 exit이면 접속 끊기

                    if("exit".Equals(msg, StringComparison.OrdinalIgnoreCase))
                    {
                        //접속 끊기 메시지
                        var remoteAddr = (IPEndPoint)socket.RemoteEndPoint;
                        Console.WriteLine($"{remoteAddr.Address.ToString()}: {remoteAddr.Port}끊김");
                        this.socket.Close();
                        return;
                    }
                    //문자열 버퍼 비우기
                    sb.Clear();
                }
                //버퍼로 메시지를 받아 Receive함수로 메시지가 올 때까지 대기
                this.socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Receive, this);

            }
        }
        //문자 보내기
        void Send(string msg)
        {
            //문자열 바이트로..
            byte[] data = Encoding.ASCII.GetBytes(msg);
            //this.socket.BeginSend(data, 0, data.Length, SocketFlags.None, Send, this);
            //클라이언트로 메시지 전송
            socket.Send(data, data.Length, SocketFlags.None);
        }
        //보내기 비동기 방식
        void Send(IAsyncResult result)
        {
            if(socket.Connected)
            {
                this.socket.EndSend(result);
            }
        }
    }
    //서버쪽 !!!
    class ProgramAPM : Socket
    {
        public ProgramAPM() : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            //포트 1만을 씀
            base.Bind(new IPEndPoint(IPAddress.Any, 10000));
            base.Listen(0);
            //비동기 소켓으로 Acept 클래스로 대기
            BeginAccept(Accept, this);

        }
        //클라이언트가 접속했을 때의 함수
        void Accept(IAsyncResult result)
        {
            //EndAccept로 접속, Client Socket을 받음, EndAcept는 대기를 끝냄
            //Client클래스 생성
            var client = new ClientAPM(EndAccept(result));
            //비동기 소켓으로 Accept 클래스로 대기
            BeginAccept(Accept, this);
        }

        static void MainAPM(string[] args)
        {
            new ProgramAPM();

            Console.WriteLine("q를 누르면 끝남");
            while(true)
            {
                string k = Console.ReadLine();
                //q를 누르면..
                if("q".Equals(k, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
        }
    }
    #endregion

    #region 클라이언트쪽
    //소켓을 상속받아 클라이언트 소켓을 활용
    class ProgramClientAPM : Socket
    {
        //메시지 구분용 개행문자
        static char CR = (char)0x0D;
        static char LF = (char)0x0A;
        //메시지를 받을 메모리용 버퍼
        byte[] buffer = new byte[1024];
        //문자열 처리(합체 분리 등등)
        StringBuilder sb = new StringBuilder();
        //생성자
        public ProgramClientAPM() : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            base.BeginConnect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000), Connect, this);
            while(true)
            {
                //콘솔로 메시지를 받으면 서버로 되돌려 보내기
                string k = Console.ReadLine();
                Send(k + "\r\n");
                if("exit".Equals(k, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
        }

        //접속하면 호출함
        void Connect(IAsyncResult result)
        {
            //접속 대기 끝내기
            base.EndConnect(result);

            //버퍼로 메시지를 받아 Receive함수로 메시지가 올 때까지 대기
            base.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Receive, this);

        }
        //메시지가 오면 호출됨
        void Receive(IAsyncResult result)
        {
            if(Connected)
            {


                int size = this.EndReceive(result);
                //문자열로 변환(보낸 쪽에 따라 달라짐 보낸 파일이 그림이면 이미지로 바꾸던가...)
                sb.Append(Encoding.ASCII.GetString(buffer, 0, size));
                if(sb.Length>=3 && sb[sb.Length-3]==CR &&sb[sb.Length-2]==LF &&sb[sb.Length-1]=='>')
                {
                    string msg = sb.ToString();

                    Console.Write(msg);
                    sb.Clear();
                }
                base.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Receive, this);

            }
        }


        void Send(string msg)
        {

            byte[] data = Encoding.ASCII.GetBytes(msg);
            Send(data, data.Length, SocketFlags.None);
        }
        //비동기 방식으로..
        void Send(IAsyncResult result)
        {
            if(base.Connected)
            {
                base.EndSend(result);
            }
        }
        static void MainClientAPM(string[] args)
        {
            //프로그램 생성...
            new ProgramClientAPM();
        }
    }
    #endregion


    #endregion
}


