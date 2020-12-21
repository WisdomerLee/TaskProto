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

namespace FileOperateExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //File, FileInfo의 사용방식 차이
            #region File/FileInfo
            //생성
            
            //이 방식과
            FileStream fs = File.Create("testfile.txt");

            //이 방식은 같은 결과를 도출해냄
            FileInfo file = new FileInfo("testfileinfo.txt");
            FileStream fsinfo = file.Create();

            //복사

            File.Copy("a.txt", "b.txt");

            FileInfo src = new FileInfo("a.txt");
            FileInfo dst = src.CopyTo("b.txt");
            //삭제
            File.Delete("a.txt");

            FileInfo fileinfo2 = new FileInfo("a.txt");
            fileinfo2.Delete();

            //이동


            #endregion
        }
    }
}
