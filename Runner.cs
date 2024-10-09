using System.Security.Cryptography.Xml;

namespace RunnerGame
{
    public partial class Runner : Form
    {
        Image player;
        List<string> playerMovements = new List<string>();

        int steps = 1;
        int slowDownFrameRate = 0;
        bool goRight, goUp;
        int playerX = 1;
        int playerY = 253;
        int playerHeight = 100;
        int playerWidth = 100;
        int playerSpeed = 10;
        bool jumping = false;
        int jumpSpeed = 0;
        int gravity = 2;
        int maxJumpHeight = -20;
        int cnt;

        Image obstacle;
        List<string> obstacles = new List<string>();
        int obstacleX = 630;
        int obstacleY = 313;
        int obstacleHeight = 40;
        int obstacleWidth = 40;

        private System.Windows.Forms.Timer gameTimer;
        Label gameOver;
        Label score;
        int playerScore = 0;

        public Runner()
        {
            InitializeComponent();
            SetUp();
        }

        private void SetUp()
        {
            this.BackgroundImage = Image.FromFile("Assets/bg.jpg");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.DoubleBuffered = true;

            playerMovements = Directory.GetFiles("Assets/Run", "*.png").ToList();
            obstacles = Directory.GetFiles("Assets/obstacles", "*.png").ToList();
            player = Image.FromFile(playerMovements[0]);
            GenerateObstacle();
            

            // Attach key event handlers
            this.KeyDown += new KeyEventHandler(KeyIsDown);
            this.KeyUp += new KeyEventHandler(KeyIsUp);
            this.KeyPreview = true;

            // Attach paint event handler
            this.Paint += new PaintEventHandler(FormPaintEvent);

            // Timer initialization - specify System.Windows.Forms.Timer
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 20; // 20 ms -> 50 FPS
            gameTimer.Tick += new EventHandler(TimerEvent);
            gameTimer.Start();

            // Game over label initialization
            gameOver = new Label();
            gameOver.Text = "";
            gameOver.Font = new Font("Arial", 24);
            gameOver.Location = new Point(300, 200);
            gameOver.AutoSize = true;
            this.Controls.Add(gameOver);

            score = new Label();
            score.Text = "Score: ";
            score.Location = new Point(0, 380);
            score.AutoSize = true;
            this.Controls.Add(score);
            score.Font = new Font("Arial", 24);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Size.Width;
            int formWidth = this.Width;
            this.Location = new Point((screenWidth - formWidth) / 2, 200);
        }

        private void FormPaintEvent(object sender, PaintEventArgs e)
        {
            Graphics Canvas = e.Graphics;
            Canvas.DrawImage(player, playerX, playerY, playerWidth, playerHeight);
            Canvas.DrawImage(obstacle, obstacleX, obstacleY, obstacleWidth, obstacleHeight);
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Up && !jumping)
            {
                goUp = true;
                jumpSpeed = maxJumpHeight;
                jumping = true;
            }
        }

        private void AnimatePlayer(int start, int end)
        {
            slowDownFrameRate += 1;
            if (slowDownFrameRate == 4)
            {
                steps++;
                slowDownFrameRate = 0;
            }
            if (steps > end || steps < start)
            {
                steps = start;
            }
            player = Image.FromFile(playerMovements[steps]);
        }

        private void GenerateObstacle()
        {  
            obstacle = Image.FromFile(obstacles[0]);    
        }

        private void TimerEvent(object sender, EventArgs e)
        {
            if (goRight)
            {
                AnimatePlayer(1, 6);
                obstacleX -= 6;
                if(obstacleX + obstacleWidth < 0)
                {
                    playerScore++;
                    score.Text = "Score: " + playerScore.ToString();
                    GenerateObstacle();
                    obstacleX = this.Width + obstacleWidth;
                }
            }
            else
            {
                AnimatePlayer(0, 0);
            }

            // Handle jumping and gravity
            if (jumping)
            {
                playerY += jumpSpeed;
                jumpSpeed += gravity;

                if (playerY >= 253)
                {
                    playerY = 253;
                    jumpSpeed = 0;
                    jumping = false;
                }
            }

            if (CheckCollision())
            {
                gameOver.Text = "Game Over";
                gameTimer.Stop();  // Stop the game timer to halt game
            }
            
            this.Invalidate(); // Repaint the game
        }

        private bool CheckCollision()
        {
        
            int playerRadius = playerWidth / 2;
            int obstacleRadius = obstacleWidth / 2;

            int playerCenterX = playerX + playerRadius;
            int playerCenterY = playerY + playerRadius;

            int obstacleCenterX = obstacleX + obstacleRadius;
            int obstacleCenterY = obstacleY + obstacleRadius;

            double distance = Math.Sqrt(Math.Pow(playerCenterX - obstacleCenterX, 2) + Math.Pow(playerCenterY - obstacleCenterY, 2));

            if ((distance + 20) <= (playerRadius + obstacleRadius))
            {
                return true;
            }

            return false;
        }
    }
}
