using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Game
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    };

    public class Snake
    {
        private int WIDTH;
        private int HEIGHT;
        private Point[] snake;
        private Point apple;
        private Direction direction;
        private int score;
        private Point prevTail;
        private delegate void ConsoleKeyEventHandler();
        private event ConsoleKeyEventHandler KeyPressEvent;

        private static Snake instanceSnake;

        private static readonly object lock_instance = new object ();
        public static Snake GameSnake
        {
            get
            {
                lock (lock_instance)
                {
                    if (instanceSnake == null)
                    {
                        instanceSnake = new Snake();
                    }
                    return instanceSnake;

                }
            }
        }

        private Snake()
        {
            WIDTH = 30;
            HEIGHT = 10;
            this.snake =new Point[]{
                new Point(WIDTH/2 + 2, HEIGHT /2),
                new Point(WIDTH/2 + 1, HEIGHT /2),
                new Point(WIDTH/2, HEIGHT /2),
                new Point(WIDTH/2 - 1, HEIGHT /2),
                new Point(WIDTH/2 - 2, HEIGHT /2),
            };
            apple = NewRandomPoint();
            direction = Direction.Right;
            score = 0;
        }

        /// <summary>
        /// vẽ con rắn lên màn hình
        /// </summary>
        private void DrawSnake()
        {
            foreach (var point in snake)
            {
                Console.SetCursorPosition(point.X, point.Y);
                Console.Write("*"); // Đây là ký tự đại diện cho mỗi phần của con rắn
            }
        }

        /// <summary>
        /// Phần màn hình để chơi
        /// </summary>
        private void DrawBox()
        {
             //WIDTH và HEIGHT là kích thước hộp được đặt trước
            for (int i = 0; i < WIDTH; i++)
            {
                Console.Write("=");
            }
            Console.SetCursorPosition(0, HEIGHT);
            for (int i = 0; i <= WIDTH; i++)
            {
                Console.Write("=");
            }
            for (int i = 0; i < HEIGHT; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("=");
            }
            for (int i = 0; i < HEIGHT; i++)
            {
                Console.SetCursorPosition(WIDTH, i);
                Console.Write("=");
            }

        }

        /// <summary>
        /// tạo ngẫu nhiên mồi, ở đây gọi là quả táo
        /// </summary>
        private void GenApple(bool isNewApple = true)
        {
            if(isNewApple)
            {
                
                apple = NewRandomPoint();
                // Sau khi có tọa độ quả táo thì vẽ lên màn hình
                Console.SetCursorPosition(apple.X, apple.Y);
                Console.Write("*");
            }
            else
            {
                Console.SetCursorPosition(apple.X, apple.Y);
                Console.Write("*");
            }
            
        }

        /// <summary>
        /// tạo vị trí ngấu nhiên
        /// </summary>
        /// <returns></returns>
        private Point NewRandomPoint()
        {
            Random rnd = new Random();
            int x = rnd.Next((WIDTH - 1)) + 1;
            int y = rnd.Next((HEIGHT - 1)) + 1;
            return new Point(x, y);
        }

        /// <summary>
        /// Táo đã ăn chưa
        /// </summary>
        /// <returns></returns>
        private bool IsEatApple()
        {
            bool isEatApple = snake[0].X == apple.X && snake[0].Y == apple.Y;
            if(isEatApple)
            {
                this.score += 1;
            }
            return isEatApple;
        }

        /// <summary>
        /// Nếu đã ăn táo thì tăng chiều dài rắn lên
        /// </summary>
        private void Growing()
        {
            snake = snake.Concat(new Point[] { prevTail }).ToArray();
        }

        /// <summary>
        /// khi con rắn di chuyển
        /// </summary>
        private void Move()
        {
            // lưu phần đuôi cũ lại
            prevTail = snake[snake.Length - 1];

            // code gores here
            for (int i = snake.Length - 1; i > 0; i--)
                snake[i] = snake[i - 1];
            if (direction == Direction.Up)
                snake[0].Y -= 1;
            else if (direction == Direction.Down)
                snake[0].Y += 1;
            else if (direction == Direction.Left)
                snake[0].X -= 1;
            else if (direction == Direction.Right)
                snake[0].X += 1;
        }

        /// <summary>
        ///  Vẽ đầu mới
        /// </summary>
        private void DrawHeadnTail()
        {
            //di chuyển
            this.Move();
            Console.SetCursorPosition(snake[0].X, snake[0].Y);
            Console.Write("*");
            // vẽ phần đầu mới
            Point tail = snake[snake.Length - 1];
            Console.SetCursorPosition(prevTail.X, prevTail.Y);
            Console.Write(" "); // xóa phần đuôi cũ đi
        }

        /// <summary>
        /// Nhận sự hiện khi nhấn phím để set hướng di chuyển
        /// </summary>
        /// <param name="keyInfo"></param>
        private void HandleKeyPressEvent()
        {
            // Đọc phím vừa nhấn
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    if (direction != Direction.Right)
                    {
                        direction = Direction.Left;
                    }
                    break;
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    if (direction != Direction.Down)
                    {
                        direction = Direction.Up;
                    }
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    if (direction != Direction.Up)
                    {
                        direction = Direction.Down;
                    }
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    if (direction != Direction.Left)
                    {
                        direction = Direction.Right;
                    }
                    break;
                case ConsoleKey.Q: // Quit game
                                   // Handle game exit logic if needed
                    break;
            }
        }

        /// <summary>
        /// Kiểm tra xem con rắn có đụng tường không
        /// </summary>
        /// <returns></returns>
        private bool IsHitWall()
        {
            return snake[0].X == 0 || snake[0].Y == 0 || snake[0].X == WIDTH || snake[0].Y == HEIGHT;
        }

        /// <summary>
        /// Kiểm tra xem con rắn có tự cắn mình không
        /// </summary>
        /// <returns></returns>
        bool IsBiteItself()
        {
            Point head = snake[0];
            for (int i = 1; i < snake.Length; i++)
                if (head.X == snake[i].X && head.Y == snake[i].Y)
                    return true;
            return false;
        }

        /// <summary>
        /// Khởi động game, truyền vào level games
        /// </summary>
        public void PlayGame(int level)
        {
            KeyPressEvent += HandleKeyPressEvent;
            //vẽ hộp chơi
            DrawBox();
            //vẽ con rắn
            DrawSnake();
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    KeyPressEvent?.Invoke();
                }
                // nếu đâm đầu vào tường hoặc nếu tự cắn vào người
                if (IsHitWall() || IsBiteItself())
                {
                    Console.Clear();
                    Console.Write(String.Format("Game over! Your score is {0}", score));
                    break;
                }
                //vẽ mồi hiện tại
                GenApple(false);
                //rắn liên tục di chuyển
                DrawHeadnTail();
                //nếu rắn ăn mồi
                if (IsEatApple())
                {
                    Growing();
                    //vẽ mồi mới
                    GenApple();
                }
                // tốc độ của con rắn
                Thread.Sleep(500 / level); // set ở đây mặc định là 0.5s
                //Console.Clear();
            }
        }

    }
}
