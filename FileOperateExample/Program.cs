//blog.hexabrain.net/154 참고
//파일 입출력 관련(대개 파일을 읽어오거나 쓰는 방식은 멀티스레드 방식으로 활용하는 경우가 많다(파일을 불러오는 동안 화면이 정지된다거나 입력이 먹통이 된다거나 하는 문제를 방지하기 위함)
//파일: 하나의 단위로 처리되는 서로 엮여있는 기록 묶음
//System.IO 네임스페이스 아래에 파일의 입출력 클래스들이 모여있음
//File: 파일 생성, 복사, 삭제, 이동, 열기 등을 할 수 있는 정적 메서드 제공
//FileInfo: 파일 생성, 복사, 삭제, 이동, 열기를 할 수 있는 속성, 객체(인스턴스) 메서드 제공
//FileStream: 파일에 대한 스트림을 제공하여 동기, 비동기 읽기/쓰기 작업을 지원
//StreamReader: 문자열에서 읽어오는 TextReader를 구현
//StreamWriter: TextWriter를 구현, 특정 인코딩의 스트림에 문자를 씀

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace FileOperateExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //File, FileInfo의 사용방식 차이
            #region File/FileInfo 비교
            //생성
            
            //이 방식과
            FileStream fs = File.Create("testfile.txt");
            fs.Close();//스트림을 닫지 않으면 파일에 접근이 안 됨

            //이 방식은 같은 결과를 도출해냄
            FileInfo file = new FileInfo("testfileinfo.txt");
            FileStream fsinfo = file.Create();
            fsinfo.Close();//마찬가지

            //파일 생성 부분에 FileStream이라는 클래스가 활용되었는데 얘는 입출력과정을 도와주는 중간 매개체 역할을 수행


            //복사

            File.Copy("a.txt", "b.txt");

            FileInfo src = new FileInfo("a.txt");
            FileInfo dst = src.CopyTo("b.txt");
            //삭제
            File.Delete("a.txt");

            FileInfo fileinfo2 = new FileInfo("a.txt");
            fileinfo2.Delete();

            //이동
            File.Move("a.txt", "b.txt");

            FileInfo file3 = new FileInfo("a.txt");
            file3.MoveTo("b.txt");

            

            #endregion
            #region 파일 생성
            //파일 확인
            FileStream fsa = File.Create("a.txt");

            FileInfo fileInfo = new FileInfo("b.txt");
            FileStream fsb = fileInfo.Create();

            fsa.Close();
            fsb.Close();
            File.Copy("a.txt", "c.txt");
            FileInfo dst = fileInfo.CopyTo("d.txt");
            #endregion
            #region StreamReader, StreamWriter
            //문자열을 읽고 쓰는데 중요한 클래스
            //StreamReader
            // Read: 입력 스트림의 다음 문자를 불러옴
            // ReadLine: 현재 스트림의 한줄의 문자를 읽고 데이터를 문자열로 바꾸어 반환
            // Peek: 파일 끝에 도달허가너 다른 오류가 발생하였는지 확인하기 위한 정수 반환 문자가 없으면 -1
            // Close: 스트림을 닫고 메모리 할당 해제
            //StreamWriter
            // Write: 스트림에 쓰기
            // WriteLine: 스트림에 쓰고 마지막에 다음 줄로 바꾸고 나옴
            // Close: 스트림을 닫고 메모리 할당 해제

            #endregion
            #region 파일 읽고 쓰기
            //StreamWriter
            StreamWriter sw = new StreamWriter("a.txt");
            sw.Write("sw.Write()");
            sw.Write(" sw.Write()");
            sw.WriteLine(" sw.WriteLine()");
            sw.WriteLine("sw.WriteLine()");

            sw.Close();

            //StreamReader
            StreamReader sr = new StreamReader("a.txt");

            while(sr.Peek() >= 0)//만약 파일에 문자가 있으면..?
            {
                Console.WriteLine(sr.ReadLine());//쓰기!
            }
            sr.Close();
            #endregion

        }
    }
}
