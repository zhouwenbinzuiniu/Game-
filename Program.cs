using System;
using System.Collections.Generic;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        // 游戏区域大小
        private static int width = 20;
        private static int height = 20;

        // 蛇的位置和方向
        private static List<Position> snake = new List<Position>();
        private static Direction direction = Direction.Right;

        // 食物位置
        private static Position food;

        // 游戏状态
        private static bool gameOver = false;
        private static int score = 0;

        // 方向枚举
        enum Direction
        {
            Up, Down, Left, Right
        }

        // 位置结构
        struct Position
        {
            public int X;
            public int Y;

            public Position(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        static void Main(string[] args)
        {
            // 初始化游戏
            InitializeGame();

            // 游戏主循环
            while (!gameOver)
            {
                // 处理用户输入
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    ChangeDirection(key);
                }

                // 更新游戏状态
                UpdateGame();

                // 绘制游戏
                DrawGame();

                // 控制游戏速度
                Thread.Sleep(150);
            }

            // 游戏结束
            Console.Clear();
            Console.WriteLine("游戏结束!");
            Console.WriteLine($"最终得分: {score}");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }

        // 初始化游戏
        private static void InitializeGame()
        {
            // 设置控制台窗口大小
            Console.SetWindowSize(width + 2, height + 4);
            Console.SetBufferSize(width + 2, height + 4);

            // 初始化蛇
            snake.Clear();
            snake.Add(new Position(width / 2, height / 2));
            direction = Direction.Right;

            // 生成第一个食物
            GenerateFood();

            // 隐藏光标
            Console.CursorVisible = false;
        }

        // 生成食物
        private static void GenerateFood()
        {
            Random random = new Random();
            bool validPosition;

            do
            {
                validPosition = true;
                food = new Position(random.Next(1, width - 1), random.Next(1, height - 1));

                // 确保食物不会生成在蛇身上
                foreach (Position segment in snake)
                {
                    if (segment.X == food.X && segment.Y == food.Y)
                    {
                        validPosition = false;
                        break;
                    }
                }
            } while (!validPosition);
        }

        // 改变蛇的方向
        private static void ChangeDirection(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (direction != Direction.Down)
                        direction = Direction.Up;
                    break;
                case ConsoleKey.DownArrow:
                    if (direction != Direction.Up)
                        direction = Direction.Down;
                    break;
                case ConsoleKey.LeftArrow:
                    if (direction != Direction.Right)
                        direction = Direction.Left;
                    break;
                case ConsoleKey.RightArrow:
                    if (direction != Direction.Left)
                        direction = Direction.Right;
                    break;
                case ConsoleKey.Escape:
                    gameOver = true;
                    break;
            }
        }

        // 更新游戏状态
        private static void UpdateGame()
        {
            // 获取蛇头位置
            Position head = snake[0];

            // 根据方向移动蛇头
            switch (direction)
            {
                case Direction.Up:
                    head.Y--;
                    break;
                case Direction.Down:
                    head.Y++;
                    break;
                case Direction.Left:
                    head.X--;
                    break;
                case Direction.Right:
                    head.X++;
                    break;
            }

            // 检查是否撞到边界
            if (head.X < 0 || head.X >= width || head.Y < 0 || head.Y >= height)
            {
                gameOver = true;
                return;
            }

            // 检查是否撞到自己
            foreach (Position segment in snake)
            {
                if (segment.X == head.X && segment.Y == head.Y)
                {
                    gameOver = true;
                    return;
                }
            }

            // 将新的头部添加到蛇身体
            snake.Insert(0, head);

            // 检查是否吃到食物
            if (head.X == food.X && head.Y == food.Y)
            {
                // 吃到食物，增加分数并生成新食物
                score++;
                GenerateFood();
            }
            else
            {
                // 没吃到食物，移除尾部
                snake.RemoveAt(snake.Count - 1);
            }
        }

        // 绘制游戏
        private static void DrawGame()
        {
            Console.SetCursorPosition(0, 0);

            // 绘制顶部边界
            for (int x = 0; x < width; x++)
                Console.Write("#");
            Console.WriteLine();

            // 绘制游戏区域
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == 0 || x == width - 1)
                    {
                        // 绘制左右边界
                        Console.Write("#");
                    }
                    else if (x == food.X && y == food.Y)
                    {
                        // 绘制食物
                        Console.Write("F");
                    }
                    else
                    {
                        // 检查是否是蛇的身体部分
                        bool isSnake = false;
                        foreach (Position segment in snake)
                        {
                            if (segment.X == x && segment.Y == y)
                            {
                                Console.Write("O");
                                isSnake = true;
                                break;
                            }
                        }

                        // 如果不是蛇的部分，绘制空格
                        if (!isSnake)
                            Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }

            // 绘制底部边界
            for (int x = 0; x < width; x++)
                Console.Write("#");
            Console.WriteLine();

            // 显示分数
            Console.WriteLine($"得分: {score}");
            Console.WriteLine("使用方向键控制蛇的移动，ESC键退出");
        }
    }
}