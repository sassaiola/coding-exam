using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Final_solo_project
{
    internal class ScreenPlay : Screen
    {
        private Doodler doodler;
        private List<PlatformBase> platforms;

        private SpriteFont font;
        private Texture2D pixel;
        private Texture2D doodlerTexture;
        private Texture2D enemyTexture;
        private Texture2D attackTexture;
        private Texture2D trapTexture;
        private Texture2D platformBaseTexture;
        private Texture2D doubleJumpTexture;
        private Texture2D breakablePlatformTexture;
        private Texture2D movingPlatformTexture;
        private Texture2D bulletTexture;
        private Texture2D backgroundBaseTexture;
        private Texture2D backgroundTileTexture;
        private Texture2D hyperJumpTexture;

        private float backgroundScrollY;




        private readonly Random random = new Random();

        private float score;
        private float elapsedSeconds;

        private int debugCollisions;

        private float nextSpawnY;
        private const float PlatformStepY = 80f;

        private const int PlatformWidth = 110;
        private const int PlatformHeight = 20;
        private float difficulty01; // 0..1

        private const float MinGapY = 45f;
        private const float MaxGapEasy = 85f;
        private const float MaxGapHard = 125f;

        private List<Trap> traps;
        private const float TrapChanceOnStatic = 0.18f;

        private List<JumpBoost> jumpBoosts;
        private const float JumpBoostChanceOnStatic = 0.1f;

        private List<Enemy> enemies;
        private int killScore;
        private const float EnemyChance = 0.12f;

        private List<Bullet> bullets;

        private float shootCooldownTimer;
        private const float ShootCooldown = 0.18f;

        // ✅ Attack “pre-collision” contro enemy
        private const float EnemyAttackPreTriggerPx = 20f;  
        private float enemyAttackCueCooldownTimer;
        private const float EnemyAttackCueCooldown = 0.18f; 

        public override void Initialize()
        {
            score = 0f;
            elapsedSeconds = 0f;

            if (pixel == null) return;

            ResetLevel();
        }

        public override void LoadContent(ContentManager content)
        {
            doodlerTexture = content.Load<Texture2D>("SpriteSheetAnimation/Zorroverde");
            enemyTexture = content.Load<Texture2D>("SpriteSheetAnimation/enemy"); 
            attackTexture = content.Load<Texture2D>("SpriteSheetAnimation/SpriteSheet_Attack");
            trapTexture = content.Load<Texture2D>("SpriteSheetAnimation/trap");
            platformBaseTexture = content.Load<Texture2D>("SpriteSheetAnimation/platform_base");
            doubleJumpTexture = content.Load<Texture2D>("SpriteSheetAnimation/double_jump");
            breakablePlatformTexture = content.Load<Texture2D>("SpriteSheetAnimation/breakable_platform");
            movingPlatformTexture = content.Load<Texture2D>("SpriteSheetAnimation/cloud_shape2_2");
            bulletTexture = content.Load<Texture2D>("SpriteSheetAnimation/bullet_4");

            hyperJumpTexture = content.Load<Texture2D>("SpriteSheetAnimation/hyperjump");

            backgroundBaseTexture = content.Load<Texture2D>("SpriteSheetAnimation/2");
            backgroundTileTexture = content.Load<Texture2D>("SpriteSheetAnimation/Upper_Sfondo");




            pixel = new Texture2D(GameSetting.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            font = content.Load<SpriteFont>("fonts/UIFont2");

            ResetLevel();
        }

        private void ResetLevel()
        {
            platforms = new List<PlatformBase>();
            traps = new List<Trap>();
            jumpBoosts = new List<JumpBoost>();
            enemies = new List<Enemy>();
            bullets = new List<Bullet>();

            killScore = 0;
            shootCooldownTimer = 0f;
            enemyAttackCueCooldownTimer = 0f;
            backgroundScrollY = 0f;


            CreateDoodler();
            CreateStartPlatform();
            SpawnInitialPlatforms();

            float highestY = float.MaxValue;
            foreach (var p in platforms)
                if (p.TopLeftPosition.Y < highestY)
                    highestY = p.TopLeftPosition.Y;

            nextSpawnY = highestY - PlatformStepY;
        }

        private bool CanPlaceEnemy(Rectangle candidate)
        {
            foreach (var p in platforms)
            {
                if (!p.IsActive) continue;
                if (candidate.Intersects(GetPlatformRect(p, padding: 10))) return false;
            }

            foreach (var t in traps)
            {
                if (!t.IsActive) continue;
                if (candidate.Intersects(GetHitbox(t))) return false;
            }

            foreach (var jb in jumpBoosts)
            {
                if (!jb.IsActive) continue;
                if (candidate.Intersects(GetHitbox(jb))) return false;
            }

            foreach (var e in enemies)
            {
                if (!e.IsActive) continue;
                if (candidate.Intersects(GetHitbox(e))) return false;
            }

            return true;
        }

        private float FindNonOverlappingEnemyX(float y, float w, float h, int attemptsMax = 40)
        {
            float x = 0;
            int attempts = 0;
            Rectangle cand;

            do
            {
                x = random.Next(0, GameSetting.WindowWidth - (int)w);
                cand = new Rectangle((int)x, (int)y, (int)w, (int)h);
                attempts++;
            }
            while (!CanPlaceEnemy(cand) && attempts < attemptsMax);

            return x;
        }

        private void TryShoot()
        {
            if (shootCooldownTimer > 0f) return;

            if (UserInput.IsNewKeyPress(Keys.Space))
            {
                CreateBullet();
                shootCooldownTimer = ShootCooldown;
            }
        }

        private void CreateBullet()
        {
            //  bullet size
            float w = doodler.Size.X * 1.25f;   // puoi alzare/abbassare
            float h = w * 0.5f;                // sottile (tunable)

            // spawn dal centro del doodler
            float x = doodler.TopLeftPosition.X + doodler.Size.X / 2f - w / 2f;
            float y = doodler.TopLeftPosition.Y + 10f;

            AudioManager.PlayEnemyKillBullet();
            doodler.StartAttack();

            var bulletSprite = new SpriteSheet(
                bulletTexture ?? pixel,
                rows: 1,
                columns: 1,
                topLeftPosition: new Vector2(x, y),
                size: new Vector2(w, h)
            );

            var b = new Bullet(bulletSprite, speedY: 16f);

            //   hitbox size
            b.HitboxOffset = Vector2.Zero;
            b.HitboxSize = bulletSprite.Size;

            bullets.Add(b);
        }


        private float GetHighestPlatformY()
        {
            float highestY = float.MaxValue;
            foreach (var p in platforms)
            {
                if (!p.IsActive) continue;
                if (p.TopLeftPosition.Y < highestY)
                    highestY = p.TopLeftPosition.Y;
            }
            return highestY;
        }

        private void CreateDoodler()
        {
            Texture2D tex = doodlerTexture ?? pixel;

            var doodlerSprite = new SpriteSheet(
                tex, 1, 4,
                new Vector2(GameSetting.WindowWidth / 2f, GameSetting.WindowHeight / 3f),
                new Vector2(70, 85)
            );

            doodlerSprite.CropX = 5;
            doodlerSprite.CropY = 0;

            doodler = new Doodler(doodlerSprite);

            // attack sheet (2x4 = 8 frame)
            if (attackTexture != null)
            {
                var attackSheet = new SpriteSheet(
     attackTexture,
     rows: 1,
     columns: 3,                 
     topLeftPosition: doodler.TopLeftPosition,
     size: doodler.Size
 );

                attackSheet.CropX = 0;
                attackSheet.CropY = 0;

                attackSheet.BuildNormalizedTightSourceRects(alphaThreshold: 10, padding: 1);

                doodler.SetAttackSprite(attackSheet, totalFrames: 3);


                attackSheet.CropX = 0;
                attackSheet.CropY = 0;

                doodler.SetAttackSprite(attackSheet, totalFrames: 3);

            }
        }

        private void CreateStartPlatform()
        {
            float startPlatformX = doodler.TopLeftPosition.X - 15f;
            float startPlatformY = doodler.TopLeftPosition.Y + doodler.Size.Y + 10f;

            var startPlatformSprite = new SpriteSheet(
                platformBaseTexture ?? pixel,
                rows: 1,
                columns: 1,
                topLeftPosition: new Vector2(startPlatformX, startPlatformY),
                size: new Vector2(PlatformWidth, PlatformHeight)
            );

            platforms.Add(new StaticPlatform(startPlatformSprite));
        }


        private void SpawnInitialPlatforms()
        {
            int platformCount = 80;
            float verticalStep = PlatformStepY;

            for (int i = 0; i < platformCount; i++)
            {
                float y = GameSetting.WindowHeight - i * verticalStep;

                float x = FindNonOverlappingX(
                    y,
                    PlatformWidth,
                    PlatformHeight,
                    attemptsMax: 30,
                    ignore: null
                );

                double r = random.NextDouble();
                PlatformBase created;

                // === scegli tipo piattaforma ===
                bool isMoving = r < 0.15;
                bool isBreakable = !isMoving && r < 0.30;

                // === scegli texture + size in base al tipo ===
                Texture2D texToUse = platformBaseTexture ?? pixel;
                Vector2 sizeToUse = new Vector2(PlatformWidth, PlatformHeight);

                if (isMoving)
                {
                    texToUse = movingPlatformTexture ?? platformBaseTexture ?? pixel;
                    sizeToUse = new Vector2(190f, 46f); 
                }
                else if (isBreakable)
                {
                    texToUse = breakablePlatformTexture ?? platformBaseTexture ?? pixel;
                    sizeToUse = new Vector2(PlatformWidth, PlatformHeight); 
                }


                // ricentra X se la size è più grande della base
                x = MathHelper.Clamp(
                    x,
                    0,
                    GameSetting.WindowWidth - sizeToUse.X
                );

                // === sprite ===
                var platformSprite = new SpriteSheet(
                    texToUse,
                    rows: 1,
                    columns: 1,
                    topLeftPosition: new Vector2(x, y),
                    size: sizeToUse
                );

                // === crea oggetto piattaforma ===
                if (isMoving) created = new MovingPlatform(platformSprite);
                else if (isBreakable) created = new BreakablePlatform(platformSprite);
                else created = new StaticPlatform(platformSprite);

                platforms.Add(created);

                // === attachments SOLO su static ===
                if (created is StaticPlatform sp)
                {
                    if (!sp.HasAttachment && random.NextDouble() < TrapChanceOnStatic)
                    {
                        CreateTrapOnPlatform(sp);
                        sp.HasAttachment = true;
                    }
                    else if (!sp.HasAttachment && random.NextDouble() < JumpBoostChanceOnStatic)
                    {
                        CreateJumpBoostOnPlatform(sp);
                        sp.HasAttachment = true;
                    }
                }

                // === enemies ===
                if (random.NextDouble() < EnemyChance)
                {
                    CreateEnemyAtY(GameSetting.WindowHeight - i * PlatformStepY - 40f);
                }
            }
        }



        public override void Update(GameTime gameTime)
        {
            doodler.IsOnPlatform = false;
            debugCollisions = 0;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedSeconds += dt;

            shootCooldownTimer -= dt;
            if (shootCooldownTimer < 0f) shootCooldownTimer = 0f;

            enemyAttackCueCooldownTimer -= dt;
            if (enemyAttackCueCooldownTimer < 0f) enemyAttackCueCooldownTimer = 0f;

            foreach (var platform in platforms)
            {
                if (!platform.IsActive) continue;
                platform.Update(gameTime);

            }

            foreach (var jb in jumpBoosts)
            {
                if (!jb.IsActive) continue;
                jb.Update(gameTime);
            }

            foreach (var e in enemies)
            {
                if (!e.IsActive) continue;
                e.Update(gameTime);
            }



            Rectangle prevDoodlerBox = GetHitbox(doodler);

            doodler.Update(gameTime);

            TryShoot();

            foreach (var b in bullets)
            {
                if (!b.IsActive) continue;
                b.Update(gameTime);
            }

            Rectangle currDoodlerBox = GetHitbox(doodler);

            HandleLanding(prevDoodlerBox, currDoodlerBox);
            HandleJumpBoostCollisions(prevDoodlerBox, currDoodlerBox);

            // ✅ pre-trigger attack 
            CueAttackBeforeEnemyPass(prevDoodlerBox, currDoodlerBox);

            HandleEnemyCollisions(prevDoodlerBox, currDoodlerBox);
            HandleBulletEnemyCollisions();

            HandleTrapCollisions(prevDoodlerBox, currDoodlerBox);

            ResolveMovingPlatformOverlaps();
            HandleScrollAndScore();
            RecyclePlatforms(); 

            if (!doodler.IsActive)
            {
                int finalScore = (int)score + killScore;
                GameSetting.LastScore = finalScore;

                AudioManager.PlayGameOver();

                GameSetting.LastScoreQualifiesTop10 = LeaderboardManager.WouldEnterTop10(finalScore);
                GameSetting.ActiveScreen = GameSetting.EndScreen;
                GameSetting.ActiveScreen.Initialize();
                return;
            }

            doodler.UpdateAnimation(gameTime);
        }

        private void CueAttackBeforeEnemyPass(Rectangle prev, Rectangle curr)
        {
            if (enemyAttackCueCooldownTimer > 0f) return;

            bool movingUp = curr.Top < prev.Top;
            if (!movingUp) return;

            foreach (var e in enemies)
            {
                if (!e.IsActive) continue;

                Rectangle eb = GetHitbox(e);

                bool overlapX = curr.Right > eb.Left && curr.Left < eb.Right;
                if (!overlapX) continue;

                //entro 20px dal bottom del nemico (prima del contatto reale)
                float triggerY = eb.Bottom + EnemyAttackPreTriggerPx;

                bool crossedPreTrigger =
                    prev.Top >= triggerY &&
                    curr.Top <= triggerY;

                if (crossedPreTrigger)
                {
                    doodler.StartAttack();
                    enemyAttackCueCooldownTimer = EnemyAttackCueCooldown;
                    return;
                }
            }
        }

        private void HandleBulletEnemyCollisions()
        {
            foreach (var b in bullets)
            {
                if (!b.IsActive) continue;
                Rectangle bb = GetHitbox(b);

                foreach (var e in enemies)
                {
                    if (!e.IsActive) continue;

                    if (bb.Intersects(GetHitbox(e)))
                    {
                        b.IsActive = false;
                        e.IsActive = false;

                        killScore += e.KillScore;

                        //audio melee quando uccidi davvero
                        AudioManager.PlayEnemyKillTop();

                        break;
                    }
                }
            }
        }

        private void CreateEnemyAtY(float y)
        {
            float w = 69f;  // dimensione a schermo 
            float h = 69f;

            float x = FindNonOverlappingEnemyX(y, w, h);

            var enemySprite = new SpriteSheet(
                enemyTexture ?? pixel,
                rows: 2,
                columns: 2,
                topLeftPosition: new Vector2(x, y),
                size: new Vector2(w, h)
            );

            enemySprite.CropX = 0;
            enemySprite.CropY = 0;

            var enemy = new Enemy(enemySprite);

            // hitbox con lo sprite a schermo
            enemy.HitboxOffset = Vector2.Zero;
            enemy.HitboxSize = enemySprite.Size;

            enemies.Add(enemy);
        }

        private void HandleLanding(Rectangle prevDoodlerBox, Rectangle currDoodlerBox)
        {
            if (doodler.Velocity.Y <= 0) return;

            foreach (var platform in platforms)
            {
                if (!platform.IsActive) continue;

                Rectangle platBox = GetHitbox(platform);

                bool crossedPlatformTop =
                    prevDoodlerBox.Bottom <= platBox.Top &&
                    currDoodlerBox.Bottom >= platBox.Top;

                bool overlapX =
                    currDoodlerBox.Right > platBox.Left &&
                    currDoodlerBox.Left < platBox.Right;

                if (crossedPlatformTop && overlapX)
                {
                    doodler.IsOnPlatform = true;
                    debugCollisions++;

                    float newTopLeftY = platBox.Top - doodler.HitboxSize.Y - doodler.HitboxOffset.Y;
                    doodler.TopLeftPosition = new Vector2(doodler.TopLeftPosition.X, newTopLeftY);
                    doodler.Visualization.TopLeftPosition = doodler.TopLeftPosition;

                    float jump = doodler.JumpSpeed * platform.JumpMultiplier;
                    doodler.Velocity = new Vector2(doodler.Velocity.X, -jump);

                    platform.OnPlayerLanding(doodler);

                    if (platform is not BreakablePlatform)
                    {
                        AudioManager.PlayJump();
                    }

                    break;
                }
            }
        }

        private void CreateJumpBoostOnPlatform(PlatformBase platform)
        {
            float w = platform.Size.X / 3f * 1.8f;
            float h = 22f * 1.8f;




            var sprite = new SpriteSheet(
                hyperJumpTexture ?? pixel,
                rows: 1,
                columns: 4,               
                topLeftPosition: Vector2.Zero,
                size: new Vector2(w, h)
            );

            sprite.CropX = 0;
            sprite.CropY = 0;

            var boost = new JumpBoost(sprite, platform);
            boost.SnapToPlatformTop();
            jumpBoosts.Add(boost);
        }


        private void HandleJumpBoostCollisions(Rectangle prev, Rectangle curr)
        {
            bool isFalling = curr.Bottom > prev.Bottom;
            if (!isFalling) return;

            foreach (var jb in jumpBoosts)
            {
                if (!jb.IsActive) continue;

                Rectangle boostBox = GetHitbox(jb);

                bool crossedTop =
                    prev.Bottom <= boostBox.Top &&
                    curr.Bottom >= boostBox.Top;

                bool overlapX =
                    curr.Right > boostBox.Left &&
                    curr.Left < boostBox.Right;

                if (crossedTop && overlapX)
                {
                    float boostedJump = doodler.JumpSpeed * jb.BoostMultiplier;
                    doodler.Velocity = new Vector2(doodler.Velocity.X, -boostedJump);

                    AudioManager.PlayIperJump();

                    jb.TriggerBounceAnim();  

                    return;
                }

            }
        }

        private void HandleEnemyCollisions(Rectangle prev, Rectangle curr)
        {
            foreach (var e in enemies)
            {
                if (!e.IsActive) continue;

                Rectangle eb = GetHitbox(e);
                bool overlapX = curr.Right > eb.Left && curr.Left < eb.Right;

                bool movingUp = curr.Top < prev.Top;
                bool hitFromBelow =
                    movingUp &&
                    prev.Top >= eb.Bottom &&
                    curr.Top <= eb.Bottom &&
                    overlapX;

                if (hitFromBelow)
                {
                    e.IsActive = false;
                    killScore += e.KillScore;

                    AudioManager.PlayEnemyKillTop();

                    float damp = 0.55f;
                    doodler.Velocity = new Vector2(doodler.Velocity.X, doodler.Velocity.Y * damp);
                    return;
                }

                bool movingDown = curr.Bottom > prev.Bottom;
                bool landOnEnemy =
                    movingDown &&
                    prev.Bottom <= eb.Top &&
                    curr.Bottom >= eb.Top &&
                    overlapX;

                if (landOnEnemy)
                {
                    doodler.IsActive = false;
                    return;
                }
            }
        }

        private void ResolveMovingPlatformOverlaps()
        {
            foreach (var p in platforms)
            {
                if (p is not MovingPlatform mp) continue;
                if (!mp.IsActive) continue;

                Rectangle r1 = new Rectangle(
                    (int)mp.TopLeftPosition.X,
                    (int)mp.TopLeftPosition.Y,
                    (int)mp.Size.X,
                    (int)mp.Size.Y
                );

                foreach (var other in platforms)
                {
                    if (other == mp) continue;
                    if (!other.IsActive) continue;

                    Rectangle r2 = new Rectangle(
                        (int)other.TopLeftPosition.X,
                        (int)other.TopLeftPosition.Y,
                        (int)other.Size.X,
                        (int)other.Size.Y
                    );

                    if (r1.Intersects(r2))
                    {
                        mp.Velocity = new Vector2(-mp.Velocity.X, 0f);
                        break;
                    }
                }
            }
        }

        private void HandleScrollAndScore()
        {
            float scrollThreshold = GameSetting.WindowHeight * 0.6f;

            if (doodler.TopLeftPosition.Y < scrollThreshold && doodler.Velocity.Y < 0)
            {
                float delta = scrollThreshold - doodler.TopLeftPosition.Y;

                backgroundScrollY -= delta;


                score += delta;

                doodler.TopLeftPosition = new Vector2(doodler.TopLeftPosition.X, scrollThreshold);
                doodler.Visualization.TopLeftPosition = doodler.TopLeftPosition;

                foreach (var platform in platforms)
                {
                    platform.TopLeftPosition = new Vector2(platform.TopLeftPosition.X, platform.TopLeftPosition.Y + delta);
                    platform.Visualization.TopLeftPosition = platform.TopLeftPosition;
                }

                foreach (var trap in traps)
                {
                    trap.TopLeftPosition = new Vector2(trap.TopLeftPosition.X, trap.TopLeftPosition.Y + delta);
                    trap.Visualization.TopLeftPosition = trap.TopLeftPosition;
                }

                foreach (var jb in jumpBoosts)
                {
                    jb.TopLeftPosition = new Vector2(jb.TopLeftPosition.X, jb.TopLeftPosition.Y + delta);
                    jb.Visualization.TopLeftPosition = jb.TopLeftPosition;
                }

                foreach (var e in enemies)
                {
                    if (!e.IsActive) continue;
                    e.TopLeftPosition = new Vector2(e.TopLeftPosition.X, e.TopLeftPosition.Y + delta);
                    e.Visualization.TopLeftPosition = e.TopLeftPosition;
                }

                foreach (var b in bullets)
                {
                    if (!b.IsActive) continue;
                    b.TopLeftPosition = new Vector2(b.TopLeftPosition.X, b.TopLeftPosition.Y + delta);
                    b.Visualization.TopLeftPosition = b.TopLeftPosition;
                }
            }
        }

        private void RecyclePlatforms()
        {
            foreach (var p in platforms)
            {
                if (p.TopLeftPosition.Y > GameSetting.WindowHeight)
                {
                    float highestY = GetHighestPlatformY();

                    difficulty01 = MathHelper.Clamp(score / 3000f, 0f, 1f);
                    float maxGap = MathHelper.Lerp(MaxGapEasy, MaxGapHard, difficulty01);

                    float gap = (float)(MinGapY + random.NextDouble() * (maxGap - MinGapY));
                    float newY = highestY - gap;

                    float newX = FindNonOverlappingX(
                        newY,
                        p.Size.X,
                        p.Size.Y,
                        attemptsMax: 30,
                        ignore: p
                    );

                    p.HasAttachment = false;

                    p.TopLeftPosition = new Vector2(newX, newY);
                    p.Visualization.TopLeftPosition = p.TopLeftPosition;

                    p.IsActive = true;
                    if (p is BreakablePlatform bp) bp.Repair();

                    foreach (var trap in traps)
                    {
                        if (trap.ParentPlatform == p)
                        {
                            trap.IsActive = true;
                            trap.SnapToPlatformTop();
                            break;
                        }
                    }

                    foreach (var jb in jumpBoosts)
                    {
                        if (jb.ParentPlatform == p)
                        {
                            jb.IsActive = true;
                            jb.SnapToPlatformTop();
                            break;
                        }
                    }

                    if (random.NextDouble() < EnemyChance)
                    {
                        float enemyY = newY - 60f;
                        CreateEnemyAtY(enemyY);
                    }

                    foreach (var e in enemies)
                    {
                        if (!e.IsActive) continue;
                        if (e.TopLeftPosition.Y > GameSetting.WindowHeight)
                            e.IsActive = false;
                    }
                }
            }
        }

        private float FindNonOverlappingX(float y, float w, float h, int attemptsMax, PlatformBase ignore)
        {
            float x = 0;
            int attempts = 0;
            Rectangle cand;

            do
            {
                x = random.Next(0, GameSetting.WindowWidth - (int)w);
                cand = new Rectangle((int)x, (int)y, (int)w, (int)h);
                attempts++;
            }
            while (!CanPlacePlatform(cand, ignore) && attempts < attemptsMax);

            return x;
        }

        private bool CanPlacePlatform(Rectangle candidate, PlatformBase ignore = null)
        {
            foreach (var p in platforms)
            {
                if (p == ignore) continue;
                if (!p.IsActive) continue;

                var r = GetPlatformRect(p, padding: 8);
                if (candidate.Intersects(r))
                    return false;
            }
            return true;
        }

        private Rectangle GetPlatformRect(PlatformBase p, int padding = 0)
        {
            return new Rectangle(
                (int)p.TopLeftPosition.X - padding,
                (int)p.TopLeftPosition.Y - padding,
                (int)p.Size.X + padding * 2,
                (int)p.Size.Y + padding * 2
            );
        }

        private Rectangle GetHitbox(GameObject o)
        {
            return new Rectangle(
                (int)(o.TopLeftPosition.X + o.HitboxOffset.X),
                (int)(o.TopLeftPosition.Y + o.HitboxOffset.Y),
                (int)o.HitboxSize.X,
                (int)o.HitboxSize.Y
            );
        }

        private void CreateTrapOnPlatform(PlatformBase platform)
        {
            float trapW = platform.Size.X / 3f;
            float trapH = 22f; // un filo più visibile del 18

            var trapSprite = new SpriteSheet(
                trapTexture ?? pixel,
                rows: 1,
                columns: 1,
                topLeftPosition: Vector2.Zero,
                size: new Vector2(trapW, trapH)
            );

            var trap = new Trap(trapSprite, platform);
            trap.HitboxOffset = Vector2.Zero;
            trap.HitboxSize = trap.Size;

            trap.SnapToPlatformTop();
            traps.Add(trap);
        }


        private void HandleTrapCollisions(Rectangle prevDoodlerBox, Rectangle currDoodlerBox)
        {
            bool isMovingDown = currDoodlerBox.Bottom > prevDoodlerBox.Bottom;
            if (!isMovingDown) return;

            foreach (var t in traps)
            {
                if (!t.IsActive) continue;

                Rectangle trapBox = GetHitbox(t);

                bool crossedTrapTop =
                    prevDoodlerBox.Bottom <= trapBox.Top &&
                    currDoodlerBox.Bottom >= trapBox.Top;

                bool overlapX =
                    currDoodlerBox.Right > trapBox.Left &&
                    currDoodlerBox.Left < trapBox.Right;

                if (crossedTrapTop && overlapX)
                {
                    doodler.IsActive = false;
                    return;
                }
            }
        }

        private void DrawBackground(SpriteBatch spriteBatch)
        {
            int screenW = GameSetting.WindowWidth;
            int screenH = GameSetting.WindowHeight;

            // ===== BASE =====
            spriteBatch.Draw(
                backgroundBaseTexture,
                destinationRectangle: new Rectangle(0, 0, screenW, screenH),
                color: Color.White
            );

            // ===== TILE RIPETUTE =====
            int tileH = backgroundTileTexture.Height;

            float offsetY = backgroundScrollY % tileH;

            for (int y = -tileH; y < screenH + tileH; y += tileH)
            {
                spriteBatch.Draw(
                    backgroundTileTexture,
                    new Rectangle(0, (int)(y - offsetY), screenW, tileH),
                    Color.White
                );
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // 🔹 PRIMA lo sfondo
            DrawBackground(spriteBatch);

            // 🔹 POI tutto il resto
            doodler.Draw(spriteBatch);

            foreach (var platform in platforms)
                if (platform.IsActive)
                    platform.Draw(spriteBatch);

            foreach (var jb in jumpBoosts)
                jb.Draw(spriteBatch);

            foreach (var e in enemies)
                if (e.IsActive)
                    e.Draw(spriteBatch);

            foreach (var b in bullets)
                if (b.IsActive)
                    b.Draw(spriteBatch);

            foreach (var t in traps)
                if (t.IsActive)
                    t.Draw(spriteBatch);

            // HUD
            // UI
            int totalSeconds = (int)elapsedSeconds;
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            spriteBatch.DrawString(font, $"Time: {minutes:00}:{seconds:00}", new Vector2(10, 30), Color.White);

            spriteBatch.DrawString(font, $"Kills: {killScore / 250}", new Vector2(10, 50), Color.White);

            int totalScore = (int)score + killScore;
            spriteBatch.DrawString(font, $"Score: {totalScore}", new Vector2(10, 10), Color.White);

        }


    }
}
