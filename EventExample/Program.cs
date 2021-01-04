//csharpstudy.com참고
using System;
//이벤트: 클래스 내에 특정한 일이 일어났음을 외부의 이벤트 가입자에 알려주는 기능을 함
//event라는 키워드로 표시되며 클래스 내부에서는 필드처럼 정의됨
//이벤트 핸들러: 이벤트에 가입하는 외부 가입자 측에서는 이벤트가 발생했을 때 어떤 명령을 실행할지 지정
//이벤트 가입은 이벤트 핸들러를 += 연산자로 이벤트에 추가함
//이벤트 해지는 이벤트 핸들러를 -= 연산자로 이벤트에서 삭제
//하나의 이벤트에 여러 이벤트 핸들러를 추가할 수 있음
//이벤트는 델리게이트보다 외부에서 접근이 제한적이고 덮어쓰기가 안 되기 때문에 실수를 할 가능성이 줄어든다..
//델리게이트는 =으로 덮어 씌울 수 있고 이벤트는 =의 사용없이 +=과 -=으로 추가, 제거만 가능하기 때문

namespace EventExample
{
    #region 이벤트에서 활용할 클래스
    //클래스 내부의 이벤트 정의
    class MyButton
    {
        public string Text;
        //클릭했을 때 어떤 이벤트를 발생시켜 담아둘지를 여기에...
        public event EventHandler Click;
        //마우스 버튼을 누르면..?
        public void MouseButtonDown()
        {
            //눌이 아니라면...?
            if(this.Click != null)
            {
                //이벤트 핸들러 호출
                Click(this, EventArgs.Empty);
            }
        }
    }
    //이벤트에서는 클래스의 속성에서 get, set을 사용하는 것처럼 add, remove를 사용할 수 있음
    //event의 add, remove에서도 get, set처럼 간단한 체크 코드를 넣을 수도 있음

    //또다른 이벤트 정의
    class CustomButton
    {
        private EventHandler _click;
        public event EventHandler Click
        {
            add
            {
                //이벤트 추가
                _click += value;
                //_click = value로 쓰게 되면 하나의 함수로 덮어쓰게 됨
            }
            remove
            {
                //이벤트 제거
                _click -= value;
                
            }
        }

        public void MouseButtonDown()
        {
            if(this._click != null)
            {
                //이벤트 핸들러 호출
                _click(this, EventArgs.Empty);
            }
        }
    }
    #endregion
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

        }

        public void Run()
        {
            MyButton btn = new MyButton();
            //이벤트 핸들러로 메서드 지정, 추가

            btn.Click += new EventHandler(btn_Click);
            //이렇게도 사용 가능..
            btn.Click += btn_Click;
        }

        void btn_Click(object sender, EventArgs e)
        {
            Console.WriteLine("버튼 눌림");
        }
    }
}
